using UnityEngine;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="My Visual Scripts")]
    public class GameController : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  iCS_PulseGenerator variableSmallerThanPulseGeneratorLargerThan=  new iCS_PulseGenerator();
        [iCS_InOutPort]
        public  iCS_Oscillator variableSmallerThanOscillatorLargerThan=  new iCS_Oscillator();
        [iCS_InOutPort]
        public  float spawnWait= 0.5f;
        [iCS_InOutPort]
        public  float waveWait= 4f;
        [iCS_InOutPort]
        public  float hazardCount= 10f;
        [iCS_InOutPort]
        public  float xMin= -6f;
        [iCS_InOutPort]
        public  float xMax= 6f;
        [iCS_InOutPort]
        public  Object asteroid= default(Object);

        // =========================================================================
        // PRIVATE FIELDS
        // -------------------------------------------------------------------------
        private  float p_endTime= default(float);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void Start() {
            p_endTime= iCS_Timer.StartTimer(1f);
        }

        [iCS_Function]
        public  void Update() {
            bool theIsNotElapsed;
            var theIsElapsed= iCS_Timer.IsTimeElapsed(p_endTime, out theIsNotElapsed);
            bool theInvOut;
            var theOut= variableSmallerThanOscillatorLargerThan.Oscillate(iCS_Math.Mul(hazardCount, spawnWait), waveWait, theIsElapsed, out theInvOut);
            if(variableSmallerThanPulseGeneratorLargerThan.GeneratePulse(spawnWait, theOut, true)) {
                Object.Instantiate(asteroid, new Vector3(Random.Range(xMin, xMax), 0f, 16f), new Quaternion(0f, 0f, 0f, 0f));
            }
        }
    }
}
