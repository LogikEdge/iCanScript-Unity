using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

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
    /// Base class for all objects in the library.
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
    
    public class LibraryRoot : LibraryObject {
        public LibraryRoot() {}
        internal override string GetDisplayString() { return "Library"; }
        internal override string GetRawName()       { return "Library"; }
        public LibraryRootNamespace GetRootNamespace(string name) {
            var node= GetChild<LibraryRootNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryRootNamespace(name);
                AddChild(node);
            }
            return node;
        }
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
    
    public abstract class LibraryMemberInfo : LibraryObject {
        public MemberInfo   memberInfo= null;
        
        public LibraryMemberInfo(MemberInfo memberInfo) : base() {
            this.memberInfo= memberInfo;
        }
        public MemberTypes memberType { get { return memberInfo.MemberType; }}
        internal override string GetRawName()       { return memberInfo.Name; }
        internal override string GetDisplayString() { return memberInfo.Name; }
        public bool isField       { get { return memberType == MemberTypes.Field; }}
        public bool isProperty    { get { return isGetProperty || isSetProperty; }}
        public bool isConstructor { get { return memberType == MemberTypes.Constructor; }}
        public bool isMethod      { get { return memberType == MemberTypes.Method; }}
        public bool isGetProperty { get { return isMethod && memberInfo.Name.StartsWith("get_"); }}
        public bool isSetProperty { get { return isMethod && memberInfo.Name.StartsWith("set_"); }}

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
    
    public class Constructor : LibraryMemberInfo {
        public Constructor(MemberInfo memberInfo) : base(memberInfo) {}
    }
    public class Field : LibraryMemberInfo {
        public Field(MemberInfo memberInfo) : base(memberInfo) {}
    }
    public class Function : LibraryMemberInfo {
        public Function(MemberInfo memberInfo) : base(memberInfo) {}
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
