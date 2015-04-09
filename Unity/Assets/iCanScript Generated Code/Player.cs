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
        public  Object bolt= default(Object);
        [iCS_InOutPort]
        public  Transform boltSpawnPoint= default(Transform);
        [iCS_InOutPort]
        public  float maxSpeed= 10f;
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
            p_velocity= maxSpeed * new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if(Input.GetButton("Fire1")) {
                if(pulseGenerator.GeneratePulse(0.25f, true, true)) {
                    Object.Instantiate(bolt, boltSpawnPoint.position, boltSpawnPoint.rotation);
                    theAudioSource.Play();
                }
            }
        }

        [iCS_Function]
        public  void FixedUpdate() {
            var theRigidbody= GetComponent<Rigidbody>();
            theRigidbody.velocity= p_velocity;
            var thePosition= theRigidbody.position;
            var theVelocity= theRigidbody.velocity;
            theRigidbody.position= new Vector3(Mathf.Clamp(thePosition.x, left, right), thePosition.y, Mathf.Clamp(thePosition.z, bottom, top));
            theRigidbody.rotation= Quaternion.Euler(0f, 0f, theVelocity.x * tiltFactor);
        }
    }
}
