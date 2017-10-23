using UnityEngine;
using System;
using System.IO;

public static class iCS_NETClasses {
    // ----------------------------------------------------------------------
    // Install the desired .NET classes
    public static void PopulateDataBase() {
        DecodeNETClassInfo(typeof(string));
        DecodeNETClassInfo(typeof(char));
        DecodeNETClassInfo(typeof(int));
        DecodeNETClassInfo(typeof(float));
        DecodeNETClassInfo(typeof(Array));
        DecodeNETClassInfo(typeof(Path));
        DecodeNETClassInfo(typeof(File));
        DecodeNETClassInfo(typeof(Directory));
    }

    // ----------------------------------------------------------------------
    // Helper function to simplify .NET class decoding.
    public static void DecodeNETClassInfo(Type type) {
        iCS_Reflection.DecodeClassInfo(type, "NET", ".NET class "+type.Name, null, true);
    }
}
