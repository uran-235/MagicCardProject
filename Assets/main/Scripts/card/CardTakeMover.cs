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
    public class CardTakeMover : MonoBehaviour, IUsableCard
    {
        private CardCore cardCore;
        private RectTransform rectTransform;
        private bool isTouch = false;

        private void Start()
        {
            cardCore = GetComponent<CardCore>();
            rectTransform = GetComponent<RectTransform>();
        }

        public bool Use()
        {
            if (cardCore.canUse)
            {
                isTouch = true;
            }
            return cardCore.canUse;
        }

        public bool Release()
        {
            if (!isTouch) return false;
            isTouch = false;
            return true;
        }

        private void Update()
        {
            if (isTouch)
            {
                var touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                rectTransform.position = (Vector2)touchPos;
            }
        }
    }
}