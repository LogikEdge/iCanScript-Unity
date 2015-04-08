using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Player : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  iCS_PulseGenerator pulseGenerator=  new iCS_PulseGenerator();
        [iCS_InOutPort]
        public  float maxSpeed= 0f;
        [iCS_InOutPort]
        public  Object boltTemplate= default(Object);
        [iCS_InOutPort]
        public  Transform boltSpawnPosition= default(Transform);
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
        private  Vector3 p_velocity= default(Vector3);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Update() {
            var theAudioSource= GetComponent<AudioSource>();
            if(Input.GetButton("Fire1")) {
                if(pulseGenerator.GeneratePulse(0.25f, true, true)) {
                    Object.Instantiate(boltTemplate, boltSpawnPosition.position, boltSpawnPosition.rotation);
                    theAudioSource.Play();
                }
            }
            p_velocity= iCS_Math.ScaleVector(maxSpeed, iCS_FromTo.ToVector(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
        }

        [iCS_Function]
        public  void FixedUpdate() {
            var theRigidbody= GetComponent<Rigidbody>();
            var thePosition= theRigidbody.position;
            var theVelocity= theRigidbody.velocity;
            theRigidbody.velocity= p_velocity;
            theRigidbody.position= iCS_FromTo.ToVector(Mathf.Clamp(thePosition.x, left, right), thePosition.y, Mathf.Clamp(thePosition.z, bottom, top));
            theRigidbody.rotation= Quaternion.Euler(0f, 0f, iCS_Math.Mul(theVelocity.x, tiltFactor));
        }
    }
}
