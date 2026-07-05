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
        float clientDelayTimer = 0;
        OrderStage currentStage;
        int ordersCompleted = 0;
        int ordersSucceded = 0;

        [Header("Order Feedback")]
        [SerializeField] TMPro.TextMeshProUGUI completionRateText;
        float completionRate = 0;
        [SerializeField] OrderReviewUI orderReviewUI;
        bool lastOrderSucceded = false;

        [Header("Time")]
        [SerializeField] float dayDuration = 2*60f;
        [SerializeField] Transform clockThingy;
        float dayTimer = 0;
        
        public UnityEvent OnOrdersEnded;

        [ContextMenu("Start")]
        public void GameStart()
        {
            client.Disappeared.AddListener(OnClientDisappeared);
            
            dayTimer = dayDuration;
            completionRate = 1;
            completionRateText.text = $"{completionRate*100}%";
            StartOrder();
        }

        void Update()
        {
            switch (currentStage)
            {
                case OrderStage.Tourcraft:
                    if (dayTimer > 0)
                    {
                        dayTimer -= Time.deltaTime;
                        clockThingy.rotation = Quaternion.Euler(0, 0, 360f * dayTimer / dayDuration);
                        if (dayTimer <= 0)
                        {
                            //EndOrder(false);
                            DisappearClient();
                            OnOrdersEnded?.Invoke();
                        }
                    }

                    break;
                case OrderStage.Outro:
                    if (clientDelayTimer > 0)
                    {
                        clientDelayTimer -= Time.deltaTime;
                        if (clientDelayTimer <= 0)
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
            clientDelayTimer = timeBetweenClients;
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

        void DisappearClient() => client.Disappear();

        void UpdateRate()
        {
            completionRate = (float)ordersSucceded / ordersCompleted;
            completionRateText.text = (completionRate*100).ToString("F0") + "%";
            
            
            //Update Tablet
            requests[ordersCompleted-1].SetReviewData(orderReviewUI, lastOrderSucceded);
            lastOrderSucceded = false;
        }
    }
}