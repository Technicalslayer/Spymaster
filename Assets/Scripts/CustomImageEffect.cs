using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{
    public Material EffectMaterial;
    [Range(0f, 1f)]
    public float effectMagnitude;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        EffectMaterial.SetFloat(Shader.PropertyToID("_Magnitude"), effectMagnitude);
        Graphics.Blit(source, destination, EffectMaterial);
    }
}
