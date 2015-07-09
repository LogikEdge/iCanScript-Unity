using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
public static class iCS_GuiUtilities {
    class GUIFieldInfo {
        public bool    Foldout= false;
        public object  Value= null;
		public int	   ControlID= -1;
        public GUIFieldInfo(bool foldout) { Foldout= foldout; }
        public GUIFieldInfo(object value) { Value= value; }
    }
    
    // -----------------------------------------------------------------------
    static void   AddFoldout(Dictionary<string,object> db, string key, bool foldout) { db.Add(key, new GUIFieldInfo(foldout)); }
    static bool   Foldout(Dictionary<string,object> db, string key)                  { return ((GUIFieldInfo)(db[key])).Foldout; }
    static void   Foldout(Dictionary<string,object> db, string key, bool foldout)    { ((GUIFieldInfo)(db[key])).Foldout= foldout; }
    static void   AddValue(Dictionary<string,object> db, string key, object value)   { db.Add(key, new GUIFieldInfo(value)); }
    static void   RemoveValue(Dictionary<string,object> db, string key)              { db.Remove(key); }
    static object Value(Dictionary<string,object> db, string key)                    { return ((GUIFieldInfo)(db[key])).Value; }
    static void   Value(Dictionary<string,object> db, string key, object value)      { ((GUIFieldInfo)(db[key])).Value= value; }
    static int    ControlID(Dictionary<string,object> db, string key)                { return ((GUIFieldInfo)(db[key])).ControlID; }
    static void   ControlID(Dictionary<string,object> db, string key, int value)     { ((GUIFieldInfo)(db[key])).ControlID= value; }

    // -----------------------------------------------------------------------
    public static void OnInspectorDataPortGUI(iCS_EditorObject port, int indentLevel, Dictionary<string,object> foldoutDB) {
        OnInspectorDataPortGUI(port.DisplayName, port, indentLevel, foldoutDB);
    }
    // -----------------------------------------------------------------------
    public static void OnInspectorDataPortGUI(string labelName, iCS_EditorObject port, int indentLevel, Dictionary<string,object> foldoutDB) {
        var iStorage= port.IStorage;
        // Only accept data ports.
        if(!port.IsDataOrControlPort) return;
        // Extract port information
		Type portType= port.RuntimeType;
        iCS_EditorObject parent= port.Parent;
        iCS_EditorObject sourcePort= port.ProducerPort;
        bool hasSource= sourcePort != null;
        // Determine if we are allowed to modify port value.
        bool isReadOnly= !(!hasSource && (port.IsInputPort || port.IsKindOfPackagePort));
        // Nothing to display if we don't have a runtime object and we are in readonly.
        if(isReadOnly) return;
        // Update port value from runtime object in priority or the descriptor string if no runtime.
		object portValue= port.Value;
        // Determine section name (used for foldout parent).
        string foldoutName= (port.IsInputPort ? "in" : "out")+"."+parent.DisplayName;
        // Display primitives.
        bool isDirty= false;
        object newPortValue= ShowInInspector(labelName, isReadOnly, hasSource, foldoutName, portType, portValue, indentLevel, foldoutDB, ref isDirty, iStorage);
        if(!isReadOnly && isDirty && newPortValue != null /*&& !newPortValue.Equals(portValue)*/) {
			iCS_UserCommands.ChangeValue(port, newPortValue);
        }
    }

