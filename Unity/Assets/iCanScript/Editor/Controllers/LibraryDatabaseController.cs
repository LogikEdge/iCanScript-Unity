using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace iCanScriptEditor {
    
    public class LibraryDatabaseController {
        // ======================================================================
        // Common Controller activation/deactivation
        // ----------------------------------------------------------------------
    	static LibraryDatabaseController() {
            // Extract nodes from the active assemblies.
            iCS_Reflection.ParseAppDomain();
    //        AddClass("iCS_EngineObject");
    //        AddClass("iCS_EngineObject");
    	}
        
        /// Start the application controller.
    	public static void Start() {}
        /// Shutdowns the application controller.
        public static void Shutdown() {}
    
        // ======================================================================
        // Library Database Management
        // ----------------------------------------------------------------------
        // Register each public class in the given dll
        public static void AddDLL(string dllNameWithoutExtension) {
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {    // Install all public type of the Unity Engine assembly
                AssemblyName name= assembly.GetName();
                Debug.Log("AssemblyName=> "+name.Name);
    //            if(assembly.FullName.StartsWith("UnityEngine")) {
    //                foreach(var classType in assembly.GetTypes()) {
    //                    if(classType.IsPublic && !classType.IsGenericType) {
    //                        DecodeClassInfo(classType);
    //                    }
    //                }
    //            }
            }
        }
        // ----------------------------------------------------------------------
        // Register the given public class
        public static void AddClass(string className, string assemblyName= null) {
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {    // Install all public type of the Unity Engine assembly
                AssemblyName name= assembly.GetName();
                // -- Determine if we have the good assembly --
                bool isGoodAssembly= false;
                if(assemblyName != null) {
                    isGoodAssembly= name.Name == assemblyName;
                } else {
                    isGoodAssembly= name.Name == "Assembly-UnityScript" || name.Name == "Assembly-CSharp";
                }
                // -- Serach for the given class --
                if(isGoodAssembly) {
                    foreach(var classType in assembly.GetTypes()) {
                        if(classType.Name == className) {
                            Debug.Log("Adding=> "+classType.Name);
    //                        DecodeClassInfo(classType);
                            iCS_Reflection.DecodeClassInfo(classType, "(*) Imported Library", name.Name, null, null, true, true);
                        }
                    }
                    
                    
                }
            }
        }
        
        // ======================================================================
        // The following are helper functions to register Unity3D classes
        // ----------------------------------------------------------------------
        // Use this function to register Unity3d classes.
        // All public fields/properties and methods will be registered.
        //
        // This function can be called by the iCanScript user to add to the
        // existing Unity library.
        //
    //    public static void DecodeClassInfo(Type classType, string package= "UnityEngine", string iconPath= null, string description= null) {
    //        string company = "Unity";
    //        bool decodeAllPublicMembers= true;
    //        if(package == null) package = kUnityEnginePackage;
    //        if(description == null) description = null;
    //        if(iconPath == null) iconPath = kUnityIcon;
    //        iCS_Reflection.DecodeClassInfo(classType, company, package, description, iconPath, decodeAllPublicMembers,true);
    //    }
        
    }
    
}