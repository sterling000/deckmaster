using deckmaster;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public class Context : MonoBehaviour
{
    public DeckPresenter deckPresenter;
    
    public rotatetotate loadingSymbol;

    public Button SlotsButton;

    public TextMeshProUGUI SlotsText;
    public GameObject SlotsScrollView;
    public Toggle JsonToggle;

    public Button RefreshButton;
    
    private List<DeckModel> deckLists;

    public List<string> SlotsList;

    private DataProvider dataProvider;

    private ReplaySubject<UserModel> userModelSubject = new ReplaySubject<UserModel>();
    private ReplaySubject<DeckModel> DeckModelSubject = new ReplaySubject<DeckModel>();

    public CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource cts;

    // Start is called before the first frame update
    void Start()
    {
        cts = new CancellationTokenSource();
        deckLists = new List<DeckModel>();
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

        dataProvider.GetUserModelTask(userModelSubject).Forget();
    }

    private void RefreshDecks()
    {
        loadingSymbol.rotateSpeed = 200;
        RefreshButton.interactable = false;
        deckPresenter.gameObject.SetActive(true);
        SlotsScrollView.gameObject.SetActive(false);
        SlotsList.Clear();
        deckPresenter.Clear();
        disposables.Clear();
        SlotsButton.interactable = false;

        userModelSubject.Subscribe(OnNextUserModel).AddTo(disposables);
    }

    private async void OnNextUserModel(UserModel model)
    {
        IEnumerable<UniTask> tasks = from userDeckData in model.results select dataProvider.GetDeckModelTask(userDeckData.id, DeckModelSubject);
        UniTask[] uniTaskVoids = tasks.ToArray();
        DeckModelSubject.Take(model.count).Subscribe(OnNextDeckModel, OnDeckModelsCompleted).AddTo(disposables);
        await UniTask.WhenAll(uniTaskVoids);
    }

    private void OnDeckModelsCompleted()
    {
        Debug.Log($"OnDeckModelsCompleted");
        OnAllDecksCompleted();
    }

    private void OnNextDeckModel(DeckModel model)
    {
        deckLists.Add(model);
    }

    private void OnAllDecksCompleted()
    {
        Debug.Log("AllDecksCompleted");
        
        List<CardModel> unsortedCardModels = new List<CardModel>();
        foreach (DeckModel deckModel in deckLists)
        {
            List<CardModel> filteredCardModels = deckModel.cards.Where(cardModel =>
            {
                bool hasBasicSuperType = cardModel.card.oracleCard.SuperTypes.HasFlag(SuperTypes.Basic);
                return !hasBasicSuperType && cardModel.Category != Category.Maybeboard &&
                       cardModel.Category != Category.Sideboard;
            }).ToList();
            unsortedCardModels.AddRange(filteredCardModels);
        }

        IEnumerable<IGrouping<int, CardModel>> modelsGroupedById = unsortedCardModels.GroupBy(model => model.card.id);
        StringBuilder debugGroupsById = new StringBuilder("modelsGroupedById:");
        debugGroupsById.AppendLine("--------------------------");
        foreach (IGrouping<int, CardModel> grouping in modelsGroupedById)
        {
            debugGroupsById.AppendLine($"{grouping.Key}");
            foreach (CardModel cardModel in grouping)
            {
                debugGroupsById.AppendLine($"{cardModel.card.oracleCard.name}");
            }
        }

        debugGroupsById.AppendLine("--------------------------");
        Debug.Log(debugGroupsById);
        
        IOrderedEnumerable<IGrouping<int, CardModel>> modelsGroupedByIdOrderedByCategory = modelsGroupedById.OrderBy(grouping => grouping.FirstOrDefault().Category).ThenByDescending(models => models.Count()).ThenBy(grouping => grouping.Key);
        IEnumerable<IGrouping<int, CardModel>> modelsGroupedAndSorted = modelsGroupedByIdOrderedByCategory.Where(models => models.Count() > 1);
        
        StringBuilder debugOrderedGroupsByCategory = new StringBuilder("modelsGroupedByIdOrdererdByCategory");
        debugOrderedGroupsByCategory.AppendLine("--------------------------");
        foreach (IGrouping<int, CardModel> grouping in modelsGroupedAndSorted)
        {
            debugOrderedGroupsByCategory.AppendLine($"{grouping.Key}");
            foreach (CardModel cardModel in grouping)
            {
                debugOrderedGroupsByCategory.AppendLine($"{cardModel.card.oracleCard.name} | {cardModel.Category}");
            }
            SlotsList.Add(grouping.FirstOrDefault().card.oracleCard.name);
        }
        debugOrderedGroupsByCategory.AppendLine("--------------------------");
        Debug.Log(debugOrderedGroupsByCategory);
        List<int> stapleIds = modelsGroupedAndSorted.Select(models => models.Key).ToList();
        foreach (DeckModel deckModel in deckLists)
        {
            foreach (CardModel cardModel in deckModel.cards)
            {
                if (stapleIds.Contains(cardModel.card.id))
                {
                    cardModel.slot = stapleIds.IndexOf(cardModel.card.id) + 1;
                    deckModel.Staples.Add(cardModel);
                }
            }
            deckPresenter.Create(deckModel);
        }

        StringBuilder builder = new StringBuilder();
        for (var slot = 0; slot < SlotsList.Count; slot++)
        {
            string card = SlotsList[slot];
            builder.AppendLine($"{slot + 1} -> {card}");
        }
        
        SlotsText.text = builder.ToString();
        builder.Clear();
        loadingSymbol.rotateSpeed = 0;
        RefreshButton.interactable = true;
        SlotsButton.interactable = true;
    }

    private void ToggleSlotsPanel()
    {
        deckPresenter.gameObject.SetActive(!deckPresenter.gameObject.activeInHierarchy);
        SlotsScrollView.gameObject.SetActive(!SlotsScrollView.gameObject.activeInHierarchy);
    }

    void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}
