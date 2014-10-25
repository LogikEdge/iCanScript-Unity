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
    public static TimedAction CreateTimedAction(float time, Action action, bool isLooping= false) {
        return new TimedAction(time, action, isLooping);
    }
    public static void Schedule(TimedAction timedAction) {
        if(timedAction == null) return;
        if(!IsActive(timedAction)) {
            myTimers.Add(timedAction);            
        }
    }
    public static void Restart(TimedAction timedAction) {
        if(!IsActive(timedAction)) {
            myTimers.Add(timedAction);
        }
    }
    public static void Stop(TimedAction timedAction) {
        myTimers.Remove(timedAction);
    }
    public static bool IsActive(TimedAction timedAction) {
        return myTimers.Contains(timedAction);
    }
    
    // ======================================================================
    public class TimedAction {
        P.Timer myTimer;
        Action  myAction;
        bool    myIsLooping;
        
        public TimedAction(float delay, Action action, bool isLooping= false) {
            myAction   = action;
            myTimer    = new P.Timer(delay);
            myIsLooping= isLooping;
        }
        public bool  IsElapsed           { get { return myTimer.IsElapsed; }}
        public bool  IsActive            { get { return iCS_TimerService.IsActive(this); }}
        public bool  IsLooping           { get { return myIsLooping; }}
        public void  RunAction()         { myAction(); }
        public void  Schedule()          { iCS_TimerService.Schedule(this); }
        public void  Restart()           { myTimer.Restart(); iCS_TimerService.Restart(this); }
        public void  Restart(float time) { myTimer.Restart(time); iCS_TimerService.Restart(this); }
        public void  Stop()              { iCS_TimerService.Stop(this); }
		public float RemainingTime		 { get { return myTimer.RemainingTime; }}
    }
    
    // ======================================================================
    static List<TimedAction>    myTimers= new List<TimedAction>();

    // ======================================================================
    static void PeriodicUpdate() {
        var elapsedTimers= P.filter(t=> t.IsElapsed, myTimers);
        P.forEach(t=> { if(t.IsLooping) { t.Restart(); } else { myTimers.Remove(t);}}, elapsedTimers);
        P.forEach(t=> { t.RunAction(); }, elapsedTimers);
    }
}
