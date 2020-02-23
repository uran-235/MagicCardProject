using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace main.card
{
    public class CardImageChanger : MonoBehaviour
    {
        private CardCore cardCore;
        private Image image;
        private Color originalColor;
        private readonly Color disabledColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        public void Init(Color originalColor)
        {
            this.originalColor = originalColor;
        }

        void Start()
        {
            cardCore = GetComponent<CardCore>();
            image = GetComponent<Image>();
            cardCore.CurrentCanUse
                .DistinctUntilChanged()
                .Subscribe(canUse =>
                {
                    if (canUse)
                    {
                        image.color = originalColor;
                    }
                    else
                    {
                        image.color = originalColor - disabledColor;
                    }
                })
                .AddTo(this);
        }
    }
}
