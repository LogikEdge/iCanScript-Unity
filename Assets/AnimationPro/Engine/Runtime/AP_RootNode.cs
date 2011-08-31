using UnityEngine;
using System.Collections;

public class AP_RootNode : AP_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_Graph Graph;
    public AP_Top   UpdateTop= null;
    public AP_Top   LateUpdateTop= null;
    public AP_Top   FixedUpdateTop= null;
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_RootNode CreateInstance(string _name, AP_Graph graph) {
        return CreateInstance<AP_RootNode>().Init(_name, graph);
    }
    // ----------------------------------------------------------------------
    AP_RootNode Init(string _name, AP_Graph graph) {
        // This is a special case.  We need to initialize our attributes
        // since the base class expects Top to always be configured.
        Top= null;
        Graph= graph;
        // Initialize the base classes.
        base.Init(_name, null);
        // Override the base class initialization since we don't want the RootNode
        // to show in the graph.
        IsVisible= false;
        return this;
    }

    // ======================================================================
    // GRAPH UPDATES
    // ----------------------------------------------------------------------
    // Called on every frame.
    public void Update() {
        if(UpdateTop) {
            UpdateTop.Execute();                        
        }
    }
    // Called on evry frame after all Update have been called.
    public void LateUpdate() {
        if(LateUpdateTop) {
            LateUpdateTop.Execute();
        }
    }
    // Fix-time update to be used instead of Update
    public void FixedUpdate() {
        if(FixedUpdateTop) {
            FixedUpdateTop.Execute();
        }
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(AP_Object _object) {
        // Child type validation.
        AP_Top top= _object as AP_Top;
        if(!top) {
            Debug.LogError("Only Top can be added on the RootNode.");
            return;
        }
        if(top.Name == "Update") {
            UpdateTop= top;
            base.AddChild(_object); 
        }
        else if(_object.Name == "LateUpdate") {
            LateUpdateTop= top;
            base.AddChild(_object);
        }
        else if(_object.Name == "FixedUpdate") {
            FixedUpdateTop= top;
            base.AddChild(_object);
        }
        else {
            Debug.Log("Only objects with names Update, LateUpdate, and FixedUpdate can be added to a RootNode.");
        }
    }
    public override void RemoveChild(AP_Object _object) {
        // Child type validation.
        AP_Top top= _object as AP_Top;
        if(!top) {
            Debug.LogError("Only Top can be removed from the RootNode.");
            return;
        }
        if(top == UpdateTop) {
            UpdateTop= null;
            base.RemoveChild(_object); 
        }
        else if(top == LateUpdateTop) {
            LateUpdateTop= null;
            base.RemoveChild(_object);
        }
        else if(top == FixedUpdateTop) {
            FixedUpdateTop= null;
            base.RemoveChild(_object);
        }
        else {
            Debug.Log("Only objects with names Update, LateUpdate, and FixedUpdate can be added to a RootNode.");
        }
    }

    // ======================================================================
    // OBJECT PICKING
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public AP_Node GetNodeAt(Vector2 _position) {
        AP_Node foundNode= this;
        ForEachRecursiveDepthLast<AP_Node>(
            (node)=> {
                if(node.IsInside(_position)) {
                    if(foundNode == null) {
                        foundNode= node;
                    }
                        else {
                        if(foundNode.IsParentOf(node)) {
                            foundNode= node;
                        }
                    }
                }                
            }
        );
        return foundNode;
    }
    
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public AP_Port GetPortAt(Vector2 _position) {
        AP_Port bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        ForEachRecursive<AP_Port>(
            (port)=> {
                float distance= Vector2.Distance(port.Position, _position);
                if(distance < 1.5f * AP_EditorConfig.PortRadius && distance < bestDistance) {
                    bestDistance= distance;
                    bestPort= port;
                }
            }
        );
        return bestPort;
    }
}
