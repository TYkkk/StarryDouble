using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using UnityEngine.UI;

public class EntityCardInfo : BaseMonoBehaviour
{
    public Color[] BgColors;

    public CardEntityData Data => entityData;

    public Text StarText;
    public Image BgImg;
    public Transform ObjectParent;

    private CardEntityData entityData;

    private GameObject cardObject;

    public void SetData(CardEntityData data)
    {
        Clear();

        entityData = data;

        if (data == null)
        {
            return;
        }

        string starContent = "";

        switch (data.Star)
        {
            case 1:
                starContent = "1星！";
                BgImg.color = BgColors[0];
                break;
            case 2:
                starContent = "2星！！";
                BgImg.color = BgColors[1];
                break;
            case 3:
                starContent = "3星！！！";
                BgImg.color = BgColors[2];
                break;
        }
        StarText.text = starContent;

        if (!string.IsNullOrEmpty(data.ConfigData.CardPrefabPath))
        {
            cardObject = Instantiate(Resources.Load<GameObject>(data.ConfigData.CardPrefabPath));
            cardObject.transform.SetParent(ObjectParent, false);
        }
    }

    private void Clear()
    {
        StarText.text = "";
        BgImg.color = BgColors[0];
        if (cardObject != null)
            Destroy(cardObject);
        entityData = null;
    }
}
