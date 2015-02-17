using UnityEngine;
using System.Collections;

//namespace Subspace {
//    public class SSNodeAction {
//    
//        public enum SSActionState {
//            WAITING, DISABLED, RAN
//        };
//    
//        // ======================================================================
//        // Fields
//        // ----------------------------------------------------------------------
//        bool            myReentrancyFlag= false;
//        SSActionState   myState= SSActionState.WAITING;
//    
//        public SSActionState Evaluate() {
//            // We have already evaluated for this run cycle.
//            if(myState == SSActionState.DISABLED || myState == SSActionState.RAN) {
//                return myState;
//            }
//            // Don't allow reentrency.
//            if(myReentrancyFlag == true) {
//                return SSActionState.WAITING;
//            }
//            try {
//                myState= doEvaluate();
//            }
//            catch(System.Exception) {
//                myState= SSActionState.DISABLED;
//            }
//            myReentrancyFlag= false;
//            return myState;
//        }
//        public SSActionState doEvaluate() {
//            if(myEnable.IsReady == false) return SSActionState.WAITING;
//            if(myEnable.Value == false) return SSActionState.DISABLED;
//            if(myInput.IsReady == false) return SSActionState.WAITING;
//            Execute();
//            myPostAction.Execute();
//            return SSActionState.RAN;
//        }
//        public void Execute() {
//        
//        }
//    }
//    
//}
