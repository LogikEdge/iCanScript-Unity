using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ExecutionBlockDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        public List<CodeBase>    myExecutionList= new List<CodeBase>();

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an execution code block.
        ///
        /// @param vsObject The visual script objects associated with this code.
        /// @param codeBlock The code block this assignment belongs to.
        /// @return The newly created code context.
        ///
        public ExecutionBlockDefinition(iCS_EditorObject vsObject, CodeBase parent)
        : base(vsObject, parent) {
        }

        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			foreach(var e in myExecutionList.ToArray()) {
				e.ResolveDependencies();
			}
		}

        // -------------------------------------------------------------------
        /// Adds an execution child.
        ///
        /// @param child The execution child to add.
        ///
        public override void AddExecutable(CodeBase child) {
            myExecutionList.Add(child);
            child.Parent= this;
        }

        // -------------------------------------------------------------------
        /// Removes a code context from the function.
        ///
        /// @param toRemove The code context to be removed.
        ///
        public override void Remove(CodeBase toRemove) {
            if(myExecutionList.Remove(toRemove)) {
                toRemove.Parent= null;
            }
        }
                
        // -------------------------------------------------------------------
        /// Replace the given code element by a list of elements
        ///
        /// @param toReplace The code to be replaced.
        /// @param theCodeList The list to be added.
        ///
        public void Replace(CodeBase toReplace, List<CodeBase> theCodeList) {
            var idx= myExecutionList.IndexOf(toReplace);
            if(idx >= 0) {
                myExecutionList.RemoveAt(idx);
                myExecutionList.InsertRange(idx, theCodeList);
                foreach(var c in theCodeList) {
                    c.Parent= this;
                }
            }
            else {
                Debug.LogWarning("iCanScript: Internal Error: Unable to replace execution list.");
                foreach(var cb in myExecutionList) {
                    if(cb.VSObject != null) {
                        Debug.Log("node: "+cb.VSObject.FullName);
                    }
                 }
            }
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the execution list code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the if-statement.
        ///
        public override string GenerateBody(int indentSize) {
            var result= new StringBuilder(1024);
            foreach(var c in myExecutionList) {
                result.Append(c.GenerateCode(indentSize));
            }
            return result.ToString();
        }
        
    }

}