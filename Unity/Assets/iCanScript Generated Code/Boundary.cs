using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Boundary : MonoBehaviour {

        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void OnTriggerExit(Collider aColliderInfo) {
            Object.Destroy(aColliderInfo.gameObject as Object);
        }
    }
}
