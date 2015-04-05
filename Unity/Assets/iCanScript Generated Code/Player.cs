using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Player : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  float left= -6f;
        [iCS_InOutPort]
        public  float right= 6f;
        [iCS_InOutPort]
        public  float bottom= -4f;
        [iCS_InOutPort]
        public  float top= 8f;
        [iCS_InOutPort]
        public  float tiltFactor= -4f;

        // =========================================================================
        // PRIVATE FIELDS
        // -------------------------------------------------------------------------
        private  Vector2 p_scaledAnalog1= default(Vector2);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Update() {
            bool theB3;
            bool theB2;
            bool theB1;
            Vector2 theAnalog1;
            p_scaledAnalog1= iCS_GameController.GameController(out theAnalog1, out theB1, out theB2, out theB3, 10f);
        }

        [iCS_Function]
        public  void FixedUpdate() {
            var theRigidbody= GetComponent<Rigidbody>();
            var thePosition= theRigidbody.position;
            var theVelocity= theRigidbody.velocity;
            float theY;
            float theX;
            iCS_FromTo.FromVector(p_scaledAnalog1, out theX, out theY);
            theRigidbody.velocity= iCS_FromTo.ToVector(theX, 0f, theY);
            theRigidbody.position= iCS_FromTo.ToVector(Mathf.Clamp(thePosition.x, left, right), thePosition.y, Mathf.Clamp(thePosition.z, bottom, top));
            theRigidbody.rotation= Quaternion.Euler(0f, 0f, iCS_Math.Mul(theVelocity.x, tiltFactor));
        }
    }
}
