using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the base class for all library object derived from MethodBase.
    public class LibraryMethodBase : LibraryMemberInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public MethodBase       methodBase  		{ get { return memberInfo as MethodBase; }}
		public bool				isStatic			{ get { return methodBase.IsStatic; }}
        public bool             isPublic    		{ get { return methodBase.IsPublic; }}
        public bool             isProtected 		{ get { return methodBase.IsFamily; }}
        public bool             isPrivate   		{ get { return methodBase.IsPrivate; }}
		public bool				isGeneric			{ get { return methodBase.IsGenericMethod; }}
        public ParameterInfo[]  parameters  		{ get { return methodBase.GetParameters(); }}
		public Type[]			genericArguments	{ get { return methodBase.GetGenericArguments(); }}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryMethodBase(MethodBase methodBase) : base(methodBase) {}
		
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
		internal override string GetRawName()       { return memberInfo.Name; }
		internal override string GetNodeName() {
			var displayName= new StringBuilder(64);
			displayName.Append(NameUtility.ToDisplayName(memberInfo.Name));
			if(isGeneric) {
				displayName.Append(NameUtility.ToDisplayGenericArguments(genericArguments));
			}
			return displayName.ToString();			
		}
        internal override string GetDisplayString() {
			var displayName= new StringBuilder("<b>", 64);
			displayName.Append(NameUtility.ToDisplayName(memberInfo.Name));
			displayName.Append("</b>");
			if(isGeneric) {
				displayName.Append("<i>");
				displayName.Append(NameUtility.ToDisplayGenericArguments(genericArguments));
				displayName.Append("</i>");
			}
			return displayName.ToString();

		}
		
        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
		/// Builds the input parameter string for user display.
        public string ToDisplayInputParameters {
            get {
                // -- We need a method to get parameters. --
    			var result= new StringBuilder(32);
                bool needComma= false;
                if(!isStatic && !isConstructor) {
					var libraryType= parent as LibraryType;
                    result.Append(NameUtility.ToDisplayName(libraryType.type));
                    needComma= true;
                }
                foreach(var param in parameters) {
                    var paramType= param.ParameterType;
    				if(!paramType.IsByRef) {
                        if(needComma) {
                            result.Append(", ");
                        }
    	                result.Append(NameUtility.ToDisplayName(paramType));
                        needComma= true;
    				}
                }
    			// Add inputs to signature.
                var inputStr= result.ToString();
                if(inputStr.Length == 0) {
                    return "";
                }
                return inputStr;
            }
        }
        public string ToDisplayOutputParameters {
            get {
                // -- We need a method to get parameters. --
                var methodInfo= memberInfo as MethodInfo;
                if(methodInfo == null) return "";
    			// Build output string
    			var result= new StringBuilder(32);
                bool needComma= false;
    			int nbOfOutputs= 0;
                foreach(var param in methodInfo.GetParameters()) {
                    var paramType= param.ParameterType;
    				if(paramType.IsByRef) {
                        if(needComma) {
                            result.Append(", ");
                        }
    	                result.Append(NameUtility.ToDisplayName(paramType.GetElementType()));
    					++nbOfOutputs;
                        needComma= true;
    				}
                }
                var returnType= methodInfo.ReturnType;
    			if(returnType != null && returnType != typeof(void)) {
                    if(needComma) {
                        result.Append(", ");
                    }
    				++nbOfOutputs;
    				result.Append(NameUtility.ToDisplayName(returnType));
    			}
                if(nbOfOutputs == 0) {
                    return "";
                }
    			if(nbOfOutputs == 1) {
                    return result.ToString();
    			}
                return result.ToString();
            }
        }

    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class LibraryConstructor : LibraryMethodBase {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryConstructor(ConstructorInfo constructorInfo) : base(constructorInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()       { return declaringType.Name; }
        internal override string GetDisplayString() {
			var displayString= new StringBuilder(64);
			displayString.Append("<b><color=magenta>");
			displayString.Append("new ");
			displayString.Append("</color></b>");
			displayString.Append(mainValueBegin);
			displayString.Append(GetNodeName());
			displayString.Append(mainValueEnd);
			displayString.Append(firstPartBegin);
			displayString.Append(" (");
			displayString.Append(ToDisplayInputParameters);
			displayString.Append(")");
			displayString.Append(firstPartEnd);
			displayString.Append(" --> ");
			displayString.Append(secondPartBegin);
			displayString.Append(NameUtility.ToDisplayName(declaringType));
			displayString.Append(secondPartEnd);
			return displayString.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
			return NameUtility.ToDisplayName(declaringType);
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a constructor node.
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kConstructorIcon);
		}

    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a function in the library.
    public class LibraryMethodInfo : LibraryMethodBase {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public MethodInfo	methodInfo	{ get { return memberInfo as MethodInfo; }}
		public Type			returnType  { get { return methodInfo.ReturnType; }}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryMethodInfo(MethodInfo methodInfo) : base(methodInfo) {}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a function in the library.
    public class LibraryFunction : LibraryMethodInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public string	functionName	{ get { return methodInfo.Name; }}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryFunction(MethodInfo methodInfo) : base(methodInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetDisplayString() {
            // -- Determine function name --
            var name= GetNodeName();
            // -- Get input parameters --
            var inputs= ToDisplayInputParameters;
			if(string.IsNullOrEmpty(inputs)) {
				inputs= "";
			}
			else {
				inputs= "("+inputs+")";				
			}
            // -- Get output parameters --
            var outputs= ToDisplayOutputParameters;
			var separatorIdx= outputs.IndexOf(',');
			if(separatorIdx >= 0 && separatorIdx < outputs.Length) {
	            outputs= "("+outputs+")";				
			}
            // -- Build formatted display string --
            var inputTypesHeader= (EditorGUIUtility.isProSkin ? "<color=lime><i>" : "<color=green><i>");
            var inputTypesTrailer= "</i></color>";
            var outputTypesHeader= (EditorGUIUtility.isProSkin ? "<color=yellow><i>" : "<color=brown><i>");
            var outputTypesTrailer= "</i></color>";
            var part1= displayNameHeader+name+displayNameTrailer + " "
                     + inputTypesHeader+inputs+inputTypesTrailer;
            if(string.IsNullOrEmpty(outputs)) {
                return part1;
            }
            var result= part1 + " --> " + outputTypesHeader+outputs+outputTypesTrailer;
			return result;
        }
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            var name= NameUtility.ToDisplayName(memberInfo.Name);
            if(isGeneric) {
				name+= NameUtility.ToDisplayGenericArguments(genericArguments);
            }
			return name;
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a function node.
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kFunctionIcon);
		}
		
    }

}
