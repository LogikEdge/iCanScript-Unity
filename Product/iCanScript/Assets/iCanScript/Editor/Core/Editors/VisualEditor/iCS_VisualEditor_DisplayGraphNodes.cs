//#define WORKFLOW_ASSISTANT
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

/*
    TODO: Cleanup conditional tree descent VS full tree descent when drawing graph.
*/
namespace iCanScript.Internal.Editor {
    using Pref= PreferencesController;

    public partial class iCS_VisualEditor : iCS_EditorBase {
        // ======================================================================
        // Fields.
        // ----------------------------------------------------------------------
        P.Animate<Vector2>    myAnimatedScrollPosition= new P.Animate<Vector2>(TimerService.EditorTime);
        P.Animate<float>      myAnimatedScale         = new P.Animate<float>(TimerService.EditorTime);
    
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
            
            // -- Show workflow assistant --
    #if WORKFLOW_ASSISTANT
            ShowWorkflowAssistant();
    #endif
            HotZoneGUI();
            
            // Draw nodes and their connections.
            DisplayGraphNodes();
    
            myGraphics.End(IStorage);
    
            // Show scroll zone (is applicable).
            if(IsDragStarted) DrawScrollZone();
    
    		// Show toolbar
    		Toolbar();
            
            // Show errors/warnings
            DisplayErrorsAndWarnings();
    	}
    	
    	// ----------------------------------------------------------------------
        void DrawGrid() {
    		// No grid if editor snapshot without background requested.
    		if(iCS_DevToolsConfig.IsFrameWithoutBackground) return;
    		// Draw Grid
            myGraphics.DrawGrid(position, GridOffset);
        }                       
        
    	// ----------------------------------------------------------------------
        void DisplayGraphNodes() {
            var floatingNodes= DisplayNonFloatingNormalNode(DisplayRoot);
            DisplayConnections(DisplayRoot);
            DisplayPortsAndMinimizedNodes(DisplayRoot);
            // Display floating nodes.
            DisplayFloatingNormalNodes(floatingNodes);            
        }
        
        // ======================================================================
        // Normal nodes
    	// ----------------------------------------------------------------------
        iCS_EditorObject[] DisplayNonFloatingNormalNode(iCS_EditorObject rootNode, iCS_EditorObject floatingRootNode= null) {
            var floatingNodes= new List<iCS_EditorObject>();
            IStorage.ForEachRecursiveDepthLast(rootNode,
                node=> {
                    if(node.IsNode) {
    					if(node.IsBehaviour || node.IsHidden) return;
    					if(node == rootNode && !IStorage.ShowDisplayRootNode) return;
                        if(node == rootNode) {}
                        if( !node.IsParentFloating ) {
                            if(node.IsFloating) {
                                floatingNodes.Add(node);
                            } else {
    	                        myGraphics.DrawNormalNode(node, IStorage);							
                            }                        
                        }
                    }
                }
            );
            return floatingNodes.ToArray();
        }
    	// ----------------------------------------------------------------------
        void DisplayFloatingNormalNodes(iCS_EditorObject[] floatingNodes) {
            foreach(var rootNode in floatingNodes) {
                IStorage.ForEachRecursiveDepthLast(rootNode,
                    child=> {
                        if(child.IsNode) {
                            if(child.IsHidden) return;
        					if( child.IsIconizedInLayout ) {
        						myGraphics.DrawMinimizedNode(child, IStorage);						
        					}
        					else {
        						myGraphics.DrawNormalNode(child, IStorage);
        					}
        				}
                        if(child.IsPort) {
        					myGraphics.DrawPort(child, IStorage);
        					myGraphics.DrawBinding(child, IStorage);
        				}
                    }
                );            
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
}
