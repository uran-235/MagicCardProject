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

        public GameObject CreateHandCard(int index, int maxCardNum)
        {
            var handCardObj = Instantiate(cardPrefab, cardInitializeRectTransform.position, Quaternion.identity, cardInitializeRectTransform.parent);
            var initialPos = new Vector2(cardInitializeRectTransform.localPosition.x - cardPrefab.GetComponent<RectTransform>().sizeDelta.x * (maxCardNum - index + 1), cardInitializeRectTransform.localPosition.y);
            var isNextCard = index == maxCardNum + 1;
            var cardInfo = new CardInfo { cardType = CreateCardType() };
            handCardObj.GetComponent<CardCore>().Init(index, initialPos, cardInfo, isNextCard);
            return handCardObj;
        }

        private CardType CreateCardType()
        {
            switch (Random.Range(0, 2))
            {
                case 2:
                    return CardType.Thunder;
                case 1:
                    return CardType.Water;
                default:
                    return CardType.Fire;
            }
        }
    }
}
