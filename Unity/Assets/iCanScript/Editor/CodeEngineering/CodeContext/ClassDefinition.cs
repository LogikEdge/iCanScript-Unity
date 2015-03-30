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
        iCS_EditorObject        myClassNode= null;  ///< VS objects associated with code context
        Type                    myBaseClass= null;  ///< The base class for this class
        List<iCS_EditorObject>  myFields   = new List<iCS_EditorObject>();
        List<iCS_EditorObject>  myFunctions= new List<iCS_EditorObject>();
        
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
        /// @return The newly created code context.
        ///
        public ClassDefinition(iCS_EditorObject classNode, Type baseClass)
        : base(CodeContextType.CLASS) {
            myClassNode= classNode;
            myBaseClass= baseClass;
            // Search for child constructors
            var constructors= myClassNode.FilterChildRecursive(c=> c.IsConstructor);
            foreach(var c in constructors) {
                AddFieldDefinition(c);
            }
        }
        
        // -------------------------------------------------------------------
        /// Adds a field definition to the class.
        ///
        /// @param vsObj VS object that represents the field.
        ///
        public void AddFieldDefinition(iCS_EditorObject vsObject) {
            myFields.Add(vsObject);
        }
        
        // -------------------------------------------------------------------
        /// Adds a function definition to the class.
        ///
        /// @param functionNode VS node that represents the function definition.
        ///
        public void AddFunctionDefinition(iCS_EditorObject functinNode) {
            myFunctions.Add(functinNode);
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
            AccessType accessType= AccessType.PUBLIC;
            ScopeType  scopeType = ScopeType.NONSTATIC;
            
            // Generate the class outline code.
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 1024);
            // Access Type
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_Class(Library=\"Visual Scripts\")]\n");
                result.Append(indent);
            }
            result.Append(ToAccessString(accessType));
            // Scope Type
            if(scopeType != ScopeType.NONSTATIC) {
                result.Append(" ");
                result.Append(ToScopeString(scopeType));
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
                result.Append(GenerateField(indentSize, f));
            }
            // Functions
            foreach(var f in myFunctions) {
                result.Append(GenerateFunction(indentSize, f));
            }
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the code for all fields of the class.
        ///
        /// @param indentSize The indentation of the fields.
        /// @param field The field for which to generate code.
        /// @return The generated code for the given field.
        ///
        string GenerateField(int indentSize, iCS_EditorObject field) {
            var result= new StringBuilder(128);
    		// Generate non-static variables.
            if(field.IsConstructor) {
                var nbOfParams= GetNbOfParameters(field);
                var initValues= new string[nbOfParams];
                field.ForEachChildPort(
                    p=> {
                        if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                            var v= p.InitialValue;
                            initValues[p.PortIndex]= ToValueString(v);
                        }
                    }
                );
                var initializer= GenerateAllocatorFragment(field.RuntimeType, initValues);
                var accessType= field.ParentId == 0 ? AccessType.PUBLIC : AccessType.PRIVATE;
                //myNameMgr.SetCodeParent(n, classNode);
                //var variableName= myNameMgr.GetNameFor(n);
                string variableName;
                if(accessType == AccessType.PUBLIC) {
                    variableName= ToPublicFieldName(field.CodeName);
                }
                else {
                    variableName= ToPrivateFieldName(field.CodeName);
                }
    			result.Append(GenerateField(indentSize, accessType, ScopeType.NONSTATIC, field.RuntimeType, variableName, initializer));                    
            }
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for a class field.
        ///
		public static string GenerateField(int indentSize, AccessType accessType, ScopeType scopeType,
										   Type variableType, string variableName, string initializer) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_InOutPort]\n");
                result.Append(indent);
            }
            result.Append(ToAccessString(accessType));
            result.Append(" ");
            result.Append(ToScopeString(scopeType));
            result.Append(" ");
			result.Append(ToTypeName(variableType));
			result.Append(" ");
			result.Append(variableName);
			if(!String.IsNullOrEmpty(initializer)) {
				result.Append("= ");
				result.Append(initializer);
			}
			result.Append(";\n");
			return result.ToString();
		}


        // -------------------------------------------------------------------
        string GenerateFunction(int indentSize, iCS_EditorObject function) {
            var result= new StringBuilder(1024);
            return result.ToString();            
        }
        
        
    }
}