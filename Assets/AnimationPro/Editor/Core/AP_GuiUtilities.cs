using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class AP_GuiUtilities {
    public static void OnInspectorGUI(AP_DataPort port) {
        string niceName= port.name == null || port.name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(port.name);

        if(port.IsVirtual) {
            if(port.AsVirtual.GetProducerPort() == null) {
                EditorGUILayout.TextField(niceName, "(not connected)");
                return;
            }
            port= port.AsVirtual.GetProducerPort();
        }
        AP_Node parent= port.Parent as AP_Node;
        FieldInfo fieldInfo= parent.GetType().GetField(port.name);
        System.Type fieldType= fieldInfo.FieldType;
        System.Object obj= fieldInfo.GetValue(parent);            
        
        // Display primitives.
        if(fieldType == typeof(bool)) {
            bool newValue= EditorGUILayout.Toggle(niceName, (bool)obj);
            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(fieldType == typeof(int)) {
            int newValue= EditorGUILayout.IntField(niceName, (int)obj);
            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(fieldType == typeof(float)) {
            float newValue= EditorGUILayout.FloatField(niceName, (float)obj);
            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(fieldType == typeof(string)) {
            string newValue= EditorGUILayout.TextField(niceName, (string)obj);
            fieldInfo.SetValue(parent, newValue);
            return;
        }
        if(fieldType == typeof(Vector2)) {
            Vector2 newValue= EditorGUILayout.Vector2Field(niceName, (Vector2)obj);
            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(fieldType == typeof(Vector3)) {
            Vector3 newValue= EditorGUILayout.Vector3Field(niceName, (Vector3)obj);
            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(fieldType == typeof(Vector4)) {
            Vector4 newValue= EditorGUILayout.Vector4Field(niceName, (Vector4)obj);
            fieldInfo.SetValue(parent, newValue);
            return;            
        }
        if(fieldType == typeof(GameObject)) {
            GameObject newValue= EditorGUILayout.ObjectField(niceName, obj as GameObject, typeof(GameObject), true) as GameObject;
            fieldInfo.SetValue(parent, newValue);
            return;                        
        }            

        // Display array of primitives.
        if(fieldType.IsArray) {
            System.Type elementType= fieldType.GetElementType();
            if(elementType == typeof(bool)) {
                bool[] values= (bool[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.Toggle(niceName+"["+i+"]", values[i]);
                }
                return;
            }
            if(elementType == typeof(int)) {
                int[] values= (int[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.IntField(niceName+"["+i+"]", values[i]);
                }
                return;
            }
            if(elementType == typeof(float)) {
                float[] values= (float[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.FloatField(niceName+"["+i+"]", values[i]);
                }
                return;            
            }
            if(elementType == typeof(string)) {
                string[] values= (string[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.TextField(niceName+"["+i+"]", values[i]);
                }
                return;
            }
            if(elementType == typeof(Vector2)) {
                Vector2[] values= (Vector2[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.Vector2Field(niceName+"["+i+"]", values[i]);
                }
                return;                        
            }            
            if(elementType == typeof(Vector3)) {
                Vector3[] values= (Vector3[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.Vector3Field(niceName+"["+i+"]", values[i]);
                }
                return;                        
            }            
            if(elementType == typeof(Vector4)) {
                Vector4[] values= (Vector4[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.Vector4Field(niceName+"["+i+"]", values[i]);
                }
                return;                        
            }            
            if(elementType == typeof(GameObject)) {
                GameObject[] values= (GameObject[])obj;
                for(int i= 0; i < values.Length; ++i) {
                    values[i]= EditorGUILayout.ObjectField(niceName+"["+i+"]", values[i], typeof(GameObject), true) as GameObject;
                }
                return;                        
            }                        
            Debug.LogWarning("OnInspectorGUI: Unsupport array type: "+fieldInfo.FieldType.Name);
            return;
        }

        // Display list of primitives.
        if(obj is List<bool>) {
            List<bool> values= (List<bool>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.Toggle(niceName+"["+i+"]", values[i]);
            }
            return;
        }
        if(obj is List<int>) {
            List<int> values= (List<int>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.IntField(niceName+"["+i+"]", values[i]);
            }
            return;
        }
        if(obj is List<float>) {
            List<float> values= (List<float>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.FloatField(niceName+"["+i+"]", values[i]);
            }
            return;            
        }
        if(obj is List<string>) {
            List<string> values= (List<string>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.TextField(niceName+"["+i+"]", values[i]);
            }
            return;
        }
        if(obj is List<Vector2>) {
            List<Vector2> values= (List<Vector2>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.Vector2Field(niceName+"["+i+"]", values[i]);
            }
            return;                        
        }            
        if(obj is List<Vector3>) {
            List<Vector3> values= (List<Vector3>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.Vector3Field(niceName+"["+i+"]", values[i]);
            }
            return;                        
        }            
        if(obj is List<Vector4>) {
            List<Vector4> values= (List<Vector4>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.Vector4Field(niceName+"["+i+"]", values[i]);
            }
            return;                        
        }            
        if(obj is List<GameObject>) {
            List<GameObject> values= (List<GameObject>)obj;
            for(int i= 0; i < values.Count; ++i) {
                values[i]= EditorGUILayout.ObjectField(niceName+"["+i+"]", values[i], typeof(GameObject), true) as GameObject;
            }
            return;                        
        }            

        Debug.LogWarning("OnInspectorGUI: Unknown type: "+fieldInfo.FieldType.Name);
    }
}
