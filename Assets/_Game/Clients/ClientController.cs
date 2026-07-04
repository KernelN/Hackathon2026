using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hackathon.Game.Clients
{
    public class ClientController : MonoBehaviour
    {
        [Header("Client")]
        [SerializeField] Universal.FadeController fader;
        [SerializeField] Image image;
        TextAsset cDialogue;

        [Header("Dialogue")] 
        [SerializeField] DialoguePlayer dialoguePlayer;
        [SerializeField] Universal.FadeController dialogueFader;

        public UnityEvent Disappeared;
        
        void Start()
        {
            fader.completedFade.AddListener(OnFadeCompleted);
        }
        public void Appear(Sprite clientSprite, TextAsset dialogue)
        {
            fader.FadeIn();
            image.sprite = clientSprite;
            cDialogue = dialogue;
        }
        public void Disappear()
        {
            fader.FadeOut();
            dialogueFader.FadeOut();
        }

        void OnFadeCompleted(bool didFadeIn)
        {
            if(didFadeIn) dialoguePlayer.StartDialogue(cDialogue);
            else Disappeared?.Invoke();
        }
        public void StartDialogue(TextAsset dialogue)
        {
            cDialogue = dialogue;
            dialoguePlayer.StartDialogue(cDialogue);
        }
    }
}
