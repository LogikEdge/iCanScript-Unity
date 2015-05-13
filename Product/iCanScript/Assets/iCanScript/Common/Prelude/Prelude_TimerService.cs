using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        public class TimerService {
            // ======================================================================
            // Initialization
            // ----------------------------------------------------------------------
            public TimerService() {}
            public TimerService(Func<float> timeFnc) {
                myTimeFnc= timeFnc;
            }
        
            // ======================================================================
            // Properties
            // ----------------------------------------------------------------------
            Func<float> myTimeFnc= ()=> Time.realtimeSinceStartup;
        
            // ======================================================================
            public TimedAction CreateTimedAction(float time, Action action, bool isLooping= false) {
                return new TimedAction(time, action, this, isLooping);
            }
            public void Schedule(TimedAction timedAction) {
                if(timedAction == null) return;
                if(!IsActive(timedAction)) {
                    myTimers.Add(timedAction);            
                }
            }
            public void Restart(TimedAction timedAction) {
                if(!IsActive(timedAction)) {
                    myTimers.Add(timedAction);
                }
            }
            public void Stop(TimedAction timedAction) {
                myTimers.Remove(timedAction);
            }
            public bool IsActive(TimedAction timedAction) {
                return myTimers.Contains(timedAction);
            }
        
            // ======================================================================
            public class TimedAction {
                Timer           myTimer;
                Action          myAction;
                bool            myIsLooping;
                TimerService    myTimerService;
            
                public TimedAction(float delay, Action action, TimerService timerService, bool isLooping= false) {
                    myAction      = action;
                    myTimerService= timerService;
                    myTimer       = new Timer(delay, timerService.myTimeFnc);
                    myIsLooping   = isLooping;
                }
                public bool  IsElapsed           { get { return myTimer.IsElapsed; }}
                public bool  IsActive            { get { return myTimerService.IsActive(this); }}
                public bool  IsLooping           { get { return myIsLooping; }}
                public void  RunAction()         { myAction(); }
                public void  Schedule()          { myTimerService.Schedule(this); }
                public void  Restart()           { myTimer.Restart(); myTimerService.Restart(this); }
                public void  Restart(float time) { myTimer.Restart(time); myTimerService.Restart(this); }
                public void  Stop()              { myTimerService.Stop(this); }
        		public float RemainingTime		 { get { return myTimer.RemainingTime; }}
                public void  SanityCheck()       { myTimer.SanityCheck(); }
            }
        
            // ======================================================================
            List<TimedAction>    myTimers= new List<TimedAction>();
    
            // ======================================================================
            public void Update() {
                var elapsedTimers= filter(t=> t.IsElapsed, myTimers);
                forEach(t=> { if(t.IsLooping) { t.Restart(); } else { myTimers.Remove(t);}}, elapsedTimers);
                forEach(t=> { t.RunAction(); }, elapsedTimers);
            }
        }
    }

}
