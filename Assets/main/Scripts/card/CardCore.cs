using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using System.Linq;
using main.handcard;
using UnityEngine.UI;
using main.input;

namespace main.card
{
    public class CardCore : MonoBehaviour
    {
        private readonly ReactiveProperty<Vector2> _initialPosition = new ReactiveProperty<Vector2>();
        public IReadOnlyReactiveProperty<Vector2> CurrentInitialPosition => _initialPosition;
        
        private readonly BoolReactiveProperty _canUse = new BoolReactiveProperty();
        public IReadOnlyReactiveProperty<bool> CurrentCanUse => _canUse;

        private HandCardManager handCardManager;
        private CardUseProvider cardUseProvider;
        private RectTransform rectTransform;

        //public bool canUse { private set; get; }
        public int handCardIndex { private set; get; }
        public bool isNextCard { private set; get; }
        public Vector3 initializePosition { private set; get; }
        public CardInfo cardInfo { private set; get; }

        public void Init(int handCardIndex, Vector2 initialPosition, CardInfo cardInfo, bool isNextCard)
        {
            _canUse.Value = !isNextCard;
            this.isNextCard = isNextCard;
            this.cardInfo = cardInfo;
            this.handCardIndex = handCardIndex;
            _initialPosition.Value = initialPosition;
            if (isNextCard)
            {
                GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 1.0f);
            }
            //SetCardMaterial(cardInfo.cardType);
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            handCardManager = GameObject.Find("Manager").GetComponent<HandCardManager>();
            handCardManager.HandCardEnabled
                .DistinctUntilChanged()
                .Where(_ => !isNextCard)
                .Subscribe(x => 
                    {
                        _canUse.Value = x;
                        //if (x)
                        //{
                        //    SetCardMaterial(cardInfo.cardType);
                        //}
                    })
                .AddTo(this);

            cardUseProvider = GameObject.Find("Manager").GetComponent<CardUseProvider>();
            cardUseProvider.OnCardTaken
                .Where(x => x.handCardIndex < handCardIndex)
                .Subscribe(takeInfo =>
                {
                    RearrangeHandCardAsync(takeInfo, this.GetCancellationTokenOnDestroy()).Forget();
                })
                .AddTo(this);

            cardUseProvider.OnCardTaken
                .Where(_ => !isNextCard && _canUse.Value)
                .Subscribe(takeInfo =>
                {
                    if ((int)takeInfo.cardInfo.cardType > (int)this.cardInfo.cardType)
                    {
                        _canUse.Value = false;
                        //GetComponent<Image>().color -= new Color(0.0f, 0.0f, 0.0f, 0.5f);
                    }
                })
                .AddTo(this);
        }

        //private void SetCardMaterial(CardType type)
        //{
        //    GetComponent<Image>().color = CARD_COLOR_TYPE[(int)type];
        //}

        private async UniTaskVoid RearrangeHandCardAsync(TakeInfo takeInfo, CancellationToken token)
        {
            if (isNextCard)
            {
                isNextCard = false;
                if ((int)takeInfo.cardInfo.cardType <= (int)this.cardInfo.cardType) _canUse.Value = true;
                //canUse = true;
            }
            var delay = (handCardIndex - takeInfo.handCardIndex - 1) * 100;
            await UniTask.Delay(delay, cancellationToken: token);

            handCardIndex--;
            _initialPosition.Value -= new Vector2(rectTransform.sizeDelta.x, 0);
        }
    }
}