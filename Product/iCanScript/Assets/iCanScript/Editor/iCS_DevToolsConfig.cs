using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_DevToolsConfig {
    	// =========================================================================
        // Snapshot definitions
    	public const string ScreenShotsFolder= "/../../../ScreenShots";
    	public static bool TakeVisualEditorSnapshot = false;
    	public static bool SnapshotWithoutBackground= false;
        public static int  SnapshotCountDown        = -1;
        public static bool UseBackgroundImage       = false;

        // ======================================================================
        // DEVTOOLS FIELDS
        // ----------------------------------------------------------------------
        public static bool ShowAssetStoreBigImageFrame  = false;
        public static bool ShowAssetStoreSmallImageFrame= false;
        public static bool ShowBoldImage                = false;
    
        // ======================================================================
        // DEVTOOLS PROPERTIES
        // ----------------------------------------------------------------------
        public static bool IsSnapshotActive {
            get {
                return TakeVisualEditorSnapshot;
            }
        }
        public static bool IsAssetStoreFrameActive {
            get {
                return ShowAssetStoreSmallImageFrame || ShowAssetStoreBigImageFrame;
            }
        }
        public static bool IsSnapshotWithoutBackground {
            get {
                return SnapshotWithoutBackground;
            }
        }
        public static bool IsFrameWithoutBackground {
            get {
                return IsSnapshotActive && IsSnapshotWithoutBackground;
            }
        }
    
        // ======================================================================
        // Utilities
        // ----------------------------------------------------------------------
        public static Rect GetAssetStoreBigImageRect(Vector2 size, out Rect liveRect) {
            const float kBigWidth= 860f;
            const float kBigHeight= 389f;
            const float kBigLiveWidth= 550f;
            const float kBigLiveHeight= 330f;
            var center= new Vector2(0.5f*size.x, 0.5f*size.y);

            float width= kBigWidth;
            float height= kBigHeight;
            float liveWidth= kBigLiveWidth;
            float liveHeight= kBigLiveHeight;
            float padding= 0.5f*(height-liveHeight);
    //        float padding= 0;
        
            float halfWidth= 0.5f*width;
            float halfHeight= 0.5f*height;
            Rect r= new Rect(center.x-halfWidth, center.y-halfHeight, width, height);
            // Live area
            liveRect= new Rect(r.xMax-padding-liveWidth, r.y+padding, liveWidth, liveHeight);
            return r;
        }
        // ----------------------------------------------------------------------
        public static Rect GetAssetStoreSmallImageRect(Vector2 size, out Rect liveRect) {
            const float kSmallWidth= 200f;
            const float kSmallHeight= 258f;
            const float kSmallLiveWidth= 175f;
            const float kSmallLiveHeight= 100f;
            var center= new Vector2(0.5f*size.x, 0.5f*size.y);

            float width= kSmallWidth;
            float height= kSmallHeight;
            float liveWidth= kSmallLiveWidth;
            float liveHeight= kSmallLiveHeight;
            float padding= 0.5f*(width-liveWidth);
    
            float halfWidth= 0.5f*width;
            float halfHeight= 0.5f*height;
            Rect r= new Rect(center.x-halfWidth, center.y-halfHeight, width, height);
            // Live area
            liveRect= new Rect(r.xMax-padding-liveWidth, r.y+padding, liveWidth, liveHeight);
            return r;
        }
    }
}
