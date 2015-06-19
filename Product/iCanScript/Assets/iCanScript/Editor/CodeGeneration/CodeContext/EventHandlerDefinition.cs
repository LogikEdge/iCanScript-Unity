using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor.CodeGeneration {

public class EventHandlerDefinition : FunctionDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param node VS objects associated with the function.
        /// @return The newly created code context.
        ///
        public EventHandlerDefinition(iCS_EditorObject node, CodeBase parent, AccessSpecifier accessType, ScopeSpecifier scopeType)
        : base(node, parent, accessType, scopeType) {
            CreateLocalVariables();
        }
    
        // -------------------------------------------------------------------
        /// Scan the input ports to create special local variables.
        void CreateLocalVariables() {
    		VSObject.ForEachChildPort(
    			p=> {
    				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        if(p.IsInProposedDataPort && iCS_Types.IsA<Component>(p.RuntimeType)) {
                            new LocalVariableDefinition(p, this);
                        }
    				}
    			}
    		);
        }
        
        // -------------------------------------------------------------------
        /// Builds the list of function parameters.
        protected override void BuildParameterList() {
            var parameters= GetParameters(VSObject);
            parameters= P.filter(p=> p.IsFixDataPort, parameters);
            myParameters= new FunctionDefinitionParameter[parameters.Length];
            foreach(var p in parameters) {
                myParameters[p.PortIndex]= new FunctionDefinitionParameter(p, this);                    
            }
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the code for a function definition.
        ///
        /// @param indentSize The indentation of the function.
        /// @return The generated code for the given function.
        ///
        public override string GenerateHeader(int indentSize) {
            var result= new StringBuilder(1024);
            result.Append(base.GenerateHeader(indentSize));
            // Build components
            var indent= ToIndent(indentSize+1);
    		VSObject.ForEachChildPort(
    			p=> {
    				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        if(p.IsInProposedDataPort && iCS_Types.IsA<Component>(p.RuntimeType)) {
                            result.Append(indent);
                            result.Append("var ");
                            result.Append(GetLocalVariableName(p));
                            result.Append("= GetComponent<");
                            result.Append(ToTypeName(p.RuntimeType));
                            result.Append(">();\n");
                        }
    				}
    			}
    		);
            return result.ToString();
        }

    }

}