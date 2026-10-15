using UnityEngine;
using UnityEngine.UI;
using Universal;

namespace Hackathon.Game.Clients
{
    public class OrderReviewUI : MonoBehaviour
    {
        [SerializeField] FadeController fader;
        [SerializeField] AudioSource reviewNotificationPlayer;
        
        [Header("Client Details")]
        [SerializeField] Image clientFaceImg;
        [SerializeField] TMPro.TextMeshProUGUI clientNameText;
        [Header("Review Details")]
        [SerializeField] TMPro.TextMeshProUGUI reviewText;
        [SerializeField] Image[] starImgs;
        [SerializeField] Color filledStarColor;
        [SerializeField] Color emptyStarColor;

        public void SetReviewClient(Sprite clientFace, string clientName)
        {
            fader.FadeIn();
            clientFaceImg.sprite = clientFace;
            clientNameText.text = clientName;
        }
        public void SetReviewDetails(string review, int reviewPoints)
        {
            reviewText.text = review;

            //Fill as many stars as review points rated
            for (int i = 0; i < starImgs.Length; i++) 
                starImgs[i].color = i < reviewPoints ? filledStarColor: emptyStarColor;
            
            if (Hackathon.Audio.AudioManager.inst != null && reviewNotificationPlayer != null && reviewNotificationPlayer.clip != null)
                Hackathon.Audio.AudioManager.inst.PlaySFX(reviewNotificationPlayer.clip);
            else
                reviewNotificationPlayer?.Play();
        }
    }
}