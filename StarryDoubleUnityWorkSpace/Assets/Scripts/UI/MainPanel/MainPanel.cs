using BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainPanel : UIBase
{
    public Button ReStartBtn_7;
    public Button ReStartBtn_8;

    public Text ProbabilityText;
    public Text GoldText;

    public ShopCardInfo[] ShopCardInfos;
    public Transform CenterRoot;

    public Text RefreshIndexText;
    public Text InfoText;

    struct ProbabilityData
    {
        public int Level_1;
        public int Level_2;
        public int Level_3;
        public int Level_4;
        public int Level_5;

        private int[] arr;

        public int[] ArrInterval => arrInterval;
        private int[] arrInterval;

        public ProbabilityData(int level_1, int level_2, int level_3, int level_4, int level_5)
        {
            Level_1 = level_1;
            Level_2 = level_2;
            Level_3 = level_3;
            Level_4 = level_4;
            Level_5 = level_5;

            arr = new int[5] { level_1, level_2, level_3, level_4, level_5 };

            arrInterval = new int[5];
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == 0)
                {
                    arrInterval[i] = arr[i];
                }
                else
                {
                    arrInterval[i] = arrInterval[i - 1] + arr[i];
                }
            }
        }
    }

    private ProbabilityData probability_7 = new ProbabilityData(19, 30, 35, 15, 1);
    private ProbabilityData probability_8 = new ProbabilityData(16, 20, 35, 25, 4);

    private const int level_3CardType = 13;
    private const int level_5CardType = 8;
    private const int level_3CardAloneMaxNum = 18;
    private const int level_5CardAloneMaxNum = 10;

    private const int initGoldNum = 100;
    private const int refreshCost = 2;

    private Dictionary<int, CardData> configCardDatas;

    private Dictionary<int, Dictionary<int, int>> cardDataPool;

    private List<EntityCardInfo> centerAreaCardEntityDatas;

    private List<int> centerAreaMaxStarCardIDs;

    private int currentGoldNum = 0;

    private bool gameInit = false;

    private ProbabilityData effectiveProbability;

    private int refreshIndex = 0;

    private int refreshCount = 0;

    public override void Awake()
    {
        base.Awake();

        ReStartBtn_7.onClick.AddListener(ReStartBtn_7Clicked);
        ReStartBtn_8.onClick.AddListener(ReStartBtn_8Clicked);
    }

    public override void OnDestroy()
    {
        ReStartBtn_7.onClick.RemoveListener(ReStartBtn_7Clicked);
        ReStartBtn_8.onClick.RemoveListener(ReStartBtn_8Clicked);

        base.OnDestroy();
    }

    public override void Register()
    {
        base.Register();

        EventManager.Register(ConstEvent.ShopCardClickedEvent, ShopCardClickedEventHandler);
    }

    public override void UnRegister()
    {
        EventManager.UnRegister(ConstEvent.ShopCardClickedEvent, ShopCardClickedEventHandler);

        base.UnRegister();
    }

    public override void Open()
    {
        base.Open();

    }

    public override bool RegisterUpdate => true;

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (Input.GetKeyDown(KeyCode.D))
        {
            RefreshBtnEvent();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckSellCard();
        }
    }

    private void RefreshBtnEvent()
    {
        if (!gameInit)
        {
            return;
        }

        if (!CheckGoldEnough(refreshCost))
        {
            return;
        }

        GoldChange(-refreshCost);

        ReturnStandbyAreaCardToPool();

        if (refreshIndex != 4)
        {
            NormalRefresh();
        }
        else
        {
            SpecialRefresh();
        }

        refreshCount += 1;
        InfoText.text = $"已刷新{refreshCount}次";
    }

    private void NormalRefresh()
    {
        var levels = GetRefreshCardLevelDatas(ShopCardInfos.Length);

        for (int i = 0; i < ShopCardInfos.Length; i++)
        {
            var data = GetRandomCardByLevel(levels[i]);
            cardDataPool[levels[i]][data.CardID]--;
            ShopCardInfos[i].SetData(data);
        }

        ChangeRefreshIndex(refreshIndex + 1);
    }

    private void SpecialRefresh()
    {
        if (centerAreaMaxStarCardIDs.Contains(CardDataConst.Target_Level_3_ID))
        {
            NormalRefresh();
        }
        else
        {
            int speCount = Random.Range(3, ShopCardInfos.Length + 1);
            int otherCount = ShopCardInfos.Length - speCount;
            var levels = GetRefreshCardLevelDatas(otherCount);
            List<CardData> result = new List<CardData>();
            for (int i = 0; i < otherCount; i++)
            {
                var data = GetRandomCardByLevel(levels[i]);
                cardDataPool[levels[i]][data.CardID]--;
                result.Add(data);
            }

            for (int i = 0; i < speCount; i++)
            {
                var data = configCardDatas[CardDataConst.Target_Level_3_ID];
                cardDataPool[data.Level][data.CardID]--;
                result.Add(data);
            }

            for (int i = 0; i < ShopCardInfos.Length; i++)
            {
                ShopCardInfos[i].SetData(result[i]);
            }
        }

        ChangeRefreshIndex(0);
    }

    private CardData GetRandomCardByLevel(int level)
    {
        Dictionary<int, int> pool = cardDataPool[level];

        List<int> canGetCardIDs = new List<int>();

        foreach (var child in pool.Keys)
        {
            if (!centerAreaMaxStarCardIDs.Contains(child))
            {
                canGetCardIDs.Add(child);
            }
        }

        int[] weight = new int[canGetCardIDs.Count];
        for (int i = 0; i < weight.Length; i++)
        {
            if (i == 0)
            {
                weight[i] = pool[canGetCardIDs[i]];
            }
            else
            {
                weight[i] = weight[i - 1] + pool[canGetCardIDs[i]];
            }
        }

        var index = GetRandomItem(weight);
        int getCardID = canGetCardIDs[index];
        return configCardDatas[getCardID];
    }

    private void ReStartBtn_7Clicked()
    {
        UpdateProbabilityData(probability_7);

        InitGame();
    }

    private void ReStartBtn_8Clicked()
    {
        UpdateProbabilityData(probability_8);

        InitGame();
    }

    private void UpdateProbabilityData(ProbabilityData target)
    {
        effectiveProbability = target;
        ProbabilityText.text = $"<Color=#C5C5C5>1:{effectiveProbability.Level_1}%</Color>  <Color=#3DD441>2:{effectiveProbability.Level_2}%</Color>  <Color=#3DAED4>3:{effectiveProbability.Level_3}%</Color>  <Color=#C53DD4>4:{effectiveProbability.Level_4}%</Color>  <Color=#D49D3D>5:{effectiveProbability.Level_5}%</Color>";
    }

    private void ClearStandbyArea()
    {
        if (ShopCardInfos == null)
        {
            return;
        }

        foreach (var child in ShopCardInfos)
        {
            if (child != null)
            {
                child.Clear();
            }
        }
    }

    private void ReturnStandbyAreaCardToPool()
    {
        if (ShopCardInfos == null)
        {
            return;
        }

        foreach (var child in ShopCardInfos)
        {
            if (child.Data != null)
            {
                cardDataPool[child.Data.Level][child.Data.CardID]++;
            }
        }

        ClearStandbyArea();
    }

    private void ClearCenterArea()
    {
        if (centerAreaCardEntityDatas == null)
        {
            centerAreaCardEntityDatas = new List<EntityCardInfo>();
        }

        if (centerAreaMaxStarCardIDs == null)
        {
            centerAreaMaxStarCardIDs = new List<int>();
        }

        for (int i = 0; i < centerAreaCardEntityDatas.Count; i++)
        {
            Destroy(centerAreaCardEntityDatas[i].gameObject);
        }

        centerAreaCardEntityDatas.Clear();
        centerAreaMaxStarCardIDs.Clear();
    }

    private void UpdateGoldData()
    {
        GoldText.text = $"金币\n{currentGoldNum}";
    }

    private bool CheckGoldEnough(int costNum)
    {
        return (currentGoldNum - costNum) >= 0;
    }

    private void GoldChange(int costNum)
    {
        currentGoldNum += costNum;
        UpdateGoldData();
    }

    private int[] GetRefreshCardLevelDatas(int length)
    {
        if (!gameInit)
        {
            return null;
        }

        int[] result = new int[length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = GetRandomItem(effectiveProbability.ArrInterval) + 1;
        }

        return result;
    }

    private int GetRandomItem(int[] weight)
    {
        if (weight == null)
        {
            return -1;
        }

        int result = Random.Range(1, weight[weight.Length - 1] + 1);
        for (int j = 0; j < weight.Length; j++)
        {
            int min;
            if (j == 0)
            {
                min = 0;
            }
            else
            {
                min = weight[j - 1];
            }

            if (result > min && result <= weight[j])
            {
                return j;
            }
        }

        return -1;
    }

    private void InitCardPool()
    {
        cardDataPool = new Dictionary<int, Dictionary<int, int>>();
        cardDataPool.Add(1, new Dictionary<int, int>() { { CardDataConst.Normal_Level_3_ID, 100 } });
        cardDataPool.Add(2, new Dictionary<int, int>() { { CardDataConst.Normal_Level_3_ID, 100 } });
        cardDataPool.Add(4, new Dictionary<int, int>() { { CardDataConst.Normal_Level_3_ID, 100 } });
        cardDataPool.Add(3, new Dictionary<int, int>() { { CardDataConst.Normal_Level_3_ID, (level_3CardType - 1) * level_3CardAloneMaxNum },
                                                         { CardDataConst.Target_Level_3_ID,level_3CardAloneMaxNum} });
        cardDataPool.Add(5, new Dictionary<int, int>() { { CardDataConst.Normal_Level_5_ID, (level_5CardType - 1) * level_5CardAloneMaxNum } ,
                                                         { CardDataConst.Target_Level_5_ID,level_5CardAloneMaxNum} });
    }

    private void InitConfigCardData()
    {
        configCardDatas = new Dictionary<int, CardData>();
        var datas = Resources.LoadAll<CardData>("ScriptableObjects/CardData");
        foreach (var child in datas)
        {
            configCardDatas.Add(child.CardID, child);
        }
    }

    private void InitGame()
    {
        InitConfigCardData();

        currentGoldNum = initGoldNum;

        ClearStandbyArea();

        ClearCenterArea();

        UpdateGoldData();

        InitCardPool();

        ChangeRefreshIndex(0);

        refreshCount = 0;
        InfoText.text = "";

        gameInit = true;
    }

    private void ChangeRefreshIndex(int index)
    {
        refreshIndex = index;
        RefreshIndexText.text = $"星界龙刷新：{index}/4";
    }

    private void ShopCardClickedEventHandler(IEventData data)
    {
        var p = data as EventData<ShopCardInfo>;

        if (!CheckGoldEnough(p.Data.Price))
        {
            return;
        }

        GoldChange(-p.Data.Price);

        CardEntityData cardEntityData = new CardEntityData();
        cardEntityData.ConfigData = p.Data.Data;
        cardEntityData.Star = 1;

        var result = CheckContainLevelUp(cardEntityData);
        CreateEntityCardInfo(result);

        p.Data.Clear();
    }

    private CardEntityData CheckContainLevelUp(CardEntityData target)
    {
        List<EntityCardInfo> cache = new List<EntityCardInfo>();

        foreach (var child in centerAreaCardEntityDatas)
        {
            if (child.Data.ConfigData == target.ConfigData && child.Data.Star == target.Star)
            {
                cache.Add(child);
            }
        }

        if (cache.Count == 2)
        {
            for (int i = 0; i < cache.Count; i++)
            {
                centerAreaCardEntityDatas.Remove(cache[i]);
                Destroy(cache[i].gameObject);
            }

            CardEntityData newCard = new CardEntityData();
            newCard.ConfigData = target.ConfigData;
            newCard.Star = target.Star + 1;

            if (newCard.Star == 3)
            {
                centerAreaMaxStarCardIDs.Add(newCard.ConfigData.CardID);

                CardEntityData cardEntityData = new CardEntityData();
                cardEntityData.ConfigData = newCard.ConfigData;
                cardEntityData.Star = 2;
                CreateEntityCardInfo(cardEntityData);

                return newCard;
            }

            return CheckContainLevelUp(newCard);
        }
        else
        {
            return target;
        }
    }

    private void CreateEntityCardInfo(CardEntityData data)
    {
        GameObject entity = Instantiate(Resources.Load<GameObject>("UI/Card/CenterCard"));
        var result = entity.GetComponent<EntityCardInfo>();
        result.SetData(data);
        entity.transform.SetParent(CenterRoot, false);

        centerAreaCardEntityDatas.Add(result);
    }

    private List<RaycastResult> RaycastUIList()
    {
        if (EventSystem.current == null)
        {
            return null;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults;
    }

    private void CheckSellCard()
    {
        if (!gameInit)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var result = RaycastUIList();
            if (result == null || result.Count == 0 || result[0].gameObject.GetComponent<CenterCardSellArea>() == null)
            {
                return;
            }

            SellCard(result[0].gameObject.GetComponent<CenterCardSellArea>().Target);
        }
    }

    private void SellCard(EntityCardInfo info)
    {
        if (info.Data.Star == 3)
        {
            centerAreaMaxStarCardIDs.Remove(info.Data.ConfigData.CardID);
        }

        centerAreaCardEntityDatas.Remove(info);

        int s = info.Data.Star;

        GoldChange((2 * s * s + (-4) * s + 3) * info.Data.ConfigData.Price - 1);

        cardDataPool[info.Data.ConfigData.Level][info.Data.ConfigData.CardID] = Mathf.Clamp(cardDataPool[info.Data.ConfigData.Level][info.Data.ConfigData.CardID] + (2 * s * s + (-4) * s + 3), 0, level_3CardAloneMaxNum);

        Destroy(info.gameObject);
    }
}