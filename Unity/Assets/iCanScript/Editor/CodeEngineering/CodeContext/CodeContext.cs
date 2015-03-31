using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeContext {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum AccessType   { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
        public enum ScopeType    { STATIC, NONSTATIC, VIRTUAL };
        public enum LocationType { LOCAL_TO_FUNCTION, LOCAL_TO_CLASS };
        public enum CodeType     {
            GLOBAL, CLASS, STRUCT, FIELD, PROPERTY, FUNCTION, VARIABLE, PARAMETER,
            IF
        };
    
        // -------------------------------------------------------------------
        public delegate string CodeProducer(int indent);

        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeContext myParent  = null;            ///< The parnt code context
        CodeType    myCodeType= CodeType.GLOBAL; ///< Type of this code context
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the type of this code context
        public CodeType TypeOfCode {
            get { return myCodeType; }
        }
        public CodeContext Parent {
            get { return myParent; }
            set { myParent= value; }
        }
        
        // -------------------------------------------------------------------
        /// Builds a Code Context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @param parentContext The code context of the parent.
        /// @return The newly created code context.
        ///
        public CodeContext(CodeType codeType) {
            myCodeType= codeType;
        }
        
        // =========================================================================
        // CONVERSION UTILITIES
    	// -------------------------------------------------------------------------
        /// Returns a white space charater stringf matching the given indent size.
        ///
        /// @param indentSize The number of indent to add.
        /// @see The _indentSize_ is a factor of ourTabSize;
        ///
        const int ourTabSize= 4;
        public static string ToIndent(int indent) {
            return new String(' ', indent*ourTabSize);
        }

    	// -------------------------------------------------------------------------
        /// Converts the given AccessType to its string representation.
        ///
        /// @param accessType The access type to be converted.
        /// @return The string representation of the acces type.
        ///
        public static string ToAccessString(AccessType accessType) {
            switch(accessType) {
                case AccessType.PUBLIC:    return "public";
                case AccessType.PRIVATE:   return "private";
                case AccessType.PROTECTED: return "protected";
                case AccessType.INTERNAL:  return "internal";
            }
            return "public";
        }
        // -------------------------------------------------------------------
        /// Converts the given ScopeType to its string representation.
        ///
        /// @param scopeType The scope type to be converted.
        /// @return The string representation of the scope type.
        ///
        public static string ToScopeString(ScopeType scopeType) {
			switch(scopeType) {
				case ScopeType.STATIC:  return "static";
				case ScopeType.VIRTUAL: return "virtual";
			}
            return ""; 
        }
        
        // -------------------------------------------------------------------
        /// Converts the given Type to its string representation.
        ///
        /// @param type The type to be converted.
        /// @return The string representation of the type.
        ///
		public static string ToTypeName(Type type) {
            type= iCS_Types.RemoveRefOrPointer(type);
			if(type == typeof(void))   return "void";
			if(type == typeof(int))    return "int";
			if(type == typeof(uint))   return "uint";
			if(type == typeof(bool))   return "bool";
			if(type == typeof(string)) return "string";
            if(type == typeof(float))  return "float";
			return type.Name;
		}
        
        // -------------------------------------------------------------------
        /// Converts the given object to its string value representation.
        ///
        /// @param type The object to be converted.
        /// @return The string representation of the value.
        ///
        public static string ToValueString(System.Object obj) {
            if(obj == null) return "null";
            var objType= obj.GetType();
            if(obj is bool) {
                return ((bool)obj) ? "true" : "false";
            }
            if(obj is string) {
                return "\""+obj.ToString()+"\"";
            }
            if(obj is char) {
                return "\'"+obj.ToString()+"\'";
            }
            if(objType.IsEnum) {
                return ToTypeName(obj.GetType())+"."+obj.ToString();
            }
            return obj.ToString();
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given name to a class name using the predefined naming
        /// scheme.
        ///
        /// @param name The name to be converted.
        /// @return The converted name.
        ///
        public static string ToClassName(string name) {
            return iCS_ObjectNames.ToTypeName(name);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given name to a public class field name using the predefined naming
        /// scheme.
        ///
        /// @param name The name to be converted.
        /// @return The converted name.
        ///
        public static string ToPublicFieldName(string name) {
            return iCS_ObjectNames.ToPublicFieldName(name);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given name to a private class field name using the predefined naming
        /// scheme.
        ///
        /// @param name The name to be converted.
        /// @return The converted name.
        ///
        public static string ToPrivateFieldName(string name) {
            return iCS_ObjectNames.ToPrivateFieldName(name);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given name to a public class field name using the predefined naming
        /// scheme.
        ///
        /// @param name The name to be converted.
        /// @return The converted name.
        ///
        public static string ToPublicStaticFieldName(string name) {
            return ToPublicStaticFieldName(name);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given name to a private class field name using the predefined naming
        /// scheme.
        ///
        /// @param name The name to be converted.
        /// @return The converted name.
        ///
        public static string ToPrivateStaticFieldName(string name) {
            return ToPrivateStaticFieldName(name);
        }
        
        // =========================================================================
        // COMMON GENERATION UTILITIES
        // -------------------------------------------------------------------
        /// Generates an allocator for the given type.
        ///
        /// @param type The type to generate an allocator for.
        /// @param paramValues The values to pass to the constrcutor.
        /// @return The format code fragment of the allocator.
        ///
        public static string GenerateAllocatorFragment(Type type, string[] paramValues) {
            var result= new StringBuilder(" new ");
            result.Append(ToTypeName(type));
            result.Append("(");
            int len= paramValues.Length;
            for(int i= 0; i < len; ++i) {
                result.Append(paramValues[i]);
                if(i+1 < len) {
                    result.Append(", ");
                }
            }
            result.Append(")");
            return result.ToString();
        }
        
        // =========================================================================
        // PARAMETER UTILITIES
    	// -------------------------------------------------------------------------
        /// Iterates through the parameters of the given node.
        ///
        /// @param node The parent node of the parameter ports to iterate on.
        /// @param fnc  The action to run for each parameter port.
        ///
        static void ForEachParameter(iCS_EditorObject node, Action<iCS_EditorObject> fnc) {
            node.ForEachChildPort(
        		p=> {
        			if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        fnc(p);
        			}
        		}        
            );
        }

    	// -------------------------------------------------------------------------
        /// Returns a list of parameters for the given node.
        ///
        /// @param node The node from which to extract the parameter ports.
        ///
        /// @return List of existing parameter ports.
        ///
        /// @note Parameters are expected to be continuous from port index 0 to
        ///       _'nbOfParameters'_.
        ///
        public static iCS_EditorObject[] GetParameters(iCS_EditorObject node) {
            var parameters= new List<iCS_EditorObject>();
            ForEachParameter(node, p=> parameters.Add(p));
            return parameters.ToArray();
        }
    	// -------------------------------------------------------------------------
        /// Returns the number of function parameters for the given node.
        ///
        /// @param node The node from which to get the parameter ports.
        ///
        /// @return The number of parameter ports that exists on the _'node'_.
        ///
        /// @note Parameters are expected to be continuous from port index 0 to
        ///       _'nbOfParameters'_.
        ///
        public static int GetNbOfParameters(iCS_EditorObject node) {
    		var nbParams= 0;
    		node.ForEachChildPort(
    			p=> {
    				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
    					if(p.PortIndex+1 > nbParams) {
    						nbParams= p.PortIndex+1;
    					}
    				}
    			}
    		);
            return nbParams;        
        }
        
    }

}