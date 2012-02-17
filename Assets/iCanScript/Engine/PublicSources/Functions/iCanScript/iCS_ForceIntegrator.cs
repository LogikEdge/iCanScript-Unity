using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public class iCS_ForceIntegrator {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    [iCS_InPort]    public Vector3  Gravity;
    [iCS_InPort]    public float    Damping;
                    public float    InvMass;
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
    [iCS_Function]
    public iCS_ForceIntegrator() : this(new Vector3(0f,-9.8f,0f), 1f, 0.005f) {}
    [iCS_Function]
    public iCS_ForceIntegrator(Vector3 gravity, float mass= 1f, float damping= 0.995f) {
        Gravity= gravity;
        Mass= mass;
        Damping= damping;
    }

	// ----------------------------------------------------------------------
    [iCS_Function(Return="Velocity")]
    public Vector3 Integrate(Vector3 actualDisplacement) {
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
        newVelocity*= Mathf.Pow(Damping, dt);

        // Update velocities.
        OutputVelocity= 0.5f*(newVelocity+PreviousVelocity);
        PreviousVelocity= newVelocity;
        return OutputVelocity;
    }
}
