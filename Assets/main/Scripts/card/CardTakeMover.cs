using main.handcard;
using main.input;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace main.card
{
    public class CardTakeMover : MonoBehaviour, IUsable
    {
        private const float ENABLE_TAKE_CARD_DISTANCE = 200.0f;
        private const float ENABLE_TAKE_CARD_ANGLE = 0.75f;

        public Subject<int> _onUseSubject = new Subject<int>();
        public IObservable<int> OnCardUsed => _onUseSubject;

        private CardCore cardCore;
        private RectTransform rectTransform;
        private Vector2 startPosition;
        private bool isTouch = false;
        //private Subject<int> _onUseSubject;

        private void Start()
        {
            //_onUseSubject = GameObject.Find("Manager").GetComponent<HandCardManager>()._onUseSubject;
            cardCore = GetComponent<CardCore>();
            rectTransform = GetComponent<RectTransform>();
            _onUseSubject.AddTo(this);
        }

        public void Use()
        {
            if (cardCore.canUse)
            {
                isTouch = true;
                startPosition = rectTransform.localPosition;
            }
        }

        public void Release()
        {
            isTouch = false;
            ChangeTakeState();
        }

        private void Update()
        {
            if (isTouch)
            {
                var touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                rectTransform.position = (Vector2)touchPos;
            }
        }

        private void ChangeTakeState()
        {
            if(Vector2.Distance(startPosition, rectTransform.localPosition) > ENABLE_TAKE_CARD_DISTANCE)
            {
                var diff = Vector2.Dot(((Vector2)rectTransform.localPosition - startPosition).normalized, new Vector2(0, 1));
                if ( diff < -1.0f * ENABLE_TAKE_CARD_ANGLE)
                {
                    //ThrowAway;
                }
                else if(diff > ENABLE_TAKE_CARD_ANGLE)
                {
                    _onUseSubject.OnNext(cardCore.handCardIndex);
                    Destroy(gameObject);
                }
                else
                {
                    rectTransform.localPosition = startPosition;
                }
            }
            else
            {
                rectTransform.localPosition = startPosition;
            }
        }
    }
}