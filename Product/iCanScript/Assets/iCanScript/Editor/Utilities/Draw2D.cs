using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {

    public class Draw2D {
        // -------------------------------------------------------------------------
        /// Draws a filled box with the given color.
        ///
        /// @param r The rectangle coordinate.
        /// @param color The color to use.
        ///
        public static void DrawFilledBox(Rect r, Color color) {
            var polygone= new Vector3[5];
            polygone[0]= Math3D.TopLeftCorner(r);
            polygone[1]= Math3D.TopRightCorner(r);
            polygone[2]= Math3D.BottomRightCorner(r);
            polygone[3]= Math3D.BottomLeftCorner(r);
            polygone[4]= Math3D.TopLeftCorner(r);
            var savedColor= Handles.color;
            Handles.color= color;
            Handles.DrawAAConvexPolygon(polygone);
            Handles.color= savedColor;
        }

        // -------------------------------------------------------------------------
        /// Draws a filled rectangle with an outline.
        ///
        /// @param r The rectangle coordinate.
        /// @param fillColor The fill color
        /// @param outlineColor The color for the outline
        /// @param borderWidth The width of the border.
        ///
        public static void FilledBoxWithOutline(Rect r, Color fillcolor, Color outlineColor, int borderWidth= 1) {
            var outterRect= new Rect(r.x-borderWidth, r.y-borderWidth, r.width+2*borderWidth, r.height+2*borderWidth);
            DrawFilledBox(outterRect, outlineColor);
            DrawFilledBox(r, fillcolor);
        }

    }
    
}
