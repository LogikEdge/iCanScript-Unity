using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WD_CommandBuffer {
    public List<WD_Command>    Commands= new List<WD_Command>();
    
    public void PushCommand(WD_Command cmd) { Commands.Add(cmd); }
}
