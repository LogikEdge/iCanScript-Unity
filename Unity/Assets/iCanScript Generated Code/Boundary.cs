using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="My Visual Scripts")]
    public class Boundary : MonoBehaviour {

        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void OnTriggerExit(Collider colliderInfo) {
            Object.Destroy(colliderInfo.gameObject);
        }
    }
}
