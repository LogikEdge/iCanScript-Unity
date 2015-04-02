using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class TypeDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject         myClassNode = null;  ///< VS objects associated with code context
        Type                     myBaseClass = null;  ///< The base class for this class
        AccessType               myAccessType= AccessType.PRIVATE;
        ScopeType                myScopeType = ScopeType.NONSTATIC;
        List<VariableDefinition> myFields    = new List<VariableDefinition>();
        List<FunctionDefinition> myFunctions = new List<FunctionDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject ClassNode {
            get { return myClassNode; }
        }
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a _class_ specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created class definition.
        ///
        public TypeDefinition(iCS_EditorObject classNode, Type baseClass,
                              AccessType accessType, ScopeType scopeType)
        : base(CodeType.CLASS) {
            myClassNode = classNode;
            myBaseClass = baseClass;
            myAccessType= accessType;
            myScopeType = scopeType;
            // Add fields
            AddChildConstructorsAsFields();
            // Add functions
            AddChildFunctions();
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
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
        public override void AddExecutable(CodeContext executableDefinition)    { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a type definition."); }
        public override void AddType(TypeDefinition typeDefinition)             { Debug.LogWarning("iCanScript: Trying to add a type definition to a type definition."); }

        // -------------------------------------------------------------------
        /// Searches for child constrcutors and adds them to class definition.
        void AddChildConstructorsAsFields() {
            var constructors= myClassNode.FilterChildRecursive(c=> c.IsConstructor);
            foreach(var c in constructors) {
                AccessType fieldAccess= AccessType.PRIVATE;
                if(c.ParentId == 0) {
                    fieldAccess= AccessType.PUBLIC;
                }
                var field= new VariableDefinition(c, fieldAccess, ScopeType.NONSTATIC);
                AddVariable(field);
            }            
        }

        // -------------------------------------------------------------------
        /// Searches for child functions and adds them to class definition.
        void AddChildFunctions() {
    		myClassNode.ForEachChildNode(
    			n=> {
    				if(n.IsPublicFunction) {
                        var functionDefinition= new FunctionDefinition(n, AccessType.PUBLIC, ScopeType.NONSTATIC);
                        AddFunction(functionDefinition);
    				}
    				if(n.IsMessage) {
                        var functionDefinition= new EventHandlerDefinition(n, AccessType.PUBLIC, ScopeType.NONSTATIC);
                        AddFunction(functionDefinition);
    				}
    			}
    		);            
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the class code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted code for the class.
        ///
        public override string GenerateCode(int indentSize) {
            // Determine class properties.
            var className= GetClassName(myClassNode);
            
            // Generate the class outline code.
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 1024);
            // Access Type
            if(myAccessType == AccessType.PUBLIC) {
                result.Append("[iCS_Class(Library=\"Visual Scripts\")]\n");
                result.Append(indent);
            }
            result.Append(ToAccessString(myAccessType));
            // Scope Type
            if(myScopeType != ScopeType.NONSTATIC) {
                result.Append(" ");
                result.Append(ToScopeString(myScopeType));
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
            // Class Body
            result.Append(GenerateClassBody(indentSize+1));
            // Class end
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the class body code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted code for the class.
        ///
        string GenerateClassBody(int indentSize) {
            var result= new StringBuilder(1024);
            // Fields
            result.Append(GenerateClassFields(indentSize));
            // Functions
            result.Append(GenerateClassFunctions(indentSize));
            return result.ToString();
        }

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
            var publicFields= P.filter(f=> f.myAccessType == AccessType.PUBLIC, myFields);
            var privateFields= P.filter(f=> f.myAccessType != AccessType.PUBLIC, myFields);
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