using QueryParser.GJTComputerFiles;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;

namespace AssembleIVM.T_reduct {
    abstract class NodeReduct {
        public readonly List<string> variables;
        public bool inFrontier;
        public string name;
        public Index index;
        public InnerNodeReduct parent;

        public Update delta;
        private Enumerator enumerator;

        public NodeReduct(string name, List<string> variables, Enumerator enumerator, bool inFrontier, string orderDimension = "") {
            this.variables = variables;
            this.name = name;
            this.enumerator = enumerator;
            this.inFrontier = inFrontier;
            this.index = new Index(orderDimension);
        }

        public List<string> CopyVars() {
            List<string> result = new List<string>();
            foreach (string r in variables) {
                result.Add(r);
            }
            return result;
        }

        public abstract List<string> RetrieveHeader();

        public void SetParent(InnerNodeReduct parent) {
            this.parent = parent;
        }

        public List<GMRTuple> SemiJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return index.SemiJoin(rightHeader, rightTuple, predicate);
        }

        public bool AnyJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return index.AnyJoin(rightHeader, rightTuple, predicate);
        }

        public IEnumerable<List<string>> Enumerate(GMRTuple t) {
            return enumerator.Enumerate(t);
        }

        public void Serialize(BinaryFormatter bf, FileStream fs) {
            bf.Serialize(fs, index);
        }

        public void Deserialize(BinaryFormatter bf, FileStream fs) {
            index = (Index)bf.Deserialize(fs);
        }


        public void ComputeParentUpdate() {
            if (this.parent != null) {
                if (parent.delta == null) parent.delta = 
                        new Update() { unprojectHeader = parent.GetUnprojectHeader()};
                parent.ComputeDelta(this);
            }
        }

        public void ProjectUpdate() {
            this.delta.ProjectTuples(this.variables);
        }

        public void ApplyUpdate() {
            foreach (GMRTuple addition in delta.projectedAddedTuples) {
                AddTuple(addition);
            }
            foreach (GMRTuple deletion in delta.projectedRemovedTuples) {
                RemoveTuple(deletion);
            }

        }

        public GMRTuple AddTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.GetOrPlace(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t != null) {
                t.count += tuple.count;
                return t;
            } else {
                if (index.orderDimension.Equals("")) {
                    section.Add(tuple);
                } else {
                    int loc = index.FindLocation(section, tuple);
                    section.Insert(loc, tuple);
                }
                return tuple;
            }
        }

        abstract protected void RemoveTuple(GMRTuple deletion);

    }

}

