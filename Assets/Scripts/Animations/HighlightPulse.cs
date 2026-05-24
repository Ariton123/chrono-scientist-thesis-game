using UnityEngine;

public class HighlightPulse : MonoBehaviour
{
    [Header("Pulse Scale")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float scaleAmount = 0.08f;

    [Header("Pulse Alpha")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float minAlpha = 0.85f;
    [SerializeField] private float maxAlpha = 1f;

    private Vector3 originalScale;
    private bool isActive = true;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isActive)
            return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        float scale = 1f + Mathf.Lerp(-scaleAmount, scaleAmount, pulse);
        transform.localScale = originalScale * scale;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, pulse);
            spriteRenderer.color = color;
        }
    }

    public void StartPulse()
    {
        isActive = true;
    }

    public void StopPulse()
    {
        isActive = false;
        transform.localScale = originalScale;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }
}