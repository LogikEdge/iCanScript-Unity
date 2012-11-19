using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // Storage accessors ====================================================
    public float PositionRatio {
        get {
            return EngineObject.LocalPosition.x;
        }
    }
    
    // Port Layout Utilities ================================================
    public void CleanupPortEdgePosition() {
        var size= Parent.DisplaySize;
        var lp= LocalPosition;
        switch(Edge) {
            case iCS_EdgeEnum.Top:      lp.y= -0.5f*size.y; break; 
            case iCS_EdgeEnum.Bottom:   lp.y=  0.5f*size.y; break;
            case iCS_EdgeEnum.Left:     lp.x= -0.5f*size.x; break;
            case iCS_EdgeEnum.Right:    lp.x=  0.5f*size.x; break;
        }
		LocalPosition= lp;
    }
    // ----------------------------------------------------------------------
    public bool IsPortOnParentEdge {
        get {
            if(!IsFloating) return true;
    		float maxDistance= 2f*iCS_Config.PortSize;
            float distance= 2f*maxDistance;
            var parentSize= Parent.DisplaySize;
            float leftX  = -0.5f*parentSize.x;
            float rightX =  0.5f*parentSize.x;
            float topY   = -0.5f*parentSize.y;
            float bottomY=  0.5f*parentSize.y;
            var edge= IsStatePort ? ClosestEdge : Edge;
            switch(edge) {
                case iCS_EdgeEnum.Top:
                    distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, topY);
                    break; 
                case iCS_EdgeEnum.Bottom:
                    distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, bottomY);
                    break;
                case iCS_EdgeEnum.Left:
                    distance= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, leftX);
                    break;
                case iCS_EdgeEnum.Right:
                    distance= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, rightX);
                    break;                
            }
            return distance <= maxDistance;
        }
    }
    // ----------------------------------------------------------------------
	public iCS_EdgeEnum ClosestEdge {
		get {
			// Don't change edge if parent is iconized.
			var parent= Parent;
			if(parent.IsIconized) return Edge;
            var parentSize= parent.DisplaySize;
            float leftX  = -0.5f*parentSize.x;
            float rightX =  0.5f*parentSize.x;
            float topY   = -0.5f*parentSize.y;
            float bottomY=  0.5f*parentSize.y;
			var edge= iCS_EdgeEnum.Top;
			float distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, topY);
			float d= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, bottomY);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Bottom;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, leftX);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Left;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, rightX); 
			if(d < distance) {
				edge= iCS_EdgeEnum.Right;
			}
			return edge;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfTopPorts {
		get {
			int cnt= 0;
			ForEachTopChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfBottomPorts {
		get {
			int cnt= 0;
			ForEachBottomChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfLeftPorts {
		get {
			int cnt= 0;
			ForEachLeftChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfRightPorts {
		get {
			int cnt= 0;
			ForEachRightChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] TopPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachTopChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] BottomPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachBottomChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] LeftPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachLeftChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] RightPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachRightChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	
	// ======================================================================
    // Layout from iCS_Port
    // ----------------------------------------------------------------------
    public void UpdatePortEdge() {
        // Enable ports are always on top of the node.
        if(IsEnablePort) {
            Edge= iCS_EdgeEnum.Top;
        }
        // Data ports are always on the left or right depending on input/output direction.
        else if(IsDataPort) {
            Edge= IsInputPort ? iCS_EdgeEnum.Left : iCS_EdgeEnum.Right;
        }
        // Selected closest edge.
        else {
            Edge= ClosestEdge;            
        }
    }
    // ----------------------------------------------------------------------
    public static void LayoutPortsOnVerticalEdge(iCS_EditorObject[] ports,
                                                 float top, float bottom, float x) {
        // Compute position according to position.
        int nbPorts= ports.Length;
        float height= bottom-top;
        float[] ys= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            ys[i]= height*ports[i].PositionRatio;
        }
        // Resolve position according to collisions.
        ResolvePortCollisions(ys, height);
    }
    // ----------------------------------------------------------------------
    // Resolves the port separartion on a given edge.
    public static float[] ResolvePortCollisions(float[] pos, float maxPos) {
        int nbPorts= pos.Length;
        bool collisionDetectionNeeded= true;
        for(int r= 0; r < nbPorts && collisionDetectionNeeded; ++r) {
            collisionDetectionNeeded= false;
            for(int i= 0; i < nbPorts-1; ++i) {
                int j= i+1;
                if(pos[i] < 0f) pos[i]= 0f;
                if(pos[j] > maxPos) pos[j]= maxPos;
                float separation= pos[j]-pos[i];
                if(separation < iCS_Config.MinimumPortSeparation) {
                    bool before= false;
                    bool after= false;
                    if(pos[i] > 0f) {
                        if(i == 0) {
                            before= true;
                        } else if((pos[i]-pos[i-1]) > iCS_Config.MinimumPortSeparation) {
                            before= true;
                        }
                    }
                    if(pos[j] < maxPos) {
                        if(j+1 == nbPorts) {
                            after= true;
                        } else if(pos[j+1]-pos[j] > iCS_Config.MinimumPortSeparation) {
                            after= true;
                        }
                    }
                    // Determine where to expand.
                    var overlap= iCS_Config.MinimumPortSeparation-separation;
                    if(before && after) {
                        pos[i]-= 0.5f*overlap;
                        pos[j]+= 0.5f*overlap;
                        collisionDetectionNeeded= true;
                    } else if(before) {
                        pos[i]-= overlap;
                        collisionDetectionNeeded= true;
                    } else if(after) {
                        pos[j]+= overlap;
                        collisionDetectionNeeded= true;
                    }
                }
            }
        }
        if(r >= nbPorts) {
            Debug.LogWarning("iCanScript: Difficulty stabilizing port layout !!!")
        }
        return pos;        
    }
}
