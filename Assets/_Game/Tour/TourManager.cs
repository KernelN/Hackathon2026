using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Hackathon.Game
{
    public class TourManager : Singleton<TourManager>
    {
        [SerializeField] Location[] locations;
        Tour currentTour;
        
        [Header("UI")]
        [SerializeField] TourEntryUI[] uiEntries;
        List<TourEntryUI> activeEntries = new List<TourEntryUI>();
        List<TourEntryUI> inactiveEntries = new List<TourEntryUI>();

        public UnityEvent<Tour> TourClosed;

        void Start()
        {
            for (var i = 0; i < locations.Length; i++) 
                locations[i].OnSelected += OnLocationSelected;

            foreach (var entryUI in uiEntries)
            {
                entryUI.onIndexChanged.AddListener(
                    indices => MoveLocationIndex(indices[0], indices[1]));
                entryUI.onRemoved.AddListener(RemoveLocationAt);
            }
            
            inactiveEntries.AddRange(uiEntries);
            
            StartTourPlanning();
        }
        public void StartTourPlanning()
        {
            Debug.Log("Starting Tour Planning");
            currentTour = new Tour();
        }

        public void MoveLocationIndex(int index, int newIndex)
        {
            currentTour.MoveLocationIndex(index, newIndex);
            
            TourEntryUI entry = activeEntries[index];
            activeEntries.RemoveAt(index);
            activeEntries.Insert(newIndex, entry);
        }
        public void RemoveLocationAt(int index)
        {
            currentTour.RemoveLocation(index);
            
            DestroyTourEntry(activeEntries[index]);
        }
        void OnLocationSelected(Location location)
        {
            //If tour contains location, remove it
            if (currentTour.ContainsLocation(location))
            {
                currentTour.RemoveLocation(location);

                for (var index = 0; index < activeEntries.Count; index++)
                {
                    if (activeEntries[index].Location != location) continue;
                    DestroyTourEntry(activeEntries[index]);
                }
            }

            //If it does not contain location, add it
            else
            {
                currentTour.AddLocation(location);
                
                inactiveEntries[0].Location = location;
                CreateTourEntry(inactiveEntries[0]);
            }
        }
        void CreateTourEntry(TourEntryUI entry)
        {
            inactiveEntries.Remove(entry);
            activeEntries.Add(entry);
            entry.Appear();
        }
        void DestroyTourEntry(TourEntryUI entry)
        {
            inactiveEntries.Add(entry);
            activeEntries.Remove(entry);
            entry.Disappear();
            entry.transform.SetAsLastSibling(); //This keeps the active index working
        }
        public void CompleteTour()
        {
            TourClosed?.Invoke(currentTour);
            currentTour = null;
            while (activeEntries.Count > 0) DestroyTourEntry(activeEntries[0]);
        }
        
#if UNITY_EDITOR
        [ContextMenu("Find Locations")]
        void GetAllLocations() 
            => locations = FindObjectsByType<Location>();
#endif
    }
}