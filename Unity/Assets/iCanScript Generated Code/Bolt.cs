using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Bolt : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  float velocity= 20f;


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Start() {
            var theTransform= GetComponent<Transform>();
            var theRigidbody= GetComponent<Rigidbody>();
            theRigidbody.velocity= iCS_Math.ScaleVector(velocity, theTransform.forward);
        }
    }
}
