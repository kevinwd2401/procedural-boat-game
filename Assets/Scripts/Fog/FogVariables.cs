using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogVariables : MonoBehaviour
{
    public Material FullScreenMat;
    public Transform FogBox;
    public Texture3D noise3D;
    private Vector3 savedScale;

    public float renderTexWorldSize;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        savedScale = FogBox.lossyScale;
        FullScreenMat.SetVector("_BoundsMin", FogBox.position - savedScale / 2);
        FullScreenMat.SetVector("_BoundsMax", FogBox.position + savedScale / 2);
        FullScreenMat.SetTexture("_NoiseTexture", noise3D);

        FullScreenMat.SetFloat("_TexWorldSize", renderTexWorldSize);

        StartCoroutine(UpdateOffset());
        timer = 0;
    }

    private IEnumerator UpdateOffset() {
        while (true) {
            yield return null;
            FullScreenMat.SetVector("_BoundsMin", FogBox.position - savedScale / 2);
            FullScreenMat.SetVector("_BoundsMax", FogBox.position + savedScale / 2);
            FullScreenMat.SetVector("_TexWorldCenter", transform.position);
            yield return null;
            FullScreenMat.SetFloat("_FogMultiplier", SmoothNoise1D(timer));
        }
    }

    void Update() {
        timer += Time.deltaTime * 10f;
    }

    float SmoothNoise1D(float t)
    {
        float n =
            0.6f * Mathf.Sin(t * 0.07f) +
            0.3f * Mathf.Sin(t * 0.19f + 1.3f) +
            0.1f * Mathf.Sin(t * 0.41f + 6.2f);

        return Mathf.Lerp(0.5f, 1.2f, (n + 1.2f) * 0.5f);
    }

    void OnDestroy() {
        
    }
}
