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

        //private HandCardManager handCardManager = default;
        private CardUseProvider cardUseProvider;
        private RectTransform rectTransform;

        public bool canUse;
        public int handCardIndex { private set; get; }
        public bool isNextCard { private set; get; }
        public Vector3 initializePosition { private set; get; }
        public CardInfo cardInfo;

        public void Init(int handCardIndex, Vector2 initialPosition, CardInfo cardInfo, bool isNextCard)
        {
            canUse = !isNextCard;
            this.isNextCard = isNextCard;
            this.cardInfo = cardInfo;
            this.handCardIndex = handCardIndex;
            _initialPosition.Value = initialPosition;
            if (isNextCard)
            {
                GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 1.0f);
            }
            SetCardMaterial(handCardIndex);
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            //handCardManager = GameObject.Find("Manager").GetComponent<HandCardManager>();
            cardUseProvider = GameObject.Find("Manager").GetComponent<CardUseProvider>();
            cardUseProvider.OnCardTaken
                .Where(x => x.handCardIndex < handCardIndex)
                .Subscribe(takeInfo =>
                {
                    RearrangeHandCardAsync(takeInfo.handCardIndex, this.GetCancellationTokenOnDestroy()).Forget();
                })
                .AddTo(this);
        }

        private void SetCardMaterial(int index)
        {
            var seed = Random.Range(0.0f, 1.0f);
            GetComponent<Image>().color = new Color(seed, seed, seed);
        }

        private async UniTaskVoid RearrangeHandCardAsync(int useIndex, CancellationToken token)
        {
            if (isNextCard)
            {
                isNextCard = false;
                canUse = true;
            }
            var delay = (handCardIndex - useIndex - 1) * 100;
            await UniTask.Delay(delay, cancellationToken: token);

            handCardIndex--;
            _initialPosition.Value -= new Vector2(rectTransform.sizeDelta.x, 0);
        }
    }
}