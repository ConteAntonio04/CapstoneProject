using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    private CardData cardData;
    private TurnDuelManager duelManager;
    private bool isPlayerCard;

    internal CardData CardData => cardData;

    internal void Setup(CardData data, TurnDuelManager manager, bool isPlayer)
    {
        cardData = data;
        duelManager = manager;
        isPlayerCard = isPlayer;

        cardImage.sprite = data.Sprite;
    }

    internal void SetBack(Sprite backSprite)
    {
        cardImage.sprite = backSprite;
    }

    internal void Reveal()
    {
        cardImage.sprite = cardData.Sprite;
    }

    public void OnClick()
    {
        if (isPlayerCard)
            duelManager.PlayerPlayCard(this);
    }
}
