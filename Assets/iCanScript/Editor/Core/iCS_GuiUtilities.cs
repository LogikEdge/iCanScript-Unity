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

    public static void OnInspectorGUI(iCS_EditorObject port, iCS_IStorage storage) {
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
        if(ShowInInspector(port.Name, dataType, portType, portValue, out newValue)) {
            if(port.IsInputPort && runtimeObject != null) runtimeObject[portId]= newValue;
            if(portValue != newValue && storage.GetSource(port) == null) {
                storage.SetDefaultValue(desc, portId, newValue);
                node.RuntimeArchive= desc.Encode(desc.Id);
                storage.SetDirty(node);
            }
            return;            
        }

//        // Display array of primitives.
//        if(fieldType.IsArray) {
//            System.Type elementType= fieldType.GetElementType();
//            if(elementType == typeof(bool)) {
//                bool[] values= (bool[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.Toggle(niceName+"["+i+"]", values[i]);
//                }
//                return;
//            }
//            if(elementType == typeof(int)) {
//                int[] values= (int[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.IntField(niceName+"["+i+"]", values[i]);
//                }
//                return;
//            }
//            if(elementType == typeof(float)) {
//                float[] values= (float[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.FloatField(niceName+"["+i+"]", values[i]);
//                }
//                return;            
//            }
//            if(elementType == typeof(string)) {
//                string[] values= (string[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.TextField(niceName+"["+i+"]", values[i]);
//                }
//                return;
//            }
//            if(elementType == typeof(Vector2)) {
//                Vector2[] values= (Vector2[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.Vector2Field(niceName+"["+i+"]", values[i]);
//                }
//                return;                        
//            }            
//            if(elementType == typeof(Vector3)) {
//                Vector3[] values= (Vector3[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.Vector3Field(niceName+"["+i+"]", values[i]);
//                }
//                return;                        
//            }            
//            if(elementType == typeof(Vector4)) {
//                Vector4[] values= (Vector4[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.Vector4Field(niceName+"["+i+"]", values[i]);
//                }
//                return;                        
//            }            
//            if(elementType == typeof(GameObject)) {
//                GameObject[] values= (GameObject[])obj;
//                for(int i= 0; i < values.Length; ++i) {
//                    values[i]= EditorGUILayout.ObjectField(niceName+"["+i+"]", values[i], typeof(GameObject), true) as GameObject;
//                }
//                return;                        
//            }                        
//            Debug.LogWarning("OnInspectorGUI: Unsupport array type: "+fieldInfo.FieldType.Name);
//            return;
//        }
//
//        // Display list of primitives.
//        if(obj is List<bool>) {
//            List<bool> values= (List<bool>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.Toggle(niceName+"["+i+"]", values[i]);
//            }
//            return;
//        }
//        if(obj is List<int>) {
//            List<int> values= (List<int>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.IntField(niceName+"["+i+"]", values[i]);
//            }
//            return;
//        }
//        if(obj is List<float>) {
//            List<float> values= (List<float>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.FloatField(niceName+"["+i+"]", values[i]);
//            }
//            return;            
//        }
//        if(obj is List<string>) {
//            List<string> values= (List<string>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.TextField(niceName+"["+i+"]", values[i]);
//            }
//            return;
//        }
//        if(obj is List<Vector2>) {
//            List<Vector2> values= (List<Vector2>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.Vector2Field(niceName+"["+i+"]", values[i]);
//            }
//            return;                        
//        }            
//        if(obj is List<Vector3>) {
//            List<Vector3> values= (List<Vector3>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.Vector3Field(niceName+"["+i+"]", values[i]);
//            }
//            return;                        
//        }            
//        if(obj is List<Vector4>) {
//            List<Vector4> values= (List<Vector4>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.Vector4Field(niceName+"["+i+"]", values[i]);
//            }
//            return;                        
//        }            
//        if(obj is List<GameObject>) {
//            List<GameObject> values= (List<GameObject>)obj;
//            for(int i= 0; i < values.Count; ++i) {
//                values[i]= EditorGUILayout.ObjectField(niceName+"["+i+"]", values[i], typeof(GameObject), true) as GameObject;
//            }
//            return;                        
//        }            
//
        Debug.LogWarning("OnInspectorGUI: Unknown type: "+port.RuntimeType);
    }

    // -----------------------------------------------------------------------
    public static bool ShowInInspector(string name, Type dataType, Type portType, object currentValue, out object newValue) {
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        // Display primitives.
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
        if(dataType == typeof(float)) {
            float value= currentValue != null ? (float)currentValue : default(float);
            newValue= EditorGUILayout.FloatField(niceName, value);
            return true;
        }
        if(dataType == typeof(string)) {
            string value= ((string)currentValue) ?? "";
            newValue= EditorGUILayout.TextField(niceName, value);
            return true;
        }
        if(portType.IsArray && dataType == typeof(Char)) {
            string value= currentValue != null ? new string(currentValue as char[]) : "";
			Debug.Log("Before: "+value);
            value= EditorGUILayout.TextField(niceName, value);
			newValue= value.ToCharArray(); 
			Debug.Log("After: "+new string(newValue as Char[]));
            return true;				
        }
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
		if(dataType == typeof(object)) {
			bool changed= false;
			object changedValue= null;
			EditorGUILayout.BeginHorizontal();
			if(currentValue != null) {
				// use current object type.
				Type valueType= currentValue.GetType();
				Type dataValueType= iCS_Types.GetElementType(valueType);
				changed= ShowInInspector(name, dataValueType, valueType, currentValue, out changedValue);
			} else {
				EditorGUILayout.TextField(name);
			}
			// Select a type.
			string[] derivedTypeNames= GetListOfDerivedTypeNames(dataType);
			int idx= currentValue != null ? GetIndexOfType(currentValue.GetType(), derivedTypeNames) : 0;
			int selection= EditorGUILayout.Popup(idx, derivedTypeNames);
			if(selection != idx) {
				// TODO...
			}
			EditorGUILayout.EndHorizontal();
			newValue= changedValue;
			return changed;
		}
        newValue= null;
        return false;
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
        Debug.LogWarning("The selected feature is unsupported in the current version iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
