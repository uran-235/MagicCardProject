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


namespace main.handcard
{
    public class HandCardManager : MonoBehaviour
    {
        private const int HANDCARD_NUM = 5;

        private readonly BoolReactiveProperty _onHandCardEnabled = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> HandCardEnabled => _onHandCardEnabled;

        public Subject<int> _onUseSubject { private set; get; } = new Subject<int>();
        public IObservable<int> OnHandCardUsed => _onUseSubject;
        
        private Subject<int> _onCardCreateSubject = new Subject<int>();
        public IObservable<int> OnHandCardCreate => _onCardCreateSubject;

        private GameStateManager gameStateManager;

        private List<GameObject> handCards = new List<GameObject>();

        private void Start()
        {
            gameStateManager = GetComponent<GameStateManager>();

            gameStateManager.CurrentState
                .FirstOrDefault(x => x == GameState.Battle)
                .Subscribe(_ => {
                    for(int i = 1; i <= HANDCARD_NUM + 1; i++)
                    {
                        _onCardCreateSubject.OnNext(i);
                        //handCards.Add(CreateHandCard(i));
                    }
                    CreateLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
                });

            _onUseSubject.AddTo(this);
            _onCardCreateSubject.AddTo(this);
            _onHandCardEnabled.AddTo(this);
        }

        private async UniTaskVoid CreateLoopAsync(CancellationToken token)
        {
            //var useCardEvent = handCards
            //    .Select(x => x.GetComponent<CardTakeMover>())
            //    .Select(x => x.OnUsed)
            //    .Merge()
            //    .Publish();

            //useCardEvent.Connect().AddTo(this);

            // 開始直後にちょっと待つ
            await UniTask.Delay(1000, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(3000, cancellationToken: token);

                _onHandCardEnabled.Value = true;
                Debug.Log("HandCardEnabled true");

                int useCardIndex = await _onUseSubject.ToUniTask(useFirstValue: true, cancellationToken: token);

                _onHandCardEnabled.Value = false;
                Debug.Log("HandCardEnabled false");

                //_onUseSubject.OnNext(useCardIndex);

                _onCardCreateSubject.OnNext(HANDCARD_NUM + 1);
                //handCards.Add(CreateHandCard(HANDCARD_NUM + 1));
            }
        }
    }
}