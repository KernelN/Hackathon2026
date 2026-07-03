using System.Collections;
using UnityEngine;

namespace NAMESPACE
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] bool startOff = true;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] CanvasGroup canvasGroup;
        Coroutine cTask;
        bool isOn;

        void Awake()
        {
            isOn = !startOff;
            if (startOff)
                canvasGroup.alpha = 0;
        }

        public void FadeIn()
        {
            if(isOn) return;
            
            if (cTask != null) StopCoroutine(cTask);
            cTask = StartCoroutine(FadeTask(false, fadeInTime));
        }

        public void FadeOut()
        {
            if(!isOn) return;
            
            if (cTask != null) StopCoroutine(cTask);
            cTask = StartCoroutine(FadeTask(true, fadeInTime));
        }

        IEnumerator FadeTask(bool fadeOut, float time, bool useTimeScale = true)
        {
            float timer = 0;

            do
            {
                timer += (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);
                float t = timer / time;
                if (fadeOut) t = 1 - t;

                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            } while (timer < time);

            isOn = !fadeOut;
        }
    }
}