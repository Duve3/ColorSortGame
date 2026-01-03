using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupToast : MonoBehaviour
{
    // this object is not well extendable, as it manually assumes that there is only one child
    private GameObject m_toast;
    private TMP_Text m_toastText;

    private Image m_toastImage;

    private void Awake()
    {
        m_toast = transform.gameObject;
        m_toastText = m_toast.GetComponentInChildren<TMP_Text>();
        m_toastImage = m_toast.GetComponent<Image>();

        // Set alpha to 0 (invisible) instead of SetActive(false)
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        var toastColor = m_toastImage.color;
        toastColor.a = alpha;
        m_toastImage.color = toastColor;

        var toastTextColor = m_toastText.color;
        toastTextColor.a = alpha;
        m_toastText.color = toastTextColor;
    }

    public IEnumerator ShowToast(string message, float duration)
    {
        m_toastText.text = message;
        m_toastText.enableAutoSizing = true;

        // Set alpha to 1 (visible) instead of SetActive(true)
        SetAlpha(1f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease-in cubic: starts slow, accelerates at the end
            float alpha = 1f - (t * t * t);

            SetAlpha(alpha);

            yield return null;
        }

        // Set alpha to 0 (invisible) instead of SetActive(false)
        SetAlpha(0f);
    }
}
