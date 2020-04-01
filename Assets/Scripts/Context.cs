using System;
using System.Collections;
using System.Collections.Generic;
using deckmaster;
using UnityEngine;
using UnityEngine.Networking;

public class Context : MonoBehaviour
{
    public DeckView deckView;

    public RectTransform contentRectTransform;

    /// <summary>
    /// Until it's determined if we can request my list of decks from the archidekt api, i'll hard code the id's here.
    /// </summary>
    public List<int> decks;

    public List<DeckModel> deckLists;

    // Start is called before the first frame update
    void Start()
    {
        // todo: need to fetch my decklists for my user somehow.

        foreach (int id in decks)
        {
            StartCoroutine(GetRequest(string.Format("https://archidekt.com/api/decks/{0}/", id)));
        }

        //StartCoroutine(GetRequest("https://archidekt.com/api/decks/365563/"));
    }

    IEnumerator GetRequest(string url)
    {
        string json = String.Empty;
        if (Application.isEditor) 
        {
            TextAsset asset = Resources.Load<TextAsset>("test-deck");
            json = asset.text;
        }
        else
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    json = webRequest.downloadHandler.text;
                }


            }
            // todo: deserialize the result into a DeckModel
        }

        if (!string.IsNullOrEmpty(json))
        {
            // populate the deckLists
            DeckView deckGameObject = Instantiate(deckView);
            deckGameObject.transform.SetParent(contentRectTransform);

            var obj = JsonUtility.FromJson<DeckModel>(json);
            // set the Name of the deck
            deckView.Name.text = obj.name;
            // perform our logic to determine the staples
            foreach (CardModel model in obj.cards)
            {
                deckGameObject.AddCard(model);
            }
        }
    }
}
