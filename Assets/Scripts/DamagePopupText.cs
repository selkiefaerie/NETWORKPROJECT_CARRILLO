using TMPro;
using UnityEngine;

public class DamagePopupText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float lifetime = 0.9f;

    private TMP_Text damageLabel;
    private float destroyTime;

    public static void Spawn(int damageAmount, Vector3 worldPosition)
    {
        GameObject popupObject = new GameObject("DamagePopup");
        popupObject.transform.position = worldPosition;

        TMP_Text text = popupObject.AddComponent<TextMeshPro>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = damageAmount.ToString();
        text.fontSize = 5f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 0.35f, 0.35f);

        DamagePopupText popup = popupObject.AddComponent<DamagePopupText>();
        popup.damageLabel = text;
        popup.destroyTime = Time.time + popup.lifetime;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(
                transform.position - Camera.main.transform.position,
                Vector3.up);
        }

        if (Time.time >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
