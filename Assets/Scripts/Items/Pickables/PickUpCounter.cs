using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickUpCounter : MonoBehaviour
{
    public SpriteRenderer sR;
    public Transform tF;
    public TMP_Text text;

    private void Awake()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(MoveUp());
    }

    private IEnumerator FadeOut()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            sR.color = new Color(1, 1, 1, sR.color.a - 0.05f);
            text.color = new Color(1, 1, 1, text.color.a - 0.05f);
        }
    }

    private IEnumerator MoveUp()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            tF.position += Vector3.up * 0.01f;
        }
    }
}
