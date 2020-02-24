using main.card;
using main.manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using System.Linq;
using Zenject;
using main.input;
using System.Collections.ObjectModel;

namespace main.handcard
{
    public class HandCardManager : MonoBehaviour
    {
        private const int HANDCARD_NUM = 5;

        private readonly BoolReactiveProperty _onHandCardEnabled = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> HandCardEnabled => _onHandCardEnabled;
        
        private Subject<TakeInfo> _onRearrangeSubject = new Subject<TakeInfo>();
        public IObservable<TakeInfo> OnRearrangeCard => _onRearrangeSubject;

        private bool enabledThrowAway = true;

        private GameStateManager gameStateManager;
        private CardUseProvider cardUseProvider;
        private HandCardFactory handCardFactory;

        private List<GameObject> handCards = new List<GameObject>();

        private void Start()
        {
            gameStateManager = GetComponent<GameStateManager>();
            cardUseProvider = GetComponent<CardUseProvider>();
            handCardFactory = GetComponent<HandCardFactory>();

            gameStateManager.CurrentState
                .FirstOrDefault(x => x == GameState.Battle)
                .Subscribe(_ => {
                    for(int i = 0; i < HANDCARD_NUM + 1; i++)
                    {
                        var obj = handCardFactory.CreateHandCard(i, HANDCARD_NUM);
                        handCards.Add(obj);
                    }
                    CreateLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
                });
            
            _onHandCardEnabled.AddTo(this);
            _onRearrangeSubject.AddTo(this);
        }

        private async UniTask CreateLoopAsync(CancellationToken token)
        {
            // 開始直後にちょっと待つ
            await UniTask.Delay(1000, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                enabledThrowAway = true;
                _onHandCardEnabled.Value = true;
                Debug.Log("HandCardEnabled true");

                while (!isAllCardDisabled())
                {
                    await UniTask.Delay(900, cancellationToken: token);

                    var takeCardInfo = await cardUseProvider.OnCardTaken.ToUniTask(useFirstValue: true, cancellationToken: token);
                    
                    if (!enabledThrowAway && takeCardInfo.takeState == TakeState.ThrowAway)
                    {
                        continue;
                    }
                    
                    if (takeCardInfo.takeState == TakeState.Use)
                    {
                        enabledThrowAway = false;
                    }

                    _onRearrangeSubject.OnNext(takeCardInfo);

                    Destroy(handCards[takeCardInfo.handCardIndex].gameObject);
                    handCards.RemoveAt(takeCardInfo.handCardIndex);

                    var obj = handCardFactory.CreateHandCard(HANDCARD_NUM, HANDCARD_NUM);
                    handCards.Add(obj);
                }

                _onHandCardEnabled.Value = false;
                Debug.Log("HandCardEnabled false");
            }
        }

        private bool isAllCardDisabled()
        {
            foreach(var obj in handCards)
            {
                if (obj.GetComponent<CardCore>().CurrentCanUse.Value) return false;
            }
            return true;
        }
    }
}