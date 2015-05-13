using UnityEngine;
using System;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal {
    
    public static class iCS_StringUtil {
    	public static float FuzzyCompare(string s1, string s2) {
    		int l1= P.length(s1);
    		int l2= P.length(s2);
    		bool[] used= new bool[l2];
    		for(int i= 0; i < l2; ++i) {
    			used[i]= false;
    		}

    		const float kExactMatch= 1f;
    		const float kNoneCaseMatch= 0.9f;
    		const float kOutOfOrderFactor= 0.5f;
    		const float kInOrderFactor= 0.75f;
    		float accum= 0f;
    		int nextIdx= 0;
    		for(int i= 0; i < l1; ++i) {
    			var c1= s1[i];
    			// First verify for exact match.
    			float exactScore= 0f;
    			int exactIdx= -1;
    			for(int j= nextIdx; j < l2; ++j) {
    				if(!used[j] && c1 == s2[j]) {
    					if(j == nextIdx) {
    						exactScore= kExactMatch;
    					}
    					else {
    						exactScore= kExactMatch*kInOrderFactor;
    					}
    					exactIdx= j;
    					break;
    				}
    			}
    			if(exactIdx == -1) {
    				for(int j= 0; j < nextIdx; ++j) {
    					if(!used[j] && c1 == s2[j]) {
    						exactScore= kExactMatch*kOutOfOrderFactor;
    						exactIdx= j;
    						break;
    					}
    				}				
    			}
    			// Verify for case mismatch.
    			float nonExactScore= 0f;
    			int nonExactIdx= -1;
    			Char uc1= Char.ToUpper(c1);
    			for(int j= nextIdx; j < l2; ++j) {
    				if(!used[j] && uc1 == Char.ToUpper(s2[j])) {
    					if(j == nextIdx) {
    						nonExactScore= kNoneCaseMatch;
    					}
    					else {
    						nonExactScore= kNoneCaseMatch*kInOrderFactor;
    					}
    					nonExactIdx= j;
    					break;
    				}
    			}
    			if(nonExactIdx == -1) {
    				for(int j= 0; j < nextIdx; ++j) {
    					if(!used[j] && uc1 == Char.ToUpper(s2[j])) {
    						nonExactScore= kNoneCaseMatch*kOutOfOrderFactor;
    						nonExactIdx= j;
    						break;
    					}
    				}				
    			}
    			// Go to next if not found.
    			if(exactIdx == -1 && nonExactIdx == -1) continue;
    			if(exactScore >= nonExactScore) {
    				used[exactIdx]= true;
    				accum+= 2f*exactScore;
    				nextIdx= exactIdx+1;
    			}
    			else {
    				used[nonExactIdx]= true;
    				accum+= 2f*nonExactScore;
    				nextIdx= nonExactIdx+1;
    			}
    		}
    		return accum/((float)(l1+l2));
    	}
    }

}
