#define USE_THREAD
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
        static int 			myNbOfTypes       = 0;
        static int 			myNbOfConstructors= 0;
        static int 			myNbOfFields      = 0;
        static int 			myNbOfFunctions   = 0;
        static LibraryRoot  myDatabase= new LibraryRoot();
        static Thread       myThread  = null;
        
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
#if USE_THREAD
            // Create a thread to parse the AppDomain types.
            myThread = new Thread(new ThreadStart(ExtractFromAppDomain));
            myThread.Start();
#else
            ExtractFromAppDomain();
#endif
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
                ExtractAssembly(assembly);
            }
            myDatabase.Sort();

            Debug.Log("# types: "+myNbOfTypes+" # constructors: "+myNbOfConstructors+" # fields: "+myNbOfFields+" # functions: "+myNbOfFunctions);
        }

        // ----------------------------------------------------------------------
        /// Extracts all types from an assembly.
        ///
        /// @param assembly The assembly from which to extract the types.
        ///
        static void ExtractAssembly(Assembly assembly) {
            foreach(var type in assembly.GetTypes()) {
                // -- Don't parse private types --
                if(!type.IsPublic) continue;
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
                
                // -- Extract type members --
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
                parentType.AddChild(new LibraryConstructor(constructor));
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
                parentType.AddChild(new LibraryField(field));
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
                parentType.AddChild(new LibraryFunction(method));
            }
        }
    }
    
}
