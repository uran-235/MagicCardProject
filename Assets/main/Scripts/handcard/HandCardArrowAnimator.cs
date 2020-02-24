using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace main.handcard
{
    [RequireComponent(typeof(MeshRenderer))]
    public class HandCardArrowAnimator : MonoBehaviour
    {
        [SerializeField] private float ANIMATION_SPEED = 1.0f;
        void Update()
        {
            float scroll = Mathf.Repeat(Time.time * 0.2f, 1);
            Vector2 offset = new Vector2(0, scroll) * ANIMATION_SPEED;
            GetComponent<MeshRenderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}