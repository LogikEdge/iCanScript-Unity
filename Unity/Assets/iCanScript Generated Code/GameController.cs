using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class GameController : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  float xMin= -6f;
        [iCS_InOutPort]
        public  float xMax= 6f;
        [iCS_InOutPort]
        public  float y= 0f;
        [iCS_InOutPort]
        public  float z= 16f;
        [iCS_InOutPort]
        public  Object asteroid= default(Object);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Start() {
            Object.Instantiate(asteroid, iCS_FromTo.ToVector(Random.Range(xMin, xMax), y, z), new Quaternion(0f, 0f, 0f, 0f));
        }
    }
}
