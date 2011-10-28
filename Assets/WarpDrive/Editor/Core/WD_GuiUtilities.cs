using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_GuiUtilities {
    public static void OnInspectorGUI(WD_EditorObject port) {
        string niceName= port.Name == null || port.Name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(port.Name);
        Type dataType= WD_Types.GetDataType(port.RuntimeType);

        // Display primitives.
        if(dataType == typeof(bool)) {
            /*bool newValue=*/ EditorGUILayout.Toggle(niceName, default(bool));
//            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(dataType == typeof(int)) {
            /*int newValue=*/ EditorGUILayout.IntField(niceName, default(int));
//            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(dataType == typeof(float)) {
            /*float newValue=*/ EditorGUILayout.FloatField(niceName, default(float));
//            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(dataType == typeof(string)) {
            /*string newValue=*/ EditorGUILayout.TextField(niceName, default(string));
//            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(dataType == typeof(Vector2)) {
            /*Vector2 newValue=*/ EditorGUILayout.Vector2Field(niceName, default(Vector2));
//            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(dataType == typeof(Vector3)) {
            /*Vector3 newValue=*/ EditorGUILayout.Vector3Field(niceName, default(Vector3));
//            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(dataType == typeof(Vector4)) {
            /*Vector4 newValue=*/ EditorGUILayout.Vector4Field(niceName, default(Vector4));
//            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(dataType == typeof(GameObject)) {
            /*GameObject newValue=*/ EditorGUILayout.ObjectField(niceName, null, typeof(GameObject), true) /*as GameObject*/;
//            fieldInfo.SetValue(parent, newValue);
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
}
