using AssembleIVM.QueryParser.TreeNodes.Terminals;
using QueryParser.GJTComputerFiles;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace AssembleIVM.T_reduct {
    class ReductTree {
        public NodeReduct root;
        public HashSet<NodeReduct> leafs;
        public string modelName;
        List<HashSet<NodeReduct>> nodesPerLevel;

        public bool splitWeekAndYearValues;

        public ReductTree(GeneralJoinTree GJT, string modelName) {
            this.modelName = modelName;
            this.splitWeekAndYearValues = GJT.splitWeekAndYearValues;
            this.nodesPerLevel = new List<HashSet<NodeReduct>>();
            this.root = GJT.root.GenerateReduct(modelName);
            this.leafs = new HashSet<NodeReduct>();
            SetDeltasAndLevels(root, 0);
        }

        public void RunModel(Dictionary<string, Update> datasetUpdates, bool isUpdate, bool saveTree, bool saveOutput) {
            if (!isUpdate) {
                InitIndices(root);
                InitLeafUpdates(datasetUpdates);
            } else {
                LoadIndices(root);
                LoadLeafUpdates(datasetUpdates);
            }
            UpdateTree();
            if (saveTree) SaveIndices(root);
        }

        private void SaveIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";
            using (FileStream fs = new FileStream(@$"{location}\{nodeReduct.name}INDEX.dat", FileMode.Create)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, nodeReduct.index);
            }
            if (!leafs.Contains(nodeReduct)) {
                InnerNodeReduct n = (InnerNodeReduct)nodeReduct;
                foreach (NodeReduct child in n.children) {
                    SaveIndices(child);
                }
            }
        }


        private void LoadIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";
            using (FileStream fs = new FileStream(@$"{location}\{nodeReduct.name}INDEX.dat", FileMode.Open)) {
                BinaryFormatter bf = new BinaryFormatter();
                nodeReduct.index = (Index)bf.Deserialize(fs);
            }
            if (!leafs.Contains(nodeReduct)) {
                InnerNodeReduct n = (InnerNodeReduct)nodeReduct;
                foreach (NodeReduct child in n.children) {
                    LoadIndices(child);
                }
            }
        }

        private void InitLeafUpdates(Dictionary<string, Update> datasetUpdates) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\input\";

            foreach (LeafReduct leaf in leafs) {
                if (datasetUpdates.ContainsKey(leaf.dataset)) {
                    leaf.delta = datasetUpdates[leaf.dataset];
                } else {
                    leaf.delta = new Update {
                        projectedAddedTuples = Utils.CSVToTupleSet(@$"{location}\{leaf.dataset}.txt", leaf.variables),
                        projectedRemovedTuples = new HashSet<GMRTuple>()
                    };
                }
                if (splitWeekAndYearValues) {
                    leaf.delta = leaf.delta.SplitWeekAndYearValues(Utils.GetWeekIndex(leaf.variables));
                }
            }
        }

        private void LoadLeafUpdates(Dictionary<string, Update> datasetUpdates) {
            foreach (LeafReduct leaf in leafs) {
                if (datasetUpdates.ContainsKey(leaf.dataset)) {
                    leaf.delta = datasetUpdates[leaf.dataset];
                } 
            }
        }

        private void UpdateTree() {
            for (int i = nodesPerLevel.Count - 1; i > -1; i--) {
                HashSet<NodeReduct> nodes = nodesPerLevel[i];
                foreach(NodeReduct node in nodes) {
                    if (node.delta != null && !node.GetType().Name.Equals("LeafReduct")) node.ProjectUpdate();
                }
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null) node.ApplyUpdate();
                }
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null && node != root) node.ComputeParentUpdate();
                }        
            }
        }

        private void SetDeltasAndLevels(NodeReduct nodeReduct, int level) {
            if (nodesPerLevel.Count == level) {
                nodesPerLevel.Add(new HashSet<NodeReduct>());
            }
            nodesPerLevel[level].Add(nodeReduct);
            if (!nodeReduct.GetType().Name.Equals("LeafReduct")) {
                InnerNodeReduct n = (InnerNodeReduct)nodeReduct;
                n.delta = null;
                foreach (NodeReduct child in n.children) {
                    SetDeltasAndLevels(child, level + 1);
                }
            } else {
                leafs.Add(nodeReduct);
            }
        }

        private void InitIndices(NodeReduct nodeReduct) {
            if (!leafs.Contains(nodeReduct)) {
                InnerNodeReduct n = (InnerNodeReduct)nodeReduct;
                foreach (NodeReduct child in n.children) {
                    child.index = NewIndex(n, child);
                    InitIndices(child);
                }
            }
            if (nodeReduct == root) {
                InnerNodeReduct r = (InnerNodeReduct)nodeReduct;
                nodeReduct.index = RootIndex(r);              
            }
        }

        private Index RootIndex(InnerNodeReduct r) {
            int i = (r.predicates[0] == null) ? 0 : 1;
            return new Index {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = new List<string>(r.variables),
                eqJoinHeader = r.children[i].index.eqJoinHeader,
                orderHeader = r.children[i].index.orderHeader
            };
        }

        private Index NewIndex(InnerNodeReduct parent, NodeReduct child) {
            Index index = new Index {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = new List<string>(child.variables)
            };
            List<string> childVars = child.CopyVars();
            HashSet<string> allEquiVars = Utils.GetEquiVars(parent.predicates);
            allEquiVars.UnionWith(parent.variables);
            index.eqJoinHeader = Utils.Intersect(childVars, allEquiVars);
            return index;
        }


    }


}
