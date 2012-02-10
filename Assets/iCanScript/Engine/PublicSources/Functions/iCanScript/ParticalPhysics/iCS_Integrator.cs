using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="ParticalPhysics")]
public class iCS_Integrator {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    [iCS_InOutPort] public Vector3   Gravity        = new Vector3(0,-9.8f,0);
    [iCS_InOutPort] public Vector3   FinalVelocity  = Vector3.zero;
    [iCS_InOutPort] public Vector3   OutputVelocity = Vector3.zero;
    [iCS_InOutPort] public float     Mass           = 1f;
    [iCS_InOutPort] public float     DragFactor     = 0.01f;
    [iCS_OutPort]   public Vector3[] ForceGenerators= null;
    
    
	// ----------------------------------------------------------------------
    [iCS_Function] public iCS_Integrator(int nbOfForceGenerators) {
        ForceGenerators= new Vector3[nbOfForceGenerators];
    }

	// ----------------------------------------------------------------------
    [iCS_Function(Return="Velocity")]
    public Vector3 Update(Vector3 actualDisplacement) {
        // Update gravity
        float dt= Time.deltaTime;
        Vector3 newFinalVelocity= FinalVelocity+dt*Gravity;
        
        // Apply all force generators.
        foreach(var force in ForceGenerators) {
            newFinalVelocity+= force*dt;
        }
        
        // Apply drag force.
        newFinalVelocity*= (1f-DragFactor);
        
        // Update velocities.
        OutputVelocity= 0.5f*(newFinalVelocity+FinalVelocity);
        FinalVelocity= newFinalVelocity;
        return OutputVelocity;
    }
}
