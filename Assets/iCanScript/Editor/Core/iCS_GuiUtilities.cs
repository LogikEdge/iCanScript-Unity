using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class iCS_GuiUtilities {
    // -----------------------------------------------------------------------
    public static void OnInspectorGUI(iCS_EditorObject eObj, iCS_IStorage storage, int indentLevel, Dictionary<string,bool> foldoutDB) {
        // Display specific node type information.
        if(eObj.IsNode) {
            if(eObj.IsState) {
                
            } else {
                
            }
        } else if(eObj.IsDataPort){
            
        }
    }

    // -----------------------------------------------------------------------
    public static void OnInspectorGUI(string parentName, bool isReadOnly, iCS_EditorObject port, iCS_IStorage storage, int indentLevel, Dictionary<string,bool> foldoutDB) {
        // Extract port information
		Type portType= port.RuntimeType;
        Type elementType= iCS_Types.GetElementType(portType);
        iCS_EditorObject node= storage.GetParent(port);
        int portId= port.PortIndex;
        // Extract parent node information.
        iCS_RuntimeDesc desc= new iCS_RuntimeDesc(node.RuntimeArchive);    
        // Get runtime object if it exists.
        iCS_FunctionBase runtimeObject= storage.GetRuntimeObject(node) as iCS_FunctionBase;
        // Update port value from runtime object in priority or the descriptor string if no runtime.
        object portValue= runtimeObject != null ? runtimeObject[portId] : (desc.PortIsOuts[portId] ? iCS_Types.DefaultValue(elementType) : storage.GetDefaultValue(desc, portId));            

        // Display primitives.
        object newValue= null;
        if(ShowInInspector(port.Name, isReadOnly, parentName, elementType, portType, portValue, out newValue, indentLevel, foldoutDB)) {
            if(!isReadOnly && runtimeObject != null) runtimeObject[portId]= newValue;
            if(portValue != newValue && storage.GetSource(port) == null) {
                storage.SetDefaultValue(desc, portId, newValue);
                node.RuntimeArchive= desc.Encode(desc.Id);
                storage.SetDirty(node);
            }
            return;            
        }

        Debug.LogWarning("OnInspectorGUI: Unknown type: "+port.RuntimeType);
    }

    // -----------------------------------------------------------------------
    public static bool ShowInInspector(string name, bool isReadOnly, string compositeParent,
                                       Type elementType, Type portType,
                                       object currentValue, out object newValue,
                                       int indentLevel, Dictionary<string,bool> foldoutDB) {
        EditorGUI.indentLevel= indentLevel;
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        // Special case for arrays
        if(portType.IsArray) {
            string compositeArrayName= compositeParent+"."+name;
            if(!foldoutDB.ContainsKey(compositeArrayName)) foldoutDB.Add(compositeArrayName, false);
            bool showArray= foldoutDB[compositeArrayName];
            showArray= EditorGUILayout.Foldout(showArray, niceName);
            foldoutDB[compositeArrayName]= showArray;
            if(showArray) {
                EditorGUI.indentLevel= indentLevel+1;
                if(currentValue == null) currentValue= Array.CreateInstance(elementType, 1);
                Array array= currentValue as Array;
                int newSize= (int)EditorGUILayout.IntField("Size", array.Length);            
                for(int i= 0; i < array.Length; ++i) {
                    object newElementValue= null;
                    ShowInInspector("["+i+"]", isReadOnly, compositeArrayName, iCS_Types.GetElementType(elementType), elementType, array.GetValue(i), out newElementValue, indentLevel+1, foldoutDB);
                }
            }
            newValue= currentValue;
            return true;            
        }
        // Special case for enumerations
        if(elementType.IsEnum) {
            if(currentValue == null) { currentValue= iCS_Types.DefaultValue(elementType); }
            if(currentValue == null) { newValue= currentValue; return true; }
            newValue= EditorGUILayout.EnumPopup(niceName, currentValue as System.Enum);
            return true;
        }
        // C# data types.
        if(elementType == typeof(byte)) {
            int value= currentValue != null ? (int)((byte)currentValue) : (int)default(byte);
            newValue= (byte)((int)EditorGUILayout.IntField(niceName, value));
            return true;
        }
        if(elementType == typeof(sbyte)) {
            int value= currentValue != null ? (int)((sbyte)currentValue) : (int)default(sbyte);
            newValue= (sbyte)((int)EditorGUILayout.IntField(niceName, value));            
            return true;
        }
        if(elementType == typeof(bool)) {
            bool value= currentValue != null ? (bool)currentValue : default(bool);
            newValue= EditorGUILayout.Toggle(niceName, value);
            return true;
        }
        if(elementType == typeof(int)) {
            int value= currentValue != null ? (int)currentValue : default(int);
            newValue= EditorGUILayout.IntField(niceName, value);
            return true;
        }
        if(elementType == typeof(uint)) {
            
        }
        if(elementType == typeof(short)) {
            int value= currentValue != null ? (int)((short)currentValue) : (int)default(short);
            newValue= (short)((int)EditorGUILayout.IntField(niceName, value));            
            return true;            
        }
        if(elementType == typeof(ushort)) {
            int value= currentValue != null ? (int)((ushort)currentValue) : (int)default(ushort);
            newValue= (ushort)((int)EditorGUILayout.IntField(niceName, value));            
            return true;            
        }
        if(elementType == typeof(long)) {
            
        }
        if(elementType == typeof(ulong)) {
            
        }
        if(elementType == typeof(float)) {
            float value= currentValue != null ? (float)currentValue : default(float);
            newValue= EditorGUILayout.FloatField(niceName, value);
            return true;
        }
        if(elementType == typeof(double)) {
            
        }
        if(elementType == typeof(decimal)) {
            
        }
        if(elementType == typeof(char)) {
            
        }
        if(elementType == typeof(string)) {
            string value= ((string)currentValue) ?? "";
            newValue= EditorGUILayout.TextField(niceName, value);
            return true;
        }
        if(elementType == typeof(Type)) {
            
        }
        if(portType.IsArray && elementType == typeof(Char)) {
            string value= currentValue != null ? new string(currentValue as char[]) : "";
			Debug.Log("Before: "+value);
            value= EditorGUILayout.TextField(niceName, value);
			newValue= value.ToCharArray(); 
			Debug.Log("After: "+new string(newValue as Char[]));
            return true;				
        }
        // Unity data types.
        if(elementType == typeof(Vector2)) {
            Vector2 value= currentValue != null ? (Vector2)currentValue : default(Vector2);
            newValue= EditorGUILayout.Vector2Field(niceName, value);
            return true;
        }
        if(elementType == typeof(Vector3)) {
            Vector3 value= currentValue != null ? (Vector3)currentValue : default(Vector3);
            newValue= EditorGUILayout.Vector3Field(niceName, value);
            return true;
        }
        if(elementType == typeof(Vector4)) {
            Vector4 value= currentValue != null ? (Vector4)currentValue : default(Vector4);
            newValue= EditorGUILayout.Vector4Field(niceName, value);
            return true;
        }
        if(elementType == typeof(Color)) {
            Color value= currentValue != null ? (Color)currentValue : default(Color);
            newValue= EditorGUILayout.ColorField(niceName, value);
            return true;
        }
        // Suport all UnityEngine objects.
        if(iCS_Types.IsA<UnityEngine.Object>(elementType)) {
            UnityEngine.Object value= currentValue != null ? currentValue as UnityEngine.Object: null;
            newValue= EditorGUILayout.ObjectField(niceName, value, elementType, true);
            return true;
        }        
		// All other types.
        string compositeName= compositeParent+"."+name;
        if(!foldoutDB.ContainsKey(compositeName)) foldoutDB.Add(compositeName, false);
        bool showCompositeObject= foldoutDB[compositeName];
        showCompositeObject= EditorGUILayout.Foldout(showCompositeObject, niceName);
        foldoutDB[compositeName]= showCompositeObject;
        if(showCompositeObject) {
    		foreach(var field in elementType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                bool shouldInspect= true;
                if(field.IsPublic) {
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is System.NonSerializedAttribute) { shouldInspect= false; break; }
                        if(attribute is HideInInspector) { shouldInspect= false; break; }
                    }
                } else {
                    shouldInspect= false;
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is SerializeField) shouldInspect= true;
                        if(attribute is HideInInspector) { shouldInspect= false; break; }
                    }                
                }
                if(shouldInspect) {
                    object newFieldValue= null;
                    if(currentValue == null) currentValue= iCS_Types.CreateInstance(elementType);
                    ShowInInspector(field.Name, isReadOnly, compositeName, iCS_Types.GetElementType(field.FieldType), field.FieldType, field.GetValue(currentValue), out newFieldValue, indentLevel+1, foldoutDB);
                }
    		}        
        }
        newValue= null;
        return true;
    }

    // -----------------------------------------------------------------------
    public static void UnsupportedFeature() {
        Debug.LogWarning("The selected feature is unsupported in the current version of iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
