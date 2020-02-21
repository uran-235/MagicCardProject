using UniRx;
using UnityEngine;

namespace main.input
{
    public interface IInputEventProvider
    {
        IReadOnlyReactiveProperty<TouchPhase> CurrentState { get; }
        IReadOnlyReactiveProperty<GameObject> TouchObject { get; }
    }
}
