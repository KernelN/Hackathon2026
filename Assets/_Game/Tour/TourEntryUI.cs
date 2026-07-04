using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hackathon.Game
{
    public class TourEntryUI : MonoBehaviour
    {
        [SerializeField] Universal.FadeController fader;
        [SerializeField] Image[] backgrounds;
        [SerializeField] TMPro.TextMeshProUGUI label;
        
        Location location;
        public Location Location
        {
            get => location;
            set
            {
                location = value;
                int index = 0;
                do
                {
                    backgrounds[index].color = 
                        LocationSO.TagColor(location.Tags[index % location.Tags.Count]);
                    index++;
                } while (index < backgrounds.Length);
                label.text = location.Name;
            }
        }

        public UnityEvent<int[]> onIndexChanged;
        public UnityEvent<int> onRemoved;

        public void Appear() => fader.FadeIn();
        public void Disappear() => fader.FadeOut();

        public void Remove()
        {
            onRemoved?.Invoke(transform.GetSiblingIndex());
        }
        public void OnIndexChanged(int[] indices) => onIndexChanged?.Invoke(indices);
    }
}
