using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class GameObject : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  GameObject owner= default(GameObject);
        [iCS_InOutPort]
        public  Object paddlePrefab= default(Object);
        [iCS_InOutPort]
        public  Object brickPrefab= default(Object);
        [iCS_InOutPort]
        public  Object brick= default(Object);

        // =========================================================================
        // PRIVATE FIELDS
        // -------------------------------------------------------------------------
        private  Object p_brick= default(Object);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Update() {
            Transform theAClonePaddleTR;
            GameObject theAClonePaddle;
            this.PublicFunction(out theAClonePaddle, out theAClonePaddleTR);
        }

        [iCS_Function]
        public  void PublicFunction(out GameObject aClonePaddle, out Transform aClonePaddleTR) {
            var theTransform= owner.transform;
            var theNoRotation= Quaternion.identity;
            var thePosition= theTransform.position;
            p_brick= Object.Instantiate(brickPrefab, thePosition, theNoRotation);
            var thePaddle= Object.Instantiate(paddlePrefab, thePosition, theNoRotation) as GameObject;
            var theTransform_25= thePaddle.transform;
            aClonePaddleTR= theTransform_25;
            aClonePaddle= thePaddle;
        }
    }
}
