using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class IfStatementDefinition : CodeContext {
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
        public IfStatementDefinition(iCS_EditorObject[] enables)
        : base(CodeType.IF) {
            myEnablePorts= enables;
        }

        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			foreach(var e in myExecutionList) {
				e.ResolveDependencies();
			}
		}

        // -------------------------------------------------------------------
        /// Adds an execution child.
        ///
        /// @param child The execution child to add.
        ///
        public override void AddExecutable(CodeContext child) {
            myExecutionList.Add(child);
            child.Parent= this;
        }
        // -------------------------------------------------------------------
        public override void AddVariable(VariableDefinition variableDefinition) { Debug.LogWarning("iCanScript: Trying to add a variable defintion to an if-statement definition."); }
        public override void AddType(TypeDefinition typeDefinition)             { Debug.LogWarning("iCanScript: Trying to add a type definition to an if-statement definition."); }
        public override void AddFunction(FunctionDefinition functionDefinition) { Debug.LogWarning("iCanScript: Trying to add a function definition to an if-statement definition."); }
        
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