using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

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
    
    // ======================================================================
    public static TimedAction CreateTimedAction(float time, Action action) {
        return new TimedAction(time, action);
    }
    public static void Schedule(TimedAction timedAction) {
        if(timedAction == null) return;
        myTimers.Add(timedAction);
    }
    public static void Restart(TimedAction timedAction) {
        if(!myTimers.Contains(timedAction)) {
            myTimers.Add(timedAction);
        }
    }
    public static void Stop(TimedAction timedAction) {
        myTimers.Remove(timedAction);
    }
    
    // ======================================================================
    public class TimedAction {
        P.Timer myTimer;
        Action  myAction;
        
        public TimedAction(float delay, Action action) {
            myAction= action;
            myTimer= new P.Timer(delay);
        }
        public bool IsElapsed            { get { return myTimer.IsElapsed; }}
        public void RunAction()          { myAction(); }
        public void Schedule()           { iCS_TimerService.Schedule(this); }
        public void Restart()            { myTimer.Restart(); iCS_TimerService.Restart(this); }
        public void Restart(float time)  { myTimer.Restart(time); iCS_TimerService.Restart(this); }
        public void Stop()               { iCS_TimerService.Stop(this); }
		public float RemainingTime		 { get { return myTimer.RemainingTime; }}
    }
    
    // ======================================================================
    static List<TimedAction>    myTimers= new List<TimedAction>();

    // ======================================================================
    static void PeriodicUpdate() {
        var elapsedTimers= P.filter(t=> t.IsElapsed, myTimers);
        P.forEach(t=> { myTimers.Remove(t); }, elapsedTimers);
        P.forEach(t=> { t.RunAction(); }, elapsedTimers);
    }
}
