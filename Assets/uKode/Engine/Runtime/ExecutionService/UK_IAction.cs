using UnityEngine;
using System.Collections;

public interface UK_IAction {
    // =======================================================================
    // Properties
    // -----------------------------------------------------------------------
    int  FrameId   { get; }
    bool IsStalled { get; set; }

    // =======================================================================
    // Methods
    // -----------------------------------------------------------------------
    void Execute(int frameId);
    void ForceExecute(int frameId);
    bool IsCurrent(int frameId);
    void MarkAsCurrent(int frameId);
}
