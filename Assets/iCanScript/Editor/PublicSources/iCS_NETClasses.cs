using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class iCS_NETClasses {
    // ----------------------------------------------------------------------
    // Install the desired .NET classes
    static iCS_NETClasses() {
        DecodeNETClassInfo(typeof(string));
        DecodeNETClassInfo(typeof(char));
        DecodeNETClassInfo(typeof(Array));
        DecodeNETClassInfo(typeof(Path));
    }

    // ----------------------------------------------------------------------
    // Use this function to assure execution of static constructor.
    public static void PopulateDataBase() {}

    // ----------------------------------------------------------------------
    // Helper function to simplify .NET class decoding.
    public static void DecodeNETClassInfo(Type type) {
        iCS_Reflection.DecodeClassInfo(type, "NET", type.Name, ".NET class "+type.Name, null, true);
    }
}
