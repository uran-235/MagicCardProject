using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace main.input
{
    public class PlayerInputProvider : MonoBehaviour, IInputEventProvider
    {
        private readonly ReactiveProperty<TouchPhase> _onActionTouch = new ReactiveProperty<TouchPhase>();
        private readonly ReactiveProperty<GameObject> _onTouchObject = new ReactiveProperty<GameObject>();
        public IReadOnlyReactiveProperty<TouchPhase> CurrentState => _onActionTouch;
        public IReadOnlyReactiveProperty<GameObject> TouchObject => _onTouchObject;

        private const float RayCastMaxDistance = 100.0f;

        private void Start()
        {
            //SetUpAsync().Forget();
            SetUpAsync();
        }

        //private async UniTaskVoid SetUpAsync()
        private void SetUpAsync()
        {
            if (Application.isEditor)
            {
                this.UpdateAsObservable()
                    .Select(_ => {
                        if (Input.GetMouseButtonDown(0))
                        {
                            return TouchPhase.Began;
                        }
                        else if (Input.GetMouseButtonUp(0))
                        {
                            return TouchPhase.Ended;
                        }
                        else if(Input.GetMouseButton(0))
                        {
                            return TouchPhase.Moved;
                        }
                        else
                        {
                            return TouchPhase.Ended;
                        }
                    })
                    .DistinctUntilChanged()
                    .Subscribe(x => 
                        _onActionTouch.Value = x
                    );

                this.UpdateAsObservable()
                    .Select(_ =>
                        {
                            if (!Input.GetMouseButtonDown(0)) return _onTouchObject.Value;
                            PointerEventData pointer = new PointerEventData(EventSystem.current);
                            pointer.position = Input.mousePosition;
                            List<RaycastResult> result = new List<RaycastResult>();
                            EventSystem.current.RaycastAll(pointer, result);
                            return result.Count <= 0? null : result?[0].gameObject;
                        })
                    .DistinctUntilChanged()
                    .Subscribe(x =>
                        {
                            _onTouchObject.Value = x;
                            Debug.Log(x);
                        }
                    );
            }
            else
            {
                this.UpdateAsObservable()
                    .Select(_ => Input.touchCount > 0)
                    .DistinctUntilChanged()
                    .Subscribe(_ => {
                        _onActionTouch.Value = Input.GetTouch(0).phase;
                        Debug.Log("Touched!");
                    });
            }
            _onTouchObject.AddTo(this);
            _onActionTouch.AddTo(this);
        }
    }
}
