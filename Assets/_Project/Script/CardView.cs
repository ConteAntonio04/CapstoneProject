using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    private CardData cardData;
    private TurnDuelManager duelManager;
    private bool isPlayerCard;

    private AudioSource hoverAudio;
    private AudioSource clickAudio;

    public CardData CardData => cardData;

    public void Setup(CardData data, TurnDuelManager manager, bool playerCard, AudioSource hover, AudioSource click)
    {
        cardData = data;
        duelManager = manager;
        isPlayerCard = playerCard;

        hoverAudio = hover;
        clickAudio = click;

        cardImage.sprite = data.Sprite;

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData evenrData)
    {
        if (!isPlayerCard) return;

        if (hoverAudio != null) hoverAudio.Play();
    }

    private void Onclick()
    {
        if (!isPlayerCard) return;

        if (clickAudio != null) clickAudio.Play();

        duelManager.PlayerPlayCard(this);
    }

    public void SetBack(Sprite backSprite)
    {
        cardImage.sprite = backSprite;
    }

    public void Reveal()
    {
        cardImage.sprite = cardData.Sprite;
    }

    public void OnClick()
    {
        if (isPlayerCard)
            duelManager.PlayerPlayCard(this);
    }
}

