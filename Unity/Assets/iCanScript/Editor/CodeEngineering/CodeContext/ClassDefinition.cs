using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class ClassDefinition : CodeContext {
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
        public ClassDefinition(iCS_EditorObject classNode, Type baseClass,
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
                AddFieldDefinition(field);
            }            
        }

        // -------------------------------------------------------------------
        /// Searches for child functions and adds them to class definition.
        void AddChildFunctions() {
    		myClassNode.ForEachChildNode(
    			n=> {
    				if(n.IsMessage || n.IsPublicFunction) {
                        var functionDefinition= new FunctionDefinition(n, AccessType.PUBLIC, ScopeType.NONSTATIC);
                        AddFunctionDefinition(functionDefinition);
    				}
    			}
    		);            
        }

        // -------------------------------------------------------------------
        /// Adds a field definition to the class.
        ///
        /// @param vsObj VS object that represents the field.
        ///
        public void AddFieldDefinition(VariableDefinition field) {
            myFields.Add(field);
            field.Parent= this;
        }
        
        // -------------------------------------------------------------------
        /// Adds a function definition to the class.
        ///
        /// @param functionNode VS node that represents the function definition.
        ///
        public void AddFunctionDefinition(FunctionDefinition functinNode) {
            myFunctions.Add(functinNode);
            functinNode.Parent= this;
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the class code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted code for the class.
        ///
        public string GenerateCode(int indentSize) {
            // Determine class properties.
            var className= ToClassName(myClassNode.CodeName);
            
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
            foreach(var f in myFields) {
                result.Append(f.GenerateCode(indentSize));
            }
            // Functions
            foreach(var f in myFunctions) {
                result.Append(f.GenerateCode(indentSize));
            }
            return result.ToString();
        }

    }
}