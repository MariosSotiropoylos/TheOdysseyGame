using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    public Vector2 flowSpeed = new Vector2(0.023f, 0f);

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (rend == null || rend.sharedMaterial == null)
            return;

        offset += flowSpeed * Time.deltaTime;
        rend.sharedMaterial.mainTextureOffset = offset;
    }
}