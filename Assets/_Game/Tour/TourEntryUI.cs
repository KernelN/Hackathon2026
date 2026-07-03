using System;
using UnityEngine;
using UnityEngine.Events;

namespace Hackathon.Game
{
    public class TourEntryUI : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI label;
        
        Location location;
        public Location Location
        {
            get => location;
            set
            {
                location = value;
                label.text = location.Name;
            }
        }

        public UnityEvent<int[]> onIndexChanged;
        public UnityEvent<int> onRemoved;
        
        public void Remove() => onRemoved?.Invoke(transform.GetSiblingIndex());

        public void OnIndexChanged(int[] indices) => onIndexChanged?.Invoke(indices);
    }
}
