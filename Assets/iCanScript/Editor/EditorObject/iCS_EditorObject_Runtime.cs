using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
        return EngineObject.GetMethodBase(EditorToEngineList(editorObjects));
    }
    public FieldInfo GetFieldInfo() {
        return EngineObject.GetFieldInfo();
    }
}
