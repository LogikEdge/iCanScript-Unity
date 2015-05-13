using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Engine {
    
    public static class iCS_Strings {
        // -----------------------------------------------------------------------
        // Common
        public const string DefaultFunctionReturnName= "out";
    
        // -----------------------------------------------------------------------
        // Behaviour Allowed Children
        public const string Start       = "Start";
        public const string Update      = "Update";
        public const string LateUpdate  = "LateUpdate";
        public const string FixedUpdate = "FixedUpdate";
        public const string OnGUI       = "OnGUI";
        public const string OnDrawGizmos= "OnDrawGizmos";
    
        // -----------------------------------------------------------------------
        // State Allowed Children
        public const string OnEntry = "OnEntry";
        public const string OnUpdate= "OnUpdate";
        public const string OnExit  = "OnExit";
    
        // -----------------------------------------------------------------------
        // Special node names.
        public const string TransitionEntryModule= "TransitionEntry";
        public const string TransitionExitModule = "TransitionExit";

        // -----------------------------------------------------------------------
        // Control port names.
        public const string EnablePort= "enable";
        public const string TriggerPort= "trigger";
    
        // -----------------------------------------------------------------------
        // Reflection methods
        public const string AddChildMethod   = "AddChild";
        public const string RemoveChildMethod= "RemoveChild";
    
        // -----------------------------------------------------------------------
        public static bool IsEmpty(string s)       { return s == null || s ==""; }
        public static bool IsNotEmpty(string s)    { return !IsEmpty(s); }
    }

}
