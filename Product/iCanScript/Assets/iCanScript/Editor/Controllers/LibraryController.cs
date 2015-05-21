#define USE_THREAD
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    public static class LibraryController {
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        /// Scans the application library and extracts the needed nodes.
    	static LibraryController() {
#if USE_THREAD
            // -- Create a thread to parse the AppDomain types. --
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
            "UnityEditorInternal", "UnityEngineInternal",
            "Microsoft", "Mono", "JetBrains", "TreeEditor", "Unity."
        };
        static string[] namespacesToInclude= new string[]{
        };
        
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        static int 			myNbOfTypes       = 0;
        static int 			myNbOfConstructors= 0;
        static int 			myNbOfFields      = 0;
        static int 			myNbOfFunctions   = 0;
        static LibraryRoot  myDatabase        = new LibraryRoot();
        static Thread       myThread          = null;
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static LibraryRoot LibraryDatabase {
            get { return myDatabase; }
        }
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static bool IsLibraryLoaded {
            get { return myThread == null || myThread.ThreadState == ThreadState.Stopped; }
        }
        
        // ======================================================================
        // EXTRACT ALL PUBLIC MEMBERS
        // ----------------------------------------------------------------------
        /// Extracts all public members (except for iCanScript).
        static void ExtractFromAppDomain() {
			// -- Extract each assembly from the application code. --
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

			// -- Install Unity Event handlers. --
			InstallUnityEventHandlers();

            // -- Update search score. --
            myDatabase.ComputeScore();
            
			// -- Sort the database. --
            myDatabase.Sort();

            // -- Update visibility. --
            myDatabase.ComputeVisibility();
            
            // -- Take a snapshot of string sort to speed up sorting. --
            myDatabase.TakeSnapshotOfStringSortIndex();
        }

        // ----------------------------------------------------------------------
		/// Installs the Unity event handlers.
		static void InstallUnityEventHandlers() {
            MonoBehaviourEventHandlers.Install();
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
                // -- Don't parse .NET attribute. --
                if(type.BaseType == typeof(Attribute)) continue;
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
				SplitNamespace(namespaceName, out level1, out level2);
                var rootNamespace = myDatabase.GetRootNamespace(level1);
                var childNamespace= rootNamespace.GetChildNamespace(level2);
                
                // -- Ignore any child namespace that starts with "internal" or "private". --
                if(level2.StartsWith("internal", true, null)) continue;
                if(level2.StartsWith("private", true, null)) continue;
                
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
                parentType.AddChild(new LibraryFieldGetter(field));
				if(!field.IsLiteral) {
	                parentType.AddChild(new LibraryFieldSetter(field));
				}
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
				var methodName= method.Name;
				var parameters= method.GetParameters();
				LibraryObject libraryObject= null;
				if(methodName.StartsWith("get_") && parameters.Length == 0) {
					libraryObject= new LibraryPropertyGetter(method);					
				}
				else if(methodName.StartsWith("set_") && parameters.Length == 1) {
					libraryObject= new LibraryPropertySetter(method);
				}
				else {
	                libraryObject= new LibraryFunction(method);	
				}
                parentType.AddChild(libraryObject);					
            }
        }

        // ----------------------------------------------------------------------
        /// Add an event handler to the given class.
        ///
		/// @param eventName The name of the event.
        /// @param declaringType The parent type for the event handler.
		/// @param parameterTypes The type for each parameter.
		/// @param parameterNamse The name of each parameter.
        ///
		public static void AddEventHandler(string eventName, Type declaringType,
										   Type[] parameterTypes, string[] parameterNames) {
            // -- Build namespace descriptors -- 
            string level1= "";
            string level2= "";
			SplitNamespace(declaringType.Namespace, out level1, out level2);
            var rootNamespace = myDatabase.GetRootNamespace(level1);
            var childNamespace= rootNamespace.GetChildNamespace(level2);
			var libraryType= childNamespace.GetLibraryType(declaringType);
			if(libraryType == null) {
				Debug.LogWarning("iCanScript: Unable to add event handler: "+eventName+". The parent type is unknown: "+declaringType.Name);
				return;
			}
			libraryType.AddChild(new LibraryEventHandler(eventName, declaringType, parameterTypes, parameterNames));
		}

		// ======================================================================
		// UTILITIES
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
    
}
