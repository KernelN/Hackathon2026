using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hackathon.Game.Clients
{
    public class OrderManager : Universal.Singleton<OrderManager>
    {
        enum OrderStage
        {
            Intro,
            Tourcraft,
            Outro
        }
        
        [Header("Orders")]
        [SerializeField] TourRequestSO[] requests;
        [SerializeField] ClientController client;
        [SerializeField] float timeBetweenClients = 1;
        OrderStage currentStage;
        int ordersCompleted = 0;
        int ordersSucceded = 0;

        [Header("Order Feedback")]
        [SerializeField] TMPro.TextMeshProUGUI completionRateText;
        float completionRate = 0;
        [SerializeField] Image reviewImg;
        [SerializeField] TMPro.TextMeshProUGUI reviewLabel;
        [SerializeField] TMPro.TextMeshProUGUI reviewDesc;
        bool lastOrderSucceded = false;

        [Header("Time")]
        [SerializeField] float timePerOrder = 30f;
        [SerializeField] Universal.FadeController timeBarFader;
        [SerializeField] Image timeBarFill;
        float currentTime = 0;
        
        public UnityEvent OnOrdersEnded;
        
        [ContextMenu("Start")]
        public void GameStart()
        {
            client.Disappeared.AddListener(OnClientDisappeared);
            
            completionRate = 1;
            completionRateText.text = $"{completionRate*100}%";
            StartOrder();
        }

        void Update()
        {
            switch (currentStage)
            {
                case OrderStage.Tourcraft:
                    if (currentTime > 0)
                    {
                        currentTime -= Time.deltaTime;
                        timeBarFill.fillAmount = currentTime / timePerOrder;
                        if (currentTime <= 0) EndOrder(false);
                    }

                    break;
                case OrderStage.Outro:
                    if (currentTime > 0)
                    {
                        currentTime -= Time.deltaTime;
                        if (currentTime <= 0)
                        {
                            if(ordersCompleted < requests.Length)
                                StartOrder();
                            else
                                OnOrdersEnded?.Invoke();
                        }
                    }

                    break;
            }
            
        }
        void StartOrder()
        {
            currentTime = timePerOrder;
            timeBarFill.fillAmount = 1;
            TourRequestSO req = requests[ordersCompleted];
            client.Appear(req.ClientSprite, req.InkIntro);
            currentStage = OrderStage.Intro;
        }
        public void OnDialogueEnded()
        {
            switch (currentStage)
            {
                case OrderStage.Intro:
                    MapManager.inst.EnableMap(true);
                    timeBarFader.FadeIn();
                    currentStage = OrderStage.Tourcraft;
                    break;
                case OrderStage.Outro:
                    DisappearClient();
                    break;
            }
        }
        public void ReceiveTour(Tour tour)
        {
            if (requests[ordersCompleted].IsTourGood(tour))
            {
                ordersSucceded++;
                lastOrderSucceded = true;
            }
            
            EndOrder(true);
        }
        public void OnClientDisappeared()
        {
            UpdateRate();
            currentTime = timeBetweenClients;
        }
        void EndOrder(bool isOrderCompleted)
        {
            currentStage = OrderStage.Outro;
            
            if (isOrderCompleted)
            {
                if (requests[ordersCompleted].InkOutro)
                    client.StartDialogue(requests[ordersCompleted].InkOutro);
                else DisappearClient();
            }
            else
            {
                if (requests[ordersCompleted].InkCancel)
                    client.StartDialogue(requests[ordersCompleted].InkCancel);
                else DisappearClient();
            }
            
            MapManager.inst.EnableMap(false);
            ordersCompleted++;
        }

        void DisappearClient()
        {
            client.Disappear();
            timeBarFader.FadeOut();
        }
        void UpdateRate()
        {
            completionRate = (float)ordersSucceded / ordersCompleted;
            completionRateText.text = (completionRate*100).ToString("F0") + "%";
            
            
            //Update Tablet
            requests[ordersCompleted-1].SetReviewData(reviewLabel, reviewDesc, 
                                                        reviewImg, lastOrderSucceded);
            lastOrderSucceded = false;
        }
    }
}