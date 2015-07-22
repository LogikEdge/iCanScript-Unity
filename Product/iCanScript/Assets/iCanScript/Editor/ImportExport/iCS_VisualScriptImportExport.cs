using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.IO;
using iCanScript;
using iCanScript.Internal.JSON;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
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
            // -- Decode JSON string. --
            JObject root= JSONFile.Read(path);
            if(root == null || root.isNull) {
                Debug.LogWarning("iCanScript: Import failure: JSON file corrupted.");
                return false;
            }
            // -- Extract version information. --
            var cache= new iCS_VisualScriptData();
            JNumber majorVersion = root.GetValueFor("Storage.MajorVersion") as JNumber;
            JNumber minorVersion = root.GetValueFor("Storage.MinorVersion") as JNumber;
            JNumber bugFixVersion= root.GetValueFor("Storage.BugFixVersion") as JNumber;
            cache.MajorVersion = (int)majorVersion.value;
            cache.MinorVersion = (int)minorVersion.value;
            cache.BugFixVersion= (int)bugFixVersion.value;
            // -- Extract visual script attributes. --
            JBool isEditorScript     = root.GetValueFor("Storage.IsEditorScript") as JBool;
            JString csharpFileName   = root.GetValueFor("Storage.CSharpFileName") as JString;
            JBool baseTypeOverride   = root.GetValueFor("Storage.BaseTypeOverride") as JBool;
            JString baseType         = root.GetValueFor("Storage.BaseType") as JString;
            JString sourceFileGUID   = root.GetValueFor("Storage.SourceFileGUID") as JString;
            cache.IsEditorScript   = isEditorScript.value;
            cache.CSharpFileName   = csharpFileName.value;
            cache.BaseTypeOverride = baseTypeOverride.value;
            cache.BaseType         = baseType.value;
            cache.SourceFileGUID   = sourceFileGUID.value;
            // -- Extract engine objects. --
            JArray engineObjects= root.GetValueFor("Storage.EngineObjects") as JArray;
            if(engineObjects == null) {
                Debug.LogWarning("iCanScript: Import failure: Unable to locate engine array.");
                return false;
            }
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
                    if(value != null) {
                        var fieldType= field.FieldType;
                        value= Convert.ChangeType(value, fieldType);
                        field.SetValue(newObj, value);
                    }
                    else {
                        field.SetValue(newObj, null);
                    }
                }
    		}
            return newObj;
        }
        // ---------------------------------------------------------------------------------
        static UnityEngine.Object ReadUnityObject(JObject jobj) {
            JString jname      = jobj.GetValueFor("Name") as JString;
            JString jtypeName  = jobj.GetValueFor("Type") as JString;
            if(jname == null || jtypeName == null) return null;
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

}
