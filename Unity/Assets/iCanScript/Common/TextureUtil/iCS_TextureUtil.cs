using UnityEngine;
using System.Collections;

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
	// ----------------------------------------------------------------------
    // Draw a circle in a texture.
    // TODO: Replace Circle by draw circle.
    public static void Circle(float radius,
                              Color borderColor, Color fillColor,
                              ref Texture2D texture, Vector2 center,
                              float borderWidth= 2f) {
        if(borderWidth < 2f) borderWidth= 2f;
		float outterRingRadius= radius+0.5f*borderWidth;
		float innerRingRadius= radius-0.5f*borderWidth;
		float outterRingRadius2= outterRingRadius*outterRingRadius;
		float innerRingRadius2= innerRingRadius*innerRingRadius;

		for(int x= 0; x < texture.width; ++x) {
			for(int y= 0; y < texture.height; ++y) {
				float rx= (float)x-center.x;
				float ry= (float)y-center.y;
				float r2= rx*rx+ry*ry;
				if(r2 > outterRingRadius2) {
					// Don't draw if outside corner.
				} else if(r2 > innerRingRadius2) {
					float r= Mathf.Sqrt(r2);
					if(r > radius) {
						float ratio= (borderWidth-2f*(r-radius))/borderWidth;
						Color c= borderColor;
						c.a= ratio*borderColor.a;
						c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));							
						texture.SetPixel(x,y,c);							
					} else {
						float ratio= (borderWidth-2f*(radius-r))/borderWidth;
						Color c;
						c.r= ratio*borderColor.r+(1f-ratio)*fillColor.r;
						c.g= ratio*borderColor.g+(1f-ratio)*fillColor.g;
						c.b= ratio*borderColor.b+(1f-ratio)*fillColor.b;
						c.a= ratio*borderColor.a+(1f-ratio)*fillColor.a;
						c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));														
						texture.SetPixel(x,y,c);						
					}
				} else {
					Color c= fillColor;
					c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));														
					texture.SetPixel(x,y,c);
				}
			}
		}
    }  
	// ----------------------------------------------------------------------
    // Draw a box in a texture.
    // TODO: Replace Box by DrawPolygon
    public static void Box(float x1, float y1, float x2, float y2,
                           Color borderColor, Color fillColor,
                           ref Texture2D texture,
                           float borderWidth= 2f) {
        if(x2 < x1) { var tmp= x1; x1= x2; x2= tmp; }
        if(y2 < y1) { var tmp= y1; y1= y2; y2= tmp; }
        float halfBorderWidth= 0.5f*borderWidth;
		float outterX1= x1-halfBorderWidth;
		float innerX1 = x1+halfBorderWidth;
		float outterX2= x2+halfBorderWidth;
		float innerX2 = x2-halfBorderWidth;
		float outterY1= y1-halfBorderWidth;
		float innerY1 = y1+halfBorderWidth;
		float outterY2= y2+halfBorderWidth;
		float innerY2 = y2-halfBorderWidth;

		for(int x= 0; x < texture.width; ++x) {
			for(int y= 0; y < texture.height; ++y) {
				if(x < outterX1 || x > outterX2 || y < outterY1 || y > outterY2) {
					// Don't draw if outside box.
				} else if(x > innerX1 && x < innerX2 && y > innerY1 && y < innerY2) {
					Color c= iCS_AlphaBlend.NormalBlend(fillColor, texture.GetPixel(x,y));														
					texture.SetPixel(x,y,c);				    
				}  else {
                    float distX1= Mathf.Abs(x-x1);
                    float distX2= Mathf.Abs(x-x2);
                    float distY1= Mathf.Abs(y-y1);
                    float distY2= Mathf.Abs(y-y2);
                    float distX= Mathf.Min(distX1, distX2);
                    float distY= Mathf.Min(distY1, distY2);
				    if(x > x1 || x < x2 ||y > y1 || y < y2) {
                        float dist= Mathf.Min(distX, distY);
						float ratio= (borderWidth-2f*(dist))/borderWidth;
						Color c;
						c.r= ratio*borderColor.r+(1f-ratio)*fillColor.r;
						c.g= ratio*borderColor.g+(1f-ratio)*fillColor.g;
						c.b= ratio*borderColor.b+(1f-ratio)*fillColor.b;
						c.a= ratio*borderColor.a+(1f-ratio)*fillColor.a;
						c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));														
						texture.SetPixel(x,y,c);						
			        } else {
			            float dist= (distX <= halfBorderWidth && distY <= halfBorderWidth) ?
			                            Mathf.Max(distX, distY) :
			                            Mathf.Min(distX, distY);
						float ratio= (borderWidth-2f*dist)/borderWidth;
						Color c= borderColor;
						c.a= ratio*borderColor.a;
						c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));							
						texture.SetPixel(x,y,c);							
                    }
				}
			}
		}
    }

}
