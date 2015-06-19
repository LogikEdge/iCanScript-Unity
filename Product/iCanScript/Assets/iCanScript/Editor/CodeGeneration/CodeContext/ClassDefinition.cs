using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ClassDefinition : TypeDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        Type                     myBaseClass = null;  ///< The base class for this class
        AccessSpecifier          myAccessSpecifier= AccessSpecifier.Private;
        ScopeSpecifier           myScopeSpecifier = ScopeSpecifier.NonStatic;
        List<VariableDefinition> myFields    = new List<VariableDefinition>();
        List<FunctionDefinition> myFunctions = new List<FunctionDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject ClassNode {
            get { return VSObject; }
        }
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a _class_ specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created class definition.
        ///
        public ClassDefinition(iCS_EditorObject typeNode, CodeBase parent, Type baseClass,
                               AccessSpecifier accessType, ScopeSpecifier scopeType)
        : base(typeNode, parent) {
            myBaseClass      = baseClass;
            myAccessSpecifier= accessType;
            myScopeSpecifier = scopeType;
            // Add fields
            AddChildConstructorsAsFields();
            AddPublicInterfaces();
            // Add functions
            AddChildFunctions();
        }
        
        // -------------------------------------------------------------------
        /// Adds a field definition to the class.
        ///
        /// @param vsObj VS object that represents the field.
        ///
        public override void AddVariable(VariableDefinition field) {
            myFields.Add(field);
            field.Parent= this;
        }
        // -------------------------------------------------------------------
        /// Adds a function definition to the class.
        ///
        /// @param functionDefinition VS node that represents the function definition.
        ///
        public override void AddFunction(FunctionDefinition functionDefinition) {
            myFunctions.Add(functionDefinition);
            functionDefinition.Parent= this;
        }        

        // -------------------------------------------------------------------
        /// Searches for child constrcutors and adds them to class definition.
        void AddChildConstructorsAsFields() {
            var constructors= ClassNode.FilterChildRecursive(c=> c.IsConstructor);
            foreach(var c in constructors) {
                if(AreAllInputsConstant(c)) {
                    AccessSpecifier fieldAccess= AccessSpecifier.Private;
                    fieldAccess= AccessSpecifier.Public;
                    var field= new VariableDefinition(c, this, fieldAccess, ScopeSpecifier.NonStatic);
                    AddVariable(field);                    
                }
            }            
        }

        // -------------------------------------------------------------------
        /// Searches public interfaces as class fields.
        void AddPublicInterfaces() {
            var publicInterfaces= ClassNode.FilterChildRecursive(c=> IsPublicClassInterface(c));
            foreach(var c in publicInterfaces) {
                var field= new VariableDefinition(c, this, AccessSpecifier.Public, ScopeSpecifier.NonStatic);
                AddVariable(field);
            }            
        }
        
        // -------------------------------------------------------------------
        /// Searches for child functions and adds them to class definition.
        void AddChildFunctions() {
    		ClassNode.ForEachChildNode(
    			n=> {
    				if(n.IsFunctionDefinition) {
                        var functionDefinition= new FunctionDefinition(n, this, AccessSpecifier.Public, ScopeSpecifier.NonStatic);
                        AddFunction(functionDefinition);
    				}
    				if(n.IsEventHandler) {
                        var functionDefinition= new EventHandlerDefinition(n, this, AccessSpecifier.Public, ScopeSpecifier.NonStatic);
                        AddFunction(functionDefinition);
    				}
    			}
    		);            
        }

        // -------------------------------------------------------------------
		/// Resolves the code dependencies.
		public override void ResolveDependencies() {
			foreach(var f in myFields) {
				f.ResolveDependencies();
			}			
			foreach(var f in myFunctions) {
				f.ResolveDependencies();
			}			
		}

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the type header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the type.
        ///
        public override string GenerateHeader(int indentSize) {
            // Determine class properties.
            var className= GetClassName(ClassNode);
            
            // Generate the class outline code.
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 1024);
            // Access Type
            result.Append(ToAccessString(myAccessSpecifier));
            // Scope Type
            if(myScopeSpecifier != ScopeSpecifier.NonStatic) {
                result.Append(" ");
                result.Append(ToScopeString(myScopeSpecifier));
            }
            // Class name
            result.Append(" class ");
            result.Append(className);
            // Base class
            if(myBaseClass != null && myBaseClass != typeof(void)) {
                result.Append(" : ");
                result.Append(ToTypeName(myBaseClass));
            }
            // Class begin
            result.Append(" {\n");
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the type body code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the type.
        ///
        public override string GenerateBody(int indentSize) {
            var result= new StringBuilder(1024);
            // Fields
            result.Append(GenerateClassFields(indentSize));
            // Functions
            result.Append(GenerateClassFunctions(indentSize));
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the type trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the type.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ToIndent(indentSize)+"}\n";
        }

        // ===================================================================
        // CODE GENERATION UTILITIES
        // -------------------------------------------------------------------
        /// Generate the code for the class functions.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The class functions code.
        ///
        string GenerateClassFunctions(int indentSize) {
            var result= new StringBuilder(1024);
			if(myFunctions.Count != 0) {
				result.Append("\n");
	            result.Append(GenerateCodeBanner(ToIndent(indentSize), "PUBLIC FUNCTIONS"));				
			}
            foreach(var f in myFunctions) {
                result.Append(f.GenerateCode(indentSize));
            }
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for the class fields.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The class fileds code.
        ///
        string GenerateClassFields(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(1024);
            // Fields
            var publicFields= P.filter(f=> f.IsPublic, myFields);
            var privateFields= P.filter(f=> !f.IsPublic, myFields);
            if(publicFields.Count != 0) {
                result.Append(GenerateCodeBanner(indent, "PUBLIC FIELDS"));
                foreach(var f in publicFields) {
                    result.Append(f.GenerateCode(indentSize));
                }
                result.Append("\n");
            }
            if(privateFields.Count != 0) {
                result.Append(GenerateCodeBanner(indent, "PRIVATE FIELDS"));
                foreach(var f in privateFields) {
                    result.Append(f.GenerateCode(indentSize));
                }
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}