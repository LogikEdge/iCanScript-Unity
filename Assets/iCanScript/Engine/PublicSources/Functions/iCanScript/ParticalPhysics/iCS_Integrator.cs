using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Integrator")]
public class iCS_Integrator {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    [iCS_InPort]    public Vector3  Gravity         = new Vector3(0,-9.8f,0);
    [iCS_InPort]    public float    DragFactor      = 0.01f;
                    public float    InvMass         = 1f;
                    public Vector3  PreviousVelocity= Vector3.zero;
                    public Vector3  OutputVelocity  = Vector3.zero;
                    Vector3[]       Forces          = new Vector3[3];
                    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public float   Mass       { [iCS_Function] set { InvMass= Math3D.IsZero(value) ? 1f : 1f/value; }}
    public Vector3 Force1     { [iCS_Function] set { Forces[0]= value; }}
    public Vector3 Force2     { [iCS_Function] set { Forces[1]= value; }}
    public Vector3 Force3     { [iCS_Function] set { Forces[2]= value; }}
    
	// ----------------------------------------------------------------------
    [iCS_Function] public iCS_Integrator() {}

	// ----------------------------------------------------------------------
    [iCS_Function(Return="Velocity")]
    public Vector3 Update(Vector3 actualDisplacement) {
        // Apply all force generators.
        Vector3 forceAccum= Vector3.zero;
        foreach(var force in Forces) {
            forceAccum+= force;
        }
        Vector3 accel= Gravity+forceAccum*InvMass;
        
        // Compute new velocity.
        float dt= Time.deltaTime;
        Vector3 newVelocity= PreviousVelocity+accel*dt;

        // Apply drag factor.
        newVelocity*= (1f-DragFactor*InvMass*dt);

        // Update velocities.
        OutputVelocity= 0.5f*(newVelocity+PreviousVelocity);
        PreviousVelocity= newVelocity;
        return OutputVelocity;
    }
}
