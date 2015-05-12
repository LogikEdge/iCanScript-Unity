using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ======================================================================
        // Events
        // ----------------------------------------------------------------------
        public static Action<iCS_EditorObject>  OnCreated        = null;
        public static Action<iCS_EditorObject>  OnWillDestroy    = null;
        public static Action<iCS_EditorObject>  OnNodeCreated    = null;
        public static Action<iCS_EditorObject>  OnWillDestroyNode= null;
        public static Action<iCS_EditorObject>  OnPortCreated    = null;
        public static Action<iCS_EditorObject>  OnWillDestroyPort= null;
    
        // ======================================================================
        // Install events
        // ----------------------------------------------------------------------
        static iCS_EditorObject() {
            OnCreated     = RunOnNodeCreated;
            OnCreated    += RunOnPortCreated;
            OnWillDestroy = RunOnWillDestroyNode;
            OnWillDestroy+= RunOnWillDestroyPort;
        }
    
        // ======================================================================
        // Run Events
        // ----------------------------------------------------------------------
        public static void RunOnCreated(iCS_EditorObject obj) {
            if(OnCreated == null) return;
            foreach(Action<iCS_EditorObject> handler in OnCreated.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
        // ----------------------------------------------------------------------
        public static void RunOnWillDestroy(iCS_EditorObject obj) {
            if(OnWillDestroy == null) return;
            foreach(Action<iCS_EditorObject> handler in OnWillDestroy.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
        // ----------------------------------------------------------------------
        public static void RunOnNodeCreated(iCS_EditorObject obj) {
            if(!obj.IsNode || OnNodeCreated == null) return;
            foreach(Action<iCS_EditorObject> handler in OnNodeCreated.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
        // ----------------------------------------------------------------------
        public static void RunOnWillDestroyNode(iCS_EditorObject obj) {
            if(!obj.IsNode || OnWillDestroyNode == null) return;
            foreach(Action<iCS_EditorObject> handler in OnWillDestroyNode.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
        // ----------------------------------------------------------------------
        public static void RunOnPortCreated(iCS_EditorObject obj) {
            if(!obj.IsPort || OnPortCreated == null) return;
            foreach(Action<iCS_EditorObject> handler in OnPortCreated.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
        // ----------------------------------------------------------------------
        public static void RunOnWillDestroyPort(iCS_EditorObject obj) {
            if(!obj.IsPort || OnWillDestroyPort == null) return;
            foreach(Action<iCS_EditorObject> handler in OnWillDestroyPort.GetInvocationList()) {
                try {
                    handler(obj);
                }
                catch(Exception) {}
            }
        }
    }
}
