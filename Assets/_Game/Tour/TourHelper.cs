using UnityEngine;

namespace Hackathon.Utils
{
#if UNITY_EDITOR
    public class TourHelper : MonoBehaviour
    {
        [SerializeField] public Game.Location locA;
        [SerializeField] public Game.Location locB;

        [ContextMenu("Get Dist from A to B")]
        void GetDistFromAToB()
        {
            Debug.Log("Dist from " + locA.Name + " to " + locB.Name + 
                      ": "+ locA.Dist(locB));
        }
    }
#endif
}