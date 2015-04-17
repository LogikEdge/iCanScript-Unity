using System;
using UnityEngine;
using UnityEngine.UI;

namespace iCanScript.Engine.GeneratedCode {

    [iCS_Class(Library="My Visual Scripts")]
    public class GameManager : MonoBehaviour {
        // =========================================================================
        // PUBLIC FIELDS
        // -------------------------------------------------------------------------
        [iCS_InOutPort]
        public  Int lives=  new Int(3);
        [iCS_InOutPort]
        public  Int bricks=  new Int(20);
        [iCS_InOutPort]
        public  string partOne= "Lives: ";
        [iCS_InOutPort]
        public  UnityEngine.Object deathParticles= default(UnityEngine.Object);
        [iCS_InOutPort]
        public  Text livesText= default(Text);
        [iCS_InOutPort]
        public  UnityEngine.Object bricksPrefab= default(UnityEngine.Object);
        [iCS_InOutPort]
        public  UnityEngine.Object paddlePrefab= default(UnityEngine.Object);
        [iCS_InOutPort]
        public  GameObject youWon= default(GameObject);
        [iCS_InOutPort]
        public  GameObject gameOver= default(GameObject);

        // =========================================================================
        // PRIVATE FIELDS
        // -------------------------------------------------------------------------
        private  GameObject p_clonePaddle= default(GameObject);


        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        public  void LooseLives() {
            livesText.text= string.Concat(partOne, iCS_TypeCasts.ToString(lives.SubAndUpdate(1)));
            var theTransform= p_clonePaddle.transform;
            UnityEngine.Object.Instantiate(deathParticles, theTransform.position, new Quaternion(0f, 0f, 0f, 0f));
            Network.Destroy(p_clonePaddle);
        }

        [iCS_Function]
        public  void Setup() {
            var theNoRotation= Quaternion.identity;
            var theGMTransformPos= transform.position;
            p_clonePaddle= UnityEngine.Object.Instantiate(paddlePrefab, theGMTransformPos, theNoRotation) as GameObject;
            UnityEngine.Object.Instantiate(bricksPrefab, theGMTransformPos, theNoRotation);
        }

        [iCS_Function]
        public  void CheckGameOver() {
            var theWeWin= bricks < 0;
            youWon.SetActive(theWeWin);
            var theWeLost= lives <= 1;
            if(theWeWin || theWeLost) {
                Reset();
            }
            gameOver.SetActive(theWeLost);
        }

        [iCS_Function]
        public  void Reset() {
            Application.LoadLevel(Application.loadedLevel);
        }

        [iCS_Function]
        public  void Awake() {
            Setup();
        }
    }
}
