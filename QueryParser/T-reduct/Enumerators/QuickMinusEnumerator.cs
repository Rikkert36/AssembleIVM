using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class QuickMinusEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            QuickMinusNode minusNode = (QuickMinusNode)rho;
            foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                    yield return new List<string>(s1);
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            QuickMinusNode minusNode = (QuickMinusNode)rho;
            foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                    yield return new List<string>(s1);
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            QuickMinusNode minusNode = (QuickMinusNode)rho;
            HashSet<string> wasDeleted = new HashSet<string>();
            HashSet<string> wasAddedAndNotDeleted = new HashSet<string>();

            if (minusNode.children[0].delta != null) {
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoinRemoved(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].EnumerateRemoved(t1)) {
                        wasDeleted.Add(String.Join(",", s1));
                        yield return new List<string>(s1);
                    }
                }
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoinAdded(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].EnumerateAdded(t1)) {
                        wasAddedAndNotDeleted.Add(String.Join(",", s1));
                    }
                }
            }
            foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                    if (!wasAddedAndNotDeleted.Contains(String.Join(",", s1))) {
                        yield return new List<string>(s1);
                    }
                }
            }
        }
    }
}
