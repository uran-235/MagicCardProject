using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace main.card
{
    public class CardInitialMover : MonoBehaviour
    {
        private const float MOVE_SPEED = 2.0f;

        private CardCore cardCore;
        private RectTransform rectTransform;
        private Vector2 initializePosition;
        private bool shouldMove = false;

        private void Start()
        {
            cardCore = GetComponent<CardCore>();
            rectTransform = GetComponent<RectTransform>();

            cardCore.CurrentInitialPosition
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    initializePosition = x;
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
                Vector2 dist = (initializePosition - (Vector2)rectTransform.localPosition) * Time.deltaTime * MOVE_SPEED;
                rectTransform.localPosition += (Vector3)dist;
                if(dist.magnitude < 0.25f)
                {
                    rectTransform.localPosition = initializePosition;
                    shouldMove = false;
                }
            }
        }
    }
}
