using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class UK_NETClasses {
    public static void PopulateDataBase() {
        UK_Reflection.DecodeNETClassInfo(typeof(string));
        UK_Reflection.DecodeNETClassInfo(typeof(char));
        UK_Reflection.DecodeNETClassInfo(typeof(Array));
        UK_Reflection.DecodeNETClassInfo(typeof(Path));
    }
}
