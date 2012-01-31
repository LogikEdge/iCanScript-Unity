using UnityEngine;

public static class iCS_Installer {
    // ------------------------------------------------------------------------
    // Use this function to install/configure iCanScript.
    // Currently, we install the Unity & .NET components in the node database.
    public static void Install() {
        iCS_NETClasses.PopulateDataBase();
        iCS_UnityClasses.PopulateDataBase();
    }
}
