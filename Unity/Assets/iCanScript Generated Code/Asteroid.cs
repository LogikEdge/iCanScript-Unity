using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="My Visual Scripts")]
    public class Asteroid : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  float velocity= -5f;
        [iCS_InOutPort]
        public  float rate= 5f;
        [iCS_InOutPort]
        public  Object playerExplosion= default(Object);
        [iCS_InOutPort]
        public  Object asteroidExplosion= default(Object);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Start() {
            var theTransform= GetComponent<Transform>();
            var theRigidbody= GetComponent<Rigidbody>();
            theRigidbody.velocity= theTransform.forward * velocity;
            theRigidbody.angularVelocity= Random.insideUnitSphere * rate;
        }

        [iCS_Function]
        public  void OnTriggerEnter(Collider colliderInfo) {
            var theTransform= GetComponent<Transform>();
            var theIsBolt= colliderInfo.CompareTag("Bolt");
            bool theTrigger= false;
            var theTransform_94= colliderInfo.transform;
            if(colliderInfo.CompareTag("Player")) {
                Object.Instantiate(playerExplosion, theTransform_94.position, theTransform_94.rotation);
                theTrigger= true;
            }
            if(theTrigger || theIsBolt) {
                Object.Destroy(colliderInfo.gameObject);
                Object.Destroy(gameObject);
                Object.Instantiate(asteroidExplosion, theTransform.position, theTransform.rotation);
            }
        }
    }
}
