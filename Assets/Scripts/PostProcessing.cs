using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    private Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromAberration;
    private void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromAberration);
    }

    private void Update()
    {
        if (vignette.intensity.value  > 0)
        {
            vignette.intensity.value -= (Time.deltaTime/2);
        }
    }

    public void BloodEffect()
    {
        vignette.intensity.value = 0.7f;
    }

    public void ChromaticEffectOn()
    {
        chromAberration.intensity.value = 1f;
    }
    public void ChromaticEffectOff()
    {
        chromAberration.intensity.value = 0f;
    }
}
