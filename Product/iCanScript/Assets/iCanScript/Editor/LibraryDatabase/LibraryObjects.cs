using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Prelude;

namespace iCanScript.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the base class to create a tree container.
    public class TreeNode {
        public TreeNode        parent= null;
        public List<TreeNode>  children= null;
        
        public T GetChild<T>(Func<T,bool> cond) where T : TreeNode {
            if(children == null) return null;
            foreach(var c in children) {
                var childAsT= c as T;
                if(cond(childAsT)) {
                    return childAsT;
                }
            }                
            return null;
        }
        public void AddChild(TreeNode child) {
            if(children == null) children= new List<TreeNode>();
            child.parent= this;
            children.Add(child);
        }
        public void Sort<T>(Func<T,T,int> sortFnc) where T : TreeNode {
            if(children == null) return;
            children.Sort((x,y)=> sortFnc(x as T, y as T));
        }
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the base class for all objects in the library.
    public abstract class LibraryObject : TreeNode {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
				Texture	myLibraryIcon  = null;
                string	myDisplayString= null;
        public  Vector2 displaySize    = Vector2.zero;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public string rawName       { get { return GetRawName(); }}
		public string nodeName 		{ get { return GetNodeName(); }}
        public string displayString {
            get {
                if(myDisplayString == null) {
                    myDisplayString= GetDisplayString();
                }
                return myDisplayString;
            }
        }
		public Texture libraryIcon {
			get {
				if(myLibraryIcon == null) {
					myLibraryIcon= GetLibraryIcon();
				}
				return myLibraryIcon;
			}
		}

        // ======================================================================
        // FORMATTING HELPERS
        // ----------------------------------------------------------------------
        public string mainValueBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=cyan><b>" : "<color=blue><b>"); }
        }
        public string mainValueEnd { get { return "</b></color>"; }}
		public string firstPartBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=lime><i>" : "<color=green><i>");	}
		}
		public string firstPartEnd { get { return "</i></color>"; }}
		public string secondPartBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=yellow><i>" : "<color=brown><i>"); }
		}
		public string secondPartEnd { get { return "</i></color>"; }}
        
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal abstract string 	GetRawName();
		internal abstract string 	GetNodeName();
        internal abstract string	GetDisplayString();
		internal virtual  Texture	GetLibraryIcon() {
			return TextureCache.GetIcon(Icons.kiCanScriptIcon);
		}

        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        public LibraryObject() {}
        
        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the library object with the given raw name.
        ///
        /// @param rawName The raw name to search for.
        /// @return The library object that matches. _null_ if not found.
        ///
        public T GetChild<T>(string rawName) where T : LibraryObject {
            return GetChild<T>(t=> t.GetRawName() == rawName);
        }
        
        // ----------------------------------------------------------------------
        /// Iterates the entire tree invoking the given action.
        ///
        /// @param fnc The action to invoke for each element in the tree.
        ///
        public void ForEach(Action<LibraryObject> fnc) {
            fnc(this);
            if(children != null) {
                foreach(var c in children) {
                    var libraryObject= c as LibraryObject;
                    if(libraryObject != null) {
                        libraryObject.ForEach(fnc);
                    }
                }
            }
        }

        // ----------------------------------------------------------------------
		/// Split the namespace into root and child namespace parts.
		///
		/// @param namespaceName The namespace to split.
		/// @param level1 The root namespace.
		/// @param level2 The child namesapces.
		///
		public static void SplitNamespace(string namespaceName, out string level1, out string level2) {
            level1= "";
            level2= "";
            if(!string.IsNullOrEmpty(namespaceName)) {
                var namespaceLen= namespaceName == null ? 0 : namespaceName.Length;
                var separator= namespaceName.IndexOf('.');
                if(separator >= 0 && separator < namespaceLen) {
                    level1= namespaceName.Substring(0, separator);
                    level2= namespaceName.Substring(separator+1, namespaceLen-separator-1);
                }
                else {
                    level1= namespaceName;
                }                    
            }	
		}

    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the root node that contains all library objects.
    public class LibraryRoot : LibraryObject {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryRoot() {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string	GetRawName()        { return "Library"; }
		internal override string	GetNodeName()		{ return GetRawName(); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the root namespace library object with the given name.
        ///
        /// @param name The name of the root namespace to search for.
        /// @return The found or created root namespace library object.
        ///
        public LibraryRootNamespace GetRootNamespace(string name) {
            var node= GetChild<LibraryRootNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryRootNamespace(name);
                AddChild(node);
            }
            return node;
        }

        // ----------------------------------------------------------------------
        /// Returns the library type object for the given type.
        ///
        /// @param type The type to search for.
        /// @return The library type if found. _null_ otherwise.
        ///
        public LibraryType GetLibraryType(Type type) {
            string nsRoot= "";
            string nsChildren= "";
			SplitNamespace(type.Namespace, out nsRoot, out nsChildren);
            var rootNamespace = GetRootNamespace(nsRoot);
            if(rootNamespace == null) return null;
            var childNamespace= rootNamespace.GetChildNamespace(nsChildren);
            if(childNamespace == null) return null;
			return childNamespace.GetLibraryType(type);
        }
        
        // ----------------------------------------------------------------------
        /// Sorts the root namespace and ask all children to perform sorting.
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryRootNamespace>(
                (x,y)=> string.Compare(x.GetRawName(), y.GetRawName())
            );
            // -- Ask our children to sort their children on so on... -- 
            foreach(var c in children) {
                (c as LibraryRootNamespace).Sort();
            }
        }
    }
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the first level namespace in the library.
    public class LibraryRootNamespace : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string myName= null;

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
		internal override Texture	GetLibraryIcon() {
			if(myName.StartsWith("iCanScript")) {
				return TextureCache.GetIcon(Icons.kiCanScriptIcon);
			}
			if(myName.StartsWith("System")) {
				return TextureCache.GetIcon(Icons.kDotNetIcon);
			}
			if(myName.StartsWith("Unity")) {
				return TextureCache.GetIcon(Icons.kUnityIcon);
			}
			return TextureCache.GetIcon(Icons.kCompanyIcon);
		}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryRootNamespace(string name) : base() { myName= name; }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the child namespace library object with the given name.
        ///
        /// @param name The name of the child namespace to search for.
        /// @return The found or created child namespace library object.
        ///
        public LibraryChildNamespace GetChildNamespace(string name) {
            var node= GetChild<LibraryChildNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryChildNamespace(name);
                AddChild(node);
            }
            return node;
        }

        // ----------------------------------------------------------------------
        /// Sorts the child namespaces and ask all children to perform sorting.
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryChildNamespace>(
                (x,y)=> string.Compare(x.GetRawName(), y.GetRawName())
            );
            // -- Ask our children to sort their children on so on... -- 
            foreach(var c in children) {
                (c as LibraryChildNamespace).Sort();
            }
        }
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the nested namespaces in the library (2nd level).
    public class LibraryChildNamespace : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string myName= null;

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kNamespaceIcon);
		}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryChildNamespace(string name) : base() { myName= name; }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the type library object for the given type.
        ///
        /// @param type The type to search for.
        /// @return The found library type object or _null_ if not found.
        ///
        public LibraryType GetLibraryType(Type type) {
            return GetChild<LibraryType>(t=> t.type == type);
        }

        // ----------------------------------------------------------------------
        /// Sorts the child types and ask all children to perform sorting.
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryType>(
                (x,y)=> string.Compare(x.GetRawName(), y.GetRawName())
            );
            // -- Ask our children to sort their children on so on... -- 
            foreach(var c in children) {
                (c as LibraryType).Sort();
            }
        }
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines a Unity event handler library object.
    public class LibraryEventHandler : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string 			myName       = null;
		public Type   	declaringType= null;
		public Type[] 	parameterTypes= null;
		public string[]	parameterNames= null;
		
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for an event handler node.
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kEventHandlerIcon);
		}				

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryEventHandler(string name, Type declaringType,
								   Type[] parameterTypes, string[] parameterNames)
		: base() {
			myName= name;
			this.declaringType= declaringType;
			this.parameterTypes= parameterTypes;
			this.parameterNames= parameterNames;
		}
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the class that represents a programming type in the library.
    public class LibraryType : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        public Type    type= null;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public Type		baseType			{ get { return type.BaseType; }}
		public bool 	isGeneric 			{ get { return type.IsGenericType; }}
		public Type[]	genericArguments	{ get { return type.GetGenericArguments(); }}
		
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryType(Type type) : base()    { this.type= type; }

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()   { return type.Name; }
		internal override string GetNodeName()	{ return NameUtility.ToDisplayName(type); }
        internal override string GetDisplayString() {
			// -- Start with the base name --
			var displayName= new StringBuilder(mainValueBegin, 64);
			displayName.Append(NameUtility.ToDisplayNameNoGenericArguments(type));
			// -- Add generic arguments --
			if(isGeneric) {
				displayName.Append("<i>");
				displayName.Append(NameUtility.ToDisplayGenericArguments(type));
				displayName.Append("</i>");
			}
			displayName.Append(mainValueEnd);
			// -- Add inheritance information --
			if(baseType != null && baseType != typeof(void)) {
				displayName.Append(firstPartBegin);
				displayName.Append(" : ");
				displayName.Append(NameUtility.ToDisplayName(baseType));
				displayName.Append(firstPartEnd);
			}
			return displayName.ToString();
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a type node.
		internal override Texture GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kTypeIcon);
		}		

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Sorts the child members according to their type.
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryMemberInfo>(
                (x,y)=> {
                    if(x.isField && !(y.isField)) return -1;
                    if(y.isField && !(x.isField)) return 1;
                    if(x.isProperty && !(y.isProperty)) return -1;
                    if(y.isProperty && !(x.isProperty)) return 1;
                    if(x.isConstructor && !(y.isConstructor)) return -1;
                    if(y.isConstructor && !(x.isConstructor)) return 1;
                    return string.Compare(x.GetRawName(), y.GetRawName());
                }
            );
        }

        // ----------------------------------------------------------------------
        /// Returns the members of type T installed on this type.
        ///
        /// @return The array of member <T> installed on this type.
        ///
        public T[] GetMembers<T>() where T : LibraryObject {
            var events= P.filter(p=> p is T, children);
            return P.map(p=> p as T, events).ToArray();
        }

    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the base class for all library objects that are based on a
    /// MemberInfo.
    public abstract class LibraryMemberInfo : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        public MemberInfo   memberInfo= null;
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public string		memberName	     { get { return memberInfo.Name; }}
        public MemberTypes  memberType       { get { return memberInfo.MemberType; }}
		public Type			declaringType    { get { return memberInfo.DeclaringType; }}
        public bool         isField          { get { return memberType == MemberTypes.Field; }}
        public bool         isProperty       { get { return isPropertyGetter || isPropertySetter; }}
        public bool         isConstructor    { get { return memberType == MemberTypes.Constructor; }}
        public bool         isMethod         { get { return memberType == MemberTypes.Method; }}
        public bool         isPropertyGetter { get { return this is LibraryPropertyGetter; }}
        public bool         isPropertySetter { get { return this is LibraryPropertySetter; }}
        public bool         isInherited   {
            get {
				var libraryType= parent as LibraryType;
            	return libraryType.type != memberInfo.DeclaringType;
            }
        }
        public string displayNameHeader {
			get { return (EditorGUIUtility.isProSkin ? "<color=cyan><b>" : "<color=blue><b>"); }
        }
        public string displayNameTrailer {
   			get { return "</b></color>"; }
        }

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryMemberInfo(MemberInfo memberInfo) : base() {
            this.memberInfo= memberInfo;
        }

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return memberInfo.Name; }
		internal override string GetNodeName()	{ return NameUtility.ToDisplayName(GetRawName()); }
        internal override string GetDisplayString() {
			var displayString= new StringBuilder(mainValueBegin,32);
			displayString.Append(GetNodeName());
			displayString.Append(mainValueEnd);
			return displayString.ToString();
		}
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the base class for all fields in the library.
    public class LibraryField : LibraryMemberInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public FieldInfo		fieldInfo	{ get { return memberInfo as FieldInfo; }}
		public bool				isStatic	{ get { return fieldInfo.IsStatic; }}
		public bool				isConst     { get { return fieldInfo.IsLiteral; }}
        public string           fieldName   { get { return fieldInfo.Name; }}
        public Type             fieldType   { get { return fieldInfo.FieldType; }}
		
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryField(MemberInfo memberInfo) : base(memberInfo) {}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a programatic type field.
    public class LibraryFieldGetter : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryFieldGetter(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "get_"+memberInfo.Name; }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a field get node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.OutEndPortIcon;
		}
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" --> ");
			result.Append(secondPartBegin);
			result.Append(NameUtility.ToDisplayName(fieldType));
			result.Append(secondPartEnd);
			return result.ToString();
		}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a programatic type field.
    public class LibraryFieldSetter : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryFieldSetter(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "set_"+memberInfo.Name; }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a field set node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.InEndPortIcon;
		}
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" <-- ");
			result.Append(firstPartBegin);
			result.Append(NameUtility.ToDisplayName(fieldType));
			result.Append(firstPartEnd);
			return result.ToString();
		}
    }

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

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the base class that represents a property in the library.
    public class LibraryProperty : LibraryMethodInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public string	propertyName	{ get { return methodInfo.Name; }}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryProperty(MethodInfo methodInfo) : base(methodInfo) {}
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a property get in the library.
    public class LibraryPropertyGetter : LibraryProperty {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryPropertyGetter(MethodInfo methodInfo) : base(methodInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" --> ");
			result.Append(secondPartBegin);
			result.Append(NameUtility.ToDisplayName(returnType));
			result.Append(secondPartEnd);
			return result.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            return NameUtility.ToDisplayName(memberInfo.Name);
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a property get node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.OutEndPortIcon;
		}
		
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a property set in the library.
    public class LibraryPropertySetter : LibraryProperty {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryPropertySetter(MethodInfo methodInfo) : base(methodInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" <-- ");
			result.Append(firstPartBegin);
			result.Append(NameUtility.ToDisplayName(parameters[0].ParameterType));
			result.Append(firstPartEnd);
			return result.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            return NameUtility.ToDisplayName(memberInfo.Name);
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a property set node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.InEndPortIcon;
		}
		
    }
    
}
