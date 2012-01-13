using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class iCS_GuiUtilities {
    // -----------------------------------------------------------------------
    public static void OnInspectorDataPortGUI(iCS_EditorObject port, iCS_IStorage storage, int indentLevel, Dictionary<string,bool> foldoutDB) {
        // Only accept data ports.
        if(!port.IsDataPort) return;
        // Extract port information
		Type portType= port.RuntimeType;
        Type elementType= iCS_Types.GetElementType(portType);
        iCS_EditorObject parent= storage.GetParent(port);
        int portId= port.PortIndex;
        iCS_EditorObject sourcePort= storage.GetSource(port);
        bool hasSource= sourcePort != null;
        // Extract parent node information.
        iCS_RuntimeDesc desc= new iCS_RuntimeDesc(parent.RuntimeArchive);    
        // Get runtime object if it exists.
        iCS_FunctionBase runtimeObject= storage.GetRuntimeObject(parent) as iCS_FunctionBase;
        // Determine if we are allowed to modify port value.
        bool isReadOnly= !(!hasSource && (port.IsInputPort || port.IsModulePort));
        // Update port value from runtime object in priority or the descriptor string if no runtime.
        object portValue= runtimeObject != null ? runtimeObject[portId] :
                                                  (isReadOnly ? null : storage.GetDefaultValue(desc, portId));            
        // Determine section name (used for foldout parent).
        string foldoutName= port.IsInputPort ? "in" : "out";
        // Display primitives.
        object newPortValue= ShowInInspector(port.Name, isReadOnly, hasSource, foldoutName, elementType, portType, portValue, indentLevel, foldoutDB);
        if(!isReadOnly) {
            if(runtimeObject != null) runtimeObject[portId]= newPortValue;
            if(portValue != newPortValue) {
                storage.SetDefaultValue(desc, portId, newPortValue);
                parent.RuntimeArchive= desc.Encode(desc.Id);
                storage.SetDirty(parent);
            }
        }
    }

    // -----------------------------------------------------------------------
    public static object ShowInInspector(string name, bool isReadOnly, bool hasSource, string compositeParent,
                                         Type elementType, Type objType, object currentValue,
                                         int indentLevel, Dictionary<string,bool> foldoutDB) {
        EditorGUI.indentLevel= indentLevel;
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        // Special case for readonly & null value.
        if(isReadOnly && currentValue == null) {
            EditorGUILayout.LabelField(niceName, hasSource ? "(see connection)":"(not available)");
            return currentValue;
        }
        // Special case for arrays
        if(objType.IsArray) {
            string compositeArrayName= compositeParent+"."+name;
            if(!foldoutDB.ContainsKey(compositeArrayName)) foldoutDB.Add(compositeArrayName, false);
            bool showArray= foldoutDB[compositeArrayName];
            showArray= EditorGUILayout.Foldout(showArray, niceName);
            foldoutDB[compositeArrayName]= showArray;
            if(showArray) {
                EditorGUI.indentLevel= indentLevel+1;
                if(currentValue == null) currentValue= Array.CreateInstance(elementType, 0);
                Array array= currentValue as Array;
                int newSize= (int)EditorGUILayout.IntField("Size", array.Length);            
                for(int i= 0; i < array.Length; ++i) {
                    object newElementValue= ShowInInspector("["+i+"]", isReadOnly, hasSource, compositeArrayName, iCS_Types.GetElementType(elementType), elementType, array.GetValue(i), indentLevel+1, foldoutDB);
                }
            }
            return currentValue;
        }
        // Special case for enumerations
        if(elementType.IsEnum) {
            if(currentValue == null) { currentValue= iCS_Types.DefaultValue(elementType); }
            if(currentValue == null) { return currentValue; }
            return EditorGUILayout.EnumPopup(niceName, currentValue as System.Enum);
        }
        // C# data types.
        if(elementType == typeof(byte)) {
            int value= currentValue != null ? (int)((byte)currentValue) : (int)default(byte);
            return (byte)((int)EditorGUILayout.IntField(niceName, value));
        }
        if(elementType == typeof(sbyte)) {
            int value= currentValue != null ? (int)((sbyte)currentValue) : (int)default(sbyte);
            return (sbyte)((int)EditorGUILayout.IntField(niceName, value));            
        }
        if(elementType == typeof(bool)) {
            bool value= currentValue != null ? (bool)currentValue : default(bool);
            return EditorGUILayout.Toggle(niceName, value);
        }
        if(elementType == typeof(int)) {
            int value= currentValue != null ? (int)currentValue : default(int);
            return EditorGUILayout.IntField(niceName, value);
        }
        if(elementType == typeof(uint)) {
            uint value= currentValue != null ? (uint)currentValue : default(uint);
            string uintAsString= (string)Convert.ChangeType(value, typeof(string));
            string newUIntAsString= EditorGUILayout.TextField(niceName, uintAsString);
            return Convert.ChangeType(newUIntAsString, typeof(uint));
        }
        if(elementType == typeof(short)) {
            int value= currentValue != null ? (int)((short)currentValue) : (int)default(short);
            return (short)((int)EditorGUILayout.IntField(niceName, value));            
        }
        if(elementType == typeof(ushort)) {
            int value= currentValue != null ? (int)((ushort)currentValue) : (int)default(ushort);
            return (ushort)((int)EditorGUILayout.IntField(niceName, value));            
        }
        if(elementType == typeof(long)) {
            long value= currentValue != null ? (long)currentValue : default(long);
            string longAsString= (string)Convert.ChangeType(value, typeof(string));
            string newLongAsString= EditorGUILayout.TextField(niceName, longAsString);
            return Convert.ChangeType(newLongAsString, typeof(long));
        }
        if(elementType == typeof(ulong)) {
            ulong value= currentValue != null ? (ulong)currentValue : default(ulong);
            string ulongAsString= (string)Convert.ChangeType(value, typeof(string));
            string newULongAsString= EditorGUILayout.TextField(niceName, ulongAsString);
            return Convert.ChangeType(newULongAsString, typeof(ulong));
        }
        if(elementType == typeof(float)) {
            float value= currentValue != null ? (float)currentValue : default(float);
            return EditorGUILayout.FloatField(niceName, value);
        }
        if(elementType == typeof(double)) {
            double value= currentValue != null ? (double)currentValue : default(double);
            string doubleAsString= (string)Convert.ChangeType(value, typeof(string));
            string newDoubleAsString= EditorGUILayout.TextField(niceName, doubleAsString);
            return Convert.ChangeType(newDoubleAsString, typeof(double));
        }
        if(elementType == typeof(decimal)) {
            decimal value= currentValue != null ? (decimal)currentValue : default(decimal);
            float valueAsFloat= (float)value;
            float newDecimalAsFloat= EditorGUILayout.FloatField(niceName, valueAsFloat);
            return (decimal)newDecimalAsFloat;
        }
        if(elementType == typeof(char)) {
            string value= ""+(currentValue != null ? (char)currentValue : default(char));
            string newCharAsString= EditorGUILayout.TextField(niceName, value);
            return (newCharAsString != null && newCharAsString.Length >= 1) ? newCharAsString[0] : default(char);
        }
        if(elementType == typeof(string)) {
            string value= ((string)currentValue) ?? "";
            return EditorGUILayout.TextField(niceName, value);
        }
        if(elementType == typeof(Type)) {
            if(currentValue == null) return true;
            EditorGUILayout.LabelField(niceName, (currentValue as Type).Name);
            return currentValue;
        }
        if(objType.IsArray && elementType == typeof(Char)) {
            string value= currentValue != null ? new string(currentValue as char[]) : "";
            value= EditorGUILayout.TextField(niceName, value);
			return value.ToCharArray(); 
        }
        // Unity data types.
        if(elementType == typeof(Vector2)) {
            Vector2 value= currentValue != null ? (Vector2)currentValue : default(Vector2);
            return EditorGUILayout.Vector2Field(niceName, value);
        }
        if(elementType == typeof(Vector3)) {
            Vector3 value= currentValue != null ? (Vector3)currentValue : default(Vector3);
            return EditorGUILayout.Vector3Field(niceName, value);
        }
        if(elementType == typeof(Vector4)) {
            Vector4 value= currentValue != null ? (Vector4)currentValue : default(Vector4);
            return EditorGUILayout.Vector4Field(niceName, value);
        }
        if(elementType == typeof(Color)) {
            Color value= currentValue != null ? (Color)currentValue : default(Color);
            return EditorGUILayout.ColorField(niceName, value);
        }
        // Suport all UnityEngine objects.
        if(iCS_Types.IsA<UnityEngine.Object>(elementType)) {
            UnityEngine.Object value= currentValue != null ? currentValue as UnityEngine.Object: null;
            return EditorGUILayout.ObjectField(niceName, value, elementType, true);
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
                    if(currentValue == null) currentValue= iCS_Types.CreateInstance(elementType);
                    object newFieldValue= ShowInInspector(field.Name, isReadOnly, hasSource, compositeName, iCS_Types.GetElementType(field.FieldType), field.FieldType, field.GetValue(currentValue), indentLevel+1, foldoutDB);
                }
    		}        
        }
        return currentValue;
    }

    // -----------------------------------------------------------------------
    public static void UnsupportedFeature() {
        Debug.LogWarning("The selected feature is unsupported in the current version of iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
