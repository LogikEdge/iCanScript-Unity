using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Player : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  float speed= 10f;
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
        private  Vector2 p_analog1= default(Vector2);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Update() {
            p_analog1= iCS_Math.Scale2Vector(Time.deltaTime, speed, iCS_FromTo.ToVector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }

        [iCS_Function]
        public  void FixedUpdate() {
            var theRigidbody= GetComponent<Rigidbody>();
            var thePosition= theRigidbody.position;
            var theVelocity= theRigidbody.velocity;
            theRigidbody.velocity= iCS_FromTo.ToVector(p_analog1.x, 0f, p_analog1.y);
            theRigidbody.position= iCS_FromTo.ToVector(Mathf.Clamp(thePosition.x, left, right), thePosition.y, Mathf.Clamp(thePosition.z, bottom, top));
            theRigidbody.rotation= Quaternion.Euler(0f, 0f, iCS_Math.Mul(theVelocity.x, tiltFactor));
        }
    }
}
