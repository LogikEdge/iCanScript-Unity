using UnityEngine;
using System.Collections;
using System.IO;
using DisruptiveSoftware;

public static class iCS_StorageImportExport {
    public static void Export(iCS_Storage storage) {
		string fileName= iCS_DevToolsConfig.ScreenShotsFolder+"/"+iCS_DateTime.DateTimeAsString()+"--"+storage.name+".json";
        Export(storage, fileName);
    }
    public static void Export(iCS_Storage storage, string path) {
        var root= new JObject(new JNameValuePair("Storage", JValue.Build(storage)));
		File.WriteAllText(Application.dataPath + path, JSONPrettyPrint.Print(root.Encode()));     
    }
    public static void Import(iCS_Storage storage) {
        
    }
    public static void Import(iCS_Storage storage, string path) {
        
    }
}
