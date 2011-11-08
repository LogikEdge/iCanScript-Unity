using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ReflectionDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string           Company;
    public string           Package;
    public string           Name;
    public string           ToolTip;
    public string           IconPath;
    public string[]         ParamNames;
    public string           ReturnName;
    public WD_RuntimeDesc   RuntimeDesc;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_ReflectionDesc(string company, string package, string name,
                             string toolTip, string iconPath,
                             WD_ObjectTypeEnum objType, Type classType, MethodInfo methodInfo,
                             bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                             string returnName, Type returnType) {
        Company = company;
        Package = package;
        Name    = name;
        ToolTip = toolTip;
        IconPath= iconPath;
        ParamNames= paramNames;
        ReturnName= returnName;

        // Fill in runtime details.
        RuntimeDesc= new WD_RuntimeDesc();
        RuntimeDesc.ObjectType= objType;
        RuntimeDesc.ClassType= classType;
        RuntimeDesc.MethodName= methodInfo != null ? methodInfo.Name : null;
        // Combine parameter & return types (as in System.Func<...>)
        if(returnType == null) {
            RuntimeDesc.ParamIsOuts= paramIsOuts;
            RuntimeDesc.ParamTypes= paramTypes;
            RuntimeDesc.ParamDefaultValues= paramDefaultValues;
        } else {
            int len= paramTypes.Length;
            bool[] newParamIsOuts= new bool[len+1];
            paramIsOuts.CopyTo(newParamIsOuts, len);
            newParamIsOuts[len]= true;
            RuntimeDesc.ParamIsOuts= newParamIsOuts;
            Type[] newParamTypes= new Type[len+1];
            paramTypes.CopyTo(newParamTypes, len);
            newParamTypes[len]= returnType;
            RuntimeDesc.ParamTypes= newParamTypes;
            object[] newParamDeafultValues= new object[len+1];
            paramDefaultValues.CopyTo(newParamDeafultValues, len);
            newParamDeafultValues[len]= null;
            RuntimeDesc.ParamDefaultValues= newParamDeafultValues;
        }
    }
}
