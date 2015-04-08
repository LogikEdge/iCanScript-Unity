using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class EnableBlockDefinition : ExecutionBlockDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject[]  myEnablePorts= null;
        CodeBase[]          myEnableCode = null;
        
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
            myEnableCode = new CodeBase[myEnablePorts.Length];
            for(int i= 0; i < myEnableCode.Length; ++i) myEnableCode[i]= null;
        }

        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
            // Ask our children to resolve their dependencies.
            base.ResolveDependencies();
            
            // Simply reposition code for simple trigger-to-enable dependencies.
            if(myEnablePorts.Length == 1) {
                var producerPort= myEnablePorts[0].FirstProducerPort;
                var parentAsExecBlock= Parent as ExecutionBlockDefinition;
                if(producerPort.IsTriggerPort && parentAsExecBlock != null) {
                    parentAsExecBlock.Replace(this, myExecutionList);
                    return;
                }
            }
            
            // Verify if we can optimize parameter ports.
            myEnableCode[0]= Context.GetCodeFor(GetCodeProducerPort(myEnablePorts[0]));
            if(myEnableCode[0] != null) {
                myEnableCode[0]= OptimizeInputParameter(myEnableCode[0], Parent);
                if(myEnableCode[0] != null) {
                    myEnableCode[0].Parent= this;
                }
            }
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the enable block header code.
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
                if(myEnableCode[i] != null) {
                    result.Append(myEnableCode[i].GenerateBody(0));
                }
                else {
                    result.Append(GetNameFor(GetCodeProducerPort(myEnablePorts[i])));                    
                }
                if(i < len-1) {
                    result.Append(" || ");
                }
            }
            result.Append(") {\n");
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the enable block trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the if-statement.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ToIndent(indentSize)+"}\n";
        }

    }

}