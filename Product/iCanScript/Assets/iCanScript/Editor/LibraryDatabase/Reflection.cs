using UnityEngine;
using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

    public class LibraryDatabaseObject {
        public int    level= 0;
        public string name = null;
        
        public LibraryDatabaseObject(string name, int level= 0) {
            this.name= name;
            this.level= level;
        }
    }
    public class RootNamespace : LibraryDatabaseObject {
        public List<ChildNamespace>    children= new List<ChildNamespace>();
        
        public RootNamespace(string name) : base(name, 0) {
        }
    }
    public class ChildNamespace : LibraryDatabaseObject {
        public ChildNamespace(string name) : base(name, 1) {
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
            "iCanScript.Editor", "iCanScript.Engine",
            "Microsoft", "Mono"
        };
        
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        static int myNbOfTypes       = 0;
        static int myNbOfConstructors= 0;
        static int myNbOfFields      = 0;
        static int myNbOfFunctions   = 0;
        static List<RootNamespace>   myDatabase= new List<RootNamespace>();
        static Assembly[]            myAssemblies= null;
        static Thread                myThread= null;
        
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        /// Scans the application library and extracts the needed nodes.
    	static Reflection() {
            // Create a thread to parse the AppDomain types.
            myThread = new Thread(new ThreadStart(ExtractFromAppDomain));
            myThread.Start();
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
                    bool ignoreNamespace= false;
                    foreach(var ns in namespacesToIgnore) {
                        if(namespaceName.StartsWith(ns)) {
                            ignoreNamespace= true;
                            break;
                        }
                    }
                    if(ignoreNamespace) continue;
                }
                // -- Ignore all types that don't start with a letter. --
                var typeName= type.Name;
                var firstLetter= typeName[0];
                if(!Char.IsLetter(firstLetter) && firstLetter != '_') continue;
                ++myNbOfTypes;
                // -- Build namespace descriptors -- 
                string level1= null;
                string level2= null;
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
                    AddRootNamespace(level1);
                }
                
                // -- Extract type internal information --
                ExtractConstructors(type);
                ExtractFields(type);
                ExtractFunctions(type);
            }        
        }
        
        // ----------------------------------------------------------------------
        /// Extracts all public constructors/fields/functions from the given type.
        ///
        /// @param type The type from which to extract the members.
        ///
        static void ExtractType(Type type) {
            ExtractConstructors(type);
            ExtractFields(type);
            ExtractFunctions(type);
        }
        
        // ----------------------------------------------------------------------
        /// Extracts all constructors of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractConstructors(Type type) {
            foreach(var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                ++myNbOfConstructors;
            }
        }

        // ----------------------------------------------------------------------
        /// Extracts all fields of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractFields(Type type) {
            foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                ++myNbOfFields;
            }
        }

        // ----------------------------------------------------------------------
        /// Extracts all functions of a given type.
        ///
        /// @param type The type from which to extract.
        ///
        static void ExtractFunctions(Type type) {
            foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                ++myNbOfFunctions;
            }
        }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        static void AddRootNamespace(string name) {
            if(!myDatabase.Exists(ns=> ns.name == name)) {
                Debug.Log("RootNamespace: "+name);
                myDatabase.Add(new RootNamespace(name));
            }
        }
        // ----------------------------------------------------------------------
        static void AddChildNamespace(RootNamespace root, string name) {
            if(!root.children.Exists(ns=> ns.name == name)) {
                Debug.Log("ChildNamespace: "+name);
                root.children.Add(new ChildNamespace(name));
            }
        }
    }
    
}
