using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    abstract class NodeReduct {
        public readonly string[] variables;
        public bool inFrontier = false;
        public string name;
        public Index index;
        public InnerNodeReduct parent;

        public Update delta;

        public NodeReduct(string name, string[] variables) {
            this.variables = variables;
            this.name = name;
            this.index = new Index();
        }

        public List<string> CopyVars() {
            List<string> result = new List<string>();
            foreach (string r in variables) {
                result.Add(r);
            }
            return result;
        }

        public void SetParent(InnerNodeReduct parent) {
            this.parent = parent;
        }

        public void ComputeParentUpdate() {
            if (this.parent != null) {
                if (parent.delta == null ) parent.delta = new Update();
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
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t != null) {
                t.count += tuple.count;
                return t;
            } else {
                if (index.orderHeader.Count == 0) {
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

