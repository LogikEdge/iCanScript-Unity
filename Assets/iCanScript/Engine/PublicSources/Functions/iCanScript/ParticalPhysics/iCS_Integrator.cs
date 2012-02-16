using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Integrator")]
public class iCS_Integrator {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    [iCS_InPort]    public Vector3  Gravity        = new Vector3(0,-9.8f,0);
    [iCS_InPort]    public float    Mass           = 1f;
    [iCS_InPort]    public float    DragFactor     = 0.01f;
                    public Vector3  FinalVelocity  = Vector3.zero;
                    public Vector3  OutputVelocity = Vector3.zero;
                    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public Vector3 Force1 { [iCS_Function] set { Forces[0]= value; }}
    public Vector3 Force2 { [iCS_Function] set { Forces[1]= value; }}
    public Vector3 Force3 { [iCS_Function] set { Forces[2]= value; }}
    Vector3[]   Forces= new Vector3[3];
    
	// ----------------------------------------------------------------------
    [iCS_Function] public iCS_Integrator() {}

	// ----------------------------------------------------------------------
    [iCS_Function(Return="Velocity")]
    public Vector3 Update(Vector3 actualDisplacement) {
        // Update gravity
        float dt= Time.deltaTime;
        Vector3 newFinalVelocity= FinalVelocity+dt*Gravity;
        
        // Apply all force generators.
        foreach(var force in Forces) {
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
