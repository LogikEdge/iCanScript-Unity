using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public static class iCS_ForceGenerators {

    [iCS_Class(Company="iCanScript")]
    class DesiredVelocity {
        float MaxAcceleration;
    }
    
    [iCS_Function(Return= "acceleration")]
    public static Vector3 DesireVelocityForceGenerator(Vector3 desiredVelocity, float maxAcceleration, float maxDeceleration) {
        return Vector3.zero;
    }
}
