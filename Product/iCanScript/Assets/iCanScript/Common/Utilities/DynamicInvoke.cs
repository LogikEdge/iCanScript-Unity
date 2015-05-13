using UnityEngine;
using System.Reflection;
using System.Collections;

namespace iCanScript.Internal {
    
    public class DynamicInvoke {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        MethodInfo  myMethodInfo= null;     ///< The .NET reference to the function
    
        // ======================================================================
        // Constructors/Destructors
        // ----------------------------------------------------------------------
        /// Builds a DynamicInvoke object for the given class name and function
        /// name.
        ///
        /// @param typeString The combination of _'namespace.type'_ that uniquely
        ///                   identifies the type.
        /// @param functionName The name of the function to invoke.
        ///
        public DynamicInvoke(string typeString, string functionName) {
            myMethodInfo= iCS_Types.FindFunction(typeString, functionName);
        }


        // ======================================================================
        // Invocation
        // ----------------------------------------------------------------------
        /// Invokes the dynamic function without a _'this'_ and without parameters.
        /// @return Returns the function return value or null if function is not found.
        public System.Object Invoke() {
            if(myMethodInfo == null) return null;
            return myMethodInfo.Invoke(null, new System.Object[0]);
        }
        /// Invokes the dynamic function without parameters.
        /// @param theThis  The _'this'_ of the function.
        /// @return         Returns the function return value or null if function is not found.
        public System.Object Invoke(System.Object theThis) {
            if(myMethodInfo == null) return null;
            return myMethodInfo.Invoke(theThis, new System.Object[0]);        
        }
        /// Invokes the dynamic function.
        /// @param theThis  The _'this'_ of the function.
        /// @param arg1     First argument of the function.
        /// @return         Returns the function return value or null if function is not found.
        public System.Object Invoke(System.Object theThis, System.Object arg1) {
            if(myMethodInfo == null) return null;
            return myMethodInfo.Invoke(theThis, new System.Object[1]{arg1});                
        }
        /// Invokes the dynamic function.
        /// @param theThis  The _'this'_ of the function.
        /// @param arg1     First argument of the function.
        /// @param arg2     Second argument of the function.
        /// @return         Returns the function return value or null if function is not found.
        public System.Object Invoke(System.Object theThis, System.Object arg1, System.Object arg2) {
            if(myMethodInfo == null) return null;
            return myMethodInfo.Invoke(theThis, new System.Object[2]{arg1, arg2});                        
        }
        /// Invokes the dynamic function.
        /// @param theThis  The _'this'_ of the function.
        /// @param arg1     First argument of the function.
        /// @param arg2     Second argument of the function.
        /// @param arg3     Third argument of the function.
        /// @return         Returns the function return value or null if function is not found.
        public System.Object Invoke(System.Object theThis, System.Object arg1, System.Object arg2, System.Object arg3) {
            if(myMethodInfo == null) return null;
            return myMethodInfo.Invoke(theThis, new System.Object[3]{arg1, arg2, arg3});                                
        }
    }

}
