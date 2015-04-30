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
        public string displayString {
            get {
                if(myDisplayString == null) {
                    myDisplayString= GetDisplayString();
                }
                return myDisplayString;
            }
        }
        
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal abstract string GetRawName();
        internal abstract string GetDisplayString();

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
        internal override string GetDisplayString() { return "Library"; }
        internal override string GetRawName()       { return "Library"; }

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
        /// Sorts the entire library using the active sort algorithm. 
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
        public string name= null;
        public LibraryRootNamespace(string name) : base() { this.name= name; }
        internal override string GetDisplayString()  { return name; }
        internal override string GetRawName()        { return name; }
        public LibraryChildNamespace GetChildNamespace(string name) {
            var node= GetChild<LibraryChildNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryChildNamespace(name);
                AddChild(node);
            }
            return node;
        }
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
        public string name= null;
        public LibraryChildNamespace(string name) : base() { this.name= name; }
        internal override string GetDisplayString()   { return name; }
        internal override string GetRawName()          { return name; }
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
        public Type    type= null;

        public LibraryType(Type type) : base()    { this.type= type; }
        internal override string GetRawName()    { return type.Name; }
        internal override string GetDisplayString() {
            var name= type.Name;
            if(type.IsGenericType) {
                // -- Remove number of parameter info --
                int end= name.IndexOf('`');
                if(end > 0 && end < name.Length) {
                    name= name.Substring(0, end);
                }
                name+= "<";
                var genericArguments= type.GetGenericArguments();
                var len= genericArguments.Length;
                for(int i= 0; i < len; ++i) {
                    name+= genericArguments[i].Name;
                    if(i < len-1) {
                        name+=",";
                    }
                }
                name+= ">";
            }
            return name;
        }
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
        internal override string GetDisplayString() { return memberInfo.Name; }

    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Field : LibraryMemberInfo {
        public Field(MemberInfo memberInfo) : base(memberInfo) {}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class LibraryMethodBase : LibraryMemberInfo {
        public LibraryMethodBase(MethodBase methodBase) : base(methodBase) {}

        public MethodBase       methodBase  { get { return memberInfo as MethodBase; }}
        public bool             isPublic    { get { return methodBase.IsPublic; }}
        public bool             isProtected { get { return methodBase.IsFamily; }}
        public bool             isPrivate   { get { return methodBase.IsPrivate; }}
        public ParameterInfo[]  parameters  { get { return methodBase.GetParameters(); }}

        public string FunctionSignatureInputTypes {
            get {
                // -- We need a method to get parameters. --
                var methodInfo= memberInfo as MethodInfo;
                if(methodInfo == null) return "";
    			var result= new StringBuilder(32);
                bool needComma= false;
                if(!methodInfo.IsStatic) {
                    result.Append(iCS_Types.TypeName(methodInfo.DeclaringType));
                    needComma= true;
                }
                foreach(var param in methodInfo.GetParameters()) {
                    var paramType= param.ParameterType;
    				if(!paramType.IsByRef) {
                        if(needComma) {
                            result.Append(", ");
                        }
    	                result.Append(iCS_Types.TypeName(paramType));
                        needComma= true;
    				}
                }
    			// Add inputs to signature.
                var inputStr= result.ToString();
                if(inputStr.Length == 0) {
                    return "";
                }
                return "("+inputStr+")";
            }
        }
        public string FunctionSignatureOutputTypes {
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
    	                result.Append(iCS_Types.TypeName(paramType.GetElementType()));
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
    				result.Append(iCS_Types.TypeName(returnType));
    			}
                if(nbOfOutputs == 0) {
                    return "";
                }
    			if(nbOfOutputs == 1) {
                    return result.ToString();
    			}
                return "("+result.ToString()+")";
            }
        }

    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Constructor : LibraryMethodBase {
        public Constructor(ConstructorInfo constructorInfo) : base(constructorInfo) {}
    }
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Function : LibraryMethodBase {
        public Function(MethodInfo methodInfo) : base(methodInfo) {}
        internal override string GetDisplayString() {
            // -- Determine function name --
            var name= NameUtility.ToDisplayName(memberInfo.Name);
            var methodBase= memberInfo as MethodBase;
            if(methodBase.IsGenericMethod) {
                name+= "<";
                var genericArguments= methodBase.GetGenericArguments();
                var len= genericArguments.Length;
                for(int i= 0; i < len; ++i) {
                    name+= genericArguments[i].Name;
                    if(i < len-1) {
                        name+=",";
                    }
                }
                name+= ">";
            }
            // -- Get input parameters --
            var inputs= FunctionSignatureInputTypes;
            // -- Get output parameters --
            var outputs= FunctionSignatureOutputTypes;
            // -- Build formatted display string --
            var displayNameHeader= (EditorGUIUtility.isProSkin ? "<color=cyan><b>" : "<color=blue><b>");
            var displayNameTrailer= "</b></color>";
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
    }
    
}
