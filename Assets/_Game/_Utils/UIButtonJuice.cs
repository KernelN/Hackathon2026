using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hackathon.UI
{
    [DisallowMultipleComponent]
    public class UIButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Toggles")]
        [SerializeField] bool enableHoverScale = true;
        [SerializeField] bool enablePressScale = true;
        [SerializeField] bool enableClickPunch = true;

        [Header("Scale Settings")]
        [SerializeField] float hoverScaleMultiplier = 1.05f;
        [SerializeField] float pressScaleMultiplier = 0.94f;
        [SerializeField] float animationSpeed = 16f;

        [Header("Audio")]
        [SerializeField] bool playClickSound = true;
        [SerializeField] bool playHoverSound = false;

        public bool EnableHoverScale { get => enableHoverScale; set => enableHoverScale = value; }
        public bool EnablePressScale { get => enablePressScale; set => enablePressScale = value; }
        public bool EnableClickPunch { get => enableClickPunch; set => enableClickPunch = value; }
        public bool PlayClickSound { get => playClickSound; set => playClickSound = value; }
        public bool PlayHoverSound { get => playHoverSound; set => playHoverSound = value; }

        Vector3 baseScale = Vector3.one;
        Vector3 targetScale = Vector3.one;
        Coroutine scaleRoutine;
        bool isHovered = false;
        bool isPressed = false;

        void Awake()
        {
            baseScale = transform.localScale;
            targetScale = baseScale;
        }

        void OnEnable()
        {
            transform.localScale = baseScale;
            targetScale = baseScale;
            isHovered = false;
            isPressed = false;
        }

        void OnDisable()
        {
            if (scaleRoutine != null)
            {
                StopCoroutine(scaleRoutine);
                scaleRoutine = null;
            }
            transform.localScale = baseScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            if (enableHoverScale && !isPressed)
                StartScaleTween(baseScale * hoverScaleMultiplier);

            if (playHoverSound && Audio.AudioManager.inst != null)
                Audio.AudioManager.inst.PlayButtonHover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isPressed = false;
            if (enableHoverScale || enablePressScale)
                StartScaleTween(baseScale);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            if (enablePressScale)
                StartScaleTween(baseScale * pressScaleMultiplier);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            if (enableHoverScale && isHovered)
                StartScaleTween(baseScale * hoverScaleMultiplier);
            else if (enablePressScale)
                StartScaleTween(baseScale);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (playClickSound && Audio.AudioManager.inst != null)
                Audio.AudioManager.inst.PlayButtonClick();

            if (enableClickPunch)
                StartCoroutine(PunchRoutine());
        }

        void StartScaleTween(Vector3 target)
        {
            targetScale = target;
            if (scaleRoutine != null) StopCoroutine(scaleRoutine);
            scaleRoutine = StartCoroutine(ScaleTweenRoutine());
        }

        IEnumerator ScaleTweenRoutine()
        {
            while (Vector3.Distance(transform.localScale, targetScale) > 0.002f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
                yield return null;
            }
            transform.localScale = targetScale;
            scaleRoutine = null;
        }

        IEnumerator PunchRoutine()
        {
            Vector3 punchScale = baseScale * pressScaleMultiplier * 0.96f;
            float t = 0f;
            while (t < 0.08f)
            {
                t += Time.unscaledDeltaTime;
                transform.localScale = Vector3.Lerp(transform.localScale, punchScale, t / 0.08f);
                yield return null;
            }

            Vector3 bounceScale = (isHovered && enableHoverScale) ? baseScale * hoverScaleMultiplier : baseScale;
            t = 0f;
            while (t < 0.12f)
            {
                t += Time.unscaledDeltaTime;
                transform.localScale = Vector3.Lerp(punchScale, bounceScale, t / 0.12f);
                yield return null;
            }

            transform.localScale = bounceScale;
        }
    }
}
