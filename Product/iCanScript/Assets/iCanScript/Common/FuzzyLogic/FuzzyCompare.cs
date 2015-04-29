using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.FuzzyLogic {
    public static class FuzzyString {
        // ----------------------------------------------------------------------
        public static float GetScore(string search, string dest) {
            // Local function to compute damping factor
            Func<float,float> dampingFactor= (distance)=> {
                distance= Mathf.Abs(distance);
                if(distance > 10f) distance= 10f;
                return 1f-(distance/100f);
            };
            // Convert to uppercase.
            search= search.ToUpper();
            dest  = dest.ToUpper();
            // Compute fuzzy score.
            float score= 0f;
            var searchLen= search.Length;
            var destLen  = dest.Length;
            var len= searchLen+destLen;
            if(len == 0) return 0f;
            int destCursor= -1;
            for(int i= 0; i < searchLen; ++i) {
                var c= search[i];
                // Start by serach forward.
                var found= false;
                for(int j= destCursor+1; j < destLen; ++j) {
                    if(dest[j] == c) {
                        score+= 2f * dampingFactor(j-destCursor+1) * dampingFactor(0.5f*(i-j));
                        destCursor= j;
                        found= true;
                        break;
                    }
                }
                // If not found, try serach backward (for wrong order).
                if(found == false) {
                    for(int j= destCursor; j >= 0; --j) {
                        if(dest[j] == c) {
                            score+= 1f * dampingFactor(j-destCursor+1) * dampingFactor(0.35f*(i-j));
                            destCursor= j;
                            found= true;
                            break;
                        }
                    }
                }
                // Character not found.
                if(found == false) {
                    score-= 1f;
                }
            }
            // Adjust for fuzzy output (0 to 1f).
            score= score/(2*searchLen);
            score= (score+0.5f)/1.5f;
            return score * dampingFactor(destLen-searchLen);
        }
        // ----------------------------------------------------------------------
        public static float[] GetScores(string search, string[] lst) {
            return P.map(x=> GetScore(search, x), lst);
        }
        // ----------------------------------------------------------------------
        public static string[] FilterAndSort(string search, string[] lst, float minScore) {
            var scores= GetScores(search, lst);
            var ts= P.zip(scores, lst);
            var filtered= P.filter((t)=> t.Item1 > minScore, ts);
            P.sort(
                filtered,
                (t1, t2)=> {
                    var diff= t2.Item1 - t1.Item1;
                    return Math3D.IsZero(diff) ? 0 : (diff < 0 ? -1 : 1);
                }
            );
            return P.map((t)=> P.uncurry((score, str)=> str, t), filtered);
        }
        // ----------------------------------------------------------------------
        public static string[] SortAndTake(string search, string[] lst, int maxNbOfResults) {
            var scores= GetScores(search, lst);
            var toSort= P.zip(scores, lst);
            P.sort(
                toSort,
                (t1, t2)=> {
                    var diff= t2.Item1 - t1.Item1;
                    return Math3D.IsZero(diff) ? 0 : (diff < 0 ? -1 : 1);
                }
            );
            var result= P.take(maxNbOfResults, toSort);
            return P.map((t)=> P.uncurry((score, str)=> str, t), result);
        }
        // ----------------------------------------------------------------------
        public static string[] SortAndTake_(string search, string[] lst, int maxNbOfResults= 0, float minScore= 0.5f) {
            // Define local sort function
            Action<P.Tuple<float,string>[]> sort= (ts)=> {
                P.sort(
                    ts,
                    (t1, t2)=> {
                        var diff= t2.Item1 - t1.Item1;
                        return Math3D.IsZero(diff) ? 0 : (diff < 0 ? -1 : 1);
                    }
                );                
            };
            // Correctly size result.
            var len= lst.Length;
            if(maxNbOfResults == 0) maxNbOfResults= len;
            if(maxNbOfResults > len) maxNbOfResults= len;
            var result= new P.Tuple<float,string>[maxNbOfResults];
            int resultLen= 0;
            // Filter out the input list
            bool isSorted= false;
            foreach(var l in lst) {
                var score= GetScore(search, l);
                if(score <= minScore) continue;
                if(resultLen < maxNbOfResults) {
                    result[resultLen]= P.curry(t=> P.id(t), score, l);
                    ++resultLen;
                    if(resultLen == maxNbOfResults) {
                        sort(result);
                        isSorted= true;
                    }
                }
                else {
                    if(score > result[resultLen-1].Item1) {
                        result[resultLen-1]= P.curry(t=> P.id(t), score, l);
                        sort(result);
                        isSorted= true;
                    }
                } 
                // TODO: finish SortAndTake_
            }
            // Perform a last sort in case we have not filled the max number of results.
            if(isSorted == false) {
                result= P.take(resultLen, result);
                sort(result);
            }
            return P.map((t)=> P.uncurry((score, str)=> str, t), result);
        }
    }

}
