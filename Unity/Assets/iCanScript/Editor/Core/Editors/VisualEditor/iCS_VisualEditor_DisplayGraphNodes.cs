using UnityEngine;
using System.Collections;

/*
    FIXME: Cleanup conditional tree descent VS full tree descent when drawing graph.
*/
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields.
    // ----------------------------------------------------------------------
    Prelude.Animate<Vector2>    myAnimatedScrollPosition= new Prelude.Animate<Vector2>();
    Prelude.Animate<float>      myAnimatedScale         = new Prelude.Animate<float>();


    // ======================================================================
    // Properties.
	// ----------------------------------------------------------------------
    Vector2     ScrollPosition {
        get { return IStorage != null ? IStorage.ScrollPosition : Vector2.zero; }
        set { if(IStorage != null) IStorage.ScrollPosition= value; }
    }
    float       Scale {
        get { return IStorage != null ? IStorage.GuiScale : 1.0f; }
        set {
            if(value > 2f) value= 2f;
            if(value < 0.15f) value= 0.15f;
            if(IStorage != null) IStorage.GuiScale= value;
        }
    }
    
    
    // ======================================================================
    // Node graph display.
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        IStorage.Update();

		// Start graphics
        myGraphics.Begin(UpdateScrollPosition(), UpdateScale(), ClipingArea, SelectedObject, GraphMousePosition);
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw nodes and their connections.
        DisplayGraphNodes();

        myGraphics.End(IStorage);

        // Show scroll zone (is applicable).
        if(IsDragStarted) DrawScrollZone();

		// Show toolbar
		Toolbar();			
	}
	
	// ----------------------------------------------------------------------
    void DrawGrid() {
        myGraphics.DrawGrid(position,
                            iCS_PreferencesEditor.CanvasBackgroundColor,
                            iCS_PreferencesEditor.GridColor,
                            iCS_PreferencesEditor.GridSpacing);
    }                       
    
	// ----------------------------------------------------------------------
    void DisplayGraphNodes() {
        var floatingNormalNode= DisplayNonFloatingNormalNode(myDisplayRoot);
        DisplayConnections(myDisplayRoot);
        DisplayPortsAndMinimizedNodes(myDisplayRoot);
        // Display floating nodes.
        if(floatingNormalNode != null) {
            DisplayFloatingNormalNode(floatingNormalNode);            
        }
    }
    
    // ======================================================================
    // Normal nodes
	// ----------------------------------------------------------------------
    iCS_EditorObject DisplayNonFloatingNormalNode(iCS_EditorObject rootNode, iCS_EditorObject floatingRootNode= null) {
        IStorage.ForEachRecursiveDepthLast(rootNode,
            node=> {
                if(node.IsNode) {
                    if(node.IsFloating && floatingRootNode == null) {
                        floatingRootNode= node;
                    } else {
                        myGraphics.DrawNormalNode(node, IStorage);
                    }
                }
            }
        );
        return floatingRootNode;
    }
	// ----------------------------------------------------------------------
    void DisplayFloatingNormalNode(iCS_EditorObject rootNode) {
        IStorage.ForEachRecursiveDepthLast(rootNode,
            child=> {
                if(child.IsNode) myGraphics.DrawNormalNode(child, IStorage);
                if(child.IsPort) myGraphics.DrawPort(child, IStorage);
            }
        );
    }

    // ======================================================================
    // Connections
	// ----------------------------------------------------------------------
    void DisplayConnections(iCS_EditorObject rootNode) {
        IStorage.ForEachRecursiveDepthLast(rootNode,
            child=> { if(child.IsPort) myGraphics.DrawConnection(child, IStorage); }
        );
    }

    // ======================================================================
    // Ports & Minimized Nodes
	// ----------------------------------------------------------------------
    void DisplayPortsAndMinimizedNodes(iCS_EditorObject rootNode) {
        iCS_EditorObject floatingRootNode= null;
        IStorage.ForEachRecursiveDepthLast(rootNode,
            child=> {
                if(child.IsPort) {
                    myGraphics.DrawPort(child, IStorage);
                }
                if(child.IsNode) {
                    if(child.IsFloating && floatingRootNode == null) {
                        floatingRootNode= child;
                    } else {
                        myGraphics.DrawMinimizedNode(child, IStorage);
                    }
                }
            }
        );
//        IStorage.ForEachChild(rootNode,
//            child=> {
//                if(child.IsPort) {
//                    myGraphics.DrawPort(child, IStorage);
//                }
//                if(child.IsNode) {
//                    if(IStorage.IsVisible(child)) {
//                        if(child.IsIconized) {
//                            if(child.IsFloating && floatingRootNode == null) floatingRootNode= child;
//                            myGraphics.DrawMinimizedNode(child, IStorage);
//                        }
//                        DisplayPortsAndMinimizedNodes(child);
//                    }                        
//                }
//            }
//        );
        if(floatingRootNode != null) {
            myGraphics.DrawMinimizedNode(floatingRootNode, IStorage);
        }        
    }

	// ======================================================================
	// Graph animation processing
	// ----------------------------------------------------------------------
	Vector2 UpdateScrollPosition() {
        Vector2 graphicScrollPosition= ScrollPosition;
        if(myAnimatedScrollPosition.IsActive) {
            myAnimatedScrollPosition.Update();
            graphicScrollPosition= myAnimatedScrollPosition.CurrentValue;
        }
		return graphicScrollPosition;
	}
	// ----------------------------------------------------------------------
    float UpdateScale() {
        float scale= Scale;
        if(myAnimatedScale.IsActive) {
            myAnimatedScale.Update();
            scale= myAnimatedScale.CurrentValue;
        }
        return scale;
    }
}
