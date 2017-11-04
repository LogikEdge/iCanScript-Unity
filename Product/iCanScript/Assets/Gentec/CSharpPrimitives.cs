using System;

namespace CSharp.Primitives {
    
    public struct Array {
        public static Object get_Item(System.Array a, int idx)                  { return default(Object); }
        public static void   set_Item(System.Array a, int idx, Object value)    { ; }
    }

    public struct Object {
        public static bool op_Equality(System.Object a, System.Object b)          { return a == b; }
        public static bool op_Inequality(System.Object a, System.Object b)        { return a != b; }
    }

    public struct Bool {
        public Bool(bool value)                       				{}

        public bool op_Assignment(bool value)						{ return false;}

        public static bool op_LogicalNot(bool a)                    { return !a; }
        public static bool op_LogicalOr(bool a, bool b)             { return a || b; }
        public static bool op_LogicalAnd(bool a, bool b)            { return a && b; }
        public static bool op_BitwiseAnd(bool a, bool b)            { return a & b; }
        public static bool op_BitwiseOr(bool a, bool b)             { return a | b; }
        public static bool op_ExclusiveOr(bool a, bool b)           { return a ^ b; }

        public bool op_BitwiseAndAssign(bool b)   					{ return false; }
        public bool op_BitwiseOrAssign(bool b)    					{ return false; }
        public bool op_ExclusiveOrAssign(bool b)  					{ return false; }

        public static bool op_Equality(bool a, bool b)              { return a == b; }
        public static bool op_Inequality(bool a, bool b)            { return a != b; }
    }

    public struct Int {
        public Int(int value)                           				{}
                                                                        
        public int op_Assignment(int value)     						{ return 0;}
        public static int  op_UnaryNegation(int a)                      { return -a; }
                                                                        
        public static int  op_BitwiseAnd(int a, int b)                  { return a & b; }
        public static int  op_BitwiseOr(int a, int b)                   { return a | b; }
        public static int  op_ExclusiveOr(int a, int b)                 { return a ^ b; }
        public int  op_BitwiseAndAssign(int value)   					{ return 0; }
        public int  op_BitwiseOrAssign(int value)    					{ return 0; }
        public int  op_ExclusiveOrAssign(int value)  					{ return 0; }

        public static int op_RightShift(Int toBeShifted, int nbOfBits)  { return 0; }
        public static int op_LeftShift(Int toBeShifted, int nbOfBits)   { return 0; }
        public int op_RightShiftAssign(int nbOfBits)                    { return 0; }
        public int op_LeftShiftAssign(int nbOfBits)                     { return 0; }
        
        public static int op_Addition(int a, int b)                     { return a + b; }
        public static int op_Subtraction(int value, int toRemove)       { return value - toRemove; }
        public static int op_Multiply(int a, int b)                     { return a * b; }
        public static int op_Division(int quantity, int divider)        { return quantity / divider; }
        
		public int op_Increment() 										{ return 0; }
		public int op_Decrement() 										{ return 0; }
        public int op_AdditionAssign(int toAdd)              			{ return 0; }
        public int op_SubtractionAssign(int toRemove)    				{ return 0; }
        public int op_MultiplyAssign(int value)              			{ return 0; }
        public int op_DivisionAssign(int divider)        				{ return 0; }

        public static bool op_Equality(int a, int b)                    { return a == b; }
        public static bool op_Inequality(int a, int b)                  { return a != b; }

		public static bool op_GreaterThan(int value, int bias)          { return value > bias; }
		public static bool op_LessThan(int value, int bias)             { return value < bias; }
		public static bool op_GreaterThanOrEqual(int value, int bias)   { return value >= bias; }
		public static bool op_LessThanOrEqual(int value, int bias)      { return value <= bias; }

    }
    public struct Float {
		public Float(float value)											{}
                                                                        
        public float op_Assignment(float value)    							{ return 0;}
        public static float op_UnaryNegation(float a)                       { return -a; }
                                                                        
        public static float op_Addition(float a, float b)                   { return a + b; }
        public static float op_Subtraction(float value, float toRemove)     { return value - toRemove; }
        public static float op_Multiply(float a, float b)                   { return a * b; }
        public static float op_Division(float quantity, float divider)      { return quantity / divider; }
        
		public float op_Increment() 										{ return 0; }
		public float op_Decrement() 										{ return 0; }
        public float op_AdditionAssign(float toAdd)              			{ return 0; }
        public float op_SubtractionAssign(float toRemove)    				{ return 0; }
        public float op_MultiplyAssign(float b)              				{ return 0; }
        public float op_DivisionAssign(float divider)        				{ return 0; }

        public static bool op_Equality(float a, float b)                    { return a == b; }
        public static bool op_Inequality(float a, float b)                  { return a != b; }

		public static bool op_GreaterThan(float value, float bias)          { return value > bias; }
		public static bool op_LessThan(float value, float bias)             { return value < bias; }
		public static bool op_GreaterThanOrEqual(float value, float bias)   { return value >= bias; }
		public static bool op_LessThanOrEqual(float value, float bias)      { return value <= bias; }

    }
	public class String {
		public String(string value)											{}
        public static string op_Addition(string a, Object b)				{ return a + b; }		
		public string op_AdditionAssign(string toAppend)					{ return toAppend; }
	}

}
