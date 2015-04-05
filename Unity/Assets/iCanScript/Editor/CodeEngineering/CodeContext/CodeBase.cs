﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public abstract class CodeBase {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public delegate string  CodeProducer(int indent);
        public enum AccessType   { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
        public enum ScopeType    { STATIC, NONSTATIC, VIRTUAL };
        public enum LocationType { LOCAL_TO_FUNCTION, LOCAL_TO_CLASS };
        public enum CodeType     {
            GLOBAL, CLASS, STRUCT, VARIABLE, PROPERTY, FUNCTION, PARAMETER,
            IF, FUNCTION_CALL, VALUE
        };
    
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject                     myVSObject  = null;            ///< Visual script associated object
        CodeBase                             myParent    = null;            ///< The parnt code context
        CodeType                             myCodeType  = CodeType.GLOBAL; ///< Type of this code context
        Dictionary<iCS_EditorObject, string> myLocalNames= new Dictionary<iCS_EditorObject, string>();
        CodeContext                          myContext   = null;
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the type of this code context
        public CodeType TypeOfCode {
            get { return myCodeType; }
        }
        public CodeBase Parent {
            get { return myParent; }
            set { myParent= value; }
        }
        public iCS_EditorObject VSObject {
        	get { return myVSObject; }
        }
        public CodeContext Context {
            get { return myContext; }
        }
		
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the core code structure.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @param parentContext The code context of the parent.
        /// @return The newly created code context.
        ///
        public CodeBase(CodeType codeType, iCS_EditorObject vsObject, CodeBase parent) {
			myParent  = parent;
            myVSObject= vsObject;
            myCodeType= codeType;
            // Build or assign shared code Context.
            if(parent == null) {
                myContext= new CodeContext(vsObject);
            }
            else {
                myContext= parent.Context;
            }
            // Register object <-> code association
            myContext.Register(vsObject, this);
        }
        public virtual void ResolveDependencies() {}
        public virtual void AddVariable(VariableDefinition variableDefinition) {}
        public virtual void AddExecutable(CodeBase executableDefinition)    {}
        public virtual void AddType(TypeDefinition typeDefinition)             {}
        public virtual void AddFunction(FunctionDefinition functionDefinition) {}
        public virtual void Remove(CodeBase toRemove)                       {}

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        public virtual string GenerateCode(int indentSize) {
			var result= new StringBuilder(2048);
            result.Append(GenerateHeader(indentSize));
            result.Append(GenerateBody(indentSize+1));
            result.Append(GenerateTrailer(indentSize));
			return result.ToString();
        }
        public virtual string GenerateHeader(int indentSize)  { return ""; }
		public virtual string GenerateBody(int indentSize)    { return ""; }
        public virtual string GenerateTrailer(int indentSize) { return ""; }
        
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
        /// Returns or creates the pre-built name for a given visual script object.
        ///
        /// @param vsObj The visual script object to search.
        /// @param valueInsteadOfSelf Forces usage of port value if port does not
        ///                           have a producer.
        /// @return The pre-built name for the visual script object.
        ///
        public string GetNameFor(iCS_EditorObject vsObj, bool valueInsteadOfSelf= false) {
            if(vsObj.IsInputPort) {
                var producerPort= vsObj.FirstProducerPort;
                if(producerPort.IsInputPort) {
                    // Return value if we are the producer port and client want the value.
                    if(producerPort == vsObj && valueInsteadOfSelf) {
                        return ToValueString(producerPort.InitialValue);
                    }
                    if(!IsPublicClassInterface(producerPort) && !(producerPort.IsInProposedDataPort && producerPort.ParentNode.IsMessageHandler)) {
                        return ToValueString(producerPort.InitialValue);
                    }                    
                }
                vsObj= producerPort;
            }
            var name= TryGetNameFor(vsObj);
            if(name == null) {
                Debug.Log("Unable to find name for=> "+vsObj.FullName);
                return vsObj.CodeName;
            }
            return name;
        }

    	// -------------------------------------------------------------------------
        /// Returns the pre-built name for a given visual script object.
        ///
        /// @param vsObj The visual script object to search.
        /// @return The pre-built name for the visual script object.
        ///
        public string TryGetNameFor(iCS_EditorObject vsObj) {
            string name= null;
            if(myLocalNames.TryGetValue(vsObj, out name)) {
                return name;
            }
            if(myParent == null) return null;
            return myParent.TryGetNameFor(vsObj);
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
            if(obj is float) {
                return obj.ToString()+"f";
            }
            if(obj is Vector2) {
                Vector2 v2= (Vector2)obj;
                if(v2 == Vector2.zero)  return "Vector2.zero";
                if(v2 == Vector2.one)   return "Vector2.one";
                if(v2 == Vector2.up)    return "Vector2.up";
                if(v2 == Vector2.right) return "Vector2.right";
                return "new Vector2("+v2.x+"f, "+v2.y+"f)";
            }
            if(obj is Vector3) {
                Vector3 v3= (Vector3)obj;
                if(v3 == Vector3.zero)    return "Vector3.zero";
                if(v3 == Vector3.one)     return "Vector3.one";
                if(v3 == Vector3.up)      return "Vector3.up";
                if(v3 == Vector3.right)   return "Vector3.right";
                if(v3 == Vector3.left)    return "Vector3.left";
                if(v3 == Vector3.back)    return "Vector3.back";
                if(v3 == Vector3.down)    return "Vector3.down";
                if(v3 == Vector3.forward) return "Vector3.forward";
                return "new Vector3("+v3.x+"f, "+v3.y+"f, "+v3.z+"f)";
            }
            if(obj is Vector4) {
                Vector4 v4= (Vector4)obj;
                if(v4 == Vector4.zero)    return "Vector4.zero";
                if(v4 == Vector4.one)     return "Vector4.one";
                return "new Vector4("+v4.x+"f, "+v4.y+"f, "+v4.z+"f, "+v4.w+"f)";                
            }
            if(objType.IsEnum) {
                return ToTypeName(obj.GetType())+"."+obj.ToString();
            }
            return obj.ToString();
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a class name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetClassName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            return MakeNameUnique(iCS_ObjectNames.ToTypeName(vsObject.CodeName), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a public class field name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPublicFieldName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            name= vsObject.IsConstructor ? vsObject.DisplayName : vsObject.CodeName;
            return MakeNameUnique(iCS_ObjectNames.ToPublicFieldName(name), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a private class field name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPrivateFieldName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            name= vsObject.IsConstructor ? vsObject.DisplayName : vsObject.CodeName;
            return MakeNameUnique(iCS_ObjectNames.ToPrivateFieldName(name), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a public static class field name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPublicStaticFieldName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            name= vsObject.IsConstructor ? vsObject.DisplayName : vsObject.CodeName;
            return MakeNameUnique(iCS_ObjectNames.ToPublicStaticFieldName(name), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a private static class field name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPrivateStaticFieldName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            name= vsObject.IsConstructor ? vsObject.DisplayName : vsObject.CodeName;
            return MakeNameUnique(iCS_ObjectNames.ToPrivateStaticFieldName(name), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a function parameter name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetFunctionParameterName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            name= iCS_ObjectNames.ToFunctionParameterName(vsObject.CodeName);
            return MakeNameUnique(name, vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a public function name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPublicFunctionName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            return MakeNameUnique(iCS_ObjectNames.ToPublicFunctionName(vsObject.CodeName), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS objec to a private function name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetPrivateFunctionName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            return MakeNameUnique(iCS_ObjectNames.ToPrivateFunctionName(vsObject.CodeName), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a functionlocal variable name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string GetLocalVariableName(iCS_EditorObject vsObject) {
            var name= TryGetNameFor(vsObject);
            if(name != null) return name;
            bool isConstructor= vsObject.IsConstructor;
            name= isConstructor ? vsObject.DisplayName : vsObject.CodeName;
            return MakeNameUnique(iCS_ObjectNames.ToLocalVariableName(name), vsObject);
        }
        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a property name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string ToPropertyName(iCS_EditorObject vsObject) {
            return vsObject.MethodName.Substring(4);
        }

        // ---------------------------------------------------------------------------------
        /// Convert the given VS object to a field name.
        ///
        /// @param paramObject Visual Script object for whom to generate the name.
        /// @return The converted name.
        ///
        public string ToFieldName(iCS_EditorObject vsObject) {
            return vsObject.MethodName;
        }

        // =========================================================================
        // UNIQUE NAME MANAGEMENT
        // -------------------------------------------------------------------------
        /// Creates a unique code name from the proposedName.
        ///
        /// @param proposedName The desired name for the code fragment associated
        ///                     with the visual script object.
        /// @param vsObj    Visual Script object of the code fragment.
        ///
        public string MakeNameUnique(string name, iCS_EditorObject vsObject) {
            while(DoesNameAlreadyExist(name)) {
                name= name+"_"+vsObject.InstanceId;
            }
            myLocalNames.Add(vsObject, name);
            // Special case for constructor output.
            if(vsObject.IsConstructor) {
                var returnPort= GetReturnPort(vsObject);
                if(returnPort != null) {
                    myLocalNames.Add(returnPort, name);
                }
            }
            return name;
        }
        // -----------------------------------------------------------------------
        /// Determines if the given _'name'_ already exists in the code scope.
        ///
        /// This algorithm first seeks for a name collison in the code scope.
        /// The name collision is detected if the names are the same and:
        /// - the parent code is the same or;
        /// - the name exists in one of the code parent scope.
        ///
        /// @param name The proposed name of the code fragment
        /// @param vsObj The visual script object of the cod efragment
        ///
        public bool DoesNameAlreadyExist(string name) {
            if(myLocalNames.ContainsValue(name)) return true;
            if(myParent == null) return false;
            return myParent.DoesNameAlreadyExist(name);
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
        
        // ---------------------------------------------------------------------------------
        /// Returns the common base for all consumers of a producer port.
        ///
        /// @param producerPort The producer port.
        /// @return The common base type for all consumers.
        ///
        public Type GetCommonBaseTypeForProducerPort(iCS_EditorObject producerPort) {
            var consumers= producerPort.EndConsumerPorts;
            var consumersLen= consumers.Length;
            if(consumersLen == 0) return typeof(void);
            var commonType= consumers[0].RuntimeType;
            for(int i= 1; i < consumersLen; ++i) {
                var t= consumers[i].RuntimeType;
                if(t != commonType) {
                    if(iCS_Types.IsA(t, commonType)) {
                        commonType= t;
                    }
                }
            }
            return commonType;
        }
        
        // ---------------------------------------------------------------------------------
        /// Generates a code banner.
        ///
        /// @param indent White space string to be prepended to each line.
        /// @param bannerText The banner text.
        /// @return A foramtted code banner.
        ///
        public string GenerateCodeBanner(string indent, string bannerText) {
            var result= new StringBuilder(indent, 256);
            result.Append(CodeBannerTop);
            result.Append(indent);
            result.Append("// ");
            result.Append(bannerText);
            result.Append("\n");
            result.Append(indent);
            result.Append(CodeBannerBottom);
            return result.ToString();
        }
        // ---------------------------------------------------------------------------------
        /// Returns a double header bar for code comments.
        public string CodeBannerTop {
            get { return "// =========================================================================\n"; }
        }
        // ---------------------------------------------------------------------------------
        /// Returns a single header bar for code comments.
        public string CodeBannerBottom {
            get { return "// -------------------------------------------------------------------------\n"; }
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
            parameters.Sort((a,b)=> b.PortIndex-a.PortIndex);
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

    	// -------------------------------------------------------------------------
        /// Returns the function return port.
        ///
        /// @param node The node in which to search for a return port.
        ///
        /// @return _'null'_ is return if no return port is found.
        ///
        public static iCS_EditorObject GetReturnPort(iCS_EditorObject node) {
            iCS_EditorObject result= null;
            node.ForEachChildPort(
                p=> {
                    if(p.PortIndex == (int)iCS_PortIndex.Return) {
                        result= p;
                    }
                }
            );
            return result;
        }

    	// -------------------------------------------------------------------------
        /// Returns _'true'_ if port should be promoted to a public class interface.
        ///
        /// @param port The port to be tested.
        /// @return TRUE if port should be promoted to a class public interface.
        ///
        public bool IsPublicClassInterface(iCS_EditorObject port) {
            var parentNode= port.ParentNode;
            if(!port.IsDynamicDataPort) return false;
            if(!parentNode.IsKindOfPackage) return false;
            if(parentNode.IsInstanceNode) return false;
            if(port.IsInDataPort) {
                var producerPort= port.ProducerPort;
                if(producerPort != null && producerPort != port) return false;
                // Don't generate public interface if already connected to a constructor.
                var consumers= port.EndConsumerPorts;
                foreach(var c in consumers) {
                    if(c.ParentNode.IsConstructor) {
                        return false;
                    }
                }
            }
            else if(port.IsOutDataPort) {
                if(port.ConsumerPorts.Length != 0) return false;
            }
            else {
                return false;
            }
            return true;
        }

    	// -------------------------------------------------------------------------
        /// Returns the producer port usable by the code.
        ///
        /// @param consumerPort The VS consumer port.
        /// @return The producer port usable by the code.
        ///
        public static iCS_EditorObject GetCodeProducerPort(iCS_EditorObject consumerPort) {
            var producerPort= consumerPort;
            do {
                if(producerPort.IsOutInstancePort) {
                    producerPort= GetThisPort(producerPort.ParentNode);
                }
                producerPort= producerPort.FirstProducerPort;                
            } while(producerPort != null && producerPort.IsOutInstancePort);
            return producerPort;
        }

    	// -------------------------------------------------------------------------
        /// Returns the consumer ports usbale by the code.
        ///
        /// @param producerPort The visual script producer port.
        /// @return The consumer ports usable by the code.
        ///
        public iCS_EditorObject[] GetCodeConsumerPorts(iCS_EditorObject producerPort) {
            // TODO: GetCodeConsumerPorts needs to be completed.
            return producerPort.EndConsumerPorts;
        }
        
    	// -------------------------------------------------------------------------
        /// Returns the input port representing the _'self'_ connection.
        ///
        /// @param node The node in which to search for the _'self'_ port.
        ///
        /// @return _'null'_ is returned if the port is not found.
        ///
        public static iCS_EditorObject GetThisPort(iCS_EditorObject node) {
            iCS_EditorObject result= null;
            node.ForEachChildPort(
                p=> {
                    if(p.PortIndex == (int)iCS_PortIndex.InInstance) {
                        result= p;
                    }
                }
            );
            return result;
        }

        // =========================================================================
        // ITERATION UTILITIES
    	// -------------------------------------------------------------------------
        /// Returns the code root.
        GlobalDefinition GetGlobalContext() {
            if(this is GlobalDefinition) return this as GlobalDefinition;
            if(Parent == null) return null;
            return Parent.GetGlobalContext();
        }

    	// -------------------------------------------------------------------------
        /// Finds the code base for the given visual script object.
        ///
        /// @param vsObject The visual scriptobject to search for.
        /// @return The found code context.  _'null'_ is return if not found.
        ///
        public CodeBase FindCodeBase(iCS_EditorObject vsObject) {
            return Context.GetCodeFor(vsObject);
        }

    	// -------------------------------------------------------------------------
        /// Returns the proper parent context for the given producer Code.
        ///
        /// @param producerCode The producer port code.
        /// @return The proper parent context for the producer port.
        ///
        // TODO: To be tested...
        public CodeBase GetProperParentCodeForProducerPort(CodeBase producerPortCode) {
            var consumerPorts=  GetCodeConsumerPorts(producerPortCode.VSObject);
            var commonParent= producerPortCode;
            foreach(var c in consumerPorts) {
                var consumerParent= Context.GetCodeFor(c.ParentNode).Parent;
                commonParent= GetCommonParent(commonParent, consumerParent);
            }
            return commonParent;
        }

    	// -------------------------------------------------------------------------
        /// Returns the common code parent of two visual script objects.
        ///
        /// @param vsObject1 First visual script object.
        /// @param vsObject2 Second visual script object.
        /// @return The common code parent of both visual script object.
        ///
        public CodeBase GetCommonParent(iCS_EditorObject vsObject1, iCS_EditorObject vsObject2) {
            var code1= Context.GetCodeFor(vsObject1);
            var code2= Context.GetCodeFor(vsObject2);
            return GetCommonParent(code1, code2);
        }

    	// -------------------------------------------------------------------------
        /// Returns the common code parent of two Code base.
        ///
        /// @param code1 First code base.
        /// @param code2 Second code base.
        /// @return The common code parent of both visual script object.
        ///
        public CodeBase GetCommonParent(CodeBase code1, CodeBase code2) {
            var parents1= GetListOfParents(code1);
            var parents2= GetListOfParents(code2);
            CodeBase commonParent= null;
            int len= Mathf.Min(parents1.Length, parents2.Length);
            for(int i= 0; i < len && parents1[i] == parents2[i]; ++i) {
                commonParent= parents1[i];
            }
            return commonParent;
        }

    	// -------------------------------------------------------------------------
        /// Returns the Code parent list.
        ///
        /// @param code The code from which to create the list.
        /// @return The ordered list of parents.
        ///
        public CodeBase[] GetListOfParents(CodeBase code) {
            var parents= new List<CodeBase>();
            for(; code != null; code= code.Parent) {
                parents.Add(code);
            }
            parents.Reverse();
            return parents.ToArray();
        }
    }

}