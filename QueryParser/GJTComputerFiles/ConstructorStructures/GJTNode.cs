using AssembleIVM;
using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    abstract class GJTNode {
        public readonly string[] variables;
        public bool inFrontier = false;
        public string name;

        public GJTNode(string name, string[] variables) {
            this.name = name;
            this.variables = variables;
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
