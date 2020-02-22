using main.card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace main.handcard
{
    public class CardComboSystem : MonoBehaviour
    {
        private CardType beforeCardType = CardType.Fire;

        public bool isChainCombo(CardType cardType)
        {
            return cardType <= beforeCardType;
        }
    }
}