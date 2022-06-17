using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    abstract class Enumerator {
        public abstract IEnumerable<List<string>> Enumerate(GMRTuple t);
        public abstract IEnumerable<List<string>> EnumerateAdded(GMRTuple t);
        public abstract IEnumerable<List<string>> EnumerateRemoved(GMRTuple t);

        public NodeReduct rho;        
    }
}
