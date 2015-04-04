using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class ParameterDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        Type    myType= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public ParameterDefinition(iCS_EditorObject port, CodeContext parent, Type neededType= null)
        : base(CodeType.PARAMETER, port, parent) {
            myType= neededType;    
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the parameter code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the parameter.
        ///
        public override string GenerateBody(int indentSize) {
            var result= new StringBuilder(GetNameFor(VSObject), 64);
            if(myType != null) {
                var producerTypeName= ToTypeName(VSObject.RuntimeType);
                var desiredTypeName= ToTypeName(myType);
                if(producerTypeName != desiredTypeName) {
                    result.Append(" as ");
                    result.Append(desiredTypeName);
                }
            }
            return result.ToString();
        }

    }

}