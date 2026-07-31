using UnityEngine;
using System;

public class CellPhysics : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)] private float disabledAlpha = 0.3f;

    // turret처럼 spriterender가 많을 때
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Serializable]
    private class DestroyedSprite
    {
        public SpriteRenderer target;
        public Sprite destroyedSprite;
    }

    [Header("Damage Visual")]
    [SerializeField]
    [Range(0f, 0.5f)] private float damagedBrightness = 0.75f;

    [Header("Destroyes Cell Sprite")]
    [SerializeField]
    private DestroyedSprite[] destroyedSprites;
    private Color[] normalColors;
    private Sprite[] normalSprites;
    private bool[] normalEnabledStates;

    private bool isDark;
    private bool isDisabledVisual;
    
    [SerializeField] private SpriteRenderer[] hideOnDisabled;


    private void Awake()
    {
        Debug.Log("실행됨");
        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"{name} SpriteRenderer {spriteRenderers.Length}");

        normalColors = new Color[spriteRenderers.Length];
        for(int i = 0; i< spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }
            normalColors[i] = spriteRenderers[i].color;
        }

        if(destroyedSprites == null)
        {
            destroyedSprites = Array.Empty<DestroyedSprite>();
        }

        normalSprites = new Sprite[destroyedSprites.Length];
        for(int i = 0; i<destroyedSprites.Length; i++)
        {
            if (destroyedSprites[i].target == null)
            {
                continue;
            }
            normalSprites[i] = destroyedSprites[i].target.sprite;
        }

        normalEnabledStates = new bool[hideOnDisabled.Length];
        for(int i = 0; i < hideOnDisabled.Length; i++)
        {
            if (hideOnDisabled[i]== null)
            {

            continue;
            }
            normalEnabledStates[i] = hideOnDisabled[i].enabled;
        }


    }

    public void UpdateVisual(float durabilityRatio)
    {
        durabilityRatio = Mathf.Clamp01(durabilityRatio);

        
        
        if(durabilityRatio <= 0f)
        {
            SetDisabledVisual();
            return;
        }
        if (durabilityRatio < 0.5f)
        {
            DecreaseBrightness();

        }
        else
        {
            RestoreBrightness();
        }

    }

    private void DecreaseBrightness()
    {
        if (isDark || isDisabledVisual)
        {
            return;
        }
        isDark = true;
        
        for(int i = 0; i<spriteRenderers.Length; i++)
        {
            SpriteRenderer target = spriteRenderers[i];

            if(target == null)
            {
                continue;
            }

            Color normalColor = normalColors[i];

            target.color = new Color(normalColor.r * damagedBrightness, normalColor.g * damagedBrightness, normalColor.b * damagedBrightness, normalColor.a);

        }
    }


    public void SetDisabledVisual()
    {
        if (isDisabledVisual == true) { return; }
        isDisabledVisual = true;
        RestoreBrightness();

        for(int i = 0; i < destroyedSprites.Length; i++)
        {
            DestroyedSprite data = destroyedSprites[i];
            if(data.target == null || data.destroyedSprite == null)
            {
                continue;
            }
            data.target.sprite = data.destroyedSprite;

        }

        foreach(SpriteRenderer target in hideOnDisabled)
        {
            if(target != null)
            {
                target.enabled = false;
            }
        }
    }

    private void RestoreBrightness()
    {
        if(spriteRenderers == null || normalColors == null)
        {
            return;
        }

        for(int i= 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }
            spriteRenderers[i].color = normalColors[i];
        }
        isDark = false;
    }

    public void RestoreVisual()
    {
        isDisabledVisual = false;

        for(int i = 0; i < destroyedSprites.Length; i++)
        {
            if (destroyedSprites[i].target == null)
            {
                continue;
            }
            destroyedSprites[i].target.sprite = normalSprites[i];
        }

        for(int i=0; i < hideOnDisabled.Length; i++)
        {
            if (hideOnDisabled[i] == null)
            {
                continue;
            }

            hideOnDisabled[i].enabled = normalEnabledStates[i];
        }
        RestoreBrightness();

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
