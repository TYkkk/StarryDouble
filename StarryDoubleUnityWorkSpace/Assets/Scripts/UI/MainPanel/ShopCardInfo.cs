using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using UnityEngine.UI;

public class ShopCardInfo : BaseMonoBehaviour
{
    public int Price;
    public bool CanBuy;
    public Button ClickBtn;

    public CardData Data => data;
    private CardData data;
    private GameObject cardObject;

    private void Awake()
    {
        ClickBtn.onClick.AddListener(ClickBtnEvent);
    }

    private void OnDestroy()
    {
        ClickBtn.onClick.RemoveListener(ClickBtnEvent);
    }

    private void ClickBtnEvent()
    {
        if (data == null)
        {
            return;
        }
        EventManager.Fire(ConstEvent.ShopCardClickedEvent, new EventData<ShopCardInfo>(this));
    }

    public void SetData(CardData data)
    {
        if (data == null)
        {
            return;
        }

        this.data = data;
        Price = data.Price;
        CanBuy = data.CanBuy;

        if (!string.IsNullOrEmpty(data.CardPrefabPath))
        {
            cardObject = Instantiate(Resources.Load<GameObject>(data.CardPrefabPath));
            cardObject.transform.SetParent(transform, false);
        }
    }

    public void Clear()
    {
        data = null;
        Price = 0;
        CanBuy = false;
        Destroy(cardObject);
    }
}
