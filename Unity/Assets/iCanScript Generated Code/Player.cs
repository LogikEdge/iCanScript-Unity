using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="Visual Scripts")]
    public class Player : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  iCS_ImpulseForceGenerator jumpConfig=  new iCS_ImpulseForceGenerator(Vector3.up, 0.2f, new Vector3(0f, 5f, 0f), 0.3f, 1.2f, 10f);
        [iCS_InOutPort]
        public  iCS_DesiredVelocityForceGenerator roamingConfig=  new iCS_DesiredVelocityForceGenerator(25f, 35f, new Vector3(1f, 0f, 1f));
        [iCS_InOutPort]
        public  iCS_ForceIntegrator forceIntegrator=  new iCS_ForceIntegrator(new Vector3(0f, -20f, 0f), 1f, 0.995f);
        [iCS_InOutPort]
        public  float maxSpeed= 10f;


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Update() {
            var theTransform= GetComponent<Transform>();
            Vector2 theRawAnalog1;
            bool theJump;
            bool theB2;
            bool theB3;
            var theAnalog1= iCS_GameController.GameController(out theRawAnalog1, out theJump, out theB2, out theB3, maxSpeed);
            var theX= theAnalog1.x;
            var theY= theAnalog1.y;
            var theVelocity= iCS_FromTo.ToVector(theX, 0f, theY);
            var theAcceleration= jumpConfig.Update(theJump);
            forceIntegrator.Acceleration1= theAcceleration;
            var theOutput= gameObject.GetComponent("CharacterController") as CharacterController;
            var theVelocity_101= theOutput.velocity;
            var theAcceleration_60= roamingConfig.Update(theVelocity, theVelocity_101, 1f);
            forceIntegrator.Acceleration2= theAcceleration_60;
            Vector3 theDisplacement;
            forceIntegrator.Integrate(theVelocity_101, out theDisplacement);
            theOutput.Move(theDisplacement);
            var theMagnitude= theVelocity_101.magnitude;
            var theOutput_138= iCS_Math.NormalizedCross(theVelocity_101, Vector3.down);
            var theValueTimesDt= iCS_TimeUtility.ScaleByDeltaTime(theMagnitude);
            var theOutput_130= iCS_Math.Mul(theValueTimesDt, 114.59f);
            theTransform.Rotate(theOutput_138, theOutput_130, Space.World);
        }
    }
}
