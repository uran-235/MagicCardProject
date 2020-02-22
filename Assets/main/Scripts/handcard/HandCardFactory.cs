﻿using main.card;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace main.handcard
{
    public class HandCardFactory : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab = default;
        [SerializeField] private RectTransform cardInitializeRectTransform = default;

        //private const int HANDCARD_NUM = 5;

        private void Start()
        {
            //GetComponent<HandCardManager>().OnHandCardCreate
            //    .Select(index => index)
            //    .Subscribe(index => CreateHandCard(index))
            //    .AddTo(this);
        }

        public GameObject CreateHandCard(int index, int maxCardNum)
        {
            var handCardObj = Instantiate(cardPrefab, cardInitializeRectTransform.position, Quaternion.identity, cardInitializeRectTransform.parent);
            var initialPos = new Vector2(cardInitializeRectTransform.localPosition.x - cardPrefab.GetComponent<RectTransform>().sizeDelta.x * (maxCardNum - index + 1), cardInitializeRectTransform.localPosition.y);
            var isNextCard = index == maxCardNum + 1;
            handCardObj.GetComponent<CardCore>().Init(index, initialPos, isNextCard);
            return handCardObj;
        }
    }
}
