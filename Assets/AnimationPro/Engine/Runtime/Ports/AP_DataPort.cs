using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public abstract class AP_DataPort : AP_Port {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public enum DirectionEnum { In, Out };

    [SerializeField] DirectionEnum  myDirection;    
	[SerializeField] AP_DataPort    mySource= null;
    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsInput                     { get { return myDirection == DirectionEnum.In; }}
    public bool IsOutput                    { get { return myDirection == DirectionEnum.Out; }}
    public bool IsVirtual                   { get { return AsVirtual != null; }}
    public AP_VirtualDataPort  AsVirtual    { get { return this as AP_VirtualDataPort; }}
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
    // ----------------------------------------------------------------------
    // Returns the display color of the port.
    public Color DisplayColor {
        get {
            if(IsVirtual) {
                var virtualPort= AsVirtual;
                if(mySource != null) return AP_TypeSystem.GetDisplayColor(mySource.ElementType);
                if(virtualPort.ConcretePort != null) return AP_TypeSystem.GetDisplayColor(virtualPort.ConcretePort.ElementType);
                return Color.white;
            }
            else {
                return AP_TypeSystem.GetDisplayColor(ElementType);
            }
        }
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        AP_DataPort sourcePort= GetProducerPort();
        if(sourcePort == null || sourcePort == this) return true;
        AP_Action sourceAction= sourcePort.Parent as AP_Action;
        if(!sourceAction) return true;
        return sourceAction.IsCurrent();        
    }
    public override AP_Port GetConnectedPort() {
        return mySource;
    }

    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    protected AP_DataPort Init(string thePortName, AP_Node theParent, DirectionEnum theDirection) {
        myDirection= theDirection;
        base.Init(thePortName, theParent);
        switch(myDirection) {
            case DirectionEnum.In : Edge= EdgeEnum.Left; break;
            case DirectionEnum.Out: Edge= EdgeEnum.Right; break;
        }

        // Allow streams to also be used as non-stream ports.
        if(IsStream) {
            FieldInfo fieldInfo= Parent.GetType().GetField(Name);
            System.Type fieldType= fieldInfo.FieldType;
            System.Array array= fieldInfo.GetValue(Parent) as System.Array;
            if(array == null || array.Length == 0) fieldInfo.SetValue(Parent, System.Array.CreateInstance(fieldType.GetElementType(), 1));
        }
        return this;
    }

    // ----------------------------------------------------------------------
    public override void Dealloc() {
        Disconnect();
        base.Dealloc();
    }
    
    // ----------------------------------------------------------------------
    public AP_DataPort Source {
        get { return mySource; }
        set {
            // Nothing to do if we are already connected to source.
            if(value == mySource) return;

            // Compute new type.
            AP_DataPort thePrevSource= mySource;
            mySource= value;
            List<AP_DataPort> connectedPorts= GetConnectedPorts();
            AP_DataPort bestUpConversionPort= GetBestUpConversionPort(connectedPorts);

            // Reject request if type mismatch.
            if(bestUpConversionPort == null) {
                mySource= thePrevSource;
                return;
            }
            ReconfigurePortChain();
            
            // Recompute type for disconnected source.
            if(thePrevSource != null) {
                connectedPorts= thePrevSource.GetConnectedPorts();
                thePrevSource.ReconfigurePortChain();
            }            
        }
    }
    // ----------------------------------------------------------------------
    public bool TrySetSource(AP_DataPort theNewSource) {
        Source= theNewSource;
        return Source == theNewSource;
    }
    
    // ----------------------------------------------------------------------
    public void ReconfigurePortChain() {
        // Cleanup virtual ports that are not the last port in chain.
        List<AP_DataPort> connectedPorts= GetConnectedPorts();
        if(connectedPorts.Count == 1) {
            if(connectedPorts[0].IsVirtual) {
                connectedPorts[0].AsVirtual.ConcretePort= null;
            }
            return;
        }
        foreach(var port in connectedPorts) {
            if(port.IsVirtual && port.mySource != null) {
                AP_VirtualDataPort vPort= port.AsVirtual;
                vPort.ConcretePort= null;
            }
        }
        // The chain is properly configured if the last port is concrete.
        AP_DataPort lastPort= GetLastSourcePort();
        if(!lastPort.IsVirtual) return;
        
        // Assign the virtual port to the best up conversion port if it is not initialized.
        AP_VirtualDataPort virtualPort= lastPort.AsVirtual;
        AP_DataPort bestUpConversionPort= GetBestUpConversionPort(connectedPorts);
        if(virtualPort.ConcretePort == null || bestUpConversionPort == null) {
            virtualPort.ConcretePort= bestUpConversionPort;
            return;
        }
        // Verify that the virtual port is of the proper type.
        if(virtualPort.ConcretePort.ValueType != bestUpConversionPort.ValueType) {
            bestUpConversionPort.UpdateValue();
            virtualPort.ConcretePort= bestUpConversionPort;
            return;
        }
        // Verify that the virtual port actually points to a connected port.
        foreach(var port in connectedPorts) {
            if(port == virtualPort.ConcretePort) return;
        }
        bestUpConversionPort.UpdateValue();
        virtualPort.ConcretePort= bestUpConversionPort;
    }
    
    // ----------------------------------------------------------------------
    public AP_DataPort GetLastSourcePort() {
        AP_DataPort port= this;
        for(; port.mySource != null; port= port.mySource);
        return port;
    }

    // ----------------------------------------------------------------------
    public AP_DataPort GetProducerPort() {
        AP_DataPort lastPort= GetLastSourcePort();
        AP_VirtualDataPort virtualPort= lastPort.AsVirtual;
        return virtualPort != null ? virtualPort.ConcretePort : lastPort;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerValueType() {
        AP_DataPort producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ValueType;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerElementType() {
        AP_DataPort producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ElementType;
    }
    
    // ----------------------------------------------------------------------
    public virtual void UpdateValue() {
        AP_DataPort sourcePort= GetProducerPort();
        if(sourcePort == null || sourcePort == this) return;
        AP_Aggregate sourceNode= sourcePort.Parent;
        System.Type sourceNodeType= sourceNode.GetType();
        FieldInfo sourceFieldInfo= sourceNodeType.GetField(sourcePort.Name);
        if(sourceFieldInfo==null) {
            Debug.LogWarning("Invalid port name");            
        }
        System.Type sourceFieldType= sourceFieldInfo.FieldType;
        AP_Aggregate destNode= Parent;
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
    // Returns a list of all ports connected together.
    public List<AP_DataPort> GetConnectedPorts() {
        // Filled the result with the connected ports.
        List<AP_DataPort> theConnectedPorts= new List<AP_DataPort>();
        for(AP_DataPort linkedPort= this; linkedPort != null; linkedPort= linkedPort.Source) {
            theConnectedPorts.Add(linkedPort);            
        }
        DoGetConnectedPorts(theConnectedPorts);
        return theConnectedPorts;
    }
    
    // ----------------------------------------------------------------------
    // Core function to add ports to the connection port list.
    public virtual void DoGetConnectedPorts(List<AP_DataPort> theConnectedPorts) {
        Top.ForEachRecursive<AP_DataPort>(
            (port)=> {
                AP_DataPort source= port.Source;
                if(source != null) {
                    foreach(var foundPort in theConnectedPorts) {
                        if(foundPort == source) {
                            if(AddPortUniqu(theConnectedPorts, port)) {
                                DoGetConnectedPorts(theConnectedPorts);
                                return;
                            }
                        }
                    }
                }
            }
        );
    }

    // ----------------------------------------------------------------------
    // Adds a port into the port list if it does not already exists.
    protected static bool AddPortUniqu(List<AP_DataPort> thePortList, AP_DataPort theNewPort) {
        foreach(AP_DataPort thePortIter in thePortList) {
            if(thePortIter == theNewPort) return false;
        }
        thePortList.Add(theNewPort);
        return true;
    }
    
    // ----------------------------------------------------------------------
    public static AP_DataPort GetBestUpConversionPort(List<AP_DataPort> portList) {
        if(portList.Count < 1) return null;
        AP_DataPort bestPort= null;
        int i= 0;
        for(; i < portList.Count; ++i) {
            if(!portList[i].IsVirtual) {
                bestPort= portList[i];
                ++i;
                break;
            }
        }
        if(bestPort == null) return null;
        System.Type bestType= bestPort.ElementType;
        for(; i < portList.Count; ++i) {
            if(!portList[i].IsVirtual) {
                System.Type portType= portList[i].ElementType;
                System.Type upConversionType= AP_TypeSystem.GetBestUpConversionType(bestType, portType);
                if(upConversionType == null) return null;
                if(upConversionType != bestType) {
                    bestPort= portList[i];
                    bestType= portType;
                }                
            }
        }
        return bestPort;
    }
    
	// ----------------------------------------------------------------------
    // Removes this port from all connections.
    public void Disconnect() {
        // Disconnect source connection
        Source= null;
        
        // Disconnect all other port being sourced by the given port.
        Top.ForEachRecursive<AP_DataPort>(
            (port)=> {
                if(port.Source == this)
                    port.Source= null;
            }
        );
    }

}
