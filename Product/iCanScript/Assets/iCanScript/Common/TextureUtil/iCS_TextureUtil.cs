using UnityEngine;
using System.Collections;

namespace iCanScript.Internal {
    
    public static class iCS_TextureUtil {
        // ======================================================================
        // Field & Properties.
    	// ----------------------------------------------------------------------
        public static int AntiAliasFactor= 3;
    
        // ======================================================================
        // Texture fill.
    	// ----------------------------------------------------------------------
        public static void Fill(ref Texture2D texture, Color color) {
            for(int x= 0; x < texture.width; ++x) {
                for(int y= 0; y < texture.height; ++y) {
                    texture.SetPixel(x,y,color);
                }
            }
        }
    	// ----------------------------------------------------------------------
        // Clears the given texture.
        public static void Clear(ref Texture2D texture) {
            Fill(ref texture, Color.clear);
        }
    	// ----------------------------------------------------------------------
        public static void DrawCircle(ref Texture2D texture, float radius, Vector2 center, Color color, float width= 1f) {
            width= width > 1f ? width-1f : 0f;
            float maxRadius= radius+width;
            float minRadius= radius-width;
            float aaMaxRadius= maxRadius+1f;
            float aaMinRadius= minRadius-1f;

            float aaMaxRadius2= aaMaxRadius*aaMaxRadius;
            float maxRadius2= maxRadius*maxRadius;
            float minRadius2= minRadius*minRadius;
            float aaMinRadius2= aaMinRadius*aaMinRadius;
    		for(int x= 0; x < texture.width; ++x) {
    			for(int y= 0; y < texture.height; ++y) {
    				float rx= (float)x;
    				float ry= (float)y;
    				float ci= rx-center.x;
    				float cj= ry-center.y;
    				float r2= ci*ci+cj*cj;
    				if(r2 > aaMaxRadius2) {
    					// we are outside the circle.
    				} else if(r2 > maxRadius2) {
    					float r= Mathf.Sqrt(r2);
        				float ratio= (aaMaxRadius-r);
        				Color c= color;
        				c.a= ratio;
                        iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
    				} else if(r2 > minRadius2) {
    					texture.SetPixel(x,y,color);
    				} else if(r2 > aaMinRadius2) {
    					float r= Mathf.Sqrt(r2);
    					float ratio= (r-aaMinRadius);
    					Color c= color;
    					c.a= ratio;
                        iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
    				}
    			}
    		}		
        }
    	// ----------------------------------------------------------------------
    	public static void DrawLine(ref Texture2D texture, Vector2 p1, Vector2 p2, Color color, float width= 1f) {
            width= width > 1f ? width-1f : 0f;
            var halfWidth= 0.5f*width;
            for(int x= 0; x < texture.width; ++x) {
                for(int y= 0; y < texture.height; ++y) {
                    var point= new Vector2(x,y);
                    Vector2 closestPoint= Math3D.ClosestPointOnLineSegmentToPoint(p1, p2, point);
                    var delta= point-closestPoint; 
                    var distance= delta.magnitude;
                    if(distance < halfWidth) {
                        texture.SetPixel(x,y,color);                    
                    } else {
                        distance-= halfWidth;
                        if(distance < 1f) {
                            Color c= color;
                            c.a= 1f-distance;                        
                            iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
                        }
                    }
                }
            }        	    
    	}
    	// ----------------------------------------------------------------------
        public static void DrawPolygonOutline(ref Texture2D texture, Vector2[] polygon, Color color, float width= 1f) {
            width= width > 1f ? width-1f : 0f;
            var halfWidth= 0.5f*width;
            for(int x= 0; x < texture.width; ++x) {
                for(int y= 0; y < texture.height; ++y) {
                    var point= new Vector2(x,y);
                    var closestPoint= Math3D.ClosestPointOnPolygonToPoint(polygon, point);
                    var delta= point-closestPoint; 
                    var distance= delta.magnitude;
                    if(distance < halfWidth) {
                        texture.SetPixel(x,y,color);                    
                    } else {
                        distance-= halfWidth;
                        if(distance < 1f) {
                            Color c= color;
                            c.a= 1f-distance;                        
                            iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
                        }
                    }
                }
            }        
        }
    	// ----------------------------------------------------------------------
        public static void DrawFilledCircle(ref Texture2D texture, float radius, Vector2 center, Color color) {
            float aaRadius= radius+1f;
            float aaRadius2= aaRadius*aaRadius;
            float radius2= radius*radius;
    		for(int x= 0; x < texture.width; ++x) {
    			for(int y= 0; y < texture.height; ++y) {
    				float rx= (float)x;
    				float ry= (float)y;
    				float ci= rx-center.x;
    				float cj= ry-center.y;
    				float r2= ci*ci+cj*cj;
    				if(r2 > aaRadius2) {
    					// we are outside the circle.
    				} else if(r2 > radius2) {
    					float r= Mathf.Sqrt(r2);
        				float ratio= (aaRadius-r);
        				Color c= color;
        				c.a= ratio;
                        iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
        			} else {
    					texture.SetPixel(x,y,color);
    				}
    			}
    		}		        
        }
    	// ----------------------------------------------------------------------
        public static void DrawFilledPolygon(ref Texture2D texture, Vector2[] polygon, Color color) {
            var center= Math3D.Average(polygon);
            for(int x= 0; x < texture.width; ++x) {
                for(int y= 0; y < texture.height; ++y) {
                    var point= new Vector2(x,y);
                    var closestPoint= Math3D.ClosestPointOnPolygonToPoint(polygon, point);
                    var delta= point-closestPoint; 
                    if(Vector2.Dot(delta, center-closestPoint) > 0) {
                        texture.SetPixel(x,y,color);
                    } else {
                        var distance= (delta).magnitude;
                        if(distance < 1f) {
                            Color c= color;
                            c.a= 1f-distance;
                            iCS_AlphaBlend.NormalBlend(ref texture, x, y, c);
                        }
                    }
                }
            }
        }
    }

}
