using UnityEngine;

public class CellPhysics : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)] private float disabledAlpha = 0.3f;

    // turret처럼 spriterender가 많을 때
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    

    private void Awake()
    {
        Debug.Log("실행됨");
        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"{name} SpriteRenderer {spriteRenderers.Length}");
        
    }

    public void SetDisabledVisual()
    {
        SetAlpha(disabledAlpha);
    }
    public void RestoreVisual()
    {
        SetAlpha(1f);

    }
    private void SetAlpha(float alpha)
    {
        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
