using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hackathon
{
    public class GameFlowButton : MonoBehaviour
    {
        [SerializeField] SceneAsset sceneToChange;
        public void ChangeScene()
        {
            if(Time.timeScale == 0) Time.timeScale = 1;
            SceneManager.LoadSceneAsync(sceneToChange.name);
        }

        public void Quit() => Application.Quit();

        public void Pause() => Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}