using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeEngineering {

    public class InlineCodeDefinition : CodeBase {

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a statement block local variable.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created type.
        ///
        public InlineCodeDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {
        }

        // -------------------------------------------------------------------
        /// Returns a list of all enable ports that affects this function call.
        public override iCS_EditorObject[] GetRelatedEnablePorts() {
            return ControlFlow.GetAllRelatedEnablePorts(VSObject);
        }

        // -------------------------------------------------------------------
        /// Returns the list of all visual script objects this function call
        /// depends on.
        public override iCS_EditorObject[] GetDependencies() {
            return GetNodeCodeDependencies(VSObject);
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            var code= VSObject.Value;
            if(code == null) return "";
            return code as string;
//            var result= new StringBuilder(128);
//            result.Append(FunctionCallPrefix(VSObject));
//            result.Append(ToFieldOrPropertyName());
//            result.Append(GenerateReturnTypeCastFragment(VSObject));
//            return result.ToString();
        }

    }

}