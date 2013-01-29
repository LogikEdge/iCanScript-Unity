using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  LAYOUT UTILITIES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
//   /*
//       TODO: Is "InitialGlobalPosition" valid for nodes ?
//   */
//   // Initializes the global position and saves the ratio =================
//   public Vector2 InitialGlobalPosition {
//   	set {
//   		GlobalPosition= value;
//   		SavePosition();
//   		AnimatedPosition.Reset(GlobalRect);
//   	}
//   }
//
//   
//   // ----------------------------------------------------------------------
//   static Vector2 ComputeNodeRatio(Rect r, Vector2 p) {
//       if(Math3D.IsZero(r.width) || Math3D.IsZero(r.height)) {
//           Debug.LogWarning("iCanScript: Invalid area in which to compute node ratio");
//           return new Vector2(0.5f, 0.5f);
//       }
//       var x= (p.x-r.x)/r.width;
//       var y= (p.y-r.y)/r.height;
//       return new Vector2(x,y);
//   }
//   // ----------------------------------------------------------------------
//   static Vector2 ComputeNodePositionFromRatio(Rect rect, Vector2 ratio) {
//       var x= rect.x+ratio.x*rect.width;
//       var y= rect.y+ratio.y*rect.height;
//       return new Vector2(x,y);
//   }
}