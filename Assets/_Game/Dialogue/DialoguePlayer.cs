using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hackathon
{
    public class DialoguePlayer : MonoBehaviour
    {
        TextAsset inkJSON = null;
        Story story;
        
        [SerializeField] RectTransform uiParent = null;
        [SerializeField] Universal.FadeController popUp = null;

        // UI Prefabs
        [SerializeField] TextMeshProUGUI textUI = null;
        [SerializeField] Button buttonUI = null;
        TextMeshProUGUI storyText;  
        
        public UnityEvent OnDialogueEnded;

        public void StartDialogue(TextAsset dialogue)
        {
            if(!dialogue) return;
            
            inkJSON = dialogue;
            
            // Remove the default message
            ResetButton();
            StartStory();
        }

        // Creates a new Story object with the compiled story which we can then play!
        void StartStory()
        {
            story = new Story(inkJSON.text);
            RefreshView();
        }

        // This is the main function called every time the story changes. It does a few things:
        // Destroys all the old content and choices.
        // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
        void RefreshView()
        {
            // Remove all the UI on screen
            ResetButton();

            // Read all the content until we can't continue any more
            while (story.canContinue)
            {
                // Continue gets the next line of the story
                string text = story.Continue();
                // This removes any white space from the text.
                text = text.Trim();
                // Display the text on screen!
                CreateContentView(text);
            }

            // Display all the choices, if there are any!
            if(story.currentChoices.Count > 0) {
                for (int i = 0; i < story.currentChoices.Count; i++) {
                    Choice choice = story.currentChoices [i];
                    Button button = CreateChoiceView (choice.text.Trim ());
                    // Tell the button what to do when we press it
                    button.onClick.AddListener (delegate {
                        OnClickChoiceButton (choice);
                    });
                }
            }
            // If we've read all the content and there's no choices, the story is finished!
            else OnDialogueEnded?.Invoke();
        }

        void OnClickChoiceButton (Choice choice) {
            story.ChooseChoiceIndex (choice.index);
            RefreshView();
        }
        
        // Creates a textbox showing the line of text
        void CreateContentView(string text)
        {
            TextMeshProUGUI storyText = textUI;
            storyText.text = text;
            storyText.transform.SetParent(uiParent.transform, false);
            popUp.FadeIn();
        }

        // Creates a button showing the choice text
        Button CreateChoiceView(string text)
        {
            // Creates the button from a prefab
            Button choice = buttonUI;
            choice.transform.SetParent(uiParent.transform, false);
            popUp.FadeIn();

            // Gets the text from the button prefab
            TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
            choiceText.text = text;

            // Make the button expand to fit the text
            HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childForceExpandHeight = false;

            return choice;
        }

        // Destroys all the children of this gameobject (all the UI)
        void ResetButton()
        {
            // int childCount = uiParent.transform.childCount;
            // for (int i = childCount - 1; i >= 0; --i) 
            //     Destroy(uiParent.transform.GetChild(i).gameObject);
            buttonUI.onClick.RemoveAllListeners();
        }
    }
}