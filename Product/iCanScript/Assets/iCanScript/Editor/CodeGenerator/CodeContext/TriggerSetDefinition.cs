using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor.CodeGeneration {
    public class TriggerSetDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject    myTriggerPort= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an output parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public TriggerSetDefinition(iCS_EditorObject port, CodeBase parent)
        : base(null, parent) {
            myTriggerPort= port;
        }

        // -------------------------------------------------------------------
        /// Returns a list of all enable ports that affects this function call.
        public override iCS_EditorObject[] GetRelatedEnablePorts() {
            return ControlFlow.GetAllRelatedEnablePorts(myTriggerPort.ParentNode);
        }

        // -------------------------------------------------------------------
        /// Returns the list of all visual script objects this function call
        /// depends on.
        public override iCS_EditorObject[] GetDependencies() {
            var parentNode= myTriggerPort.ParentNode;
            if(parentNode.IsKindOfFunction) {
                return new iCS_EditorObject[1]{ parentNode };
            }
            var result= new List<iCS_EditorObject>();
            parentNode.ForEachChildRecursiveDepthFirst(
                o=> {
                    if(o.IsKindOfFunction) {
                        result.Add(o);
                    }
                }
            );
            return result.ToArray();
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        /// Generate the trigger set code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The code for the trigger set.
        ///
        public override string GenerateCode(int indentSize) {
            var result= new StringBuilder(ToIndent(indentSize),128);
            result.Append(GetNameFor(myTriggerPort));
            result.Append("= true;\n");
            return result.ToString();
        }
    }
}