using AssembleIVM;
using AssembleIVM.GJTs;
using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles.ConstructorStructures {
    class GJTLeaf : GJTNode {
        public string dataset;
        public GJTLeaf(string name, List<string> variables, string dataset, Enumerator enumerator, string orderDimension = "") : base(name, variables, enumerator, orderDimension) {
            this.dataset = dataset;
        }

        /*public GJTLeaf(string name, HashSet<string> variables, ManualGJT inputGJT): base(name, variables) {
            Type t = inputGJT.GetType();
            this.dataset = t.Name;
            Console.WriteLine(t.Name);
        }*/

        public override NodeReduct GenerateReduct(string modelName) {
            LeafReduct result = new LeafReduct(this.name, this.variables, this.dataset, this.enumerator, this.inFrontier);
            if (enumerator != null) enumerator.rho = result;
            return result;
        }
    }
}
