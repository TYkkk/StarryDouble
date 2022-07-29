using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData_", menuName = "CreateCardData")]
public class CardData : ScriptableObject
{
    public int Price;
    public bool CanBuy;
    public string CardPrefabPath;
    public int Level;
    public int CardID;
}


public class CardEntityData
{
    public CardData ConfigData;
    public int Star;
}