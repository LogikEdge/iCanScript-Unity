using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor.CodeGeneration {

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
        /// @param codeBlock The code block this assignment belongs to.
        /// @param enables The enable ports that affect this code block.
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
            // -- Ask our children to resolve their dependencies. --
            base.ResolveDependencies();
            
            // -- Don't generate code if all enables are false. --
            if(ControlFlow.IsAllEnablesAlwaysFalse(myEnablePorts)) {
                Parent.Remove(this);
                return;
            }
            // -- Merge with parent if one of the enables is always true. --
            if(ControlFlow.IsAtLeastOneEnableAlwaysTrue(myEnablePorts)) {
				(Parent as ExecutionBlockDefinition).Replace(this, myExecutionList);
				return;					
            }
            
            // -- Determine if enable producers where optimized out. --
            var enableLen= myEnablePorts.Length;
            bool hasProducer= false;
            for(int i= 0; i < enableLen; ++i) {
                var enable= myEnablePorts[i];
                var producerPort= GraphInfo.GetProducerPort(enable);
                if(FindCodeBase(producerPort) != null || producerPort.ParentNode.IsConstructor) {
                    hasProducer= true;
                    break;
                }
            }
            if(!hasProducer) {
				(Parent as ExecutionBlockDefinition).Replace(this, myExecutionList);                
                return;
            }
            
            // -- Reposition code for simple trigger->enable --
			CodeBase commonParent= null;
            if(AreAllInSameExecutionContext(out commonParent)) {
				var parentAsExecBlock= commonParent as ExecutionBlockDefinition;
    			if(AreAllProducersTriggers()) {
    				if(parentAsExecBlock != null) {
                        Debug.Log("Removing enable for=> "+myEnablePorts[0].FullName);
    					parentAsExecBlock.Replace(this, myExecutionList);
                        return;					
    				}
    			}
                else if(parentAsExecBlock != null) {
                    if(Parent != parentAsExecBlock) {
                        Parent.Remove(this);
                        parentAsExecBlock.AddExecutable(this);                        
                    }
                }
            }
            
            // -- Determine best enable order for code optimization. --
            if(enableLen > 1) {
                for(int i= 0; i < enableLen; ++i) {
                    var enable= myEnablePorts[i];
                    if(enable.IsTheOnlyConsumer && !enable.SegmentProducerPort.IsTriggerPort) {
                        if(i != 0) {
                            var t= myEnablePorts[0];
                            myEnablePorts[0]= enable;
                            myEnablePorts[i]= t;
                        }
                        break;
                    }
                }
            }
            
            // -- Verify if we can optimize parameter ports. --
            myEnableCode[0]= Context.GetCodeFor(GraphInfo.GetProducerPort(myEnablePorts[0]));
            if(myEnableCode[0] != null) {
                myEnableCode[0]= OptimizeInputParameter(myEnableCode[0], myParent);
                if(myEnableCode[0] != null) {
                    myEnableCode[0].Parent= this;
                }
            }
        }
        
        // -------------------------------------------------------------------
		/// Determines of all producers are connected to triggers.
		bool AreAllProducersTriggers() {
			bool areAllProducersTriggers= true;
			foreach(var e in myEnablePorts) {
                var producerPort= e.SegmentProducerPort;
                if(!producerPort.IsTriggerPort) {
					areAllProducersTriggers= false;
                }	
			}
			return areAllProducersTriggers;			
		}

        // -------------------------------------------------------------------
		/// Determines if all producer are in the same execution context.
		bool AreAllInSameExecutionContext(out CodeBase code) {
			code= null;
			foreach(var e in myEnablePorts) {
                var producerPort= e.SegmentProducerPort;
				var producerParent= producerPort.ParentNode;
				var function= GetFunction(producerParent);
				if(function == null) return false;
				var c= Context.GetCodeFor(function).Parent;
				if(code == null) {
					code= c;
				}
				else if(code != c) {
					return false;
				}
			}
			return true;
		}
		 
        // -------------------------------------------------------------------
		/// Finds a function node within the given node.
		///
		/// @param node The node to search.
		/// @return The found function node. _null_ if not found.
		///
		iCS_EditorObject GetFunction(iCS_EditorObject node) {
			if(node.IsKindOfFunction) return node;
			return null;
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
                    result.Append(GetNameFor(GraphInfo.GetProducerPort(myEnablePorts[i])));                    
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