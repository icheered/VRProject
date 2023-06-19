using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMilkCarton : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string[] blendShapeNames = new string[] { "Height", "Width", "Thickness", "Smoothness", "Warped", "Bulge" };

    private float[] frequencies = new float[] { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f };
    private float[] phases = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        for (int i = 0; i < blendShapeNames.Length; i++)
        {
            float blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeNames[i]);

            float value = 50f + 50f * Mathf.Sin(frequencies[i] * Time.time + phases[i]);
            skinnedMeshRenderer.SetBlendShapeWeight((int)blendShapeIndex, value);

        }
    }
}
