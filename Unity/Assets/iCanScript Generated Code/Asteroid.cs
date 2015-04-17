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
            var theRigidbody= GetComponent<Rigidbody>();
            theRigidbody.velocity= transform.forward * velocity;
            theRigidbody.angularVelocity= Random.insideUnitSphere * rate;
        }

        [iCS_Function]
        public  void OnTriggerEnter(Collider colliderInfo) {
            var theIsBolt= colliderInfo.CompareTag("Bolt");
            bool thePlayerCollisionTrigger= false;
            if(colliderInfo.CompareTag("Player")) {
                var theTransform= colliderInfo.transform;
                Object.Instantiate(playerExplosion, theTransform.position, theTransform.rotation);
                thePlayerCollisionTrigger= true;
            }
            if(thePlayerCollisionTrigger || theIsBolt) {
                Object.Destroy(colliderInfo.gameObject);
                Object.Destroy(gameObject);
                Object.Instantiate(asteroidExplosion, transform.position, transform.rotation);
            }
        }
    }
}
