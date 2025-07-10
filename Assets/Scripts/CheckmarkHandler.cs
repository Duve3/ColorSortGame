using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckmarkHandler : MonoBehaviour
{
    private Image image;

    public void BeginDestroy()
    {
        image = GetComponent<Image>();
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 0.05f);
            yield return new WaitForSeconds(0.1f);
        }

        DestroyImmediate(gameObject);
    }
}
