using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using Zenject;

namespace main.manager
{
    public class PhaseManager : MonoBehaviour
    {
        [Inject] private GameStateManager _gameStateManager = default;

        private void Start()
        {
            _gameStateManager.CurrentState
                .FirstOrDefault(x => x == GameState.Battle)
                .Subscribe(_ => CreateLoopAsync(this.GetCancellationTokenOnDestroy()).Forget());
        }

        private async UniTaskVoid CreateLoopAsync(CancellationToken token)
        {
            // 開始直後にちょっと待つ
            await UniTask.Delay(1000, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {

            }
        }
    }
}