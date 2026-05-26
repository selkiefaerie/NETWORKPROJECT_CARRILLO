using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small health bar floating above the player — visible to everyone.
/// </summary>
public class PlayerWorldHealthBar : MonoBehaviour
{
    private Image fillImage;
    private TMP_Text healthText;

    public static PlayerWorldHealthBar Create(Transform playerTransform, int maxHealth)
    {
        GameObject root = new GameObject("WorldHealthBar");
        root.transform.SetParent(playerTransform);
        root.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform canvasRect = root.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 0.35f);
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        GameObject background = new GameObject("Background");
        background.transform.SetParent(root.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.65f);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(background.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.9f, 0.25f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject labelObject = new GameObject("HealthText");
        labelObject.transform.SetParent(root.transform, false);
        TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.text = maxHealth + "/" + maxHealth;
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        PlayerWorldHealthBar bar = root.AddComponent<PlayerWorldHealthBar>();
        bar.fillImage = fillImage;
        bar.healthText = label;
        return bar;
    }

    public void SetHealth(int current, int max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
        }

        if (healthText != null)
        {
            healthText.text = current + "/" + max;
        }
    }

    private void LateUpdate()
    {
        if (Camera.main == null)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(
            transform.position - Camera.main.transform.position,
            Vector3.up);
    }
}
