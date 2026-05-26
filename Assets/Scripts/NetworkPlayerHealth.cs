using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private PlayerWorldHealthBar worldHealthBar;
    private TextMeshProUGUI screenHealthLabel;
    private Slider screenHealthSlider;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        worldHealthBar = PlayerWorldHealthBar.Create(transform, maxHealth);

        if (IsOwner)
        {
            CreateScreenHealthHud();
        }

        currentHealth.OnValueChanged += OnHealthValueChanged;
        RefreshHealthUI(currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthValueChanged;
    }

    private void OnHealthValueChanged(int previousValue, int newValue)
    {
        RefreshHealthUI(newValue);

        if (newValue < previousValue)
        {
            int damageTaken = previousValue - newValue;
            DamagePopupText.Spawn(damageTaken, transform.position + Vector3.up * 2.2f);
        }

        if (newValue <= 0)
        {
            Debug.Log("Player has died.");
        }
    }

    private void RefreshHealthUI(int healthValue)
    {
        if (worldHealthBar != null)
        {
            worldHealthBar.SetHealth(healthValue, maxHealth);
        }

        if (!IsOwner)
        {
            return;
        }

        if (screenHealthLabel != null)
        {
            screenHealthLabel.text = "HP: " + healthValue + "/" + maxHealth;
        }

        if (screenHealthSlider != null)
        {
            screenHealthSlider.value = (float)healthValue / maxHealth;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsServer)
        {
            return;
        }

        currentHealth.Value -= damageAmount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);

        if (currentHealth.Value <= 0)
        {
            RespawnAtSpawnPoint();
        }
    }

    private void CreateScreenHealthHud()
    {
        GameObject hudRoot = new GameObject("ScreenHealthHUD");
        Canvas canvas = hudRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        hudRoot.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        hudRoot.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(hudRoot.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.45f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 0f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = new Vector2(20f, 20f);
        panelRect.sizeDelta = new Vector2(260f, 70f);

        GameObject labelObject = new GameObject("HealthLabel");
        labelObject.transform.SetParent(panel.transform, false);
        screenHealthLabel = labelObject.AddComponent<TextMeshProUGUI>();
        screenHealthLabel.font = TMP_Settings.defaultFontAsset;
        screenHealthLabel.text = "HP: " + maxHealth + "/" + maxHealth;
        screenHealthLabel.fontSize = 22f;
        screenHealthLabel.color = Color.white;
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0.5f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.offsetMin = new Vector2(10f, 0f);
        labelRect.offsetMax = new Vector2(-10f, -5f);

        GameObject sliderObject = new GameObject("HealthSlider");
        sliderObject.transform.SetParent(panel.transform, false);
        screenHealthSlider = sliderObject.AddComponent<Slider>();
        screenHealthSlider.minValue = 0f;
        screenHealthSlider.maxValue = 1f;
        screenHealthSlider.value = 1f;
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 0f);
        sliderRect.anchorMax = new Vector2(1f, 0.5f);
        sliderRect.offsetMin = new Vector2(15f, 10f);
        sliderRect.offsetMax = new Vector2(-15f, -5f);

        GameObject sliderBackground = new GameObject("Background");
        sliderBackground.transform.SetParent(sliderObject.transform, false);
        Image sliderBackgroundImage = sliderBackground.AddComponent<Image>();
        sliderBackgroundImage.color = new Color(0.15f, 0.15f, 0.15f);
        RectTransform sliderBackgroundRect = sliderBackground.GetComponent<RectTransform>();
        sliderBackgroundRect.anchorMin = Vector2.zero;
        sliderBackgroundRect.anchorMax = Vector2.one;
        sliderBackgroundRect.offsetMin = Vector2.zero;
        sliderBackgroundRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.85f, 0.3f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        screenHealthSlider.fillRect = fillRect;
        screenHealthSlider.targetGraphic = fillImage;
    }

    private void RespawnAtSpawnPoint()
    {
        currentHealth.Value = maxHealth;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No SpawnPoint objects found in the scene.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (characterController != null && IsOwner)
        {
            characterController.enabled = true;
        }
    }
}
