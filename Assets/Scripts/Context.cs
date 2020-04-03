using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using deckmaster;
using PersistentStorage;
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
        ///          
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
#if !UNITY_EDITOR
        TextAsset asset = Resources.Load<TextAsset>("test-deck");
        
        deckLists.Add(DeserializeDeck(asset.text));
    }
#else
        if (pendingDeckListRequests.Add(id))
        {
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
        }
    }

    private void OnGetDeckCompleted(int id)
    {
        if (pendingDeckListRequests.Remove(id) )
        {
            Debug.Log($"Pending Deck Lists: {pendingDeckListRequests.Count}");
            if (pendingDeckListRequests.Count == 0)
            {
                // lets create our views
                OnAllDecksCompleted();
            }
        }
        else
        {
            Debug.LogError($"Somehow we tried to remove a pending deck that wasn't in the hashset: {id}");
        }
    }

    private void OnNextDeck(HttpWebResponse response)
    {
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string serializedDeckList = reader.ReadToEnd();
            DeckModel model = DeserializeDeck(serializedDeckList);
            deckLists.Add(model);
            
            // cache our decklists
            DeckListDb deckListDB = new DeckListDb(PersistentDataPath);
            deckListDB.CreateOrUpdateData(new DeckListEntry(model.id.ToString(), model.name)); // AddData will have conflicts
            deckListDB.Close();
        }
    }
#endif

    private void OnAllDecksCompleted()
    {
        // todo: broadcast this to the screencontroller and let it create these
        deckLists.ForEach(model =>
        {
            DeckView deckView = Instantiate(deckViewPrefab);
            deckView.transform.SetParent(contentRectTransform);
            deckView.Name.text = model.name;

            foreach (CardModel card in model.cards)
            {
                deckView.AddCard(card);
            }
        });

        DeckListDb deckListDB = new DeckListDb(PersistentDataPath);
        IDataReader reader = deckListDB.GetAllData();
        List<DeckListEntry> deckListEntries = new List<DeckListEntry>();
        while (reader.Read())
        {
            DeckListEntry entry = new DeckListEntry(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString());
            Debug.Log($"id: {entry.Id} name: {entry.Name} created: {entry.DateCreated} updated: {entry.DateUpdated}");
            deckListEntries.Add(entry);
        }

        loadingPanel.SetActive(false);
    }

    private DeckModel DeserializeDeck(string serializedDeckList)
    {
        return JsonUtility.FromJson<DeckModel>(serializedDeckList);
    }
}
