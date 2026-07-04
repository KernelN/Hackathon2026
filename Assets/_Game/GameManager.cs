using System;
using System.Collections;
using Hackathon.Game.Clients;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hackathon.Game
{
    public class GameManager : Universal.Singleton<GameManager>
    {
        [SerializeField] float timeToStartGame = 1;
        [SerializeField] float timeToAutoQuitToMenu = 10;

        [SerializeField] Universal.FadeController gameOverPanel;

        void Start()
        {
            OrderManager oManager = OrderManager.inst;
            if (oManager)
            {
                StartCoroutine(WaitToExecute(oManager.GameStart, timeToStartGame));
                oManager.OnOrdersEnded.AddListener(OnGameOver);
            }
        }
        public void OnGameOver()
        {
            gameOverPanel.FadeIn();
            StartCoroutine(WaitToExecute(GoToMenu, timeToAutoQuitToMenu));
        }
        void GoToMenu() => SceneManager.LoadSceneAsync(0);
        IEnumerator WaitToExecute(System.Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }
    }
}
