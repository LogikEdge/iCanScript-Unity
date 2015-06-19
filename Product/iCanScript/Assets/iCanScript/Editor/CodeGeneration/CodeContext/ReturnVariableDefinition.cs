using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ReturnVariableDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
		public Type	myRuntimeType= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a return variable.
        ///
        /// @param vsObject Visual Script return port.
        /// @param parent The parent code context.
        /// @return The newly created return variable.
        ///
        public ReturnVariableDefinition(iCS_EditorObject vsObject, CodeBase parent)
        : base(vsObject, parent) {
            myRuntimeType= vsObject.RuntimeType;
        }
    
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
        /// Sets the runtime type of the return variable.
        ///
        /// @param newType The new runtime type for the return variable.
        ///
        public void SetRuntimeType(Type newType) {
            myRuntimeType= newType;
        }
        // -------------------------------------------------------------------
		/// Returns the runtime type of the variable.
		public override Type GetRuntimeType() {
			return myRuntimeType;
		}
		
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the return variable code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the return variable.
        ///
        public override string GenerateBody(int indentSize) {
            return "var "+Parent.GetLocalVariableName(VSObject);
        }
    }

}