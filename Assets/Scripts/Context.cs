using deckmaster;
using PersistentStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UniRx;
using UnityEngine;

public class Context : MonoBehaviour
{
    public DeckView deckViewPrefab;

    public RectTransform contentRectTransform;

    public GameObject loadingPanel;

    /// <summary>
    /// Until it's determined if we can request my list of decks from the archidekt api, i'll hard code the id's here.
    /// </summary>
    public List<int> decks;

    public List<DeckModel> deckLists;

    public HashSet<int> pendingDeckListRequests;

    private List<IObservable<Unit>> threadList = new List<IObservable<Unit>>();

    // i wanted to use threads for my db access but the persistent data path only seems to be accessible from the main thread, so lets get it here and pass it along.
    private string PersistentDataPath;
    
    // Start is called before the first frame update
    void Start()
    {
        PersistentDataPath = Application.persistentDataPath;
        loadingPanel.SetActive(true);
        
        /// todo:   load saved data
        ///         check timestamp
        ///         request new data
        ///         if new data analyze decks
        pendingDeckListRequests = new HashSet<int>();
        decks.ToObservable().Subscribe(GetDeck);
        Debug.Log($"threadList.Count: {threadList.Count}");
        Observable.WhenAll(threadList.ToArray())
            .ObserveOnMainThread() // we have to observe on the main thread to call instantiate for now.
            .Subscribe(OnNextAllThreads, Debug.LogException, OnAllDecksCompleted);
    }

    private void OnNextAllThreads(Unit obj)
    {
        Debug.Log($"OnNextAllThreads: {obj}");
    }

    private void GetDeck(int id)
    {
        Debug.Log($"Getting Deck: {id}");
        if (pendingDeckListRequests.Add(id))
        {
#if UNITY_EDITOR // todo: instead should use an interface with a data provider
            TextAsset asset = Resources.Load<TextAsset>($"{id}");
            ParseDeckList(asset.text);
#else
            var getDeck = Observable.Start(() =>
            {
                var webRequest = WebRequest.Create($"https://archidekt.com/api/decks/{id}/") as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.Method = WebRequestMethods.Http.Get;

                    HttpWebResponse response;
                    try
                    {
                        response = webRequest.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException ex)
                    {
                        Debug.LogError(ex.Message);
                        return;
                    }

                    OnNextDeck(response);
                    response?.GetResponseStream()?.Dispose();
                }
            });
            
            threadList.Add(getDeck);
#endif
        }
    }

    private void OnNextDeck(HttpWebResponse response)
    {
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string serializedDeckList = reader.ReadToEnd();
            ParseDeckList(serializedDeckList);
        }
    }

    private void ParseDeckList(string serializedDeckList)
    {
        DeckModel model = DeserializeDeck(serializedDeckList);
        deckLists.Add(model);

        // cache our decklists
        DeckInfoDb deckListDB = new DeckInfoDb(PersistentDataPath);
        deckListDB.CreateOrUpdateData(new DeckInfoEntry(model.id, model.name)); // AddData will have conflicts
        deckListDB.Close();
    }

    private void OnAllDecksCompleted()
    {
        // this is a heavy method...
        // todo: broadcast this to the screencontroller and let it create these
        // todo: these for loops are probably not the fastest algorithm.
        DeckCardDb deckCardDb = new DeckCardDb(PersistentDataPath);
        Debug.Log("Initializing DeckCardDB");
        // file our deckCardDb with cards
        foreach (var model in deckLists)
        {
            List<CardEntry> cardEntries = new List<CardEntry>();
            // todo: add a filter for basic lands sideboard and maybeboard categories that is more elegant.
            var filteredCards = model.cards.Where(cardModel =>
            {
                bool hasBasicSuperType = cardModel.card.oracleCard.SuperTypes.HasFlag(SuperTypes.Basic);
                return ! hasBasicSuperType && cardModel.Category != Category.Maybeboard && cardModel.Category != Category.Sideboard;
            });
            foreach (CardModel card in filteredCards) 
            {
                cardEntries.Add(new CardEntry(card.card.id, card.card.oracleCard.name, model.id));
            }
            deckCardDb.CreateOrUpdateData(cardEntries);
        }

        //query the db for staples
        Dictionary<int, int> staples = deckCardDb.QueryStaples();
        
        // create our decklist views
        foreach (var deckModel in deckLists)
        {
            DeckView deckView = Instantiate(deckViewPrefab);
            deckView.transform.SetParent(contentRectTransform);
            deckView.Name.text = deckModel.name;
            deckView.StaplesCount.text = staples[deckModel.id].ToString();

            List<int> deckStaples = deckCardDb.QueryDeckStaples(deckModel.id);
            foreach (int staple in deckStaples)
            {
                CardModel cardModel = deckModel.cards.ToList().Find(card => card.card.id == staple);
                if (cardModel != null)
                {
                    deckView.AddCard(cardModel);
                }
            }
        }

        loadingPanel.SetActive(false);
    }

    private DeckModel DeserializeDeck(string serializedDeckList)
    {
        return JsonUtility.FromJson<DeckModel>(serializedDeckList);
    }
}
