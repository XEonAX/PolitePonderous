using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Explosion : MonoBehaviour
{

    [SerializeField, Range(0f, 1f)]
    float duration = 0.5f;

    float age;
    [SerializeField]
    AnimationCurve opacityCurve = default;
    [SerializeField]
    AnimationCurve emissionCurve = default;

    [SerializeField]
    AnimationCurve scaleCurve = default;


    [SerializeField]
    public Color StartColor = default;

    [SerializeField]
    public Color EndColor = default;

    static int emissionPropertyID = Shader.PropertyToID("_EmissionColor");
    static int basecolorPropertyID = Shader.PropertyToID("_BaseColor");

    static MaterialPropertyBlock propertyBlock;
    float scale;

    MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Debug.Assert(meshRenderer != null, "Explosion without renderer!");
    }
    public void Initialize(Vector3 position, float blastRadius)
    {
        age = 0;
        transform.localPosition = position;
        scale = 2f * blastRadius;
    }

    public void Update()
    {
        age += Time.deltaTime;
        if (age >= duration)
        {
            ObjectPool.Recycle(gameObject);
        }
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        float t = age / duration;
        Color c = Color.Lerp(StartColor, EndColor, t);
        // // Color c = Color.clear;
        c.a = emissionCurve.Evaluate(t);
        propertyBlock.SetColor(emissionPropertyID, c * emissionCurve.Evaluate(t));
        c.a = opacityCurve.Evaluate(t);
        propertyBlock.SetColor(basecolorPropertyID, c);
        meshRenderer.SetPropertyBlock(propertyBlock);

        transform.localScale = Vector3.one * (scale * scaleCurve.Evaluate(t));
    }


}