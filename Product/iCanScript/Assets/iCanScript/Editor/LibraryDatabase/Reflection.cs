using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

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
    
    public abstract class LibraryObject : TreeNode {
        public LibraryObject() {}
        
        public T GetChild<T>(string displayName) where T : LibraryObject {
            return GetChild<T>(t=> t.GetDisplayString() == displayName);
        }
        public abstract string GetDisplayString();
    }
    public class LibraryRoot : LibraryObject {
        public LibraryRoot() {}
        public override string GetDisplayString() { return "Library"; }
        public LibraryRootNamespace GetRootNamespace(string name) {
            var node= GetChild<LibraryRootNamespace>(t=> t.GetDisplayString() == name);
            if(node == null) {
                node= new LibraryRootNamespace(name);
                AddChild(node);
            }
            return node;
        }
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryRootNamespace>(
                (x,y)=> string.Compare(x.GetDisplayString(), y.GetDisplayString())
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
        public override string GetDisplayString()  { return name; }
        public LibraryChildNamespace GetChildNamespace(string name) {
            var node= GetChild<LibraryChildNamespace>(t=> t.GetDisplayString() == name);
            if(node == null) {
                node= new LibraryChildNamespace(name);
                AddChild(node);
            }
            return node;
        }
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryChildNamespace>(
                (x,y)=> string.Compare(x.GetDisplayString(), y.GetDisplayString())
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
        public override string GetDisplayString()   { return name; }
        public void Sort() {
            // -- Sort our children --
            Sort<LibraryType>(
                (x,y)=> string.Compare(x.GetDisplayString(), y.GetDisplayString())
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
        public override string GetDisplayString() {
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
                    return string.Compare(x.GetDisplayString(), y.GetDisplayString());
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
        public override string GetDisplayString() { return memberInfo.Name; }
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
        public override string GetDisplayString() {
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
    
    public static class Reflection {
        // ======================================================================
        // CONSTANTS
        // ----------------------------------------------------------------------
        static string[] assembliesToIgnore= new string[]{
            "Boo.Lang", "Boo.Lang.Parser", "Boo.Lang.Compiler",
            "Unity.IvyParser", "AssetStoreTools", "AssetStoreToolsExtra",
            "Unity.SerializationLogic", "UnityScript.Lang",
            "ICSharpCode.NRefactory", "UnityScript",
            "Unity.Locator", "Unity.PackageManager",
            "Mono.Cecil", "Mono.Security",
            "Unity.DataContract"
        };
        static string[] namespacesToIgnore= new string[]{
            "iCanScript",
            "UnityEditorInternal", "UnityEngineInternal",
            "Microsoft", "Mono"
        };
        static string[] namespacesToInclude= new string[]{
            "iCanScript.Nodes"
        };
        
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        static int myNbOfTypes       = 0;
        static int myNbOfConstructors= 0;
        static int myNbOfFields      = 0;
        static int myNbOfFunctions   = 0;
        static LibraryRoot      myDatabase= new LibraryRoot();
        static Thread           myThread  = null;
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static LibraryRoot LibraryDatabase {
            get { return myDatabase; }
        }
        
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        /// Scans the application library and extracts the needed nodes.
    	static Reflection() {
            // Create a thread to parse the AppDomain types.
            myThread = new Thread(new ThreadStart(ExtractFromAppDomain));
            myThread.Start();
//            ExtractFromAppDomain();
//            myDatabase.Sort();
    	}
        
        // ----------------------------------------------------------------------
        /// Start the application controller.
    	public static void Start() {}
        // ----------------------------------------------------------------------
        /// Shutdowns the application controller.
        public static void Shutdown() {}

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static bool IsLibraryLoaded {
            get { return myThread.ThreadState == ThreadState.Stopped; }
        }
        
        // ======================================================================
        // EXTRACT ALL PUBLIC MEMBERS
        // ----------------------------------------------------------------------
        /// Extracts all public members (except for iCanScript).
        static void ExtractFromAppDomain() {
            Debug.Log("Start building library");
//            try {
                var assemblies= AppDomain.CurrentDomain.GetAssemblies();
                foreach(var assembly in assemblies) {
                    // -- Don't parse assemblies that should be ignored --
                    var assemblyName= assembly.FullName;
                    bool ignoreAssembly= false;
                    foreach(var a in assembliesToIgnore) {
                        if(assemblyName.StartsWith(a)) {
                            ignoreAssembly= true;
                            break;
                        }
                    }
                    if(ignoreAssembly) continue;
    //                Debug.Log("Assembly: "+assemblyName);
                    ExtractAssembly(assembly);
                }
                Debug.Log("# types: "+myNbOfTypes+" # constructors: "+myNbOfConstructors+" # fields: "+myNbOfFields+" # functions: "+myNbOfFunctions);  
//            }
//            catch(System.Exception e) {
//                Debug.LogError(e.Message);
//            }

        }

        // ----------------------------------------------------------------------
        /// Extracts all types from an assembly.
        ///
        /// @param assembly The assembly from which to extract the types.
        ///
        static void ExtractAssembly(Assembly assembly) {
            foreach(var type in assembly.GetTypes()) {
                // -- Don't parse namespaces that should be ignored. --
                var namespaceName= type.Namespace;
                if(namespaceName == null) {
                    namespaceName= "";
                }
                else {
                    bool mustInclude= false;
                    foreach(var ns in namespacesToInclude) {
                        if(namespaceName.StartsWith(ns)) {
                            mustInclude= true;
                            break;
                        }
                    }
                    if(mustInclude == false) {
                        bool ignoreNamespace= false;
                        foreach(var ns in namespacesToIgnore) {
                            if(namespaceName.StartsWith(ns)) {
                                ignoreNamespace= true;
                                break;
                            }
                        }
                        if(ignoreNamespace) continue;
                    }
                }
                // -- Ignore all types that don't start with a letter. --
                var typeName= type.Name;
                var firstLetter= typeName[0];
                if(!Char.IsLetter(firstLetter) && firstLetter != '_') continue;
                ++myNbOfTypes;
                // -- Build namespace descriptors -- 
                string level1= "";
                string level2= "";
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
                var rootNamespace = myDatabase.GetRootNamespace(level1);
                var childNamespace= rootNamespace.GetChildNamespace(level2);
                
                // -- Extract type internal information --
                ExtractType(childNamespace, type);
            }        
        }
        
        // ----------------------------------------------------------------------
        /// Extracts all public constructors/fields/functions from the given type.
        ///
        /// @param type The type from which to extract the members.
        ///
        static void ExtractType(LibraryChildNamespace parentNamespace, Type type) {
            var libraryType= new LibraryType(type);
            parentNamespace.AddChild(libraryType);
            ExtractConstructors(libraryType, type);
            ExtractFields(libraryType, type);
            ExtractFunctions(libraryType, type);
        }
        
        // ----------------------------------------------------------------------
        /// Extracts all constructors of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractConstructors(LibraryType parentType, Type type) {
            foreach(var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                ++myNbOfConstructors;
                parentType.AddChild(new Constructor(constructor));
            }
        }

        // ----------------------------------------------------------------------
        /// Extracts all fields of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractFields(LibraryType parentType, Type type) {
            foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {                
                ++myNbOfFields;
                parentType.AddChild(new Field(field));
            }
        }

        // ----------------------------------------------------------------------
        /// Extracts all functions of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractFunctions(LibraryType parentType, Type type) {
            foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                ++myNbOfFunctions;
                parentType.AddChild(new Function(method));
            }
        }
    }
    
}
