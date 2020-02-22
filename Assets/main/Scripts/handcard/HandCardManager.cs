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

namespace main.handcard
{
    public class HandCardManager : MonoBehaviour
    {
        private const int HANDCARD_NUM = 5;

        private readonly BoolReactiveProperty _onHandCardEnabled = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> HandCardEnabled => _onHandCardEnabled;

        private GameStateManager gameStateManager;
        private CardUseProvider cardUseProvider;
        private HandCardFactory handCardFactory;

        private void Start()
        {
            gameStateManager = GetComponent<GameStateManager>();
            cardUseProvider = GetComponent<CardUseProvider>();
            handCardFactory = GetComponent<HandCardFactory>();

            gameStateManager.CurrentState
                .FirstOrDefault(x => x == GameState.Battle)
                .Subscribe(_ => {
                    for(int i = 1; i <= HANDCARD_NUM + 1; i++)
                    {
                        var obj = handCardFactory.CreateHandCard(i, HANDCARD_NUM);
                    }
                    CreateLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
                });
            
            _onHandCardEnabled.AddTo(this);
        }

        private async UniTaskVoid CreateLoopAsync(CancellationToken token)
        {
            // 開始直後にちょっと待つ
            await UniTask.Delay(1000, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: token);

                _onHandCardEnabled.Value = true;
                Debug.Log("HandCardEnabled true");
                
                var takeCardInfo = await cardUseProvider.OnCardTaken.ToUniTask(useFirstValue: true, cancellationToken: token);
                
                var obj = handCardFactory.CreateHandCard(HANDCARD_NUM + 1, HANDCARD_NUM);

                if(takeCardInfo.takeState == TakeState.Use)
                {

                }

                _onHandCardEnabled.Value = false;
                Debug.Log("HandCardEnabled false");
            }
        }
    }
}