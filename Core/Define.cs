using System.Collections.Generic;
using UnityEngine;

namespace PJH.Core
{
    public static class Define
    {
        public enum EScene
        {
            Unknown,
            Title,
            InGame,
            InGame_PJH,
            InGame_KHJ,
            PlayTestScene_LJS
        }

        public static class MLayerMask
        {
            public static readonly LayerMask WhatIsWall = LayerMask.GetMask("Wall");
            public static readonly LayerMask WhatIsEnemy = LayerMask.GetMask("Enemy");
        }
    }
}