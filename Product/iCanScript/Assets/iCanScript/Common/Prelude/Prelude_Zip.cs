using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // zipWith :: (f a->b->r)->[a]->[b]->[r]
        public static R[] zipWith_<A,B,R>(R[] result, System.Func<A,B,R> fnc, A[] l1, B[] l2) {
            int len= Math3D.Min(length(l1), length(l2));
            if(length(result) != len) result= new R[len];
            for(int i= 0; i < len; ++i) {
                result[i]= fnc(l1[i], l2[i]);
            }
            return result;
        }
        public static List<R> zipWith_<A,B,R>(List<R> result, System.Func<A,B,R> fnc, List<A> l1, List<B> l2) {
            int len= Math3D.Min(length(l1), length(l2));
            result.Clear();
            if(result.Capacity < len) result.Capacity= len;
            for(int i= 0; i < len; ++i) {
                result.Add(fnc(l1[i], l2[i]));
            }
            return result;
        }
        public static R[] zipWith<A,B,R>(System.Func<A,B,R> fnc, A[] l1, B[] l2) {
            int len= Math3D.Min(length(l1), length(l2));
            R[] result= new R[len];
            for(int i= 0; i < len; ++i) {
                result[i]= fnc(l1[i], l2[i]);
            }
            return result;
        }
        public static List<R> zipWith<A,B,R>(System.Func<A,B,R> fnc, List<A> l1, List<B> l2) {
            return zipWith_(new List<R>(), fnc, l1, l2);
        }
        public static void zipWith<A,B>(System.Action<A,B> fnc, A[] l1, B[] l2) {
            int len= Math3D.Min(length(l1), length(l2));
            for(int i= 0; i < len; ++i) {
                fnc(l1[i], l2[i]);
            }
        }
        // ----------------------------------------------------------------------
        // zip :: [a]->[b]->[(a,b)]
        public static Tuple<A,B>[] zip<A,B>(A[] l1, B[] l2) {
            return zipWith<A,B,Tuple<A,B>>((a,b)=> new Tuple<A,B>(a,b), l1, l2);
        }
        public static List<Tuple<A,B>> zip<A,B>(List<A> l1, List<B> l2) {
            return zipWith<A,B,Tuple<A,B>>((a,b)=> new Tuple<A,B>(a,b), l1, l2);
        }
        // ----------------------------------------------------------------------
        // zipWith :: (f a->b->c->r)->[a]->[b]->[c]->[r]
        public static R[] zipWith_<A,B,C,R>(R[] result, System.Func<A,B,C,R> fnc, A[] l1, B[] l2, C[] l3) {
            int len= Math3D.Min(length(l1), length(l2), length(l3));
            if(length(result) != len) result= new R[len];
            for(int i= 0; i < len; ++i) {
                result[i]= fnc(l1[i], l2[i], l3[i]);
            }
            return result;
        }
        public static List<R> zipWith_<A,B,C,R>(List<R> result, System.Func<A,B,C,R> fnc, List<A> l1, List<B> l2, List<C> l3) {
            int len= Math3D.Min(length(l1), length(l2), length(l3));
            result.Clear();
            if(result.Capacity < len) result.Capacity= len;
            for(int i= 0; i < len; ++i) {
                result[i]= fnc(l1[i], l2[i], l3[i]);
            }
            return result;
        }
        public static R[] zipWith<A,B,C,R>(System.Func<A,B,C,R> fnc, A[] l1, B[] l2, C[] l3) {
            int len= Math3D.Min(length(l1), length(l2), length(l3));
            R[] result= new R[len];
            for(int i= 0; i < len; ++i) {
                result[i]= fnc(l1[i], l2[i], l3[i]);
            }
            return result;
        }
        public static List<R> zipWith<A,B,C,R>( System.Func<A,B,C,R> fnc, List<A> l1, List<B> l2, List<C> l3) {
            return zipWith_(new List<R>(), fnc, l1, l2, l3);
        }
        // ----------------------------------------------------------------------
        // zip :: [a]->[b]->[c]->[(a,b,c)]
        public static Tuple<A,B,C>[] zip<A,B,C>(A[] l1, B[] l2, C[] l3) {
            return zipWith<A,B,C,Tuple<A,B,C>>((a,b,c)=> new Tuple<A,B,C>(a,b,c), l1, l2, l3);
        }
        public static List<Tuple<A,B,C>> zip<A,B,C>(List<A> l1, List<B> l2, List<C> l3) {
            return zipWith<A,B,C,Tuple<A,B,C>>((a,b,c)=> new Tuple<A,B,C>(a,b,c), l1, l2, l3);
        }
        // ----------------------------------------------------------------------
        // zipWith :: (f a->b->c->d->r)->[a]->[b]->[c]->[d]->[r]
        public static R[] zipWith_<A,B,C,D,R>(R[] result, System.Func<A,B,C,D,R> fnc, A[] l1, B[] l2, C[] l3, D[] l4) {
            int len= Math3D.Min(length(l1), length(l2), length(l3), length(l4));
            if(length(result) != len) result= new R[len];
            for(int i= 0; i < len; ++i) {
                fnc(l1[i], l2[i], l3[i], l4[i]);
            }
            return result;
        }
        public static List<R> zipWith_<A,B,C,D,R>(List<R> result, System.Func<A,B,C,D,R> fnc, List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
            int len= Math3D.Min(length(l1), length(l2), length(l3), length(l4));
            result.Clear();
            if(result.Capacity < len) result.Capacity= len;
            for(int i= 0; i < len; ++i) {
                result.Add(fnc(l1[i], l2[i], l3[i], l4[i]));
            }
            return result;
        }
        public static R[] zipWith<A,B,C,D,R>(System.Func<A,B,C,D,R> fnc, A[] l1, B[] l2, C[] l3, D[] l4) {
            int end= Math3D.Min(length(l1), length(l2), length(l3), length(l4));
            R[] result= new R[end];
            for(int i= 0; i < end; ++i) {
                fnc(l1[i], l2[i], l3[i], l4[i]);
            }
            return result;
        }
        public static List<R> zipWith<A,B,C,D,R>(System.Func<A,B,C,D,R> fnc, List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
            int end= Math3D.Min(length(l1), length(l2), length(l3), length(l4));
            List<R> result= new List<R>();
            for(int i= 0; i < end; ++i) {
                result.Add(fnc(l1[i], l2[i], l3[i], l4[i]));
            }
            return result;
        }
        // ----------------------------------------------------------------------
        // zip :: [a]->[b]->[c]->[d]->[(a,b,c,d)]
        public static Tuple<A,B,C,D>[] zip<A,B,C,D>(A[] l1, B[] l2, C[] l3, D[] l4) {
            return zipWith<A,B,C,D,Tuple<A,B,C,D>>((a,b,c,d)=> new Tuple<A,B,C,D>(a,b,c,d), l1, l2, l3, l4);
        }
        public static List<Tuple<A,B,C,D>> zip<A,B,C,D>(List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
            return zipWith<A,B,C,D,Tuple<A,B,C,D>>((a,b,c,d)=> new Tuple<A,B,C,D>(a,b,c,d), l1, l2, l3, l4);
        }
    
    }

}
