using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hackathon.Game
{
    public class TimeManager : Universal.Singleton<TimeManager>
    {
        [System.Serializable]
        class TimedBackground
        {
            public Sprite background;
            [Range(0,1)] public float time;
        }
        
        [SerializeField] float dayDuration = 2*60f;
        [SerializeField] Transform clockThingy;
        [SerializeField,Tooltip("Tick to start paused")] bool isPaused = true;
        float dayTimer = Mathf.Infinity; //Avoid unprompted day start
        
        [Header("Background")]
        [SerializeField] Image backgroundImg1;
        [SerializeField] Image backgroundImg2;
        [SerializeField] TimedBackground[] timedBackgrounds;
        int cBackground;

        public UnityEvent DayEnded;
        
        public void StartDay()
        {
            dayTimer = 0;   
            cBackground = 0;
            
            if(timedBackgrounds.Length > 0)
                SetUpBackgrounds();
        }
        public void Pause(bool pause)
        {
            isPaused = pause;
        }
        void Update()
        {
            if (isPaused) return;
            
            if (dayTimer >= dayDuration) return;
            
            dayTimer += Time.deltaTime;
            
            float time = dayTimer / dayDuration;
            
            
            if(clockThingy)
                //Invert time to better work as clock (rotate to the right instead of the left)
                clockThingy.rotation = Quaternion.Euler(0, 0, 360f * (1 - time));

            UpdateBackground(time);
            
            if (dayTimer >= dayDuration) DayEnded?.Invoke();
        }

        void UpdateBackground(float time)
        {
            //If no backgrounds skip
            if(timedBackgrounds.Length <= 0) return;
            //If last background skip
            if(cBackground >= timedBackgrounds.Length-1) return;
            
            TimedBackground cur = timedBackgrounds[cBackground];
            TimedBackground next = timedBackgrounds[cBackground + 1];

            if (time >= next.time)
            {
                cBackground++;
                SetUpBackgrounds();
                return;
            }
            
            //Diff between current and next background complete time
            float curNextDiff = cur.time - next.time;
            //Diff between current time and next background complete time
            float timeDiff = time - next.time;

            //Safety net to avoid timeDiff / 0
            if (curNextDiff == 0)
            {
                Debug.LogError("Timed Backgrounds" + cBackground + "&" 
                               + cBackground+1 + " share time");
                return;
            }
            
            //Delta of time going from current bg time to next bg time
            float t = timeDiff / curNextDiff; //goes from 1 to 0
            
            Color color = backgroundImg1.color;
            color.a = t; //fades out | goes from 1 to 0
            backgroundImg1.color = color;
            
            color = backgroundImg2.color;
            color.a = 1-t; //fades in | goes from 0 to 1
            backgroundImg2.color = color;
        }
        void SetUpBackgrounds()
        {
            backgroundImg1.sprite = timedBackgrounds[cBackground].background;
         
            if (cBackground < timedBackgrounds.Length - 1)
                backgroundImg2.sprite = timedBackgrounds[cBackground+1].background;

            Color color = backgroundImg1.color;
            color.a = 1;
            backgroundImg1.color = color;
            
            color = backgroundImg2.color;
            color.a = 0;
            backgroundImg2.color = color;
        }
    }
}
