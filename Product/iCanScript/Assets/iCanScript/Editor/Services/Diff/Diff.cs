using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public class LineChange {
        public enum LineChangeType { Added, Removed };
        public int oldIdx= -1;
        public int newIdx= -1;
        public string text= null;
        public LineChangeType  changeType= LineChangeType.Removed;
        public LineChange(int oldIdx, int newIdx, LineChangeType changeType, string text) {
            this.oldIdx   = oldIdx;
            this.newIdx   = newIdx;
            this.changeType= changeType;
            this.text      = text;
        }
        public LineChange Reverse() {
            var newType= (changeType == LineChangeType.Added) ?
                    LineChangeType.Removed:
                    LineChangeType.Added;
            return new LineChange(newIdx, oldIdx, newType, text);
        }

    }

    public class LineDeleted : LineChange {
        public LineDeleted(int oldIdx, int newIdx, string removedLine)
        : base(oldIdx, newIdx, LineChangeType.Removed, removedLine) {}
    }

    public class LineAdded : LineChange {
        public LineAdded(int oldIdx, int newIdx, string addedLine)
            : base(oldIdx, newIdx, LineChangeType.Added, addedLine) {}
    }

    public class Diff {
        public Diff() {
            string oldText= "fred\nbarney\nmichel\nline\nAudreanne\n";
            string newText= "fred\nmichel\naline\nAudreanne\npinotte\n";
            var differences= PerformDiff(oldText, newText);
            var test= ApplyChanges(oldText, differences);
            if(test != newText) {
                Debug.LogWarning("Redo not working=> "+test);
            }
            else {
                Debug.Log("Redo working!!!");
            }
            var undo= Reverse(differences);
            test= ApplyChanges(newText, undo);
            if(test != oldText) {
                Debug.LogWarning("Undo not working=> "+test);
            }
            else {
                Debug.Log("Undo working!!!");
            }
        
        }
        public struct LineDesc {
            public int    hash;
            public string text;
        
            public LineDesc(string text) {
                this.hash= text.GetHashCode();
                this.text= text;
            }
        }
    
        public LineChange[] PerformDiff(string oldText, string newText) {
            var result= new List<LineChange>();
            var oldLines= BuildLineDescriptors(oldText);
            var newLines= BuildLineDescriptors(newText);
            int oldIdx= 0;
            int newIdx= 0;
            for(; newIdx < newLines.Length; ++newIdx) {
                int i= oldIdx;
                var newLineDesc= newLines[newIdx];
                for(; i < oldLines.Length; ++i) {
                    var oldLineDesc= oldLines[i];
                    if(newLineDesc.hash == oldLineDesc.hash) {
                        if(newLineDesc.text == oldLineDesc.text) {
                            break;
                        }
                    }
                }
                if(i >= oldLines.Length) {
                    result.Add(new LineAdded(oldIdx, newIdx, newLineDesc.text));
                }
                else {
                    for(; oldIdx < i; ++oldIdx) {
                        result.Add(new LineDeleted(oldIdx, newIdx, oldLines[oldIdx].text));
                    }
                    ++oldIdx;
                }
            }
            return result.ToArray();
        }
    
        public LineChange[] Reverse(LineChange[] lineChanges) {
            var result= new LineChange[lineChanges.Length];
            for(int i= 0; i < lineChanges.Length; ++i) {
                result[i]= lineChanges[i].Reverse();
            }
            return result;
        }
    
        public string ApplyChanges(string text, LineChange[] lineChanges) {
            var result= new List<string>();
            var lineDescs= BuildLineDescriptors(text);
            int oldIdx= 0;
            int newIdx= 0;
            ApplyChange(oldIdx, newIdx, lineChanges, 0, lineDescs, result);
            var finalString= new StringBuilder(1024);
            foreach(var l in result) {
                finalString.Append(l);
                finalString.Append("\n");
            }
            return finalString.ToString();
        }
    
        void ApplyChange(int oldIdx, int newIdx, LineChange[] lineChanges, int changeIdx, LineDesc[] lines, List<string> result) {
            if(oldIdx >= lines.Length && changeIdx >= lineChanges.Length) return;
            if(changeIdx >= lineChanges.Length) {
                result.Add(lines[oldIdx].text);
                oldIdx++;
                newIdx++;
                ApplyChange(oldIdx, newIdx, lineChanges, changeIdx, lines, result);
                return;            
            }
            var change= lineChanges[changeIdx];
            if(change.oldIdx > oldIdx && change.newIdx > newIdx) {
                result.Add(lines[oldIdx].text);
                oldIdx++;
                newIdx++;
                ApplyChange(oldIdx, newIdx, lineChanges, changeIdx, lines, result);
                return;
            }
            if(change.changeType == LineChange.LineChangeType.Removed) {
                if(change.oldIdx != oldIdx) {
                    Debug.LogWarning("iCanScript: Wrong index!");
                }
                if(change.text != lines[oldIdx].text) {
                    Debug.LogWarning("iCanScript: Wrong text!");
                }
                ++oldIdx;
                ++changeIdx;
                ApplyChange(oldIdx, newIdx, lineChanges, changeIdx, lines, result);
                return;
            }
            if(change.changeType == LineChange.LineChangeType.Added) {
                if(change.newIdx != newIdx) {
                    Debug.LogWarning("iCanScript: Wrong index!");
                }
                result.Add(change.text);
                ++newIdx;
                ++changeIdx;
                ApplyChange(oldIdx, newIdx, lineChanges, changeIdx, lines, result);
            }
        }
    
        LineDesc[] BuildLineDescriptors(string text) {
            var result= new List<LineDesc>();
            using (StringReader sr = new StringReader(text)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    result.Add(new LineDesc(line));
                }//while
            }//using
            return result.ToArray();
        }
    }

}
