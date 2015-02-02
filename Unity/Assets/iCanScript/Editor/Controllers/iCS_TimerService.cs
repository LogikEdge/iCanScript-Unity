using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using TimedAction= Prelude.TimerService.TimedAction;

public static class iCS_TimerService {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    static iCS_TimerService() {
        EditorApplication.update+= PeriodicUpdate;
    }
    public static void Start() {}
    public static void Shutdown() {
        EditorApplication.update-= PeriodicUpdate;
    }
    
    //======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static float EditorTime() { return (float)EditorApplication.timeSinceStartup; }
    public static float EngineTime() { return Time.realtimeSinceStartup; }

    // ======================================================================
    public static P.TimerService.TimedAction CreateTimedAction(float time, Action action, bool isLooping= false) {
        return myTimerService.CreateTimedAction(time, action, isLooping);
    }
    public static void Schedule(TimedAction timedAction) {
        myTimerService.Schedule(timedAction);
    }
    public static void Restart(TimedAction timedAction) {
        myTimerService.Restart(timedAction);
    }
    public static void Stop(TimedAction timedAction) {
        myTimerService.Stop(timedAction);
    }
    public static bool IsActive(TimedAction timedAction) {
        return myTimerService.IsActive(timedAction);
    }
    
    // ======================================================================
    static P.TimerService myTimerService= new P.TimerService(EditorTime);

    // ======================================================================
    static void PeriodicUpdate() {
        myTimerService.Update();
    }
}
