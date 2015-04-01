using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class IfDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject[]  myEnablePorts= null;
        List<CodeContext>   myExecutionList= new List<CodeContext>();
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a _If_ code context.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public IfDefinition(iCS_EditorObject[] enables)
        : base(CodeType.IF) {
            myEnablePorts= enables;
        }

        // -------------------------------------------------------------------
        /// Adds an execution child.
        ///
        /// @param child The execution child to add.
        ///
        public void AddExecutableChild(CodeContext child) {
            myExecutionList.Add(child);
            child.Parent= this;
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        public override string GenerateCode(int indentSize) {
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
            foreach(var c in myExecutionList) {
                result.Append(c.GenerateCode(indentSize+1));
            }
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }
    }

}