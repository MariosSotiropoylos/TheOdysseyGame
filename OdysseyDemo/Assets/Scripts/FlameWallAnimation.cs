using UnityEngine;

public class FlameAnimator : MonoBehaviour
{
    public float fps = 9f;

    private Material mat;
    private int frame;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        if (rend == null)
            return;

        mat = rend.sharedMaterial;

        if (mat != null)
            mat.mainTextureScale = new Vector2(1f / 6f, 1f);
    }

    void Update()
    {
        if (mat == null)
            return;

        frame = (int)(Time.time * fps) % 6;
        mat.mainTextureOffset = new Vector2(frame / 6f, 0f);
    }
}