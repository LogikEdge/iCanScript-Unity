using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WD_CommandBuffer {
    public int              NextInstanceId= 0;
    public List<WD_Command> Commands    = new List<WD_Command>();
    public List<WD_Command> Undos       = new List<WD_Command>();
    
    public int Push(WD_Command cmd) {
        Undos.Clear();
        if(cmd.CommandType == WD_Command.CommandTypeEnum.Add) {
            cmd.InstanceId= NextInstanceId;
            ++NextInstanceId;
        }
        Commands.Add(cmd);
        return cmd.InstanceId;
    }
    public void Undo() {
        if(Commands.Count != 0) {
            Undos.Add(Commands[Commands.Count-1]);
            Commands.RemoveAt(Commands.Count-1);
        }
    }
    public void Redo() {
        if(Undos.Count != 0) {
            Commands.Add(Undos[Undos.Count-1]);
            Undos.RemoveAt(Undos.Count-1);
        }
    }
    public void Compress(int endOfRange= 0) {
        // Validate range to compress.
        if(endOfRange == 0 || endOfRange > Commands.Count) endOfRange= Commands.Count;

        // Nothing to compress if we have 0 or 1 commands. 
        if(endOfRange <= 1) return;

        // Scan through range and combine commands on same object.
        int srinkCount= 0;
        for(int i= 0; i < endOfRange-1; ++i) {
            for(int j= i+1; j < endOfRange; ++j) {
                if(Commands[i].InstanceId == Commands[j].InstanceId) {
                    switch(Commands[i].CommandType) {
                        case WD_Command.CommandTypeEnum.NOP:
                            ++srinkCount;
                            break;
                        case WD_Command.CommandTypeEnum.Add:
                            switch(Commands[j].CommandType) {
                                case WD_Command.CommandTypeEnum.NOP:
                                    break;
                                case WD_Command.CommandTypeEnum.Add:
                                    Debug.LogError("CommandBuffer is inconsistant: An Add command is followed by another Add command for object: "+Commands[i].InstanceId);
                                    break;
                                case WD_Command.CommandTypeEnum.Remove:
                                    Commands[i].CommandType= WD_Command.CommandTypeEnum.NOP;
                                    Commands[j].CommandType= WD_Command.CommandTypeEnum.NOP;
                                    ++srinkCount;
                                    break;
                                case WD_Command.CommandTypeEnum.Replace:
                                    Commands[i].CommandType= WD_Command.CommandTypeEnum.NOP;
                                    Commands[j].CommandType= WD_Command.CommandTypeEnum.Add;
                                    ++srinkCount;
                                    break;
                            }
                            break;
                        case WD_Command.CommandTypeEnum.Remove:
                            Debug.LogError("CommandBuffer is inconsistant: A Remove command is seen without a previous Add command for object: "+Commands[i].InstanceId);
                            break;
                        case WD_Command.CommandTypeEnum.Replace:
                            Debug.LogError("CommandBuffer is inconsistant: A Replace command is seen without a previous Add command for object: "+Commands[i].InstanceId);
                            break;
                    }
                }
            }
        }
        
        // Nothing to resize if no commands have been removed.
        if(srinkCount == 0) return;

        // Resize the command buffer.
        for(int i= 0, cnt= 0; cnt < endOfRange; ++ cnt) {
            if(Commands[i].CommandType == WD_Command.CommandTypeEnum.NOP) {
                Commands.RemoveAt(i);
            }
            else {
                ++i;
            }
        }
    }
}
