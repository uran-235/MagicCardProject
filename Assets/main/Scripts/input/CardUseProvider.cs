using main.card;
using main.handcard;
using main.input;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace main.input
{
    public class CardUseProvider : MonoBehaviour
    {
        private IUsable currentUseObject;

        private void Start()
        {
            var input = GetComponent<IInputEventProvider>();
            var handCardManager = GetComponent<HandCardManager>();

            input.CurrentState
                .Where(_ => handCardManager.HandCardEnabled.Value)
                .Where(x => (x == TouchPhase.Moved) && input.TouchObject.Value.GetComponent<IUsable>() != null)
                .Subscribe(_ =>
                {
                    currentUseObject = input.TouchObject.Value.GetComponent<IUsable>();
                    currentUseObject.Use();
                })
                .AddTo(this);

            input.CurrentState
                .Where(x => x == TouchPhase.Ended)
                    .Subscribe(_ =>
                    {
                        if(currentUseObject != null)
                        {
                            currentUseObject.Release();
                            currentUseObject = null;
                        }
                    })
                    .AddTo(this);
        }
    }
}