using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    abstract class RootEnumerator {

        public abstract IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root, List<string> headerVariables);

        protected GMRTuple CreateTuple(List<int> indices, List<string> variables) {
            List<string> result = new List<string>();
            foreach (int i in indices) {
                result.Add(variables[i]);
            }
            return new GMRTuple(result.Count, 1) {
                fields = result.ToArray()
            };
        }

        protected List<int> GetIndices(List<string> headerVariables, List<string> combinedHeader) {
            List<int> result = new List<int>();
            foreach (string headerVariable in headerVariables) {
                result.Add(combinedHeader.IndexOf(headerVariable));
            }
            return result;
        }

    }

    class EnumCopyfactgross : RootEnumerator {
        public override IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root, List<string> headerVariables) {
            List<string> combinedHeader = root.RetrieveHeader();
            List<int> indices = GetIndices(headerVariables, combinedHeader);
            foreach(List<GMRTuple> tupleList in root.index.tupleMap.Values) {
                foreach(GMRTuple t in tupleList) {
                    foreach (List<string> s in root.Enumerate(t)) {
                        yield return CreateTuple(indices, s);
                    }
                }
            }
        }
    }

    class EnumComputenet : RootEnumerator {
        public override IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root, List<string> headerVariables) {
            throw new NotImplementedException();
        }
    }

    
}
