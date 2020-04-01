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
        StartCoroutine(GetRequest("https://archidekt.com/api/decks/365563/"));
    }

    IEnumerator GetRequest(string url)
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
            }

            // todo: deserialize the result into a DeckModel

            // populate the deckLists
            DeckView deckGameObject = Instantiate(deckView);
            deckGameObject.transform.SetParent(contentRectTransform);
            
            // set the name of the deck

            // perform our logic to determine the staples

            // set the details
        }
    }
}
