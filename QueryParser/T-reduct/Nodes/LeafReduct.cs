using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AssembleIVM {
    class LeafReduct : NodeReduct {
        public string dataset;

        public LeafReduct(string name, List<string> variables, string dataset, Enumerator enumerator, bool inFrontier) :
            base(name, variables, enumerator, inFrontier) {
            this.dataset = dataset;
        }

        public override List<string> RetrieveHeader() {
            return variables;
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
            if (t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }
        }
    }
}
