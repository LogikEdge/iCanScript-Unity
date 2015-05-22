using UnityEngine;
using System.Collections;

namespace CSharp.Primitives {
    
    public struct Bool {
//        public static bool op_Ctor()                                 { return default(bool); }
//        public static bool op_Ctor(bool value)                       { return value; }

//        public static bool op_Assignment(ref bool target, bool value){ return target= value;}

        public static bool op_LogicalNot(bool a)                     { return !a; }
        public static bool op_BitwiseAnd(bool a, bool b)             { return a & b; }
        public static bool op_BitwiseOr(bool a, bool b)              { return a | b; }
        public static bool op_ExclusiveOr(bool a, bool b)            { return a ^ b; }

//        public static bool op_BitwiseAndAssign(ref bool target, bool b)   { return target &= b; }
//        public static bool op_BitwiseOrAssign(ref bool target, bool b)    { return target |= b; }
//        public static bool op_ExclusiveOrAssign(ref bool target, bool b)  { return target ^= b; }

        public static bool op_Equality(bool a, bool b)               { return a == b; }
        public static bool op_Inequality(bool a, bool b)             { return a != b; }
    }

    public struct Int {
//        public static int  op_Ctor()                                    { return default(int); }
//        public static int  op_Ctor(int value)                           { return value; }
                                                                        
//        public static int  op_Assignment(ref int target, int value)     { return target= value;}
        public static int  op_UnaryNegation(int a)                      { return -a; }
                                                                        
        public static int  op_BitwiseAnd(int a, int b)                  { return a & b; }
        public static int  op_BitwiseOr(int a, int b)                   { return a | b; }
        public static int  op_ExclusiveOr(int a, int b)                 { return a ^ b; }

//        public static int  op_BitwiseAndAssign(ref int target, int b)   { return target &= b; }
//        public static int  op_BitwiseOrAssign(ref int target, int b)    { return target |= b; }
//        public static int  op_ExclusiveOrAssign(ref int target, int b)  { return target ^= b; }

        public static int op_Addition(int a, int b)                     { return a + b; }
        public static int op_Subtraction(int value, int toRemove)       { return value - toRemove; }
        public static int op_Multiply(int a, int b)                     { return a * b; }
        public static int op_Division(int quantity, int divider)        { return quantity / divider; }
        
//        public static int op_AdditionAssign(ref int target, int b)              { return target += b; }
//        public static int op_SubtractionAssign(ref int target, int toRemove)    { return target -= toRemove; }
//        public static int op_MultiplyAssign(ref int target, int b)              { return target *= b; }
//        public static int op_DivisionAssign(ref int target, int divider)        { return target /= divider; }

        public static bool op_Equality(int a, int b)                    { return a == b; }
        public static bool op_Inequality(int a, int b)                  { return a != b; }

		public static bool op_GreaterThan(int value, int bias)          { return value > bias; }
		public static bool op_LessThan(int value, int bias)             { return value < bias; }
		public static bool op_GreaterThanOrEqual(int value, int bias)   { return value >= bias; }
		public static bool op_LessThanOrEqual(int value, int bias)      { return value <= bias; }

    }
    public static class Float {
//        public static float op_Ctor()                                       { return default(float); }
//        public static float op_Ctor(float value)                            { return value; }
                                                                        
//        public static float op_Assignment(ref float target, float value)    { return target= value;}
        public static float op_UnaryNegation(float a)                       { return -a; }
                                                                        
        public static float op_Addition(float a, float b)                   { return a + b; }
        public static float op_Subtraction(float value, float toRemove)     { return value - toRemove; }
        public static float op_Multiply(float a, float b)                   { return a * b; }
        public static float op_Division(float quantity, float divider)      { return quantity / divider; }
        
//        public static float op_AdditionAssign(ref float target, float b)              { return target += b; }
//        public static float op_SubtractionAssign(ref float target, float toRemove)    { return target -= toRemove; }
//        public static float op_MultiplyAssign(ref float target, float b)              { return target *= b; }
//        public static float op_DivisionAssign(ref float target, float divider)        { return target /= divider; }

        public static bool op_Equality(float a, float b)                    { return a == b; }
        public static bool op_Inequality(float a, float b)                  { return a != b; }

		public static bool op_GreaterThan(float value, float bias)          { return value > bias; }
		public static bool op_LessThan(float value, float bias)             { return value < bias; }
		public static bool op_GreaterThanOrEqual(float value, float bias)   { return value >= bias; }
		public static bool op_LessThanOrEqual(float value, float bias)      { return value <= bias; }

    }

}
