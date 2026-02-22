using UnityEngine;

namespace Minor
{
    [ExecuteAlways]
    public class FullscreenSprite : MonoBehaviour
    {
        private void Awake()
        {
            var rt = GetComponent<RectTransform>();

            // Set anchors to stretch in both directions
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}