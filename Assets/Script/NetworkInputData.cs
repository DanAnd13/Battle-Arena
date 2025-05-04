using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.InputSynchronize
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSEBUTTON0 = 1;

        public NetworkButtons buttons;
        public Vector3 Movement;
        public Vector2 MouseDelta;
    }
}
