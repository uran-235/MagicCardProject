using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace main.card
{
    public class CardInitialMover : MonoBehaviour
    {
        private const float MOVE_TIME = 0.5f;

        private CardCore cardCore;
        private RectTransform rectTransform;
        private bool shouldMove = false;
        private float startTime;
        private Vector2 startPosition;
        private Vector2 endPosition;

        private void Start()
        {
            cardCore = GetComponent<CardCore>();
            rectTransform = GetComponent<RectTransform>();

            cardCore.CurrentInitialPosition
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    startTime = Time.timeSinceLevelLoad;
                    startPosition = rectTransform.localPosition;
                    endPosition = x;
                    shouldMove = true;
                    if (!cardCore.isNextCard)
                    {
                        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                })
                .AddTo(this);
        }

        private void Update()
        {
            if (shouldMove)
            {
                var diff = Time.timeSinceLevelLoad - startTime;
                if(diff > MOVE_TIME)
                {
                    rectTransform.localPosition = endPosition;
                    shouldMove = false;
                }

                var rate = diff / MOVE_TIME;

                rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, rate);
            }
        }
    }
}
