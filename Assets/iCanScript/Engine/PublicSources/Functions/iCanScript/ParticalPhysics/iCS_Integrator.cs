using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Integrator")]
public class iCS_Integrator {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    [iCS_InPort]    public Vector3  Gravity        = new Vector3(0,-9.8f,0);
    [iCS_OutPort]   public Vector3  FinalVelocity  = Vector3.zero;
    [iCS_OutPort]   public Vector3  OutputVelocity = Vector3.zero;
    [iCS_InPort]    public float    Mass           = 1f;
    [iCS_InPort]    public float    DragFactor     = 0.01f;
    [iCS_InPort]    public Vector3  Force1         = Vector3.zero;
    [iCS_InPort]    public Vector3  Force2         = Vector3.zero;
    [iCS_InPort]    public Vector3  Force3         = Vector3.zero;
    
	// ----------------------------------------------------------------------
    [iCS_Function] public iCS_Integrator(int nbOfForceGenerators) {
    }

	// ----------------------------------------------------------------------
    [iCS_Function(Return="Velocity")]
    public Vector3 Update(Vector3 actualDisplacement) {
        // Update gravity
        float dt= Time.deltaTime;
        Vector3 newFinalVelocity= FinalVelocity+dt*Gravity;
        
        // Apply all force generators.
        newFinalVelocity+= Force1*dt;
        newFinalVelocity+= Force2*dt;
        newFinalVelocity+= Force3*dt;
        
        // Apply drag force.
        newFinalVelocity*= (1f-DragFactor);
        
        // Update velocities.
        OutputVelocity= 0.5f*(newFinalVelocity+FinalVelocity);
        FinalVelocity= newFinalVelocity;
        return OutputVelocity;
    }
}
