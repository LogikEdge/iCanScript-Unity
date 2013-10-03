// ==> UNCOMMENT THE FOLLOWING DEFINE TO CREATE YOUR OWN CUSTOM NODE INSTALLER <==
//#define _iCS_USE_CUSTOM_INSTALLER_
#if _iCS_USE_CUSTOM_INSTALLER_

using UnityEngine;
using System;

public static class iCS_CustomInstaller {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // IT IS RECOMMANDED THAT YOU MOVE THIS FILE INTO YOUR OWN FOLDER OUTSIDE
    // OF THE iCanScript FOLDER.  THIS WILL AVOID THAT YOUR MODIFICATIONS GET
    // OVERWRITTEN ON THE NEXT iCanScript PRODUCT UPGRADE.
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    // ------------------------------------------------------------------------
    // Use the PopulateDataBase() function to install in bulk iCanScript nodes
    // from classes present in the .NET assemblies (including your own).  You
    // can install types from an assembly for which you do not have source code.
    //
    // All public fields, properties, and functions will become available when
    // installing in bulk.  You can use the Unity or .NET installation files
    // as a template for your own code (see iCS_NETClasses & iCS_UnityClasses).
    //
    // You should consider using the iCanScript custom attributes for fine
    // grain control over the installation of your nodes (preferred appraoch).
    // iCanScript will automatically extract from the assembly your nodes
    // when custom attributes are used (you do not need to manually add the
    // classes as is done here).
    //
    // ------------------------------------------------------------------------

    // ======================================================================
    // Constants
    // ==> (CHANGE THE CONSTANTS TO MEET YOUR NEEDS) <==
    // ----------------------------------------------------------------------
    const string kDefaultCompanyName= "YourCompany";   // First level in library tree
    const string kDefaultPackageName= "YourPackage";   // Second level in library tree
    const string kDefaultIcon       = iCS_Config.ResourcePath+"/iCS_Logo_32x32.png";

    // ========================================================================
    // Add types from existing assemblies into the iCanScript library.
    // This function get called after each recompile of the Unity scripts.
    //
    public static void PopulateDataBase() {
        // ==> INSERT YOUR CODE HERE ... <==
        // ex: PopulateWithType(typeof(MyType));
        //     PopulateWithType(typeof(MyType2), "Icon_Type2_32x32.png", "This is a cool node", "MyPackage2");

        // ==> COMMENT OUT THIS LINE ONCE YOU KNOW YOUR INSTALLER WORKS <==
        Debug.Log("iCanScript: Custom Installer invoked !!!");
    }


    // ------------------------------------------------------------------------
    //  Helper function to fill-in "company", "package" and "icon" information.
    //  The default values for company, package and icon are taken from
    // 'kDefaultCompanyName', 'kDefaultPackageName' & 'kDefaultIcon'.
    //
    static void PopulateWithType(Type type, string iconPath= null, string description= null,
                                            string package= null,  string company= null) {
                                     
        // Fill-in default values if not provided.
        bool                    decodeAllPublicMembers= true;
        if(company == null)     company               = kDefaultCompanyName;
        if(package == null)     package               = kDefaultPackageName;
        if(iconPath == null)    iconPath              = kDefaultIcon;
        if(description == null) description           = company+"::"+package+"::"+type.Name;

        // Invoke the iCanScript library installer.
        iCS_Reflection.DecodeClassInfo(type, company, package, description, iconPath, decodeAllPublicMembers);        
    }
}
#endif
