using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class WD_NETClasses {
    public static void PopulateDataBase() {
        WD_Reflection.DecodeNETClassInfo(typeof(string));
        WD_Reflection.DecodeNETClassInfo(typeof(char));
        WD_Reflection.DecodeNETClassInfo(typeof(Array));
        WD_Reflection.DecodeNETClassInfo(typeof(Path));
    }
}
