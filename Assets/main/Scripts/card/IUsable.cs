using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace main.card
{
    public interface IUsable
    {
        IObservable<int> OnCardUsed { get; }
        void Use();
        void Release();
    }
}
