using UnityEngine;
using System.Text;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeEngineering {

    public class FunctionParameterDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public FunctionParameterDefinition(iCS_EditorObject port, CodeBase parent)
        : base(port, parent) {
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
            return GenerateDefinition();
        }

        // -------------------------------------------------------------------
        /// Generate the definition for the function definition parameter.
        public string GenerateDefinition() {
            var result= new StringBuilder(64);
            if(VSObject.IsOutDataPort) {
                result.Append("out ");
            }
            result.Append(ToTypeName(VSObject.RuntimeType));
            result.Append(" ");
            result.Append(GetParameterName());
            return result.ToString();            
        }

        // -------------------------------------------------------------------
        /// Generate execution code for the function definition parameter.
        public string GenerateExpression(out Type expressionType) {
            expressionType= VSObject.RuntimeType;
            return GetParameterName();
        }

        // -------------------------------------------------------------------
        /// Generate the name for the function definition parameter.
        string GetParameterName() {
            return Parent.GetFunctionParameterName(VSObject);
        }
    }

}