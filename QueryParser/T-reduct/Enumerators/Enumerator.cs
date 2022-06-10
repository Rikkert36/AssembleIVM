using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    abstract class Enumerator {
        public abstract IEnumerable<List<string>> Enumerate(GMRTuple t);
        public NodeReduct rho;

        //public abstract IEnumerable<Tuple<List<string>, List<string>>> Enumerate(GMRTuple t, index);
        
    }
}
