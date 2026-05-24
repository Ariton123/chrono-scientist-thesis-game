using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterTextUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TMP_Text targetText;

    [Header("Typing Settings")]
    [SerializeField] private float charactersPerSecond = 35f;
    [SerializeField] private bool useUnscaledTime = true;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();
    }

    public void Play(string fullText)
    {
        if (targetText == null)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        targetText.text = "";

        if (string.IsNullOrEmpty(fullText))
        {
            typingCoroutine = null;
            yield break;
        }

        float delay = charactersPerSecond > 0f ? 1f / charactersPerSecond : 0f;

        for (int i = 0; i < fullText.Length; i++)
        {
            targetText.text += fullText[i];

            if (delay > 0f)
            {
                if (useUnscaledTime)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }

        typingCoroutine = null;
    }
}