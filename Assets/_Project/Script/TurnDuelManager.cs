using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnDuelManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject cardPrefab;

    [Header("Zone")]
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform cpuHand;
    [SerializeField] private Transform playerDeckPosition;
    [SerializeField] private Transform cpuDeckPosition;
    [SerializeField] private Transform playerPlayZone;
    [SerializeField] private Transform cpuPlayZone;

    [Header("Carte")]
    [SerializeField] private CardData[] allCards;
    [SerializeField] private Sprite cardBackSprite;

    [Header("UI")]
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text cpuHealthText;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int startingCards = 3;
    [SerializeField] private float drawDelay = 0.3f;
    [SerializeField] private float moveDuration = 0.3f;

    private readonly List<CardData> playerDeck = new();
    private readonly List<CardData> cpuDeck = new();

    private readonly List<CardView> playerHandCards = new();
    private readonly List<CardView> cpuHandCards = new();

    private int playerHealth;
    private int cpuHealth;

    private bool playerTurn;

    private void Start()
    {
        playerHealth = maxHealth;
        cpuHealth = maxHealth;

        CreateDecks();
        UpdateHealthUI();

        StartCoroutine(StartGame());
    }

    private void CreateDecks()
    {
        playerDeck.Clear();
        cpuDeck.Clear();

        playerDeck.AddRange(allCards);
        cpuDeck.AddRange(allCards);

        Shuffle(playerDeck);
        Shuffle(cpuDeck);
    }

    private IEnumerator StartGame()
    {
        for (int i = 0; i < startingCards; i++)
        {
            DrawPlayerCard();
            yield return new WaitForSeconds(drawDelay);

            DrawCPUCard();
            yield return new WaitForSeconds(drawDelay);
        }

        playerTurn = true;
    }

    internal void PlayerPlayCard(CardView playerCard)
    {
        if (!playerTurn) return;

        playerTurn = false;
        StartCoroutine(ResolveTurn(playerCard));
    }

    private IEnumerator ResolveTurn(CardView playerCard)
    {
        playerHandCards.Remove(playerCard);

        CardView cpuCard = cpuHandCards[Random.Range(0, cpuHandCards.Count)];
        cpuHandCards.Remove(cpuCard);

        yield return MoveCard(playerCard.transform as RectTransform, playerPlayZone.position);
        yield return MoveCard(cpuCard.transform as RectTransform, cpuPlayZone.position);

        cpuCard.Reveal();
        cpuCard.transform.rotation = Quaternion.identity;

        ResolveCards(playerCard.CardData, cpuCard.CardData);

        UpdateHealthUI();

        yield return new WaitForSeconds(0.7f);

        Destroy(playerCard.gameObject);
        Destroy(cpuCard.gameObject);

        DrawPlayerCard();
        yield return new WaitForSeconds(drawDelay);

        DrawCPUCard();
        yield return new WaitForSeconds(drawDelay);

        playerTurn = true;
    }

    private void ResolveCards(CardData playerCard, CardData cpuCard)
    {
        bool playerFullBlock = playerCard.CardType == CardType.Special && playerCard.SpecialType == SpecialType.FullBlock;
        bool cpuFullBlock = cpuCard.CardType == CardType.Special && cpuCard.SpecialType == SpecialType.FullBlock;

        bool playerReflect = playerCard.CardType == CardType.Special && playerCard.SpecialType == SpecialType.Reflect;
        bool cpuReflect = cpuCard.CardType == CardType.Special && cpuCard.SpecialType == SpecialType.Reflect;

        bool playerBlocked = playerCard.CardType == CardType.Defense || playerFullBlock;
        bool cpuBlocked = cpuCard.CardType == CardType.Defense || cpuFullBlock;

        int playerDamage = 0;
        int cpuDamage = 0;

        if (playerCard.CardType == CardType.Attack)
            playerDamage = playerCard.Value;

        if (playerCard.CardType == CardType.Special && playerCard.SpecialType == SpecialType.HeavyAttack)
            playerDamage = playerCard.Value;

        if (cpuCard.CardType == CardType.Attack)
            cpuDamage = cpuCard.Value;

        if (cpuCard.CardType == CardType.Special && cpuCard.SpecialType == SpecialType.HeavyAttack)
            cpuDamage = cpuCard.Value;

        if (cpuReflect && playerDamage > 0)
        {
            playerHealth -= playerDamage;
            playerDamage = 0;
        }

        if (playerReflect && cpuDamage > 0)
        {
            cpuHealth -= cpuDamage;
            cpuDamage = 0;
        }

        if (!cpuBlocked)
            cpuHealth -= playerDamage;

        if (!playerBlocked)
            playerHealth -= cpuDamage;

        if (playerCard.CardType == CardType.Heal)
            playerHealth += playerCard.Value;

        if (cpuCard.CardType == CardType.Heal)
            cpuHealth += cpuCard.Value;

        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        cpuHealth = Mathf.Clamp(cpuHealth, 0, maxHealth);
    }

    private void DrawPlayerCard()
    {
        if (playerDeck.Count == 0) return;

        CardData data = playerDeck[0];
        playerDeck.RemoveAt(0);

        CardView card = CreateCard(data, playerHand, true);
        playerHandCards.Add(card);

        StartCoroutine(DrawAnimation(card.transform as RectTransform, playerDeckPosition.position));
    }

    private void DrawCPUCard()
    {
        if (cpuDeck.Count == 0) return;

        CardData data = cpuDeck[0];
        cpuDeck.RemoveAt(0);

        CardView card = CreateCard(data, cpuHand, false);
        card.SetBack(cardBackSprite);

        cpuHandCards.Add(card);

        RectTransform rect = card.transform as RectTransform;
        rect.localRotation = Quaternion.Euler(0, 0, 180);

        StartCoroutine(DrawAnimation(rect, cpuDeckPosition.position));
    }

    private CardView CreateCard(CardData data, Transform parent, bool isPlayer)
    {
        GameObject obj = Instantiate(cardPrefab, parent);
        CardView view = obj.GetComponent<CardView>();

        view.Setup(data, this, isPlayer);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 160);

        return view;
    }

    private IEnumerator DrawAnimation(RectTransform rect, Vector3 start)
    {
        yield return null;

        Vector3 target = rect.position;
        rect.position = start;

        yield return MoveCard(rect, target);
    }

    private IEnumerator MoveCard(RectTransform rect, Vector3 target)
    {
        Vector3 start = rect.position;
        float t = 0;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            rect.position = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }

        rect.position = target;
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = $"PLAYER : {playerHealth}";
        cpuHealthText.text = $"CPU : {cpuHealth}";
    }

    private void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }
}
