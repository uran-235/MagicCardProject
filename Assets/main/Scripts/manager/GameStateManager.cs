using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace main.manager
{
    public class GameStateManager : MonoBehaviour
    {
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;

        private readonly ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Ready);
        private TimeManager timeManager; 

        private void Start()
        {
            timeManager = GetComponent<TimeManager>();

            timeManager.ReadyTime
                .Where(x => x == 0)
                .Take(1)
                .Subscribe(x => { _currentState.Value = GameState.Battle; })
                .AddTo(this);
            
            timeManager.RemainingTime
                .Where(x => x == 0)
                .Take(1)
                .Subscribe(x => { _currentState.Value = GameState.Finished; })
                .AddTo(this);

            _currentState.AddTo(this);
        }
    }
}
