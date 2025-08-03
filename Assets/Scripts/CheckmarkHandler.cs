using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckmarkHandler : MonoBehaviour
{
    private Image _image;

    public void BeginDestroy()
    {
        _image = GetComponent<Image>();
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        while (_image.color.a > 0)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a - 0.05f);
            yield return new WaitForSeconds(0.1f);
        }

        DestroyImmediate(gameObject);
    }
}
