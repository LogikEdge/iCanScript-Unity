using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class iCS_GuiUtilities {
	public static Type[]	SupportedTypes= null;
	
	static iCS_GuiUtilities() {
		SupportedTypes= new Type[]{
	        typeof(bool),
	        typeof(int),
	        typeof(float),
	        typeof(string),
	        typeof(Vector2),
	        typeof(Vector3),
	        typeof(Vector4),
	        typeof(Color),
	        typeof(UnityEngine.Object)			
		};
	}

    public static void OnInspectorGUI(iCS_EditorObject port, iCS_IStorage storage, int indentLevel= 0) {
        // Extract port information
		Type portType= port.RuntimeType;
        Type dataType= iCS_Types.GetElementType(portType);
        iCS_EditorObject node= storage.GetParent(port);
        int portId= port.PortIndex;
        // Extract parent node information.
        iCS_RuntimeDesc desc= new iCS_RuntimeDesc(node.RuntimeArchive);    
        // Get runtime object if it exists.
        iCS_FunctionBase runtimeObject= storage.GetRuntimeObject(node) as iCS_FunctionBase;
        // Update port value from runtime object in priority or the descriptor string if no runtime.
        object portValue= runtimeObject != null ? runtimeObject[portId] : (desc.PortIsOuts[portId] ? iCS_Types.DefaultValue(dataType) : storage.GetDefaultValue(desc, portId));            

        // Display primitives.
        object newValue= null;
        if(ShowInInspector(port.Name, dataType, portType, portValue, out newValue, indentLevel)) {
            if(port.IsInputPort && runtimeObject != null) runtimeObject[portId]= newValue;
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
    public static bool ShowInInspector(string name, Type dataType, Type portType, object currentValue, out object newValue, int indentLevel) {
        EditorGUI.indentLevel= indentLevel;
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        // C# data types.
        if(dataType.IsEnum) {
            if(currentValue == null) { currentValue= iCS_Types.DefaultValue(dataType); }
            if(currentValue == null) { newValue= currentValue; return true; }
            newValue= EditorGUILayout.EnumPopup(niceName, currentValue as System.Enum);
            return true;
        }
        if(dataType == typeof(byte)) {
            int value= currentValue != null ? (int)((byte)currentValue) : (int)default(byte);
            newValue= (byte)((int)EditorGUILayout.IntField(niceName, value));
            return true;
        }
        if(dataType == typeof(sbyte)) {
            int value= currentValue != null ? (int)((sbyte)currentValue) : (int)default(sbyte);
            newValue= (sbyte)((int)EditorGUILayout.IntField(niceName, value));            
            return true;
        }
        if(dataType == typeof(bool)) {
            bool value= currentValue != null ? (bool)currentValue : default(bool);
            newValue= EditorGUILayout.Toggle(niceName, value);
            return true;
        }
        if(dataType == typeof(int)) {
            int value= currentValue != null ? (int)currentValue : default(int);
            newValue= EditorGUILayout.IntField(niceName, value);
            return true;
        }
        if(dataType == typeof(uint)) {
            
        }
        if(dataType == typeof(short)) {
            int value= currentValue != null ? (int)((short)currentValue) : (int)default(short);
            newValue= (short)((int)EditorGUILayout.IntField(niceName, value));            
            return true;            
        }
        if(dataType == typeof(ushort)) {
            int value= currentValue != null ? (int)((ushort)currentValue) : (int)default(ushort);
            newValue= (ushort)((int)EditorGUILayout.IntField(niceName, value));            
            return true;            
        }
        if(dataType == typeof(long)) {
            
        }
        if(dataType == typeof(ulong)) {
            
        }
        if(dataType == typeof(float)) {
            float value= currentValue != null ? (float)currentValue : default(float);
            newValue= EditorGUILayout.FloatField(niceName, value);
            return true;
        }
        if(dataType == typeof(double)) {
            
        }
        if(dataType == typeof(decimal)) {
            
        }
        if(dataType == typeof(char)) {
            
        }
        if(dataType == typeof(string)) {
            string value= ((string)currentValue) ?? "";
            newValue= EditorGUILayout.TextField(niceName, value);
            return true;
        }
        if(dataType == typeof(Type)) {
            
        }
        if(portType.IsArray && dataType == typeof(Char)) {
            string value= currentValue != null ? new string(currentValue as char[]) : "";
			Debug.Log("Before: "+value);
            value= EditorGUILayout.TextField(niceName, value);
			newValue= value.ToCharArray(); 
			Debug.Log("After: "+new string(newValue as Char[]));
            return true;				
        }
        // Unity data types.
        if(dataType == typeof(Vector2)) {
            Vector2 value= currentValue != null ? (Vector2)currentValue : default(Vector2);
            newValue= EditorGUILayout.Vector2Field(niceName, value);
            return true;
        }
        if(dataType == typeof(Vector3)) {
            Vector3 value= currentValue != null ? (Vector3)currentValue : default(Vector3);
            newValue= EditorGUILayout.Vector3Field(niceName, value);
            return true;
        }
        if(dataType == typeof(Vector4)) {
            Vector4 value= currentValue != null ? (Vector4)currentValue : default(Vector4);
            newValue= EditorGUILayout.Vector4Field(niceName, value);
            return true;
        }
        if(dataType == typeof(Color)) {
            Color value= currentValue != null ? (Color)currentValue : default(Color);
            newValue= EditorGUILayout.ColorField(niceName, value);
            return true;
        }
        // Suport all UnityEngine objects.
        if(iCS_Types.IsA<UnityEngine.Object>(dataType)) {
            UnityEngine.Object value= currentValue != null ? currentValue as UnityEngine.Object: null;
            newValue= EditorGUILayout.ObjectField(niceName, value, dataType, true);
            return true;
        }        
		// All other types.
        if(EditorGUILayout.Foldout(true, niceName)) {
    		foreach(var field in dataType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
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
                    if(currentValue == null) currentValue= iCS_Types.CreateInstance(dataType);
                    ShowInInspector(field.Name, iCS_Types.GetElementType(field.FieldType), field.FieldType, field.GetValue(currentValue), out newFieldValue, indentLevel+1);
                }
    		}        
        }
        newValue= null;
        return true;

//
//		if(dataType == typeof(object)) {
//			bool changed= false;
//			object changedValue= null;
//			EditorGUILayout.BeginHorizontal();
//			if(currentValue != null) {
//				// use current object type.
//				Type valueType= currentValue.GetType();
//				Type dataValueType= iCS_Types.GetElementType(valueType);
//				changed= ShowInInspector(name, dataValueType, valueType, currentValue, out changedValue);
//			} else {
//				EditorGUILayout.TextField(name);
//			}
//			// Select a type.
//			string[] derivedTypeNames= GetListOfDerivedTypeNames(dataType);
//			int idx= currentValue != null ? GetIndexOfType(currentValue.GetType(), derivedTypeNames) : 0;
//			int selection= EditorGUILayout.Popup(idx, derivedTypeNames);
//			if(selection != idx) {
//				// TODO...
//			}
//			EditorGUILayout.EndHorizontal();
//			newValue= changedValue;
//			return changed;
//		}
//        newValue= null;
//        return false;
    }

    // -----------------------------------------------------------------------
	static bool IsSupportedType(Type type) {
        if(type == typeof(bool)) return true;
        if(type == typeof(int)) return true;
        if(type == typeof(float)) return true;
        if(type == typeof(string)) return true;
        if(type == typeof(Vector2)) return true;
        if(type == typeof(Vector3)) return true;
        if(type == typeof(Vector4)) return true;
        if(type == typeof(Color)) return true;
        // Suport all UnityEngine objects.
        if(iCS_Types.IsA<UnityEngine.Object>(type)) return true;
		return false;
	}
    // -----------------------------------------------------------------------
	static string[] GetListOfDerivedTypeNames(Type baseType) {
		return new string[]{"string","int","float"};
	}
    // -----------------------------------------------------------------------
	static int GetIndexOfType(Type type, string[] allTypes) {
		return 0;
	}
    // -----------------------------------------------------------------------
    public static void UnsupportedFeature() {
        Debug.LogWarning("The selected feature is unsupported in the current version of iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
