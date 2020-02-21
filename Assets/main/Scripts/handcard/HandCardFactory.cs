using main.card;
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

        private const int HANDCARD_NUM = 5;

        private void Start()
        {
            GetComponent<HandCardManager>().OnHandCardCreate
                .Select(index => index)
                .Subscribe(index => CreateHandCard(index))
                .AddTo(this);
        }

        private void CreateHandCard(int index)
        {
            var handCardObj = Instantiate(cardPrefab, cardInitializeRectTransform.position, Quaternion.identity, cardInitializeRectTransform.parent);
            var initialPos = new Vector2(cardInitializeRectTransform.localPosition.x - cardPrefab.GetComponent<RectTransform>().sizeDelta.x * (HANDCARD_NUM - index + 1), cardInitializeRectTransform.localPosition.y);
            var isNextCard = index == HANDCARD_NUM + 1;
            handCardObj.GetComponent<CardCore>().Init(index, initialPos, isNextCard);
        }
    }
}
