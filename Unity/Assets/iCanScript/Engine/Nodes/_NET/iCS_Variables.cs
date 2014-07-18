[iCS_Class(Company="NET",Icon="iCS_LibraryIcon_32x32.png")]
public static class iCS_Variables {
    [iCS_Function(Name="bool")]      public static bool      _bool  (bool value)    { return value; }
    [iCS_Function(Name="int")]       public static int       _int   (int value)     { return value; }
    [iCS_Function(Name="float")]     public static float     _float (float value)   { return value; }
    [iCS_Function(Name="string")]    public static string    _string(string value)  { return value; }
}


[iCS_Class(Company="iCanScript",Library="Variables")]
public struct Bool {
    bool myValue;
    
    public bool Value {
        [iCS_Function] get { return myValue; }
        [iCS_Function] set { myValue= value; }
    }
    
    [iCS_Function] public Bool(bool init= false) { myValue= init; }
}

[iCS_Class(Company="iCanScript",Library="Variables")]
public struct Int {
    int myValue;
    
    public int Value {
        [iCS_Function] get { return myValue; }
        [iCS_Function] set { myValue= value; }
    }
    
    [iCS_Function] public Int(int init= 0) { myValue= init; }
}

[iCS_Class(Company="iCanScript",Library="Variables")]
public struct Float {
    float myValue;
    
    public float Value {
        [iCS_Function] get { return myValue; }
        [iCS_Function] set { myValue= value; }
    }
    
    [iCS_Function] public Float(float init= 0f) { myValue= init; }
}

