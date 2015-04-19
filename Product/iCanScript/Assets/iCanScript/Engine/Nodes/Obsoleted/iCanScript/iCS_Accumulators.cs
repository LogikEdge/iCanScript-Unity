using UnityEngine;
using System.Collections;

//[iCS_Class(Company="iCanScript", Library="Accumulators", HideClassFromLibrary= true)]
/// \deprecated
public class iCS_AccumulatorInt {
	public int acc= 0;
	
    [iCS_Function]
    public iCS_AccumulatorInt(int initialValue) {
        acc= initialValue;
    }
	[iCS_Function]
    public void Reset(int resetValue= 0) {
		acc= resetValue;        
    }
	[iCS_Function(Return="acc")]
	public int Accumulate(int toAdd= 1) {
		acc+= toAdd;
		return acc;
	}
}

//[iCS_Class(Company="iCanScript", Library="Accumulators", HideClassFromLibrary= true)]
/// \deprecated
public class iCS_AccumulatorFloat {
	public float acc= 0;
	
    [iCS_Function]
    public iCS_AccumulatorFloat(float initialValue) {
        acc= initialValue;
    }
	[iCS_Function]
    public void Reset(float resetValue= 0) {
		acc= resetValue;        
    }
	[iCS_Function(Return="acc")]
	public float Accumulate(float toAdd= 1f) {
		acc+= toAdd;
		return acc;
	}
}

//[iCS_Class(Company="iCanScript", Library="Accumulators", HideClassFromLibrary= true)]
/// \deprecated
public class iCS_AccumulatorVector2 {
	public Vector2 acc= default(Vector2);
	
    [iCS_Function]
    public iCS_AccumulatorVector2(Vector2 initialValue= default(Vector2)) {
        acc= initialValue;
    }
	[iCS_Function]
    public void Reset(Vector2 resetValue= default(Vector2)) {
		acc= resetValue;        
    }
	[iCS_Function(Return="acc")]
	public Vector2 Accumulate(Vector2 toAdd) {
		acc+= toAdd;
		return acc;
	}
}

//[iCS_Class(Company="iCanScript", Library="Accumulators", HideClassFromLibrary= true)]
/// \deprecated
public class iCS_AccumulatorVector3 {
	public Vector3 acc= default(Vector3);
	
    [iCS_Function]
    public iCS_AccumulatorVector3(Vector3 initialValue= default(Vector3)) {
        acc= initialValue;
    }
	[iCS_Function]
    public void Reset(Vector3 resetValue= default(Vector3)) {
		acc= resetValue;        
    }
	[iCS_Function(Return="acc")]
	public Vector3 Accumulate(Vector3 toAdd) {
		acc+= toAdd;
		return acc;
	}
}

//[iCS_Class(Company="iCanScript", Library="Accumulators", HideClassFromLibrary= true)]
/// \deprecated
public class iCS_AccumulatorVector4 {
	public Vector4 acc= default(Vector4);
	
    [iCS_Function]
    public iCS_AccumulatorVector4(Vector4 initialValue= default(Vector4)) {
        acc= initialValue;
    }
	[iCS_Function]
    public void Reset(Vector4 resetValue= default(Vector4)) {
		acc= resetValue;        
    }
	[iCS_Function(Return="acc")]
	public Vector4 Accumulate(Vector4 toAdd) {
		acc+= toAdd;
		return acc;
	}
}
