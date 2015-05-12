using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    public static class iCS_ObjectTooltips {
        public const string OnEntry=
            "OnEntry is called when the state is first activated before OnUpdate is called.";
        public const string OnUpdate=
            "OnUpdate is called on every frame when the state is active.";
        public const string OnExit=
            "OnExit is called when leaving the state before OnEntry is called on the newly active states.";
    }
    
}
