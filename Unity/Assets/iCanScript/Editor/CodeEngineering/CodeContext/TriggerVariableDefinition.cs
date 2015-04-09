using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class TriggerVariableDefinition : VariableDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an output parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public TriggerVariableDefinition(iCS_EditorObject port, CodeBase parent)
            : base(port, parent, AccessSpecifier.PRIVATE, ScopeSpecifier.NONSTATIC) {}

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
            if(iCS_Types.IsA<TypeDefinition>(Parent.GetType())) {
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
            if(iCS_Types.IsA<TypeDefinition>(Parent.GetType())) {
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