using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDrawManager : MonoBehaviour
{
    [Header("Prefab carta UI")]
    [SerializeField] private GameObject cardPrefab;

    [Header("Zone")]
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform cpuHand;
    [SerializeField] private Transform playerDeckPosition;
    [SerializeField] private Transform cpuDeckPosition;

    [Header("Sprite carte")]
    [SerializeField] private Sprite[] allCardSprites; // 12 carte
    [SerializeField] private Sprite cardBackSprite;

    [Header("Pesca")]
    [SerializeField] private int startingCards = 3;
    [SerializeField] private float drawDelay = 0.35f;
    [SerializeField] private float moveDuration = 0.35f;

    private List<Sprite> playerDeck = new();
    private List<Sprite> cpuDeck = new();

    private void Start()
    {
        CreateDecks();
        StartCoroutine(DrawStartingHands());
    }

    private void CreateDecks()
    {
        playerDeck = new List<Sprite>(allCardSprites);
        cpuDeck = new List<Sprite>(allCardSprites);

        Shuffle(playerDeck);
        Shuffle(cpuDeck);
    }

    private IEnumerator DrawStartingHands()
    {
        for (int i = 0; i < startingCards; i++)
        {
            DrawPlayerCard();
            yield return new WaitForSeconds(drawDelay);

            DrawCPUCard();
            yield return new WaitForSeconds(drawDelay);
        }
    }

    private void DrawPlayerCard()
    {
        if (playerDeck.Count == 0) return;

        Sprite drawnCard = playerDeck[0];
        playerDeck.RemoveAt(0);

        GameObject card = Instantiate(cardPrefab, playerHand);
        Image image = card.GetComponent<Image>();
        image.sprite = drawnCard;

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 160);

        StartCoroutine(DrawCardAnimation(rect, playerDeckPosition.position));
    }

    private void DrawCPUCard()
    {
        if (cpuDeck.Count == 0) return;

        Sprite drawnCard = cpuDeck[0];
        cpuDeck.RemoveAt(0);

        GameObject card = Instantiate(cardPrefab, cpuHand);
        Image image = card.GetComponent<Image>();

        // Mostriamo solo il retro
        image.sprite = cardBackSprite;

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 160);

        rect.localRotation = Quaternion.Euler(0f, 0f, 180f);

        StartCoroutine(DrawCardAnimation(rect, cpuDeckPosition.position));
    }

    private IEnumerator MoveCard(RectTransform card, Vector3 target)
    {
        Vector3 start = card.position;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;
            card.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        card.position = target;
    }

    private IEnumerator DrawCardAnimation(RectTransform rect, Vector3 startPosition)
    {
        yield return null;  //aspetta che il layout aggiorni la posizione finale

        Vector3 target = rect.position;
        rect.position = startPosition;

        yield return MoveCard(rect, target);
    }

    private void Shuffle(List<Sprite> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }
}
