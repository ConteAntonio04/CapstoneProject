using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class CardData
{
    [SerializeField] private string cardName;
    [SerializeField] private CardType cardType;
    [SerializeField] private SpecialType specialType;
    [SerializeField] private int value;
    [SerializeField] private Sprite sprite;

    public string CardName => cardName;
    public CardType CardType => cardType;
    public SpecialType SpecialType => specialType;
    public int Value => value;
    public Sprite Sprite => sprite;
}
