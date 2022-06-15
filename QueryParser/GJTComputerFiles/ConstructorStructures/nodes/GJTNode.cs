using AssembleIVM;
using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    abstract class GJTNode {
        public readonly List<string> variables;
        public bool inFrontier = false;
        public string name;
        public Enumerator enumerator;
        public string orderDimension;

        public GJTNode(string name, List<string> variables, Enumerator enumerator, string orderDimension = "") {
            this.name = name;
            this.variables = variables;
            this.enumerator = enumerator;
            this.orderDimension = orderDimension;
        }

        public HashSet<string> CopyVars() {
            HashSet<string> result = new HashSet<string>();
            foreach (string r in variables) {
                result.Add(r);
            }
            return result;
        }

        public abstract NodeReduct GenerateReduct(string modelName);
 
    }
}