    // -----------------------------------------------------------------------
    public static object ShowInInspector(string name, bool isReadOnly, bool hasSource, string compositeParent,
                                         Type baseType, object currentValue,
                                         int indentLevel, Dictionary<string,object> foldoutDB, ref bool isDirty,
                                         iCS_IStorage iStorage) {
		// Extract type information.
        Type valueType= currentValue != null ? currentValue.GetType() : baseType;
        Type baseElementType= iCS_Types.GetElementType(baseType);
        Type valueElementType= iCS_Types.GetElementType(valueType);
        EditorGUI.indentLevel= indentLevel;
		// Make nice name for field to edit.
        string niceName= name == null || name == "" ? "(Unamed)" : ObjectNames.NicifyVariableName(name);
        if(baseType.IsArray) niceName= "["+niceName+"]";
        // Special case for readonly & null value.
        if(isReadOnly && currentValue == null) {
            EditorGUILayout.LabelField(niceName, hasSource ? "(see connection)":"(not available)");
            return currentValue;
        }
        // Special case for arrays
		if(baseType.IsArray) {
			if(currentValue == null) {
				currentValue= Array.CreateInstance(baseElementType, 0);
				isDirty= true;
				return currentValue;
			}			
            string compositeArrayName= compositeParent+"."+name;
            if(!foldoutDB.ContainsKey(compositeArrayName)) AddFoldout(foldoutDB, compositeArrayName, false);
            bool showArray= Foldout(foldoutDB, compositeArrayName);
            showArray= EditorGUILayout.Foldout(showArray, niceName);
            Foldout(foldoutDB, compositeArrayName, showArray);
			if(!showArray) return currentValue;
            EditorGUI.indentLevel= indentLevel+1;
            Array array= currentValue as Array;
            int newSize= array.Length;
            if(ModalEdit("Length", "Length", ref newSize, compositeArrayName, (n,v)=> EditorGUILayout.IntField(n,v), foldoutDB)) {
                if(newSize != array.Length) {
					if(newSize < 100 || EditorUtility.DisplayDialog("Resizing array", "The new size of the array is > 100.  Are you sure you want your new array to be resized to "+newSize+".", "Resize", "Cancel")) {
	                    Array newArray= Array.CreateInstance(baseElementType, newSize);
	                    Array.Copy(array, newArray, Mathf.Min(newSize, array.Length));
	                    array= newArray;
	                    isDirty= true;							
					}
                }					
			} 
            for(int i= 0; i < array.Length; ++i) {
				bool elemDirty= false;
                object newValue= ShowInInspector("["+i+"]", isReadOnly, hasSource, compositeArrayName, baseElementType, array.GetValue(i), indentLevel+1, foldoutDB, ref elemDirty, iStorage);
				isDirty |= elemDirty;
				if(elemDirty) array.SetValue(newValue, i);
            }
            return array;
		}
        // Support all UnityEngine objects.
        if(iCS_Types.IsA<UnityEngine.Object>(baseElementType)) {
            UnityEngine.Object value= currentValue != null ? currentValue as UnityEngine.Object: null;
            UnityEngine.Object newValue= EditorGUILayout.ObjectField(niceName, value, baseElementType, true);
			if(value == null && newValue == null) return newValue;
			if(value != newValue ) {
				// Do not allow GameObject binding in Prefabs.
                if(PrefabUtility.GetPrefabType(iStorage.HostGameObject) == PrefabType.Prefab) {
                    if(newValue != null && iCS_Types.IsA<GameObject>(newValue.GetType())) {
                        var isSceneObject= UnityUtility.IsSceneGameObject(newValue as GameObject);
                        if(isSceneObject == true) {
                            iCS_EditorController.ShowNotificationOnVisualEditor(new GUIContent("Unity does not allow binding a Scene object to a Prefab."));
							iCS_EditorController.RepaintVisualEditor();
                            return currentValue;
                        }                    
                    }
                }
                isDirty= true;
            }
			return newValue;
        }        
        // Support Type type.
        if(valueElementType == typeof(Type) || currentValue is Type) {
            string typeName= currentValue != null ? iCS_Types.ToTypeString(currentValue as Type) : "";
            if(ModalEdit(niceName, name, ref typeName, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB)) {
                Type newType= iCS_Types.GetTypeFromTypeString(typeName);
                if(newType != null) {
                    isDirty= true;
                    return newType;
                }
                else {
                    Debug.LogWarning("Type: "+typeName+" was not found.");
                }
            } 
            return currentValue;
        }
        // Determine if we should create a value if the current value is null.
        if(currentValue == null) {
            // Automatically create value types.
            if(baseElementType.IsValueType || baseElementType.IsEnum) {
                currentValue= iCS_Types.CreateInstance(baseElementType);                    
                isDirty= true;
				return currentValue;
            } else {
                // -- Create reference types with a default constructor --
                var derivedTypes= P.fold(
                    (lst,t)=> {
                        // -- Append types with default constructor --
                        foreach(var constructor in t.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
                            if(constructor.GetParameters().Length == 0) {
                                lst.Add(t);
                            }
                        }
                        return lst;
                    },
                    new List<Type>(),
                    iCS_Types.GetAllTypesThatDeriveFrom(baseElementType)
                );
                if(derivedTypes.Count <= 1) {
					isDirty= true;
                    return iCS_Types.CreateInstance(baseType);
                }
                string[] typeNames= new string[derivedTypes.Count+1];
                typeNames[0]= "None";
                if(baseType.IsArray) {
                    for(int i= 0; i < derivedTypes.Count; ++i) typeNames[i+1]= iCS_Types.GetName(derivedTypes[i])+"[]";                                        
                } else {
                    for(int i= 0; i < derivedTypes.Count; ++i) typeNames[i+1]= iCS_Types.GetName(derivedTypes[i]);                    
                }
                int idx= EditorGUILayout.Popup(niceName, 0, typeNames);
                if(idx == 0) return null;
                isDirty= true;
                if(baseType.IsArray) {
                    return Array.CreateInstance(derivedTypes[idx-1],0);
                }
                return iCS_Types.CreateInstance(derivedTypes[idx-1]);
            }
        }
        // Special case for enumerations
        if(valueElementType.IsEnum) {
            if(currentValue == null) { return currentValue; }
            System.Enum value= currentValue as System.Enum;
            System.Enum newValue= EditorGUILayout.EnumPopup(niceName, value);
            if(newValue.ToString().CompareTo(value.ToString()) != 0) isDirty= true;
            return newValue;
        }
        // C# data types.
        if(valueElementType == typeof(byte)) {
            byte value= (byte)currentValue;
            byte newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> (byte)EditorGUILayout.IntField(n,(int)v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(sbyte)) {
            sbyte value= (sbyte)currentValue;
            sbyte newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> (sbyte)EditorGUILayout.IntField(n,(int)v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(bool)) {
            bool value= (bool)currentValue;
            bool newValue= EditorGUILayout.Toggle(niceName, value);
            if(newValue != value) isDirty= true;
            return newValue;
        }
        if(valueElementType == typeof(int)) {
            int value= (int)currentValue;
            int newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.IntField(n,v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(uint)) {
            string uintAsString= (string)Convert.ChangeType((uint)currentValue, typeof(string));
            string newValue= uintAsString;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
                if(uintAsString.CompareTo(newValue) != 0) {
                    isDirty= true;                    
                }
            }
            return Convert.ChangeType(newValue, typeof(uint));
        }
        if(valueElementType == typeof(short)) {
            short value= (short)currentValue;
            short newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> (short)EditorGUILayout.IntField(n,(int)v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(ushort)) {
            int value= (ushort)currentValue;
            int newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> (ushort)EditorGUILayout.IntField(n,(int)v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(long)) {
            string longAsString= (string)Convert.ChangeType((long)currentValue, typeof(string));
            string newValue= longAsString;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
                if(longAsString.CompareTo(newValue) != 0) {
                    isDirty= true;                    
                }
            }
            return Convert.ChangeType(newValue, typeof(long));
        }
        if(valueElementType == typeof(ulong)) {
            string ulongAsString= (string)Convert.ChangeType((ulong)currentValue, typeof(string));
            string newValue= ulongAsString;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
                if(ulongAsString.CompareTo(newValue) != 0) {
                    isDirty= true;                    
                }
            }
            return Convert.ChangeType(newValue, typeof(ulong));
        }
        if(valueElementType == typeof(float)) {
            float value= (float)currentValue; 
            float newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.FloatField(n,v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(double)) {
            string value= (string)Convert.ChangeType((double)currentValue, typeof(string));
            string newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return Convert.ChangeType(newValue, typeof(double));
        }
        if(valueElementType == typeof(decimal)) {
            float value= (float)((decimal)currentValue);
            float newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.FloatField(n,v), foldoutDB, true)) {
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return (decimal)newValue;
        }
        if(valueElementType == typeof(char)) {
            string value= ""+((char)currentValue);
            string newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
    			if(newValue == null || newValue == "" || newValue[0] == 0) newValue= " ";
                if(value != newValue) {
                    isDirty= true;                    
                }
            }
            return (newValue != null && newValue.Length >= 1) ? newValue[0] : default(char);
        }
        if(valueElementType == typeof(string)) {
            string value= ((string)currentValue) ?? "";
            string newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.TextField(n,v), foldoutDB, true)) {
                newValue= newValue ?? "";
                if(string.Compare(value, newValue) != 0) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        // Unity data types.
        if(valueElementType == typeof(Vector2)) {
            Vector2 value= (Vector2)currentValue;
            Vector2 newValue= value;
            if(ModalEdit(niceName, name, ref value, compositeParent, (n,v)=> EditorGUILayout.Vector2Field(n,v), foldoutDB, true)) {
                if(Math3D.IsNotEqual(value, newValue)) {
                    isDirty= true;                    
                }
            }
            return value;
        }
        if(valueElementType == typeof(Vector3)) {
            Vector3 value= (Vector3)currentValue;
            Vector3 newValue= value;
            if(ModalEdit(niceName, name, ref value, compositeParent, (n,v)=> EditorGUILayout.Vector3Field(n,v), foldoutDB, true)) {
                if(Math3D.IsNotEqual(value, newValue)) {
                    isDirty= true;                    
                }
            }
            return value;
        }
        if(valueElementType == typeof(Vector4)) {
            Vector4 value= (Vector4)currentValue;
            Vector4 newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.Vector4Field(n,v), foldoutDB, true)) {
                if(Math3D.IsNotEqual(value, newValue)) {
                    isDirty= true;                    
                }
            }
            return newValue;
        }
        if(valueElementType == typeof(Color)) {
            Color value= (Color)currentValue;
            Color newValue= value;
            if(ModalEdit(niceName, name, ref newValue, compositeParent, (n,v)=> EditorGUILayout.ColorField(n,v), foldoutDB, true)) {
                if(Math3D.IsNotEqual(newValue.r, value.r) ||
                   Math3D.IsNotEqual(newValue.g, value.g) ||
                   Math3D.IsNotEqual(newValue.b, value.b) ||
                   Math3D.IsNotEqual(newValue.a, value.a)) {
                    isDirty= true;
                }
            }
            return newValue;
        }
		// All other types.
        string compositeName= compositeParent+"."+name;
        if(!foldoutDB.ContainsKey(compositeName)) AddFoldout(foldoutDB, compositeName, false);
        bool showCompositeObject= Foldout(foldoutDB, compositeName);
        showCompositeObject= EditorGUILayout.Foldout(showCompositeObject, niceName);
        Foldout(foldoutDB, compositeName, showCompositeObject);
        if(showCompositeObject) {
    		foreach(var field in valueElementType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
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
                    bool isFieldDirty= false;
                    object newFieldValue= ShowInInspector(field.Name, isReadOnly, hasSource, compositeName, field.FieldType, currentFieldValue, indentLevel+1, foldoutDB, ref isFieldDirty, iStorage);
                    isDirty |= isFieldDirty;
                    if(!isReadOnly && isFieldDirty) {
                        field.SetValue(currentValue, newFieldValue);
                    }
                }
    		}        
        }
        return currentValue;
    }

    // ----------------------------------------------------------------------
    static bool ModalEdit<T>(string niceName, string name, ref T currentValue, string parentName,
                             Func<string,T,T> editor, Dictionary<string,object> db, bool noDelay= false) {
        // No delay processing.
        if(noDelay) {
            GUI.changed= false;
            T newValueNoDelay= editor(niceName, currentValue);
            if(GUI.changed) {
                currentValue= newValueNoDelay;
                return true;
            }
            return false;
        }
        // Wait until keyboard moves to a different field.
        string controlName= parentName+"."+name;
		if(!db.ContainsKey(controlName)) AddValue(db, controlName, currentValue);
        T value= (T)Value(db, controlName);
        GUI.changed= false;
        T newValue= editor(niceName, value);
        Value(db, controlName, newValue);
		int keyControlID= GUIUtility.keyboardControl;
        if(GUI.changed) {
            ControlID(db, controlName, keyControlID);
        }
		int savedKeyControlID= ControlID(db, controlName);
		if(savedKeyControlID == -1) return false;
        if(savedKeyControlID != keyControlID || !EditorGUIUtility.editingTextField) {
			ControlID(db, controlName, -1);
            RemoveValue(db, controlName);
		    currentValue= newValue;
		    return true;
	    }
        return false;
    }
    
    // -----------------------------------------------------------------------
    public static void UnsupportedFeature() {
        Debug.LogWarning("The selected feature is unsupported in the current version of iCanScript.  Feature is planned for a later version.  Thanks for your patience.");
    }
}
}
