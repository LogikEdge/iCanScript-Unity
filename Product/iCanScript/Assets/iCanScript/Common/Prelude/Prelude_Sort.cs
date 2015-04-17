using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    public static void sort<T>(List<T> toSort, Func<T,T,int> cmp) {
        Comparison<T> compare= (x,y)=> cmp(x,y);
        toSort.Sort(compare);
//        int reorderCnt= 0;
//        int cmpCnt= 0;
//        int len= length(toSort);
//        int step= (len >> 1) + (len & 1);
//        while(step != 0) {
//            int i= 0;
//            int j= step;
//            while(j < len) {
//                ++cmpCnt;
//                if(cmp(toSort[i], toSort[j]) > 0) {
//                    ++reorderCnt;
//                    var tmp= toSort[i];
//                    toSort[i]= toSort[j];
//                    toSort[j]= tmp;
//                    int k= i-step;
//                    while(k >= 0) {
//                        ++cmpCnt;
//                        if(cmp(toSort[k], toSort[k+step]) < 0) break;
//                        ++reorderCnt;
//                        tmp= toSort[k];
//                        toSort[k]= toSort[k+step];
//                        toSort[k+step]= tmp;
//                        k-= step;
//                    }
//                }
//                ++i;
//                ++j;
//            }
//            step >>= 1;
//        }
    }
    // ----------------------------------------------------------------------
    public static void sort<T>(T[] toSort, Func<T,T,int> cmp) {
        Comparison<T> compare= (x,y)=> cmp(x,y);
        Array.Sort<T>(toSort, compare);
//        int reorderCnt= 0;
//        int cmpCnt= 0;
//        int len= length(toSort);
//        int step= (len >> 1) + (len & 1);
//        while(step != 0) {
//            int i= 0;
//            int j= step;
//            while(j < len) {
//                ++cmpCnt;
//                if(cmp(toSort[i], toSort[j]) > 0) {
//                    ++reorderCnt;
//                    var tmp= toSort[i];
//                    toSort[i]= toSort[j];
//                    toSort[j]= tmp;
//                    int k= i-step;
//                    while(k >= 0) {
//                        ++cmpCnt;
//                        if(cmp(toSort[k], toSort[k+step]) < 0) break;
//                        ++reorderCnt;
//                        tmp= toSort[k];
//                        toSort[k]= toSort[k+step];
//                        toSort[k+step]= tmp;
//                        k-= step;
//                    }
//                }
//                ++i;
//                ++j;
//            }
//            step >>= 1;
//        }
    }
}
