using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace main.card
{
    public interface IUsableCard
    {
        bool Use();
        bool Release();
    }
}
