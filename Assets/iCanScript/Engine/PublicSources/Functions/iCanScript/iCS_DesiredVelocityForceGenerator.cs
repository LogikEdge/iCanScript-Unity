using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public class iCS_DesiredVelocityForceGenerator {
	float MaxAcceleration;
	float MaxDeceleration;
	Vector3 AffectedCoordinates;

	[iCS_Function]
	public iCS_DesiredVelocityForceGenerator(float maxAcceleration, float maxDeceleration, Vector3 affectedCoordinates) {
		MaxAcceleration= maxAcceleration;
		MaxDeceleration= maxDeceleration;
		AffectedCoordinates= affectedCoordinates;
	}

	[iCS_Function(Return="acceleration")]
	public Vector3 Update(Vector3 desiredVelocity, Vector3 currentVelocity, float accelerationScale= 1f) {
		// Can compute acceleration in zero time.
		float dt= Time.deltaTime;
		if(Math3D.IsZero(dt)) return Vector3.zero;

		// Eliminate undesired components.
		desiredVelocity.Scale(AffectedCoordinates);
		currentVelocity.Scale(AffectedCoordinates);
		
		// Compute acceleration to meet desired speed in one frame.
		var deltaVelocity= desiredVelocity-currentVelocity;
		var accelerationVector= deltaVelocity/dt;
		float accelerationMagnitude= accelerationVector.magnitude;
		
		// Compute maximum allowed acceleration/deceleration
		float maxAcceleration= accelerationScale * (Vector3.Dot(desiredVelocity, currentVelocity) > 0 ? MaxAcceleration : MaxDeceleration);
		if(accelerationMagnitude > maxAcceleration) {
			accelerationMagnitude= maxAcceleration;
		}
		
		// Return acceleration for this dt.
		accelerationVector= accelerationVector.normalized * accelerationMagnitude;
		return accelerationVector;
	}
}
