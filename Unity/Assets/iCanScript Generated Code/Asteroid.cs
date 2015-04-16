using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
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
            theRigidbody.velocity= iCS_Math.ScaleVector(velocity, theTransform.forward);
            theRigidbody.angularVelocity= iCS_Math.ScaleVector(rate, Random.insideUnitSphere);
        }

        [iCS_Function]
        public  void OnTriggerEnter(Collider aColliderInfo) {
            var theTransform= GetComponent<Transform>();
            var theIsBolt= aColliderInfo.CompareTag("Bolt");
            bool theTrigger= false;
            var theTransform_94= aColliderInfo.transform;
            if(aColliderInfo.CompareTag("Player")) {
                Object.Instantiate(playerExplosion, theTransform_94.position, theTransform_94.rotation);
                theTrigger= true;
            }
            if(theTrigger || theIsBolt) {
                Object.Destroy(aColliderInfo.gameObject);
                Object.Destroy(gameObject);
                Object.Instantiate(asteroidExplosion, theTransform.position, theTransform.rotation);
            }
        }
    }
}