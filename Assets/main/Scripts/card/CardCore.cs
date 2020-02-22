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

namespace main.card
{
    public class CardCore : MonoBehaviour
    {
        private readonly ReactiveProperty<Vector2> _initialPosition = new ReactiveProperty<Vector2>();
        public IReadOnlyReactiveProperty<Vector2> CurrentInitialPosition => _initialPosition;

        private HandCardManager handCardManager = default;
        private RectTransform rectTransform;
        public int handCardIndex { private set; get; }
        public bool isNextCard { private set; get; }
        public bool canUse;
        public Vector3 initializePosition { private set; get; }

        public void Init(int handCardIndex, Vector2 initialPosition, bool isNextCard)
        {
            canUse = !isNextCard;
            this.isNextCard = isNextCard;
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
            handCardManager = GameObject.Find("Manager").GetComponent<HandCardManager>();
            handCardManager.OnAnyCardUsed
                .Where(x => x < handCardIndex)
                .Subscribe(index =>
                {
                    RearrangeHandCardAsync(index, this.GetCancellationTokenOnDestroy()).Forget();
                });
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