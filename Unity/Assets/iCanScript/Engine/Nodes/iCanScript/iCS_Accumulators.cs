using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Accumulators")]
public class iCS_AccumulatorInt {
	[iCS_OutPort] public int acc= 0;
	
	[iCS_Function(Return="acc")]
	public int Accumulate(int toAdd= 1, bool reset= false, int resetValue= 0) {
		if(reset) {
			acc= resetValue;
			return acc;
		}
		acc+= toAdd;
		return acc;
	}
}

[iCS_Class(Company="iCanScript", Package="Accumulators")]
public class iCS_AccumulatorFloat {
	[iCS_OutPort] public float acc= 0;
	
	[iCS_Function(Return="acc")]
	public float Accumulate(float toAdd= 1f, bool reset= false, float resetValue= 0f) {
		if(reset) {
			acc= resetValue;
			return acc;
		}
		acc+= toAdd;
		return acc;
	}
}

[iCS_Class(Company="iCanScript", Package="Accumulators")]
public class iCS_AccumulatorVector2 {
	[iCS_OutPort] public Vector2 acc= default(Vector2);
	
	[iCS_Function(Return="acc")]
	public Vector2 Accumulate(Vector2 toAdd, bool reset= false, Vector2 resetValue= default(Vector2)) {
		if(reset) {
			acc= resetValue;
			return acc;
		}
		acc+= toAdd;
		return acc;
	}
}

[iCS_Class(Company="iCanScript", Package="Accumulators")]
public class iCS_AccumulatorVector3 {
	[iCS_OutPort] public Vector3 acc= default(Vector3);
	
	[iCS_Function(Return="acc")]
	public Vector3 Accumulate(Vector3 toAdd, bool reset= false, Vector3 resetValue= default(Vector3)) {
		if(reset) {
			acc= resetValue;
			return acc;
		}
		acc+= toAdd;
		return acc;
	}
}

[iCS_Class(Company="iCanScript", Package="Accumulators")]
public class iCS_AccumulatorVector4 {
	[iCS_OutPort] public Vector4 acc= default(Vector4);
	
	[iCS_Function(Return="acc")]
	public Vector4 Accumulate(Vector4 toAdd, bool reset= false, Vector4 resetValue= default(Vector4)) {
		if(reset) {
			acc= resetValue;
			return acc;
		}
		acc+= toAdd;
		return acc;
	}
}
