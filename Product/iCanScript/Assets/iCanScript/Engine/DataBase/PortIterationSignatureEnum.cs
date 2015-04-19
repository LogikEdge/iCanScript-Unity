namespace iCanScript { namespace Engine {

    [System.Serializable]
    public enum PortIterationSignatureEnum {
        /* x */                 Dont_iterate= 0,
        /* [i] */               Dont_iterate_use_index= 1,
        /* [size-n] */          Dont_iterate_use_index_from_end= 2,
        /* [] */                Iterate_all= 3,
        /* [s..] */             Iterate_starting_at= 4,
        /* [s..e] */            Iterate_starting_at_and_ending_at= 5,
        /* [s..size-n] */       Iterate_starting_at_and_ending_at_index_from_end= 6,
        /* [s1,s2..] */         Iterate_using_steps= 7,
        /* [s1,s2..e] */        Iterate_using_steps_and_ending_at= 8,
        /* [s1,s2..size-n] */   Iterate_using_steps_and_ending_at_index_from_end= 9
    }    

}}
