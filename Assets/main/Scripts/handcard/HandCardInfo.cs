using main.card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace main.handcard
{
    public struct HandCardInfo
    {
        CardTakeMover cardTakeMover;
        int cardIndex;

        public HandCardInfo(CardTakeMover cardTakeMover, int cardIndex)
        {
            this.cardTakeMover = cardTakeMover;
            this.cardIndex = cardIndex;
        }
    }
}
