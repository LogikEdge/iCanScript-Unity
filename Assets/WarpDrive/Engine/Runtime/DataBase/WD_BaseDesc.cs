using UnityEngine;
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

    public abstract WD_EditorObject CreateInstance(WD_EditorObjectMgr editorObjects, int parentId, Vector2 initialPos);

}
