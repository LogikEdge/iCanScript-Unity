using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class WD_Node : WD_Aggregate {
    // ======================================================================
    // OBJECT LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public override void Init(string _name, WD_Aggregate _parent) {
        base.Init(_name, _parent);
        
//        // Create ports for each field taged with InPort or OutPort.
//        foreach(var field in GetInputFields()) {
//            WD_InFieldPort.CreateInstance(field.Name, this);                                
//        }
//        foreach(var field in GetOutputFields()) {
//            WD_OutFieldPort.CreateInstance(field.Name, this);            
//        }
    }
}
