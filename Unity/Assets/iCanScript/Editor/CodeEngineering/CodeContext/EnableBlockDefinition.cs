using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class EnableBlockDefinition : ExecutionBlockDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject[]  myEnablePorts= null;
        List<CodeBase>      myExecutionList= new List<CodeBase>();
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an enable code block.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public EnableBlockDefinition(CodeBase parent, iCS_EditorObject[] enables)
        : base(null, parent) {
            myEnablePorts= enables;
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
        
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the if-statement header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the if-statement.
        ///
        public override string GenerateHeader(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 1024);
            result.Append("if(");
            var len= myEnablePorts.Length;
            for(int i= 0; i < len; ++i) {
                result.Append(GetNameFor(myEnablePorts[i].FirstProducerPort));
                if(i < len-1) {
                    result.Append(" || ");
                }
            }
            result.Append(") {\n");
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the if-statement code.
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

        // -------------------------------------------------------------------
        /// Generate the if-statement trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the if-statement.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ToIndent(indentSize)+"}\n";
        }

    }

}