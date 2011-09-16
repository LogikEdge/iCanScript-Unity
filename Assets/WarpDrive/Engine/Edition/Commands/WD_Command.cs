using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_Command {
    public enum CommandTypeEnum { NOP, Add, Remove, Replace };
    public CommandTypeEnum  CommandType= CommandTypeEnum.NOP;
    public string           ObjectId= "";
}
