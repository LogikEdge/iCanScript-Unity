using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_FieldPort : WD_Port {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public WD_FieldPort    Source= null;
    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsInput                     { get { return this is WD_InFieldPort; }}
    public bool IsOutput                    { get { return this is WD_OutFieldPort; }}
    public bool IsVirtual                   { get { return AsVirtual != null; }}
    public WD_VirtualDataPort  AsVirtual    { get { return this as WD_VirtualDataPort; }}
    // ----------------------------------------------------------------------
    // Returns the port value type.
    public System.Type ValueType {
        get {
            if(IsVirtual) return null;
            return Parent.GetType().GetField(Name).FieldType;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the port value element type.
    public System.Type ElementType {
        get {
            System.Type valueType= ValueType;
            if(valueType == null) return null;
            if(!valueType.IsArray) return valueType;
            return valueType.GetElementType();
            
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if this is a streaming port.
    public bool IsStream {
        get {
            System.Type valueType= ValueType;
            if(valueType == null) return false;
            return valueType.IsArray;
        }
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        WD_FieldPort sourcePort= GetProducerPort();
        if(sourcePort == null || sourcePort == this) return true;
        WD_Action sourceAction= sourcePort.Parent as WD_Action;
        if(sourceAction == null) return true;
        return sourceAction.IsCurrent();        
    }
    public override WD_Port GetConnectedPort() {
        return Source;
    }

    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    public override void Init(string thePortName, WD_Aggregate theParent) {
        base.Init(thePortName, theParent);
        // Allow streams to also be used as non-stream ports.
        if(IsStream) {
            FieldInfo fieldInfo= Parent.GetType().GetField(Name);
            System.Type fieldType= fieldInfo.FieldType;
            System.Array array= fieldInfo.GetValue(Parent) as System.Array;
            if(array == null || array.Length == 0) fieldInfo.SetValue(Parent, System.Array.CreateInstance(fieldType.GetElementType(), 1));
        }
    }

    // ----------------------------------------------------------------------
    public override void Dealloc() {
        Disconnect();
        base.Dealloc();
    }
    
    // ----------------------------------------------------------------------
    public WD_FieldPort GetLastSourcePort() {
        WD_FieldPort port= this;
        for(; port.Source != null; port= port.Source);
        return port;
    }

    // ----------------------------------------------------------------------
    public WD_FieldPort GetProducerPort() {
        WD_FieldPort lastPort= GetLastSourcePort();
        WD_VirtualDataPort virtualPort= lastPort.AsVirtual;
        return virtualPort != null ? virtualPort.ConcretePort : lastPort;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerValueType() {
        WD_FieldPort producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ValueType;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerElementType() {
        WD_FieldPort producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ElementType;
    }
    
    // ----------------------------------------------------------------------
    public virtual void UpdateValue() {
        WD_FieldPort sourcePort= GetProducerPort();
        if(sourcePort == null || sourcePort == this) return;
        WD_Aggregate sourceNode= sourcePort.Parent;
        System.Type sourceNodeType= sourceNode.GetType();
        FieldInfo sourceFieldInfo= sourceNodeType.GetField(sourcePort.Name);
        if(sourceFieldInfo==null) {
            Debug.LogWarning("Invalid port name");            
        }
        System.Type sourceFieldType= sourceFieldInfo.FieldType;
        WD_Aggregate destNode= Parent;
        System.Type destNodeType= destNode.GetType();
        FieldInfo destFieldInfo= destNodeType.GetField(Name);
        if(destFieldInfo==null) {
            Debug.LogWarning(" Invalid port name");            
        }
        System.Type destFieldType= destFieldInfo.FieldType;
        
        // Same type.  Copy source to destination.
        System.Object sourceValue= sourceFieldInfo.GetValue(sourceNode);
        if(sourceFieldType == destFieldType) {
            destFieldInfo.SetValue(destNode, sourceValue);
            return;
        }
        
        // Source is a stream and destination is a single variable.
        // Copy first element of source into destination.
        if(sourceFieldType.IsArray) {
            // Translate from stream to non-stream.
            System.Type srcElementType= sourceFieldType.GetElementType();
            if(srcElementType == destFieldType) {
                System.Array srcArray= sourceValue as System.Array;
                if(srcArray.Length >= 1) {
                    destFieldInfo.SetValue(destNode, srcArray.GetValue(0));
                }
                return;
            }
            // Convert Vector2 stream to Vector3 stream.
            if(destFieldType.IsArray) {
                System.Type destElementType= destFieldType.GetElementType();
                if(srcElementType == typeof(Vector2) && destElementType == typeof(Vector3)) {
                    Vector2[] srcArray= (Vector2[])sourceValue;
                    Vector3[] destArray= (Vector3[])destFieldInfo.GetValue(destNode);
                    int len= srcArray.Length;
                    if(destArray.Length != len) {
                        destArray= (Vector3[])System.Array.CreateInstance(destFieldType, len);
                        destFieldInfo.SetValue(destNode, destArray);                        
                    }
                    for(int i= 0; i < len; ++i) {
                        Vector2 v= srcArray[i];
                        destArray[i]= new Vector3(v.x, v.y, 0);
                    }
                    return;
                }
                return;
            }
            // Convert Vector2 stream to Vector3 non-stream.
            if(srcElementType == typeof(Vector2) && destFieldType == typeof(Vector3)) {
                Vector2[] srcArray= (Vector2[])sourceValue;
                Vector2 v= srcArray[0];
                destFieldInfo.SetValue(destNode, new Vector3(v.x, v.y, 0));
                return;
            }

        }
        
        // Source is a single variable and destination is a stream.
        // Copy source into first element of destination. 
        if(destFieldType.IsArray) {
            // Translate from non-stream to stream.
            if(destFieldType.GetElementType() == sourceFieldType) {
                System.Array destArray= destFieldInfo.GetValue(destNode) as System.Array;
                if(destArray.Length != 1) {
                    destArray= System.Array.CreateInstance(sourceFieldType, 1);
                    destFieldInfo.SetValue(destNode, destArray);
                }
                destArray.SetValue(sourceValue, 0);
                return;            
            }            
        }
        
        // Convert compatioble non-stream to non-stream ports.
        if(destFieldType == typeof(Vector3)) {
            if(sourceFieldType == typeof(Vector2)) {
                Vector2 v= (Vector2)sourceValue;
                destFieldInfo.SetValue(destNode, new Vector3(v.x, v.y, 0));
                return;
            }
            if(sourceFieldType == typeof(Vector4)) {
                Vector4 v= (Vector4)sourceValue;
                destFieldInfo.SetValue(destNode, new Vector3(v.x, v.y, v.z));
                return;
            }
        }
        if(destFieldType == typeof(Vector4)) {
            if(sourceFieldType == typeof(Vector2)) {
                Vector2 v= (Vector2)sourceValue;
                destFieldInfo.SetValue(destNode, new Vector4(v.x, v.y, 0, 0));
                return;
            }
            if(sourceFieldType == typeof(Vector3)) {
                Vector3 v= (Vector3)sourceValue;
                destFieldInfo.SetValue(destNode, new Vector4(v.x, v.y, v.z, 0));
                return;
            }
        }
        
        Debug.LogWarning("Unable to convert port types from "+sourceFieldType.Name+" to "+destFieldType.Name);
    }

	// ----------------------------------------------------------------------
    // Removes this port from all connections.
    public void Disconnect() {
        // Disconnect source connection
        Source= null;
        
        // Disconnect all other port being sourced by the given port.
        Top.ForEachRecursive<WD_FieldPort>(
            (port)=> {
                if(port.Source == this)
                    port.Source= null;
            }
        );
    }

}
