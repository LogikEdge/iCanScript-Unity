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
        // Nothing to display if we don't have a runtime object and we are in readonly.
        if(isReadOnly && runtimeObject == null) return;
        // Update port value from runtime object in priority or the descriptor string if no runtime.
        object portValue= runtimeObject != null ? runtimeObject[portId] :
                                                  (isReadOnly ? null : storage.GetDefaultValue(desc, portId));            
        // Determine section name (used for foldout parent).
        string foldoutName= port.IsInputPort ? "in" : "out";
        // Display primitives.
        object newPortValue= ShowInInspector(port.Name, isReadOnly, hasSource, foldoutName, elementType, portType, portValue, indentLevel, foldoutDB);
        if(!isReadOnly) {
            if(runtimeObject != null) runtimeObject[portId]= newPortValue;
//            if(portValue != newPortValue) {
                storage.SetDefaultValue(desc, portId, newPortValue);
                parent.RuntimeArchive= desc.Encode(desc.Id);
                storage.SetDirty(parent);
//            }
        }
    }

    // -----------------------------------------------------------------------
    public static object ShowInInspector(string name, bool isReadOnly, bool hasSource, string compositeParent,
                                         Type elementType, Type objType, object currentValue,
                                         int indentLevel, Dictionary<string,bool> foldoutDB) {
        EditorGUI.indentLevel= indentLevel;
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        if(objType.IsArray) niceName= "["+niceName+"]";
        // Special case for readonly & null value.
        if(isReadOnly && currentValue == null) {
            EditorGUILayout.LabelField(niceName, hasSource ? "(see connection)":"(not available)");
            return currentValue;
        }
        // Support all UnityEngine objects.
        if(!objType.IsArray && iCS_Types.IsA<UnityEngine.Object>(elementType)) {
            UnityEngine.Object value= currentValue != null ? currentValue as UnityEngine.Object: null;
            return EditorGUILayout.ObjectField(niceName, value, elementType, true);
        }        
        // Determine if we should create a value if the current value is null.
        if(currentValue == null) {
            // Automatically create value types.
            if(elementType.IsValueType || elementType.IsEnum) {
                currentValue= iCS_Types.CreateInstance(elementType);
            } else { // Ask to create reference types.
                Type[] derivedTypes= iCS_Reflection.GetAllTypesWithDefaultConstructorThatDeriveFrom(elementType);
                if(derivedTypes.Length <= 1) {
                    return iCS_Types.CreateInstance(objType);
                }
                string[] typeNames= new string[derivedTypes.Length+1];
                typeNames[0]= "None";
                if(objType.IsArray) {
                    for(int i= 0; i < derivedTypes.Length; ++i) typeNames[i+1]= derivedTypes[i].Name+"[]";                                        
                } else {
                    for(int i= 0; i < derivedTypes.Length; ++i) typeNames[i+1]= derivedTypes[i].Name;                    
                }
                int idx= EditorGUILayout.Popup(niceName, 0, typeNames);
                if(idx == 0) return null;
                if(objType.IsArray) {
                    return Array.CreateInstance(derivedTypes[idx-1],0);
                }
                return iCS_Types.CreateInstance(derivedTypes[idx-1]);
            }
        }
        // Automatically create value types the current value is not read only.
        if(currentValue == null && (elementType.IsValueType || elementType.IsEnum)) {
            currentValue= iCS_Types.CreateInstance(elementType);
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
            if(currentValue == null) { return currentValue; }
            return EditorGUILayout.EnumPopup(niceName, currentValue as System.Enum);
        }
        // C# data types.
        if(elementType == typeof(byte)) {
            return (byte)((int)EditorGUILayout.IntField(niceName, (int)((byte)currentValue)));
        }
        if(elementType == typeof(sbyte)) {
            return (sbyte)((int)EditorGUILayout.IntField(niceName, (int)((sbyte)currentValue)));            
        }
        if(elementType == typeof(bool)) {
            return EditorGUILayout.Toggle(niceName, (bool)currentValue);
        }
        if(elementType == typeof(int)) {
            return EditorGUILayout.IntField(niceName, (int)currentValue);
        }
        if(elementType == typeof(uint)) {
            string uintAsString= (string)Convert.ChangeType((uint)currentValue, typeof(string));
            string newUIntAsString= EditorGUILayout.TextField(niceName, uintAsString);
            return Convert.ChangeType(newUIntAsString, typeof(uint));
        }
        if(elementType == typeof(short)) {
            return (short)((int)EditorGUILayout.IntField(niceName, (int)((short)currentValue)));            
        }
        if(elementType == typeof(ushort)) {
            return (ushort)((int)EditorGUILayout.IntField(niceName, (int)((ushort)currentValue)));            
        }
        if(elementType == typeof(long)) {
            string longAsString= (string)Convert.ChangeType((long)currentValue, typeof(string));
            string newLongAsString= EditorGUILayout.TextField(niceName, longAsString);
            return Convert.ChangeType(newLongAsString, typeof(long));
        }
        if(elementType == typeof(ulong)) {
            string ulongAsString= (string)Convert.ChangeType((ulong)currentValue, typeof(string));
            string newULongAsString= EditorGUILayout.TextField(niceName, ulongAsString);
            return Convert.ChangeType(newULongAsString, typeof(ulong));
        }
        if(elementType == typeof(float)) {
            return EditorGUILayout.FloatField(niceName, (float)currentValue);
        }
        if(elementType == typeof(double)) {
            string doubleAsString= (string)Convert.ChangeType((double)currentValue, typeof(string));
            string newDoubleAsString= EditorGUILayout.TextField(niceName, doubleAsString);
            return Convert.ChangeType(newDoubleAsString, typeof(double));
        }
        if(elementType == typeof(decimal)) {
            float valueAsFloat= (float)((decimal)currentValue);
            float newDecimalAsFloat= EditorGUILayout.FloatField(niceName, valueAsFloat);
            return (decimal)newDecimalAsFloat;
        }
        if(elementType == typeof(char)) {
            string value= ""+((char)currentValue);
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
        // Unity data types.
        if(elementType == typeof(Vector2)) {
            return EditorGUILayout.Vector2Field(niceName, (Vector2)currentValue);
        }
        if(elementType == typeof(Vector3)) {
            return EditorGUILayout.Vector3Field(niceName, (Vector3)currentValue);
        }
        if(elementType == typeof(Vector4)) {
            return EditorGUILayout.Vector4Field(niceName, (Vector4)currentValue);
        }
        if(elementType == typeof(Color)) {
            return EditorGUILayout.ColorField(niceName, (Color)currentValue);
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
                    object currentFieldValue= field.GetValue(currentValue);
                    object newFieldValue= ShowInInspector(field.Name, isReadOnly, hasSource, compositeName, iCS_Types.GetElementType(field.FieldType), field.FieldType, currentFieldValue, indentLevel+1, foldoutDB);
                    if(!isReadOnly && newFieldValue.ToString() != currentFieldValue.ToString()) {
                        field.SetValue(currentValue, newFieldValue);
                        Debug.Log("Setting field to "+newFieldValue.ToString()+" from: "+currentFieldValue.ToString());
                        Debug.Log("New field value is: "+field.GetValue(currentValue).ToString());
                    }
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
