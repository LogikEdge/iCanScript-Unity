using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class iCS_NETClasses {
    // ----------------------------------------------------------------------
    // Install the desired .NET classes
    public static void PopulateDataBase() {
        DecodeNETClassInfo(typeof(string));
        DecodeNETClassInfo(typeof(char));
        DecodeNETClassInfo(typeof(Array));
        DecodeNETClassInfo(typeof(Path));        
    }

    // ----------------------------------------------------------------------
    // Helper function to simplify .NET class decoding.
    public static void DecodeNETClassInfo(Type type) {
        iCS_Reflection.DecodeClassInfo(type, "NET", "System", ".NET class "+type.Name, null, true);
    }
}
