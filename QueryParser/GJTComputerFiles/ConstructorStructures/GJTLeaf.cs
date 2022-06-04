using AssembleIVM;
using AssembleIVM.GJTs;
using AssembleIVM.T_reduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles.ConstructorStructures {
    class GJTLeaf : GJTNode {
        public string dataset;
        public GJTLeaf(string name, string[] variables, string dataset): base(name, variables) {
            this.dataset = dataset;
        }

        /*public GJTLeaf(string name, HashSet<string> variables, ManualGJT inputGJT): base(name, variables) {
            Type t = inputGJT.GetType();
            this.dataset = t.Name;
            Console.WriteLine(t.Name);
        }*/

        public override NodeReduct GenerateReduct(string modelName) {
            LeafReduct result = new LeafReduct(this.name, this.variables, this.dataset);
            result.inFrontier = this.inFrontier;
            return result;
        }
    }
}
