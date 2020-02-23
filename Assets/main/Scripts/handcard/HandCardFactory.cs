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

        private readonly Color[] CARD_COLOR_TYPE = new Color[3] { new Color(0.6f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.6f, 1.0f), new Color(0.6f, 1.0f, 0.0f, 1.0f) };

        public GameObject CreateHandCard(int index, int maxCardNum)
        {
            var handCardObj = Instantiate(cardPrefab, cardInitializeRectTransform.position, Quaternion.identity, cardInitializeRectTransform.parent);
            var initialPos = new Vector2(cardInitializeRectTransform.localPosition.x - cardPrefab.GetComponent<RectTransform>().sizeDelta.x * (maxCardNum - index), cardInitializeRectTransform.localPosition.y);
            var isNextCard = index == maxCardNum;
            var cardInfo = new CardInfo { cardType = CreateCardType() };
            handCardObj.GetComponent<CardCore>().Init(index, initialPos, cardInfo, isNextCard);
            handCardObj.GetComponent<CardImageChanger>().Init(CARD_COLOR_TYPE[(int)cardInfo.cardType]);
            return handCardObj;
        }

        private CardType CreateCardType()
        {
            switch (Random.Range(0, 3))
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
