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
        private GameObject currentUseObject;

        private Subject<TakeInfo> _onTakeSubject = new Subject<TakeInfo>();
        public IObservable<TakeInfo> OnCardTaken => _onTakeSubject;

        private Vector2 startPosition;

        //カード使用の有効距離
        private const float ENABLE_TAKE_CARD_DISTANCE = 200.0f;
        //カード使用の有効角度
        private const float ENABLE_TAKE_CARD_ANGLE = 0.75f;

        private void Start()
        {
            var input = GetComponent<IInputEventProvider>();
            var handCardManager = GetComponent<HandCardManager>();

            input.CurrentState
                .Where(_ => handCardManager.HandCardEnabled.Value)
                .Where(x => (x == TouchPhase.Moved) && input.TouchObject.Value.GetComponent<IUsableCard>() != null)
                .Subscribe(_ =>
                {
                    currentUseObject = input.TouchObject.Value;
                    bool tryUse = currentUseObject.GetComponent<IUsableCard>().Use();
                    if (tryUse)
                    {
                        startPosition = currentUseObject.GetComponent<RectTransform>().localPosition;
                    }
                })
                .AddTo(this);

            input.CurrentState
                .Where(x => x == TouchPhase.Ended)
                    .Subscribe(_ =>
                    {
                        if(currentUseObject != null)
                        {
                            bool tryRelease = currentUseObject.GetComponent<IUsableCard>().Release();
                            if (tryRelease)
                            {
                                ChangeTakeState(currentUseObject);
                            }
                            currentUseObject = null;
                        }
                    })
                    .AddTo(this);

            _onTakeSubject.AddTo(this);
        }

        private void ChangeTakeState(GameObject useObject)
        {
            Vector2 endPosition = useObject.GetComponent<RectTransform>().localPosition;
            int cardIndex = currentUseObject.GetComponent<CardCore>().handCardIndex;
            CardInfo cardInfo = currentUseObject.GetComponent<CardCore>().cardInfo;

            if (Vector2.Distance(startPosition, endPosition) > ENABLE_TAKE_CARD_DISTANCE)
            {
                var diff = Vector2.Dot((endPosition - startPosition).normalized, new Vector2(0, 1));
                if (diff < -1.0f * ENABLE_TAKE_CARD_ANGLE)
                {
                    //ThrowAway
                    _onTakeSubject.OnNext(new TakeInfo { handCardIndex = cardIndex, takeState = TakeState.ThrowAway });
                    useObject.SetActive(false);
                    //Destroy(useObject);
                }else if (diff > ENABLE_TAKE_CARD_ANGLE)
                {
                    //Use
                    _onTakeSubject.OnNext(new TakeInfo { handCardIndex = cardIndex, takeState = TakeState.Use, cardInfo = cardInfo });
                    useObject.SetActive(false);
                    //Destroy(useObject);
                }
            }
            //Undo
            useObject.GetComponent<RectTransform>().localPosition = startPosition;
        }
    }
}