using UnityEngine;
using System.Collections;

/*
    FIXME: Resolve difference between visual minimized and graph minimize and the likes.
*/
public partial class iCS_GraphEditor : iCS_EditorBase {

	// ----------------------------------------------------------------------
    void DisplayGraphNodes() {
        DisplayNormalNodes();
        DisplayConnections(myDisplayRoot);
        DisplayPortsAndMinimizedNodes(myDisplayRoot);
    }
    
    // ======================================================================
    // Normal nodes
	// ----------------------------------------------------------------------
    void DisplayNormalNodes() {
        // Display non floating nodes first.
        var floatingNodeRoot= DrawNonFloatingNormalNode(myDisplayRoot);
        // Display floating nodes.
        if(floatingNodeRoot != null) {
            DrawNonFloatingNormalNode(floatingNodeRoot);            
        }
    }	
	// ----------------------------------------------------------------------
    iCS_EditorObject DrawNonFloatingNormalNode(iCS_EditorObject rootNode, iCS_EditorObject floatingRootNode= null) {
        IStorage.ForEachChildNode(rootNode,
            node=> {
                if(!node.IsMinimized) {
                    if(node.IsFloating && floatingRootNode == null) {
                        floatingRootNode= node;
                    } else {
                        myGraphics.DrawNormalNode(node, IStorage);
                        if(node.IsMaximized) {
                            floatingRootNode= DrawNonFloatingNormalNode(node, floatingRootNode);
                        }                                            
                    }
                }
            }
        );
        return floatingRootNode;
    }
	// ----------------------------------------------------------------------
    void DrawFloatingNormalNode(iCS_EditorObject rootNode) {
        IStorage.ForEachChildNode(rootNode,
            node=> {
                if(!node.IsMinimized) {
                    if(node.IsFloating) {
                        myGraphics.DrawNormalNode(node, IStorage);
                        if(node.IsMaximized) {
                            DrawNonFloatingNormalNode(node);
                        }
                    }
                }
            }
        );
    }

    // ======================================================================
    // Connections
	// ----------------------------------------------------------------------
    void DisplayConnections(iCS_EditorObject rootNode) {
        IStorage.ForEachChild(rootNode,
            child=> {
                if(child.IsPort && IStorage.GetSource(child) != null) {
                    myGraphics.DrawConnection(child, IStorage);
                }
                if(child.IsNode && IStorage.IsVisible(child)) {
                    DisplayConnections(child);
                }                    
            }
        );
    }

    // ======================================================================
    // Ports & Minimized Nodes
	// ----------------------------------------------------------------------
    void DisplayPortsAndMinimizedNodes(iCS_EditorObject rootNode) {
        iCS_EditorObject floatingRootNode= null;
        IStorage.ForEachChild(rootNode,
            child=> {
                if(child.IsPort) {
                    myGraphics.DrawPort(child, IStorage);
                }
                if(child.IsNode) {
                    if(IStorage.IsVisible(child)) {
                        if(child.IsMinimized) {
                            if(child.IsFloating && floatingRootNode == null) floatingRootNode= child;
                            myGraphics.DrawMinimizedNode(child, IStorage);
                        }
                        DisplayPortsAndMinimizedNodes(child);
                    }                        
                }
            }
        );
        if(floatingRootNode != null) {
            myGraphics.DrawMinimizedNode(floatingRootNode, IStorage);
        }        
    }
}
