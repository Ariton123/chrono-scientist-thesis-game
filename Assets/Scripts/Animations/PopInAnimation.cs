using UnityEngine;
using System.Collections;

public class PopInAnimation : MonoBehaviour
{
    public float duration = 0.25f;
    public float startMultiplier = 0.75f;
    public float overshootMultiplier = 1.2f;

    [Header("Options")]
    [SerializeField] private bool captureScaleOnEnable = true;
    [SerializeField] private float startDelay = 0.02f;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(PlayPopAnimation());
    }

    IEnumerator PlayPopAnimation()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        if (captureScaleOnEnable)
            originalScale = transform.localScale;

        float time = 0f;

        Vector3 start = originalScale * startMultiplier;
        Vector3 overshoot = originalScale * overshootMultiplier;
        Vector3 normal = originalScale;

        transform.localScale = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            transform.localScale = Vector3.Lerp(start, overshoot, t);
            yield return null;
        }

        transform.localScale = overshoot;

        time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            transform.localScale = Vector3.Lerp(overshoot, normal, t);
            yield return null;
        }

        transform.localScale = normal;
    }
}