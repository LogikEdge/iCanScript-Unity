using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.IO;
using DisruptiveSoftware;

public static class iCS_VisualScriptImportExport {
	// =================================================================================
    // Export
    // ---------------------------------------------------------------------------------
    public static void Export(iCS_VisualScriptData storage, string path) {
        var root= new JObject(new JNameValuePair("Storage", JValue.Build(storage)));
		File.WriteAllText(path, JSONPrettyPrint.Print(root.Encode()));     
    }

	// =================================================================================
    // Import
    // ---------------------------------------------------------------------------------
    public static bool Import(iCS_VisualScriptData storage, string path) {
        // Open JSON file.
        string jsonText= File.ReadAllText(path);
        if(string.IsNullOrEmpty(jsonText)) {
            Debug.LogWarning("iCanScript: No text to import from => "+path);
            return false;
        }
        // Decode JSON string.
        JObject root= JSON.GetRootObject(jsonText);
        if(root.isNull) {
            Debug.LogWarning("iCanScript: Import failure: JSON file corrupted.");
            return false;
        }
        JArray engineObjects= root.GetValueFor("Storage.EngineObjects") as JArray;
        if(engineObjects == null) {
            Debug.LogWarning("iCanScript: Import failure: Unable to locate engine array.");
            return false;
        }
        JArray unityObjects= root.GetValueFor("Storage.UnityObjects") as JArray;
        if(unityObjects == null) {
            Debug.LogWarning("iCanScript: Import failure: Unable to locate Unity Object array.");
            return false;
        }
        // Initialize storage using JSON data.
        var cache= new iCS_VisualScriptData();
        cache.EngineObjects.Capacity= engineObjects.value.Length;
        foreach(var eobj in engineObjects.value) {
            var newObj= ReadEngineObject(eobj as JObject);
            if(newObj != null) {
                cache.EngineObjects.Add(newObj);
            }
            else {
                Debug.LogWarning("iCanScript: Unable to create instance of iCS_EngineObject");                
            }
        }
        cache.UnityObjects.Clear();
        cache.UnityObjects.Capacity= unityObjects.value.Length;
        foreach(var uobj in unityObjects.value) {
            cache.UnityObjects.Add(ReadUnityObject(uobj as JObject));
        }
        cache.CopyTo(storage);
        return true;
    }
    
    // ---------------------------------------------------------------------------------
    static iCS_EngineObject ReadEngineObject(JObject jobj) {
        return ReadObject(jobj, typeof(iCS_EngineObject)) as iCS_EngineObject;
    }
    // ---------------------------------------------------------------------------------
    static System.Object ReadObject(JObject jobj, Type type) {
        var newObj= iCS_Types.CreateInstance(type);
        if(newObj == null) return null;
		foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            bool shouldDecode= true;
            if(field.IsPublic) {
                foreach(var attribute in field.GetCustomAttributes(true)) {
                    if(attribute is System.NonSerializedAttribute) shouldDecode= false;
                }
            } else {
                shouldDecode= false;
                foreach(var attribute in field.GetCustomAttributes(true)) {
                    if(attribute is SerializeField) shouldDecode= true;
                }                
            }
            if(shouldDecode) {
                var jvalue= jobj.GetValueFor(field.Name);
                System.Object value= null;
                if(jvalue.isNull)   {}
                if(jvalue.isBool)   { value= (jvalue as JBool).value; }
                if(jvalue.isNumber) { value= (jvalue as JNumber).value; }
                if(jvalue.isString) { value= (jvalue as JString).value; }
                if(jvalue.isArray)  { Debug.Log("array is unsupported."); }
                if(jvalue.isObject) {
                    value= ReadObject(jvalue as JObject, field.FieldType);
                }
                // Convert the type as appropriate.
                var fieldType= field.FieldType;
                value= Convert.ChangeType(value, fieldType);
                field.SetValue(newObj, value);
            }
		}
        return newObj;
    }
    // ---------------------------------------------------------------------------------
    static UnityEngine.Object ReadUnityObject(JObject jobj) {
//        JNumber jinstanceId= jobj.GetValueFor("InstanceId") as JNumber;
        JString jname      = jobj.GetValueFor("Name") as JString;
        JString jtypeName  = jobj.GetValueFor("Type") as JString;
        if(/*jinstanceId == null ||*/ jname == null || jtypeName == null) return null;
//        int    instanceId= (int)jinstanceId.value;
        string name      = jname.value;
        string typeName  = jtypeName.value;
        var unityAssembly= typeof(GameObject).Assembly;
        var type= unityAssembly.GetType(typeName);
        // Attempt to read an asset.
        JString assetGUID= jobj.GetValueFor("AssetGUID") as JString;
        if(assetGUID != null) {
            var path= AssetDatabase.GUIDToAssetPath(assetGUID.value);
            if(!string.IsNullOrEmpty(path)) {
                Type assetType= type;
                if(assetType != null) {
                    var result= AssetDatabase.LoadAssetAtPath(path, assetType);
                    if(result != null) {
                        return result;
                    }
                }
            }
            Debug.LogWarning("iCanScript: Import failure: Unable to reconnect asset => "+name+" of type =>"+typeName+" with GUID => "+assetGUID.value);
            return null;
        }
        // Attempt to read GameObject.
        JString jsceneGUID= jobj.GetValueFor("SceneGUID") as JString;
        if(jsceneGUID == null) {
            Debug.LogWarning("iCanScript: Import failure: Expected a Scene GUID for => "+name+" of type => "+typeName);
            return null;
        }
//        string sceneGUID= jsceneGUID.value;
        JArray parents= jobj.GetValueFor("Parents") as JArray;
        if(parents == null) {
            Debug.LogWarning("iCanScript: Import failure: Expected parent list for => "+name+" of type =>"+typeName);
        }
        // Build GameObject path.
        var gameObjectPath= "";
        foreach(var p in parents.value) {
            var pAsJString= p as JString; 
            if(pAsJString != null) {
                gameObjectPath+= "/"+pAsJString.value;
            }
        }
        if(type == typeof(GameObject)) {
            gameObjectPath+= "/"+name;
            var go= GameObject.Find(gameObjectPath);
            if(go != null) {
                return go;
            }
            Debug.LogWarning("iCanScript: Import failure: Unable to reconnect with GameObject => "+name);
        }
        var parentGameObject= GameObject.Find(gameObjectPath);
        if(parentGameObject != null) {
            var component= parentGameObject.GetComponent(type.Name);
            if(component != null) {
                return component;
            }
        }
        Debug.LogWarning("iCanScript: Import failure: Unable to reconnect Component => "+name+" of type => "+typeName);
        return null;
    }
}
