using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Engine {
    
    public partial class iCS_VisualScriptData : iCS_IVisualScriptData {
        // =======================================================================
        // Simple Queries
        // -----------------------------------------------------------------------
        // Returns 'true' if the given engine object is valid.
        public static bool IsValid(iCS_EngineObject obj, iCS_IVisualScriptData vsd) {
            int id= obj.InstanceId;
            return id >= 0 && id < vsd.EngineObjects.Count;
        }
    
        // =======================================================================
        // General Iterations
        // -----------------------------------------------------------------------
        // Executes for each valid engine object
        public static void ForEach(Action<iCS_EngineObject> fnc, iCS_IVisualScriptData vsd) {
            P.forEach(o=> { if(IsValid(o, vsd)) fnc(o); }, vsd.EngineObjects);
        }
        // -----------------------------------------------------------------------
        // Invokes the given action if the given condition has passed the test.
        public static void FilterWith(Func<iCS_EngineObject,bool> cond, Action<iCS_EngineObject> action, iCS_IVisualScriptData vsd) {
            ForEach( o=> { if(cond(o)) action(o); }, vsd);
        }
    }

}
