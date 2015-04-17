using UnityEngine;
using System;
using System.Reflection;

public static class iCS_Installer {
    // ------------------------------------------------------------------------
    // This function is the central hub to install in bulk iCanScript nodes
    // for existing assemblies.
    //
    // A user can extend the iCanScript library by defining a custom installer.
    // A template custom installer file is available in the same folder as this
    // file and is named "iCS_CustomInstaller.cs".  Copy the template file into
    // your own project folder and uncomment the line:
    //
    //          //#define _iCS_USE_CUSTOM_INSTALLER_
    //
    // You can then add any public class for which an assembly has been compile
    // as part of the engine code.
    //
    // You should consider using the iCanScript custom attributes for fine
    // grain control over the installation of your nodes (preferred appraoch).
    // iCanScript will automatically extract from the assembly your nodes
    // when custom attributes are used (you do not need to manually add the
    // classes as is done here).
    //
    // Currently, we install Unity & .NET components in the node database.
    // ------------------------------------------------------------------------
    // Forces invokation of the constructor if not already done.
    public static void Install() {
        // The installer will get invoked once per recompile of the Unity scripts.
        if(iCS_Reflection.NeedToRunInstaller) {
            // Install the default .NET classes.
            iCS_NETClasses.PopulateDataBase();
            // Install the default Unity classes.
            iCS_UnityClasses.PopulateDataBase();

            // This is where the custom installer gets invoked ...
            // We use a dynamic call to avoid compile error when no custom
            // installer exists.
            var customInstaller= Type.GetType("iCS_CustomInstaller");
            if(customInstaller != null) {
                MethodInfo methodInfo= customInstaller.GetMethod("PopulateDataBase",BindingFlags.Public | BindingFlags.Static);
                if(methodInfo != null) {
                    methodInfo.Invoke(null, null);
                }

            }

            // Remember that the installers where already ran.
            iCS_Reflection.NeedToRunInstaller= false;                    
        }
    }
}

