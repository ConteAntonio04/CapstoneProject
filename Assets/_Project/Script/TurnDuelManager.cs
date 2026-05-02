using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource hoverAudio;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private AudioSource loseAudio;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private int startingCards = 3;
    [SerializeField] private float drawDelay = 0.6f;
    [SerializeField] private float moveDuration = 0.6f;

    private readonly List<CardView> playerHandCards = new();
    private readonly List<CardView> cpuHandCards = new();

    private int playerHealth;
    private int cpuHealth;
    private bool playerTurn;

    private void Start()
    {
        playerHealth = maxHealth;
        cpuHealth = maxHealth;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        UpdateHealthUI();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        playerTurn = false;

        for (int i = 0; i < startingCards; i++)
        {
            DrawPlayerCard();
            yield return new WaitForSeconds(drawDelay);

            DrawCPUCard();
            yield return new WaitForSeconds(drawDelay);
        }

        playerTurn = true;
    }

    public void PlayerPlayCard(CardView playerCard)
    {
        if (!playerTurn) return;
        if (playerCard == null) return;
        if (!playerHandCards.Contains(playerCard)) return;

        playerTurn = false;
        StartCoroutine(ResolveTurn(playerCard));
    }

    private IEnumerator ResolveTurn(CardView playerCard)
    {
        playerHandCards.Remove(playerCard);

        if (cpuHandCards.Count == 0)
        {
            DrawCPUCard();
            yield return new WaitForSeconds(drawDelay);
        }

        CardView cpuCard = cpuHandCards[Random.Range(0, cpuHandCards.Count)];

        if (playerCard == null || cpuCard == null)
        {
            playerTurn = true;
            yield break;
        }

        cpuHandCards.Remove(cpuCard);

        yield return MoveCard(playerCard.GetComponent<RectTransform>(), playerPlayZone.position);
        yield return MoveCard(cpuCard.GetComponent<RectTransform>(), cpuPlayZone.position);

        cpuCard.Reveal();

        RectTransform cpuRect = cpuCard.GetComponent<RectTransform>();
        cpuRect.localRotation = Quaternion.identity;

        ResolveCards(playerCard.CardData, cpuCard.CardData);
        UpdateHealthUI();

        if (IsGameOver())
        {
            EndGame();
            yield break;
        }

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
        int playerDamage = GetDamage(playerCard);
        int cpuDamage = GetDamage(cpuCard);

        int playerBlock = GetBlock(playerCard);
        int cpuBlock = GetBlock(cpuCard);

        bool playerFullBlock = IsFullBlock(playerCard);
        bool cpuFullBlock = IsFullBlock(cpuCard);

        bool playerReflect = IsReflect(playerCard);
        bool cpuReflect = IsReflect(cpuCard);

        if (cpuFullBlock)
            playerDamage = 0;
        else
            playerDamage = Mathf.Max(0, playerDamage - cpuBlock);

        if (playerFullBlock)
            cpuDamage = 0;
        else
            cpuDamage = Mathf.Max(0, cpuDamage - playerBlock);

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

        cpuHealth -= playerDamage;
        playerHealth -= cpuDamage;

        if (playerCard.CardType == CardType.Heal)
            playerHealth += playerCard.Value;

        if (cpuCard.CardType == CardType.Heal)
            cpuHealth += cpuCard.Value;

        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        cpuHealth = Mathf.Clamp(cpuHealth, 0, maxHealth);
    }

    private int GetDamage(CardData card)
    {
        if (card.CardType == CardType.Attack)
            return card.Value;

        if (card.CardType == CardType.Special &&
        card.SpecialType == SpecialType.HeavyAttack)
            return card.Value;

        return 0;
    }

    private int GetBlock(CardData card)
    {
        if (card.CardType == CardType.Defense)
            return card.Value;

        return 0;
    }

    private bool IsFullBlock(CardData card)
    {
        return card.CardType == CardType.Special &&
        card.SpecialType == SpecialType.FullBlock;
    }

    private bool IsReflect(CardData card)
    {
        return card.CardType == CardType.Special &&
        card.SpecialType == SpecialType.Reflect;
    }

    private void DrawPlayerCard()
    {
        CardData data = GetRandomCard();
        if (data == null) return;

        CardView card = CreateCard(data, playerHand, true);
        playerHandCards.Add(card);

        StartCoroutine(DrawAnimation(card.GetComponent<RectTransform>(), playerDeckPosition.position));
    }

    private void DrawCPUCard()
    {
        CardData data = GetRandomCard();
        if (data == null) return;

        CardView card = CreateCard(data, cpuHand, false);
        card.SetBack(cardBackSprite);
        cpuHandCards.Add(card);

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.localRotation = Quaternion.Euler(0f, 0f, 180f);

        StartCoroutine(DrawAnimation(rect, cpuDeckPosition.position));
    }

    private CardData GetRandomCard()
    {
        if (allCards == null || allCards.Length == 0)
        {
            Debug.LogError("All Cards è vuoto.");
            return null;
        }

        CardData card = allCards[Random.Range(0, allCards.Length)];

        if (card == null)
        {
            Debug.LogError("Carta nulla trovata in All Cards.");
            return null;
        }

        return card;
    }

    private CardView CreateCard(CardData data, Transform parent, bool isPlayer)
    {
        GameObject obj = Instantiate(cardPrefab, parent);
        CardView view = obj.GetComponent<CardView>();

        view.Setup(data, this, isPlayer, hoverAudio, clickAudio);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 160);

        return view;
    }

    private IEnumerator DrawAnimation(RectTransform rect, Vector3 startPosition)
    {
        yield return null;

        Vector3 target = rect.position;
        rect.position = startPosition;

        yield return MoveCard(rect, target);
    }

    private IEnumerator MoveCard(RectTransform rect, Vector3 targetPosition)
    {
        Vector3 startPosition = rect.position;
        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            rect.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rect.position = targetPosition;
    }

    private bool IsGameOver()
    {
        return playerHealth <= 0 || cpuHealth <= 0;
    }

    private void EndGame()
    {
        playerTurn = false;
        DisablePlayerCards();

        if (playerHealth > cpuHealth)
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(true);

            if (winAudio != null)
                winAudio.Play();
        }
        else
        {
            if (defeatPanel != null)
                defeatPanel.SetActive(true);

            if (loseAudio  != null)
                loseAudio.Play();
        }
    }

    private void DisablePlayerCards()
    {
        foreach (CardView card in playerHandCards)
        {
            if (card == null) continue;

            Button button = card.GetComponent<Button>();
            if (button != null)
                button.interactable = false;
        }
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = $"PLAYER : {playerHealth}/{maxHealth}";
        cpuHealthText.text = $"CPU: {cpuHealth}/{maxHealth}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Debug.Log("Esci dal gioco");

        Application.Quit();
    }
}
