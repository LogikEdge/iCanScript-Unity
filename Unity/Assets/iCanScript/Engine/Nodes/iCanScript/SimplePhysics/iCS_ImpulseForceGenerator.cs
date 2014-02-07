using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Library="SimplePhysic")]
public class iCS_ImpulseForceGenerator {
	bool	IsActive;
	float	StartTime;
	Vector3	MainImpulse;
	Vector3 SecondaryImpulse;
	float	MainImpulseTime;
	float	SecondaryImpulseTime;
	float	RetriggerDelay;
	float   InitialVelocityBoost;
	
	[iCS_Function]
	public iCS_ImpulseForceGenerator(Vector3 mainImpulseAccel, float mainImpulseTime,
	                                 Vector3 secondaryImpulseAccel, float secondaryImpulseTime,
	                                 float retriggerDelay=1.2f, float initialVelocityBoost= 10f) {
		MainImpulse         = mainImpulseAccel;
		MainImpulseTime     = mainImpulseTime;
		SecondaryImpulse    = secondaryImpulseAccel;
		SecondaryImpulseTime= secondaryImpulseTime;
		RetriggerDelay      = Mathf.Max(retriggerDelay, Mathf.Max(mainImpulseTime, secondaryImpulseTime));
        InitialVelocityBoost= initialVelocityBoost;
		IsActive            = false;
	}
	
	[iCS_Function(Return="acceleration")]
	public Vector3 Update(bool trigger) {
		// Can compute acceleration in zero time.
		float dt= Time.deltaTime;
		if(Math3D.IsZero(dt)) return Vector3.zero;

		// Nothing to do if we are not active.
		if(!IsActive && !trigger) return Vector3.zero;
		
		// Update impulse if we are active.
		float currentTime= Time.timeSinceLevelLoad;
		if(IsActive) {
			Vector3 accelAcc= Vector3.zero;
			float deltaTime= currentTime-StartTime;
			if(deltaTime < MainImpulseTime) {
				float remainingMainImpulseTime= MainImpulseTime-deltaTime;
				accelAcc+= remainingMainImpulseTime >= dt ? MainImpulse : MainImpulse*remainingMainImpulseTime/dt; 
			}
			if(trigger && deltaTime < SecondaryImpulseTime) {
				float remainingSecondaryImpulseTime= SecondaryImpulseTime-deltaTime;
				accelAcc+= remainingSecondaryImpulseTime >= dt ? SecondaryImpulse : SecondaryImpulse*remainingSecondaryImpulseTime/dt; 				
			}
			if(deltaTime > RetriggerDelay) {
				IsActive= false;
			}
			return accelAcc;
		}
		
		// Determine if we should start the impulse.
		IsActive= true;
		StartTime= currentTime;
		var direction= MainImpulse.normalized;
		return MainImpulse+SecondaryImpulse+direction*InitialVelocityBoost/dt;
	}
}
