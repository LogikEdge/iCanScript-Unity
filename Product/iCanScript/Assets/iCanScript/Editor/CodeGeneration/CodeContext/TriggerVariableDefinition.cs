using UnityEngine;
using System.Text;
using System.Collections;
using iCanScript.Internal.Engine;


namespace iCanScript.Internal.Editor.CodeGeneration {

    public class TriggerVariableDefinition : VariableDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
		TriggerSetDefinition	myTriggerSet= null;
		
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an output parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public TriggerVariableDefinition(iCS_EditorObject port, CodeBase parent, TriggerSetDefinition triggerSet)
            : base(port, parent, AccessSpecifier.Private, ScopeSpecifier.NonStatic) {
        		myTriggerSet= triggerSet;
            }

        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			if(Parent == myTriggerSet.Parent) {
				var executionBlock= Parent as ExecutionBlockDefinition;
				if(executionBlock != null) {
					executionBlock.Remove(myTriggerSet);
					executionBlock.Remove(this);
				}
			}
		}

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        public override string GenerateCode(int indentSize) {
            var result= new StringBuilder(128);
            result.Append(GenerateHeader(indentSize));
            result.Append(GenerateBody(indentSize+1));
            result.Append(GenerateTrailer(indentSize));
            return result.ToString();
        }
        // -------------------------------------------------------------------
        /// Generate the enable block header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the if-statement.
        ///
        public override string GenerateHeader(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 128);
            if(iCS_Types.IsA<ClassDefinition>(Parent.GetType())) {
                result.Append("private ");
            }
            result.Append("bool ");
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the execution list code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the if-statement.
        ///
        public override string GenerateBody(int indentSize) {
            if(iCS_Types.IsA<ClassDefinition>(Parent.GetType())) {
                return Parent.GetPrivateFieldName(VSObject);
            }
            return Parent.GetLocalVariableName(VSObject);
        }

        // -------------------------------------------------------------------
        /// Generate the enable block trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the if-statement.
        ///
        public override string GenerateTrailer(int indentSize) {
            return "= false;\n";
        }
    }

}