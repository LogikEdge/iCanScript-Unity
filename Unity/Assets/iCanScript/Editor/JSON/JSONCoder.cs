using UnityEngine;
using System;
using System.Collections;

public class JSONCoder {
    // =================================================================================
    // ---------------------------------------------------------------------------------
    public JSONCoder() {}


    // =================================================================================
    // Encoding functions
    // ---------------------------------------------------------------------------------
//    public JObject Encode(string key, System.Object value) {
//        // Return JNull for null values.
//        if(value == null) {
//            return new JObject(new JNameValuePair(key, new JNull()));            
//        }
//        // Encode arrays
//		Type valueType= value.GetType();
//		if(valueType.IsArray) {
//            var result= new List<System.Object>();
//            Array valueAsArray= value as Array;
//            foreach(var v in valueAsArray) {
//                result.Add(new JValue(Encode))
//            }
//            EncodeArrayOfObjects(key, value, storage);
//            return;
//		}
//		// Special case for enums.
//		iCS_Coder coder= new iCS_Coder();
//		if(valueType.IsEnum) {
//			coder.EncodeInt("Numeric", (int)value);
//			coder.EncodeString("Name", value.ToString());
//            Add(key, valueType, coder.Archive);
//			return;
//        }
//		// Primitives.
//        if(value is Type)               { EncodeType(key, (Type)value); return; }
//		if(value is byte)               { EncodeByte(key, (byte)value); return; }
//		if(value is sbyte)              { EncodeSByte(key, (sbyte)value); return; }
//		if(value is char)               { EncodeChar(key, (char)value); return; }
//		if(value is string)             { EncodeString(key, (string)value); return; }
//		if(value is bool)               { EncodeBool(key, (bool)value); return; }
//		if(value is int)                { EncodeInt(key, (int)value); return; }
//		if(value is uint)               { EncodeUInt(key, (uint)value); return; }
//		if(value is short)              { EncodeShort(key, (short)value); return; }
//		if(value is ushort)             { EncodeUShort(key, (ushort)value); return; }
//		if(value is long)               { EncodeLong(key, (long)value); return; }
//		if(value is ulong)              { EncodeULong(key, (ulong)value); return; }
//		if(value is float)              { EncodeFloat(key, (float)value); return; }
//		if(value is double)             { EncodeDouble(key, (double)value); return; }
//		if(value is decimal)            { EncodeDecimal(key, (decimal)value); return; }
//		if(value is Vector2)            { EncodeVector2(key, (Vector2)value); return; }
//		if(value is Vector3)            { EncodeVector3(key, (Vector3)value); return; }
//		if(value is Vector4)            { EncodeVector4(key, (Vector4)value); return; }
//		if(value is Color)              { EncodeColor(key, (Color)value); return; }
//		if(value is Quaternion)         { EncodeQuaternion(key, (Quaternion)value); return; }
//		if(value is Matrix4x4)          { EncodeMatrix4x4(key, (Matrix4x4)value); return; }
//        // Special case for Unity Object inside a storage.
//		if(value is UnityEngine.Object && storage != null) {
//            EncodeUnityObject(key, value as UnityEngine.Object, storage); return;
//        }
//		// All other types.
//		foreach(var field in valueType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
//            bool shouldEncode= true;
//            if(field.IsPublic) {
//                foreach(var attribute in field.GetCustomAttributes(true)) {
//                    if(attribute is System.NonSerializedAttribute) shouldEncode= false;
//                }
//            } else {
//                shouldEncode= false;
//                foreach(var attribute in field.GetCustomAttributes(true)) {
//                    if(attribute is SerializeField) shouldEncode= true;
//                }                
//            }
//            if(shouldEncode) {
//    			coder.EncodeObject(field.Name, field.GetValue(value), storage);                                
//            }
//		}
//		Add(key, valueType, coder.Archive);
//    }
}
