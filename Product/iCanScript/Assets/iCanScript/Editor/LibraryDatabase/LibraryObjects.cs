using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

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
                string  myDisplayString= null;
        public  Vector2 displaySize    = Vector2.zero;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public string nodeName 		{ get { return GetNodeName(); }}
        public string displayString {
            get {
                if(myDisplayString == null) {
                    myDisplayString= GetDisplayString();
                }
                return myDisplayString;
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
        internal abstract string GetRawName();
        internal abstract string GetDisplayString();
		internal abstract string GetNodeName();

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
        internal override string GetRawName()       { return "Library"; }
		internal override string GetNodeName()		{ return GetRawName(); }
        internal override string GetDisplayString() { return GetNodeName(); }

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
        internal override string GetRawName()        { return myName; }
		internal override string GetNodeName()		 { return NameUtility.ToDisplayName(myName); }
        internal override string GetDisplayString()  { return GetNodeName(); }

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
        internal override string GetRawName()        { return myName; }
		internal override string GetNodeName()		 { return NameUtility.ToDisplayName(myName); }
        internal override string GetDisplayString()  { return GetNodeName(); }

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryChildNamespace(string name) : base() { myName= name; }

        // ======================================================================
        // UTILITIES
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
        internal override string GetRawName()       { return type.Name; }
		internal override string GetNodeName() {
			return NameUtility.ToDisplayName(type);
		}
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
        public MemberTypes  memberType    { get { return memberInfo.MemberType; }}
		public Type			declaringType { get { return memberInfo.DeclaringType; }}
        public bool         isField       { get { return memberType == MemberTypes.Field; }}
        public bool         isProperty    { get { return isGetProperty || isSetProperty; }}
        public bool         isConstructor { get { return memberType == MemberTypes.Constructor; }}
        public bool         isMethod      { get { return memberType == MemberTypes.Method; }}
        public bool         isGetProperty { get { return isMethod && memberInfo.Name.StartsWith("get_"); }}
        public bool         isSetProperty { get { return isMethod && memberInfo.Name.StartsWith("set_"); }}
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
        internal override string GetRawName()       { return memberInfo.Name; }
		internal override string GetNodeName() {
			return NameUtility.ToDisplayName(GetRawName());
		}
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
    public class LibraryGetField : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryGetField(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "get_"+memberInfo.Name; }
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a programatic type field.
    public class LibrarySetField : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibrarySetField(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "set_"+memberInfo.Name; }
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
			return displayString.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
			return NameUtility.ToDisplayName(declaringType);
		}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a function in the library.
    public class LibraryFunction : LibraryMethodBase {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public MethodInfo	methodInfo	{ get { return memberInfo as MethodInfo; }}
		public Type			returnType  { get { return methodInfo.ReturnType; }}

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
            var inputs= "("+ToDisplayInputParameters+")";
            // -- Get output parameters --
            var outputs= "("+ToDisplayOutputParameters+")";
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
            return part1 + "->" + outputTypesHeader+outputs+outputTypesTrailer;
        }
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            var name= NameUtility.ToDisplayName(memberInfo.Name);
            if(isGeneric) {
				name+= NameUtility.ToDisplayGenericArguments(genericArguments);
            }
			return name;
		}
    }
    
}
