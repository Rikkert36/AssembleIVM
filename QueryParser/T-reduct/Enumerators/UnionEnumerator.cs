using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class UnionEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            UnionNode r = (UnionNode)rho;
            foreach (GMRTuple t1 in r.children[0].SemiJoin(new List<string>(rho.variables), t, r.predicates[0])) {
                foreach (List<string> s1 in r.children[0].Enumerate(t1)) {
                    yield return s1;
                }
            }
            foreach (GMRTuple t2 in r.children[1].SemiJoin(new List<string>(rho.variables), t, r.predicates[1])) {
                foreach (List<string> s2 in r.children[1].Enumerate(t2)) {
                    yield return s2;
                }
            }
        }
    }
}
