using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.FuzzyLogic {
    public static class FuzzyString {
        // ----------------------------------------------------------------------
        public static float GetScore(string search, string dest) {
            // -- Assume perfect match for empty search string. --
            var searchLen= search.Length;
            if(searchLen == 0) return 1f;
            
            // -- Convert to uppercase. --
//            search= search.ToUpper();
//            dest  = dest.ToUpper();

            // -- Assume 100% for exact match and 50% for bad index. --
            float score= 0f;
            var destLen= dest.Length;
            int searchIdx= 0;
            int destIdx= 0;
            while(searchIdx < searchLen && destIdx < destLen) {
                var c= search[searchIdx];
                var d= dest[destIdx];
                if(c == d) {
                    score+= 1f;
                }
                else if(Char.ToUpper(c) == Char.ToUpper(d)) {
                    score+= 0.75f;
                }
                else {
                    for(++destIdx; destIdx < destLen; ++destIdx) {
                        var nd= dest[destIdx];
                        if(c == nd) {
                            score+= 0.5f;
                            break;
                        }
                        else if(Char.ToUpper(c) == Char.ToUpper(nd)) {
                            score+= 0.375f;
                            break;
                        }
                    }
                }
                ++searchIdx;
                ++destIdx;
            }
            score= score/searchLen;
            return score*score;
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
