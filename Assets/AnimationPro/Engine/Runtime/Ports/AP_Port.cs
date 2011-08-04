using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class AP_Port : AP_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public enum EdgeEnum { Top, Bottom, Right, Left };
    public enum DirectionEnum { In, Out, Control };

    public           Vector2        LocalPosition  = Vector2.zero;
    public           bool           IsBeingDragged = false;
    public           EdgeEnum       Edge           = EdgeEnum.Left;

    [SerializeField] DirectionEnum  myDirection;    
	[SerializeField] AP_Port        mySource       = null;
    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsInput                 { get { return myDirection == DirectionEnum.In; }}
    public bool IsOutput                { get { return myDirection == DirectionEnum.Out; }}
    public bool IsControl               { get { return AsControl != null; }}
    public bool IsVirtual               { get { return AsVirtual != null; }}
    public AP_VirtualPort  AsVirtual    { get { return this as AP_VirtualPort; }}
    public AP_ControlPort  AsControl    { get { return this as AP_ControlPort; }}
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
    // Lifetime Management
    // ----------------------------------------------------------------------
    public static AP_Port CreateInstance(string portName, AP_Node parent, DirectionEnum direction) {
        return CreateInstance<AP_Port>().Init(portName, parent, direction);
    }
    // ----------------------------------------------------------------------
    protected AP_Port Init(string thePortName, AP_Node theParent, DirectionEnum theDirection) {
        myDirection= theDirection;
        base.Init(thePortName, theParent);
        switch(myDirection) {
            case DirectionEnum.In: Edge= EdgeEnum.Left; break;
            case DirectionEnum.Out: Edge= EdgeEnum.Right; break;
            case DirectionEnum.Control: Edge= EdgeEnum.Top; break;
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
    public AP_Port Source {
        get { return mySource; }
        set {
            // Nothing to do if we are already connected to source.
            if(value == mySource) return;

            // Compute new type.
            AP_Port thePrevSource= mySource;
            mySource= value;
            List<AP_Port> connectedPorts= GetConnectedPorts();
            AP_Port bestUpConversionPort= GetBestUpConversionPort(connectedPorts);

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
    public bool TrySetSource(AP_Port theNewSource) {
        Source= theNewSource;
        return Source == theNewSource;
    }
    
    // ----------------------------------------------------------------------
    public void ReconfigurePortChain() {
        // Cleanup virtual ports that are not the last port in chain.
        List<AP_Port> connectedPorts= GetConnectedPorts();
        if(connectedPorts.Count == 1) {
            if(connectedPorts[0].IsVirtual) {
                connectedPorts[0].AsVirtual.ConcretePort= null;
            }
            return;
        }
        foreach(var port in connectedPorts) {
            if(port.IsVirtual && port.mySource != null) {
                AP_VirtualPort vPort= port.AsVirtual;
                vPort.ConcretePort= null;
            }
        }
        // The chain is properly configured if the last port is concrete.
        AP_Port lastPort= GetLastSourcePort();
        if(!lastPort.IsVirtual) return;
        
        // Assign the virtual port to the best up conversion port if it is not initialized.
        AP_VirtualPort virtualPort= lastPort.AsVirtual;
        AP_Port bestUpConversionPort= GetBestUpConversionPort(connectedPorts);
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
    public AP_Port GetLastSourcePort() {
        AP_Port port= this;
        for(; port.mySource != null; port= port.mySource);
        return port;
    }

    // ----------------------------------------------------------------------
    public AP_Port GetProducerPort() {
        AP_Port lastPort= GetLastSourcePort();
        AP_VirtualPort virtualPort= lastPort.AsVirtual;
        return virtualPort != null ? virtualPort.ConcretePort : lastPort;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerValueType() {
        AP_Port producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ValueType;
    }
    
    // ----------------------------------------------------------------------
    public System.Type GetProducerElementType() {
        AP_Port producerPort= GetProducerPort();
        if(producerPort == null) return null;
        return producerPort.ElementType;
    }
    
    // ----------------------------------------------------------------------
    public void UpdateValue() {
        if(IsVirtual) return;
        AP_Port sourcePort= GetProducerPort();
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
    public List<AP_Port> GetConnectedPorts() {
        // Filled the result with the connected ports.
        List<AP_Port> theConnectedPorts= new List<AP_Port>();
        for(AP_Port linkedPort= this; linkedPort != null; linkedPort= linkedPort.Source) {
            theConnectedPorts.Add(linkedPort);            
        }
        DoGetConnectedPorts(theConnectedPorts);
        return theConnectedPorts;
    }
    
    // ----------------------------------------------------------------------
    // Core function to add ports to the connection port list.
    public virtual void DoGetConnectedPorts(List<AP_Port> theConnectedPorts) {
        Top.ForEachRecursive<AP_Port>(
            (port)=> {
                AP_Port source= port.Source;
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
    protected static bool AddPortUniqu(List<AP_Port> thePortList, AP_Port theNewPort) {
        foreach(AP_Port thePortIter in thePortList) {
            if(thePortIter == theNewPort) return false;
        }
        thePortList.Add(theNewPort);
        return true;
    }
    
    // ----------------------------------------------------------------------
    public static AP_Port GetBestUpConversionPort(List<AP_Port> portList) {
        if(portList.Count < 1) return null;
        AP_Port bestPort= null;
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
        Top.ForEachRecursive<AP_Port>(
            (port)=> {
                if(port.Source == this)
                    port.Source= null;
            }
        );
    }
    

//#if UNITY_EDITOR
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// BEGIN EDITOR SECTION
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    // ======================================================================
    // LAYOUT
    // ----------------------------------------------------------------------
    public override void DoLayout() {
        // Don't interfear with dragging.
        if(IsBeingDragged) return;
        AP_Node parentNode= Parent as AP_Node;
        if(!parentNode) return;
        Rect parentPosition= parentNode.Position;
        // Make certain that the port is on an edge.
        switch(Edge) {
            case EdgeEnum.Top:
                if(!MathfExt.IsZero(LocalPosition.y)) {
                    LocalPosition.y= 0;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.x > parentPosition.width) {
                    LocalPosition.x= parentPosition.width-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Bottom:
                if(MathfExt.IsNotEqual(LocalPosition.y, parentPosition.height)) {
                    LocalPosition.y= parentPosition.height;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.x > parentPosition.width) {
                    LocalPosition.x= parentPosition.width-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Left:
                if(!MathfExt.IsZero(LocalPosition.x)) {
                    LocalPosition.x= 0;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.y > parentPosition.height) {
                    LocalPosition.y= parentPosition.height-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Right:
                if(MathfExt.IsNotEqual(LocalPosition.x, parentPosition.width)) {
                    LocalPosition.x= parentPosition.width;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.y > parentPosition.height) {
                    LocalPosition.y= parentPosition.height-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;            
        }
    }
    // ----------------------------------------------------------------------
    public void SnapToParent() {
        AP_Node parentNode= Parent as AP_Node;
        float parentHeight= parentNode.Position.height;
        float parentWidth= parentNode.Position.width;
        float portRadius= AP_EditorConfig.PortRadius;
        if(MathfExt.IsWithin(LocalPosition.y, -portRadius, portRadius)) {
            Edge= EdgeEnum.Top;
        }        
        if(MathfExt.IsWithin(LocalPosition.y, parentHeight-portRadius, parentHeight+portRadius)) {
            Edge= EdgeEnum.Bottom;
        }
        if(MathfExt.IsWithin(LocalPosition.x, -portRadius, portRadius)) {
            Edge= EdgeEnum.Left;
        }
        if(MathfExt.IsWithin(LocalPosition.x, parentWidth-portRadius, parentWidth+portRadius)) {
            Edge= EdgeEnum.Right;
        }
        IsEditorDirty= true;
        Layout();
    }
    // ----------------------------------------------------------------------
    public bool IsOnTopEdge        { get { return Edge == EdgeEnum.Top; }}
    public bool IsOnBottomEdge     { get { return Edge == EdgeEnum.Bottom; }}
    public bool IsOnLeftEdge       { get { return Edge == EdgeEnum.Left; }}
    public bool IsOnRightEdge      { get { return Edge == EdgeEnum.Right; }}
    public bool IsOnHorizontalEdge { get { return IsOnTopEdge || IsOnBottomEdge; }}
    public bool IsOnVerticalEdge   { get { return IsOnLeftEdge || IsOnRightEdge; }}
    
    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent() {
        AP_Node parentNode= Parent as AP_Node;
        if(parentNode.IsInside(Position)) return 0;
        Rect parentPosition= parentNode.Position;
        if(Position.x > parentPosition.xMin && Position.x < parentPosition.xMax) {
            return Mathf.Min(Mathf.Abs(Position.y-parentPosition.yMin),
                             Mathf.Abs(Position.y-parentPosition.yMax));
        }
        if(Position.y > parentPosition.yMin && Position.y < parentPosition.yMax) {
            return Mathf.Min(Mathf.Abs(Position.x-parentPosition.xMin),
                             Mathf.Abs(Position.x-parentPosition.xMax));
        }
        float distance= Vector2.Distance(Position, parentNode.GetTopLeftCorner());
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetTopRightCorner()));
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetBottomLeftCorner()));
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetBottomRightCorner()));
        return distance;
    }
    
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent() {
        if(Parent == null) return false;
        return GetDistanceFromParent() <= AP_EditorConfig.PortSize*2;
    }
    
	// ----------------------------------------------------------------------
    public AP_Port GetOverlappingPort() {
        AP_Port foundPort= null;
        Top.ForEachRecursive<AP_Port>(
            (port)=> {
                if(port != this) {
                    float distance= Vector2.Distance(port.Position, Position);
                    if(distance <= 1.5*AP_EditorConfig.PortSize) {
                        foundPort= port;
                    }
                }
            }
        );
        return foundPort;
    }	

    // ----------------------------------------------------------------------
    public Vector2  Position {
        get {
            AP_Node parentNode= Parent as AP_Node;
            return new Vector2(parentNode.Position.x, parentNode.Position.y) + LocalPosition;
        }
    }
//#endif
}
