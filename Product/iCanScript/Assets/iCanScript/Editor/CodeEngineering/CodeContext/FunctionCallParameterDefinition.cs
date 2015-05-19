using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeEngineering {

    public class FunctionCallParameterDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        Type    myType= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a parameter definition.
        ///
        /// @param port The VS port of the parameter.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public FunctionCallParameterDefinition(iCS_EditorObject port, CodeBase parent, Type neededType= null)
        : base(port, parent) {
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
            var producerPort= CodeFlow.GetProducerPort(VSObject);
            var result= new StringBuilder(GetNameFor(producerPort), 64);
            if(myType != null) {
                var desiredTypeName= ToTypeName(myType);
                var producerType= Context.GetRuntimeTypeFor(producerPort);
                if(!iCS_Types.IsA(myType, producerType)) {
                    result.Append(" as ");
                    result.Append(desiredTypeName);
                }
            }
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the definition code for the function call parameter.
        public override string GenerateDefinition() {
            return "";            
        }

        // -------------------------------------------------------------------
        /// Generate execution code for the function call parameter.
        public override string GenerateExpression(out Type expressionType) {
            var producerPort= CodeFlow.GetProducerPort(VSObject);
			var producerCode= Context.GetCodeFor(producerPort);
			expressionType= producerPort.RuntimeType;
			if(producerCode == null) {
				Debug.LogWarning("iCanScript: Internal error: Unable to find code for: "+producerPort.FullName);
				return "";
			}
			Type producerType;
			var producerExpression= producerCode.GenerateExpression(out producerType);
			if(!iCS_Types.IsA(expressionType, producerType)) {
				producerExpression= "("+producerExpression+" as "+ToTypeName(expressionType)+")";
			}
			return producerExpression;
        }

    }

}