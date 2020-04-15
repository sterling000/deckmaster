using deckmaster;
using PersistentStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Context : MonoBehaviour
{
    public DeckView deckViewPrefab;

    public RectTransform contentRectTransform;

    public rotatetotate loadingSymbol;

    public Button SlotsButton;

    public TextMeshProUGUI SlotsText;
    public GameObject DeckListScrollView;
    public GameObject SlotsScrollView;
    public Toggle JsonToggle;

    public Button RefreshButton;

    [SerializeField]
    private List<int> decks;

    public List<DeckModel> deckLists;

    public List<string> SlotsList;

    public HashSet<int> pendingDeckListRequests;
    private List<IObservable<Unit>> threadList;
    // i wanted to use threads for my db access but the persistent data path only seems to be accessible from the main thread, so lets get it here and pass it along.
    private string PersistentDataPath;

    private DataProvider dataProvider;

    private Subject<UserModel> userModelSubject = new Subject<UserModel>();
    private ReplaySubject<DeckModel> DeckModelSubject = new ReplaySubject<DeckModel>();

    public CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource cts;

    // Start is called before the first frame update
    void Start()
    {
        cts = new CancellationTokenSource();
        PersistentDataPath = Application.persistentDataPath;
        JsonToggle.isOn = GnarlyMenuItems.IsEnabled;
        JsonToggle.OnValueChangedAsObservable().Subscribe(b => GnarlyMenuItems.IsEnabled = b);
        RefreshButton.OnClickAsObservable().Subscribe(_ => RefreshDecks()); // todo: make sure it dispose of this later
        SlotsButton.OnClickAsObservable().Subscribe(_ => ToggleSlotsPanel());

        if (GnarlyMenuItems.IsEnabled)
        {
            dataProvider = new ResourcesFolderDataProvider();
        }
        else
        {
            dataProvider = new ArchidektDataProvider();
        }
    }

    private void RefreshDecks()
    {
        loadingSymbol.rotateSpeed = 200;
        RefreshButton.interactable = false;
        DeckListScrollView.gameObject.SetActive(true);
        SlotsScrollView.gameObject.SetActive(false);
        SlotsList.Clear();

        SlotsButton.interactable = false;

        userModelSubject.Subscribe(OnNextUserModel).AddTo(disposables);

        dataProvider.GetUserModelTask(userModelSubject).Forget();

            // Todo: stop here and listen for the response message.

            // since we aren't using threads to get the decks in the editor, we are actually calling OnAllDecksCompleted here immediately.
            // if (GnarlyMenuItems.IsEnabled)
            // {
            //     foreach (int id in decks)
            //     {
            //         GetDeck(id);
            //     }
            // }
            // else
            // {
            //     decks.Clear();
            //     threadList = new List<IObservable<Unit>>();
            //     decks.ToReactiveCollection().ObserveAdd().Subscribe(i => GetDeck(i.Value));
            //     // fetch decklists from Archidekt
            //     var getDeckIds = Observable.Start(() =>
            //     {
            //         var webRequest =
            //             WebRequest.Create(
            //                     $"https://archidekt.com/api/decks/cards/?orderBy=-createdAt&owner=Wildcard&ownerexact=true&pageSize=50")
            //                 as HttpWebRequest;
            //
            //         if (webRequest != null)
            //         {
            //             webRequest.Method = WebRequestMethods.Http.Get;
            //
            //             HttpWebResponse response;
            //             try
            //             {
            //                 response = webRequest.GetResponse() as HttpWebResponse;
            //             }
            //             catch (WebException ex)
            //             {
            //                 Debug.LogError(ex.Message);
            //                 return;
            //             }
            //
            //             using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //             {
            //                 UserModel model = JsonUtility.FromJson<UserModel>(reader.ReadToEnd());
            //                 foreach (UserDeckData userDeckData in model.results)
            //                 {
            //                     decks.Add(userDeckData.id);
            //                 }
            //             }
            //             
            //             response?.GetResponseStream()?.Dispose();
            //         }
            //     }).Subscribe(unit => Debug.Log($"OnNextDeckIds"), e => Debug.LogException(e), () =>
            //         {
            //             Debug.Log($"OnCompleteDeckIds");
            //             
            //             Observable.WhenAll(threadList.ToArray())
            //                 .ObserveOnMainThread() // we have to observe on the main thread to call instantiate for now.
            //                 .Subscribe(OnNextAllThreads, Debug.LogException, OnAllDecksCompleted);
            //         });
            // }
    }

    private void OnNextUserModel(UserModel model)
    {
        foreach (UserDeckData userDeckData in model.results)
        {
            dataProvider.GetDeckModel(userDeckData.id, DeckModelSubject);
        }

        DeckModelSubject.Take(model.count).Subscribe(OnNextDeckModel, OnDeckModelsCompleted).AddTo(disposables);
    }

    private void OnDeckModelsCompleted()
    {
        Debug.Log($"OnDeckModelsCompleted");
        OnAllDecksCompleted();
    }

    private void OnNextDeckModel(DeckModel model)
    {
        deckLists.Add(model);

        // cache our decklists
        DeckInfoDb deckListDB = new DeckInfoDb(PersistentDataPath);
        deckListDB.CreateOrUpdateData(new DeckInfoEntry(model.id, model.name)); // AddData will have conflicts
        //deckListDB.Close();
    }

    private void OnNextAllThreads(Unit obj)
    {
        Debug.Log($"OnNextAllThreads: {obj}");
    }

    private void ParseDeckList(string serializedDeckList)
    {
        DeckModel model = DeserializeDeck(serializedDeckList);
        deckLists.Add(model);

        // cache our decklists
        DeckInfoDb deckListDB = new DeckInfoDb(PersistentDataPath);
        deckListDB.CreateOrUpdateData(new DeckInfoEntry(model.id, model.name)); // AddData will have conflicts
        //deckListDB.Close();
    }

    private void OnAllDecksCompleted()
    {
        Debug.Log("AllDecksCompleted");
        // this is a heavy method...
        // todo: broadcast this to the screencontroller and let it create these
        // todo: these for loops are probably not the fastest algorithm.
        DeckCardDb deckCardDb = new DeckCardDb(PersistentDataPath);
        // file our deckCardDb with cards
        foreach (var model in deckLists)
        {
            List<CardEntry> cardEntries = new List<CardEntry>();
            // todo: add a filter for basic lands sideboard and maybeboard categories that is more elegant.
            var filteredCards = model.cards.Where(cardModel =>
            {
                bool hasBasicSuperType = cardModel.card.oracleCard.SuperTypes.HasFlag(SuperTypes.Basic);
                return ! hasBasicSuperType && cardModel.Category != Category.Maybeboard && cardModel.Category != Category.Sideboard;
            }).OrderBy(cardModel => cardModel.Category);
            foreach (CardModel card in filteredCards) 
            {
                cardEntries.Add(new CardEntry(card.card.id, card.card.oracleCard.name, model.id));
            }
            deckCardDb.CreateOrUpdateData(cardEntries);
        }

        //query the db for staples
        Dictionary<int, int> staples = deckCardDb.QueryStaples();
        List<int> slotList = deckCardDb.QuerySlotList();
        
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
                    deckView.AddCard(cardModel, slotList.IndexOf(cardModel.card.id)+1);
                }
            }
        }

        foreach (int id in slotList)
        {
            SlotsList.Add(deckCardDb.GetNameById(id));   
        }
        StringBuilder builder = new StringBuilder();
        for (var slot = 0; slot < SlotsList.Count; slot++)
        {
            string card = SlotsList[slot];
            builder.AppendLine($"{slot + 1} -> {card}");
        }

        SlotsText.text = builder.ToString();
        builder.Clear();
        deckCardDb.Close();
        loadingSymbol.rotateSpeed = 0;
        RefreshButton.interactable = true;
        SlotsButton.interactable = true;
    }

    private DeckModel DeserializeDeck(string serializedDeckList)
    {
        return JsonUtility.FromJson<DeckModel>(serializedDeckList);
    }

    private void ToggleSlotsPanel()
    {
        DeckListScrollView.gameObject.SetActive(!DeckListScrollView.gameObject.activeInHierarchy);
        SlotsScrollView.gameObject.SetActive(!SlotsScrollView.gameObject.activeInHierarchy);
    }

    void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}
