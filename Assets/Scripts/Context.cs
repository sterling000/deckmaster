using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using deckmaster;
using UniRx;
using UnityEngine;

public class Context : MonoBehaviour
{
    public DeckView deckViewPrefab;

    public RectTransform contentRectTransform;

    /// <summary>
    /// Until it's determined if we can request my list of decks from the archidekt api, i'll hard code the id's here.
    /// </summary>
    public List<int> decks;

    public List<DeckModel> deckLists;

    public HashSet<int> pendingDeckListRequests;

    // Start is called before the first frame update
    void Start()
    {
        /// todo:   load saved data
        ///         check timestamp
        ///         request new data
        ///         if new data analyze decks
        ///          
        pendingDeckListRequests = new HashSet<int>();
        decks.ToObservable().Subscribe(GetDeck);
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
            IDisposable getDeck = Observable.Start(() =>
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
            })
                .ObserveOnMainThread() // we have to observe on the main thread to call instantiate for now.
                .Subscribe(_ => {Debug.Log($"GetDeck OnNext: {id}"); }, e => {Debug.LogException(e); }, () => OnGetDeckCompleted(id));
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
            deckLists.Add(DeserializeDeck(serializedDeckList));
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
            deckViewPrefab.Name.text = model.name;

            foreach (CardModel card in model.cards)
            {
                deckView.AddCard(card);
            }
        });
    }

    private DeckModel DeserializeDeck(string serializedDeckList)
    {
        return JsonUtility.FromJson<DeckModel>(serializedDeckList);
    }
}
