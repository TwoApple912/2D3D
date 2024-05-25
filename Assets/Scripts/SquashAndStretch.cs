using System;
using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0f, 1f)] private float animationDuration = 0.25f;
    [Space]
    [SerializeField] private bool canBeOverwritten;
    [SerializeField] private bool playOnStart;
    [Space]
    [SerializeField] private bool playEverytime = true;
    [SerializeField, Range(0f, 100f)] private float chanceToPlay = 100f; 
    
    [Flags] public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("Animation Settings")]
    [SerializeField] private float initialScale = 1;
    [SerializeField] private float maximumScale = 1.5f;
    [SerializeField] private bool resetScaleAfterAnimation = true;
    [SerializeField] private bool reverseAnimationCurveAfterPlaying = false;
    private bool isReversed;
    [Space]
    [SerializeField] private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
        );

    [Header("Looping Settings")]
    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine squashAndStretchCoroutine;
    private WaitForSeconds loopingDelaySeconds;
    private Vector3 initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (transformToAffect == null) transformToAffect = transform;

        initialScaleVector = transformToAffect.localScale;
        loopingDelaySeconds = new WaitForSeconds(loopingDelay);
    }

    void Start()
    {
        if (playOnStart) CheckForAndStartCoroutine();
    }

    private void OnDisable()
    {
        if (transformToAffect) transformToAffect.localScale = initialScaleVector;
    }

    public void PlaySquashAndStretch() // Call this to perform the animation
    {
        if (looping && !canBeOverwritten) return;
        
        CheckForAndStartCoroutine();
    }

    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None. " + gameObject);
            return;
        }

        if (squashAndStretchCoroutine != null)
        {
            StopCoroutine(squashAndStretchCoroutine);
            if (playEverytime && resetScaleAfterAnimation) transform.localScale = initialScaleVector;
        }

        squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    IEnumerator SquashAndStretchEffect()
    {
        do
        {
            if (!playEverytime)
            {
                float random = UnityEngine.Random.Range(0, 100f);
                if (random > chanceToPlay)
                {
                    yield return null;
                    continue;
                }
            }

            if (reverseAnimationCurveAfterPlaying) isReversed = !isReversed;
            
            float elapsedTime = 0;
            Vector3 originalScale = initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition;
                if (isReversed) curvePosition = 1 - (elapsedTime / animationDuration);
                else curvePosition = elapsedTime / animationDuration;

                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold) remappedValue = minimumThreshold;

                if (affectX) modifiedScale.x = originalScale.x * remappedValue;
                else modifiedScale.x = originalScale.x / remappedValue;
                if (affectY) modifiedScale.y = originalScale.y * remappedValue;
                else modifiedScale.y = originalScale.y / remappedValue;
                if (affectZ) modifiedScale.z = originalScale.z * remappedValue;
                else modifiedScale.z = originalScale.z / remappedValue;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetScaleAfterAnimation) transformToAffect.localScale = originalScale;

            if (looping) yield return loopingDelaySeconds;
        } while (looping);
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }
}