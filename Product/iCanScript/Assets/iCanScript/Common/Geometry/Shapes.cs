using UnityEngine;
using System.Collections;

namespace iCanScript.Internal {

	public static class Shapes {
		// ========================================================================
		/// Returns a square of the given width and height..
		public static Vector3[] Rectangle2D(float x, float y, float width, float height) {
			var shape= Square2D();
			Scale(new Vector3(width, height, 0), ref shape);
			Translate(new Vector3(x, y, 0), ref shape);
			return shape;
		}

		// ========================================================================
		/// Returns a unitary square.
		public static Vector3[] Square2D() {
			var shape= new Vector3[4];
			shape[0].x= 0f; shape[0].y= 0f; shape[0].z= 0f;
			shape[1].x= 1f; shape[1].y= 0f; shape[1].z= 0f;
			shape[2].x= 1f; shape[2].y= 1f; shape[2].z= 0f;
			shape[3].x= 0f; shape[3].y= 1f; shape[3].z= 0f;
			return shape;
		}

		// ========================================================================
		/// Translates a vector list.
		///
		/// @param translation The translation value.
		/// @param Reference to the shape to translate.
		///
		public static void Translate(Vector3 translation, ref Vector3[] shape) {
			var len= shape.Length;
			for(int i= 0; i < len; ++ i) {
				shape[i]+= translation;
			}
		}

		// ========================================================================
		/// Scale a vector list.
		///
		/// @param scale The scale value.
		/// @param Reference to the shape to scale.
		///
		public static void Scale(Vector3 scale, ref Vector3[] shape) {
			var len= shape.Length;
			for(int i= 0; i < len; ++ i) {
				shape[i]= Vector3.Scale(scale, shape[i]);
			}
		}

	}
	
}
