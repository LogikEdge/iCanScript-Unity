using UnityEngine;
using System.Collections;
using Pref=iCS_PreferencesController;
using System;

/*
    TODO: Cleanup conditional tree descent VS full tree descent when drawing graph.
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
        set {
            if(IStorage != null) {
                IStorage.ScrollPosition= value;
            }
        }
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
        myGraphics.Begin(UpdateScrollPosition(), UpdateScale(), VisibleGraphRect, SelectedObject, GraphMousePosition);

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
		// No grid if editor snapshot without background requested.
		if(iCS_DevToolsConfig.IsFrameWithoutBackground) return;
		// Draw Grid
        myGraphics.DrawGrid(position, GridOffset,
                            Pref.CanvasBackgroundColor,
                            Pref.GridColor,
                            Pref.GridSpacing);
    }                       
    
	// ----------------------------------------------------------------------
    void DisplayGraphNodes() {
        var floatingNormalNode= DisplayNonFloatingNormalNode(DisplayRoot);
        DisplayConnections(DisplayRoot);
        DisplayPortsAndMinimizedNodes(DisplayRoot);
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
					if(node.IsBehaviour || node.IsHidden) return;
					if(node == rootNode && !IStorage.ShowDisplayRootNode) return;
                    if(node.IsFloating && floatingRootNode == null) {
                        floatingRootNode= node;
                    } else {
						if( !node.IsParentFloating ) {
                            if(node == rootNode) {
                            }
							PopulateHelp(node);
	                        myGraphics.DrawNormalNode(node, IStorage);							
						}
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
                if(child.IsNode) {
                    if(child.IsHidden) return;
					if( child.IsIconizedInLayout ) {
						PopulateHelp(child);
						myGraphics.DrawMinimizedNode(child, IStorage);						
					}
					else {
						myGraphics.DrawNormalNode(child, IStorage);
					}
				}
                if(child.IsPort) {
					PopulateHelp(child);
					myGraphics.DrawPort(child, IStorage);
					myGraphics.DrawBinding(child, IStorage);
				}
            }
        );
    }

    // ======================================================================
    // Poplulates the help string which will be displayed on on GUI when 
	// a node/port is floated over.
	// ----------------------------------------------------------------------
	void PopulateHelp(iCS_EditorObject node) {
		Rect position= node.AnimatedRect;
		bool isMouseOver= position.Contains(GraphMousePosition);
		if(isMouseOver) {
			myHelpText="";
			// Polpulate help if pointed object is a port
			if(node.IsPort) {
				// Get consumer side port, memberInfo, and tooltip
				iCS_EditorObject consumerNode= iCS_LibraryDatabase.GetAssociatedConsumerPortNode(node);
				iCS_MemberInfo consumerMemberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(consumerNode);
				string consumerTooltip= consumerMemberInfo != null ? consumerMemberInfo.Summary : null;
				
				// Get producer side port, memberInfo, and tooltip
				iCS_EditorObject producerNode= iCS_LibraryDatabase.GetAssociatedProducerPortNode(node);
				iCS_MemberInfo producerMemberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(producerNode);
				string producerTooltip= producerMemberInfo != null ? producerMemberInfo.Summary : null;
				
				// Display Port info for both consumer and producer side, in correct order 
				// pointed to first, connected node (if any) second.
				if(node.IsInputPort) {
					if(!String.IsNullOrEmpty(consumerTooltip)) {
						myHelpText= myHelpText + consumerNode.DisplayName + "\n";
						myHelpText= myHelpText + consumerTooltip + "\n\n";
					}
					if(!String.IsNullOrEmpty(producerTooltip) && consumerNode != producerNode) {
						myHelpText= myHelpText + "connected -> " + producerNode.DisplayName + "\n";
						myHelpText= myHelpText + producerTooltip;
					}
				}
				else if (node.IsOutputPort) {
					if(!String.IsNullOrEmpty(producerTooltip)) {
						myHelpText= myHelpText + producerNode.DisplayName + "\n";
						myHelpText= myHelpText + producerTooltip + "\n\n";
					}
					if(!String.IsNullOrEmpty(consumerTooltip) && consumerNode != producerNode) {
						myHelpText= myHelpText + "connected -> " + consumerNode.DisplayName + "\n";
						myHelpText= myHelpText + consumerTooltip;	
					}				
				}
			}
			// Polpulate help if pointed object is a node
			else {
				iCS_MemberInfo memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(node);
				myHelpText= memberInfo != null ? memberInfo.Summary : null;
				if(String.IsNullOrEmpty(myHelpText)) {
					myHelpText= "no tip available";
				}
				else {
					myHelpText= node.DisplayName + "\n" + myHelpText;
				}
			}
		}
	}


    // ======================================================================
    // Connections
	// ----------------------------------------------------------------------
    void DisplayConnections(iCS_EditorObject rootNode) {
        IStorage.ForEachRecursiveDepthLast(rootNode,
            child=> {
				if(child.IsPort) {
					var parent= child.ParentNode;
                    var source= child.ProducerPort;
                    var srcParent= source != null ? source.ParentNode : null;
                    if(!IStorage.ShowDisplayRootNode) {
                        if(parent == rootNode) {
                            return;
                        }
                        if(source != null) {
                            if(srcParent == rootNode) {
                                return;                        
                            }
                        }
                    }
                    else {
                        if(srcParent != rootNode && !rootNode.IsParentOf(srcParent)) {
                            return; 
                        }
                    }
					if( !parent.IsParentFloating ) {
						myGraphics.DrawBinding(child, IStorage);						
					}
				}
			}
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
                    if(!IStorage.ShowDisplayRootNode && child.ParentNode == rootNode) return;
					PopulateHelp(child);
                    myGraphics.DrawPort(child, IStorage);
                }
                if(child.IsNode) {
                    if(child.IsFloating && floatingRootNode == null) {
                        floatingRootNode= child;
                    } else {
						PopulateHelp(child);
                        myGraphics.DrawMinimizedNode(child, IStorage);
                    }
                }
            }
        );
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
