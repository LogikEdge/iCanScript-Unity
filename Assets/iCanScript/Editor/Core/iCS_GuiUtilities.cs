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
        bool isDirty= false;
        object newPortValue= ShowInInspector(port.Name, isReadOnly, hasSource, foldoutName, elementType, portType, portValue, indentLevel, foldoutDB, ref isDirty);
        if(!isReadOnly) {
//            if(!isEqual(portValue, newPortValue)) {
                if(runtimeObject != null) runtimeObject[portId]= newPortValue;
                storage.SetDefaultValue(desc, portId, newPortValue);
                parent.RuntimeArchive= desc.Encode(desc.Id);
                storage.SetDirty(parent);
//            }
        }
    }

    // -----------------------------------------------------------------------
    public static object ShowInInspector(string name, bool isReadOnly, bool hasSource, string compositeParent,
                                         Type elementType, Type objType, object currentValue,
                                         int indentLevel, Dictionary<string,bool> foldoutDB, ref bool isDirty) {
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
                    object newElementValue= ShowInInspector("["+i+"]", isReadOnly, hasSource, compositeArrayName, iCS_Types.GetElementType(elementType), elementType, array.GetValue(i), indentLevel+1, foldoutDB, ref isDirty);
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
                    object newFieldValue= ShowInInspector(field.Name, isReadOnly, hasSource, compositeName, iCS_Types.GetElementType(field.FieldType), field.FieldType, currentFieldValue, indentLevel+1, foldoutDB, ref isDirty);
                    if(!isReadOnly && !isEqual(currentFieldValue, newFieldValue)) {
//                        Debug.Log("Is different");
                        field.SetValue(currentValue, newFieldValue);
                    }
                }
    		}        
        }
        return currentValue;
    }

	// ----------------------------------------------------------------------
    public static bool isEqual(object o1, object o2) {
        if(o1 == o2) return true;
        if(o1 == null && o2 == null) return true;
        if(o1 == null || o2 == null) return false;
        Type t1= o1.GetType();
        Type t2= o2.GetType();
        if(t1.IsArray && !t2.IsArray) return false;
        if(!t1.IsArray && t2.IsArray) return false;
        if(t1.IsArray) {
            Array a1= o1 as Array;
            Array a2= o2 as Array;
            if(a1.Length != a2.Length) return false;
            for(int i= 0; i < a1.Length; ++i) {
                if(!isEqual(a1.GetValue(i), a2.GetValue(i))) return false;
            }
            return true;
        }
        // Don't consider reference decoration.
        t1= t1.HasElementType ? t1.GetElementType() : t1;
        t2= t2.HasElementType ? t2.GetElementType() : t2;
        if(t1 != t2) return false;
        // C# primitives
        if(o1 is byte) return ((byte)o1) == ((byte)o2);
        if(o1 is sbyte) return ((sbyte)o1) == ((sbyte)o2);
        if(o1 is int) return ((int)o1) == ((int)o2);
        if(o1 is uint) return ((uint)o1) == ((uint)o2);
        if(o1 is short) return ((short)o1) == ((short)o2);
        if(o1 is ushort) return ((ushort)o1) == ((ushort)o2);
        if(o1 is long) return ((long)o1) == ((long)o2);
        if(o1 is ulong) return ((ulong)o1) == ((ulong)o2);
        if(o1 is float) return ((float)o1) == ((float)o2);
        if(o1 is double) return ((double)o1) == ((double)o2);
        if(o1 is decimal) return ((decimal)o1) == ((decimal)o2);
        if(o1 is char) return ((char)o1) == ((char)o2);
        if(o1 is string) return ((string)o1).CompareTo((string)o2) == 0;
        if(o1 is Type) return ((Type)o1).AssemblyQualifiedName.CompareTo(((Type)o2).AssemblyQualifiedName) == 0;
        // Composite objects
		foreach(var field in t1.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
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
                if(!isEqual(field.GetValue(o1), field.GetValue(o2))) return false;
            }
		}        
        return true;
    }

    // -----------------------------------------------------------------------
    public static void UnsupportedFeature() {
        Debug.LogWarning("The selected feature is unsupported in the current version of iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
