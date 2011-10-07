using UnityEngine;
using System;
using System.Collections;

public abstract class WD_BaseDesc {
    public string   Company;
    public string   Package;
    public string   Name;
    public WD_BaseDesc(string company, string package, string name) {
        Company= company;
        Package= package;
        Name   = name;
    }    

    public abstract int CreateInstance(WD_EditorObjectMgr editorObjects, int parentId, Vector2 initialPos);

    protected int CreatePort(WD_EditorObjectMgr editorObjects, string name, int parentId, Type type, WD_ObjectTypeEnum displayType) {
        int portId= editorObjects.GetNextAvailableId();
        editorObjects[portId]= new WD_EditorObject(portId, name, type, parentId, displayType, new Rect(0,0,0,0));        
        return portId;
    }
}
