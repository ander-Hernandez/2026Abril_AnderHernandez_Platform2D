using System.Collections;
using UnityEngine;

public class HitPauseManager : MonoBehaviour
{
    public static HitPauseManager Instance { get; private set; }

    private Coroutine hitPauseCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerHitPause(float duration)
    {
        if (duration <= 0f)
            return;

        if (hitPauseCoroutine != null)
            StopCoroutine(hitPauseCoroutine);

        hitPauseCoroutine = StartCoroutine(HitPauseCoroutine(duration));
    }

    private IEnumerator HitPauseCoroutine(float duration)
    {
        float previousTimeScale = Time.timeScale;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = previousTimeScale;

        hitPauseCoroutine = null;
    }
}