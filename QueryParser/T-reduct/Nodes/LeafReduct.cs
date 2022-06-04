using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AssembleIVM {
    class LeafReduct : NodeReduct {
        public string dataset;

        public LeafReduct(string name, string[] variables, string dataset):
            base(name, variables) {
            this.dataset = dataset;
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
