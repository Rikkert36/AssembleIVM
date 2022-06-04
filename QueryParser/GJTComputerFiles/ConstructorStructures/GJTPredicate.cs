using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures {
    class GJTPredicate {
        public readonly string var1;
        public readonly string comparor;
        public readonly string var2;
        public GJTPredicate(string var1, string comparor, string var2) {
            this.var1 = var1;
            this.var2 = var2;
            this.comparor = comparor;
        }
    }
}
