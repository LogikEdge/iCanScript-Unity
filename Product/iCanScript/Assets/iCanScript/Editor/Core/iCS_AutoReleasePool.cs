using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    public class iCS_AutoReleasePool {
        static List<P.Tuple<float,object>> myAutoReleasePool= new List<P.Tuple<float,object>>();
    
        public static void AutoRelease(object obj, float deltaTime= 1f) {
            if(obj != null) {
                myAutoReleasePool.Add(new Prelude.Tuple<float,object>(Time.realtimeSinceStartup+deltaTime, obj));            
            }
        } 
        public static void Update() {
            if(myAutoReleasePool.Count == 0) return;
            if(myAutoReleasePool[0].Item1 <= Time.realtimeSinceStartup) {
                object toDestroy= myAutoReleasePool[0].Item2;
                myAutoReleasePool.RemoveAt(0);
                if(toDestroy is UnityEngine.Object) {
                    UnityEngine.Object.DestroyImmediate(toDestroy as UnityEngine.Object);
                }
            }
        }
    }
    
}
