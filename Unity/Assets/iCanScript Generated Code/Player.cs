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
            bool theB3;
            bool theB2;
            bool theJumpButton;
            Vector2 theRawAnalog1;
            var theAnalog1= iCS_GameController.GameController(out theRawAnalog1, out theJumpButton, out theB2, out theB3, maxSpeed);
            forceIntegrator.Acceleration1= jumpConfig.Update(theJumpButton);
            var theCharacter= gameObject.GetComponent("CharacterController") as CharacterController;
            var theVelocity= theCharacter.velocity;
            Vector3 theDisplacement;
            forceIntegrator.Integrate(theVelocity, out theDisplacement);
            forceIntegrator.Acceleration2= roamingConfig.Update(iCS_FromTo.ToVector(theAnalog1.x, 0f, theAnalog1.y), theVelocity, 1f);
            theCharacter.Move(theDisplacement);
            theTransform.Rotate(iCS_Math.NormalizedCross(theVelocity, Vector3.down), iCS_Math.Mul(iCS_TimeUtility.ScaleByDeltaTime(theVelocity.magnitude), 114.59f), Space.World);
        }
    }
}
