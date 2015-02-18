using UnityEngine;
using System;
using System.Collections;

namespace Subspace {

    public class SSInvoke {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        // .NET Signature
        object      myThis      = null;  
        object[]    myParameters= null;
        object      myReturn    = null;

        // Function to invoke
        Func<object, object[], object>  myFnc= null;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public object This {
            get { return myThis; }
            set { myThis= value; }
        }
        public object Return {
            get { return myReturn; }
            set { myReturn= value; }
        }
        public object this[int idx] {
            get { return myParameters[idx]; }
            set { myParameters[idx]= value; }
        }
        public object[] Parameters {
            get { return myParameters; }
        }

        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSInvoke(Func<object, object[], object> fnc, int nbOfParameters) {
            myFnc= fnc;
            myParameters = new object[nbOfParameters];
            for(int i= 0; i < nbOfParameters; ++i) {
                myParameters[i]= null;
            }
        }

        // ======================================================================
        // Execution
        // ----------------------------------------------------------------------
        public void Execute() {
            myReturn= myFnc(myThis, myParameters);
        }
    }
    
}
