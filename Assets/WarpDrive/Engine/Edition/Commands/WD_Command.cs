using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_Command {
    public enum CommandTypeEnum { NOP, Add, Remove, Replace, Move };
    public CommandTypeEnum  CommandType= CommandTypeEnum.NOP;
    public string           ObjectId= "";
}
