using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_Command {
    public enum CommandTypeEnum { NOP, Add, Remove, Replace };
    public CommandTypeEnum  CommandType= CommandTypeEnum.NOP;
    public int              InstanceId = -1;
    public int              ParentId   = -1;

    public WD_Command(WD_Object obj) {
        ParentId= obj.Parent != null ? obj.Parent.InstanceId : -1;       
    }
}
