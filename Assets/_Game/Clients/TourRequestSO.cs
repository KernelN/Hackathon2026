using UnityEngine;

namespace Hackathon.Game.Clients
{
    [CreateAssetMenu(fileName = "TourRequest", menuName = "Hackathon/TourRequest")]
    public class TourRequestSO : ScriptableObject
    {
        [Header("Tour Request")]
        [SerializeField] LocationRequest[] locationRequests;
        
        [Header("Client & Dialogue")]
        [SerializeField] Sprite clientSprite;
        [SerializeField] TextAsset inkIntro;
        [SerializeField] TextAsset inkOutro;
        [SerializeField] TextAsset inkCancel;
        
        [Header("Review")]
        [SerializeField] string clientName;
        [SerializeField] string badReviewDesc;
        [SerializeField] Sprite badReviewImg;
        [SerializeField] string goodReviewDesc;
        [SerializeField] Sprite goodReviewImg;
        
        public Sprite ClientSprite => clientSprite;
        public TextAsset InkIntro => inkIntro;
        public TextAsset InkOutro => inkOutro;
        public TextAsset InkCancel => inkCancel;
        public LocationRequest[] LocationRequests => locationRequests;

        public bool IsTourGood(Tour tour)
        {
            // //Tour needs at least as many locations as requested
            // if(tour.locations.Count < locationRequests.Length) return false;
            
            //Tour needs the same amount of location as requested 
            if(tour.locations.Count != locationRequests.Length) return false;
            
            for (int i = 0; i < tour.locations.Count; i++)
            {
                if (!locationRequests[i].IsLocationGood(tour.locations[i], tour.Distances[i]))
                    return false;
            }
            
            return true;
        }

        public void SetReviewData(TMPro.TextMeshProUGUI reviewName,
            TMPro.TextMeshProUGUI reviewDesc,
            UnityEngine.UI.Image reviewImg, bool isOrderGood)
        {
            reviewName.text = clientName;
            
            reviewDesc.text = isOrderGood ? goodReviewDesc : badReviewDesc;
            reviewImg.sprite = isOrderGood ? goodReviewImg : badReviewImg;
            reviewImg.gameObject.SetActive(true);
        }
    }
}
