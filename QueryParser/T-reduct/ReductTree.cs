﻿using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.QueryParser.TreeNodes.Terminals;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.NewParser;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;


namespace AssembleIVM.T_reduct {
    class ReductTree {
        public InnerNodeReduct root;
        public HashSet<NodeReduct> leafs;
        public string modelName;
        public List<string> unionData;
        List<HashSet<NodeReduct>> nodesPerLevel;
        public List<string> outputHeader;
        protected List<string> outputVariables;

        public bool splitWeekAndYearValues;
        public bool uniteWeekAndYearValues;

        public ReductTree(GeneralJoinTree GJT, string modelName) {
            this.modelName = modelName;
            this.unionData = GJT.unionData;
            this.splitWeekAndYearValues = GJT.splitWeekAndYearValues;
            this.uniteWeekAndYearValues = GJT.uniteWeekAndYearValues;
            this.nodesPerLevel = new List<HashSet<NodeReduct>>();
            this.root = (InnerNodeReduct)GJT.root.GenerateReduct(modelName);
            this.leafs = new HashSet<NodeReduct>();
            this.outputHeader = GJT.outputHeader;
            this.outputVariables = GJT.outputVariables;
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
            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> delta;
            List<GMRTuple> outputDataset;
            if (!isUpdate) {
                List<GMRTuple> result = Enumerate();
                List<GMRTuple> unionData = LoadUnionData(datasetUpdates);
                outputDataset = result.Union(unionData).ToList();
                delta = new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(new HashSet<GMRTuple>(outputDataset), new HashSet<GMRTuple>());
            } else {
                delta = null;
                outputDataset = null;
            }
            if (saveOutput) SaveOutput(outputDataset);
            datasetUpdates.Add(modelName, new Update() { projectedAddedTuples = delta.Item1, projectedRemovedTuples = delta.Item2 });
        }

        private List<GMRTuple> Enumerate() {
            if (uniteWeekAndYearValues) {
                int firstWeekIndex = Utils.GetWeekIndex(outputVariables);
                return Enumerate(root)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToList();
            } else {
                return Enumerate(root).ToList();
            }
        }

        virtual protected IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root) {
            List<string> combinedHeader = root.RetrieveHeader();
            foreach (List<GMRTuple> tupleList in root.index.tupleMap.Values) {
                foreach (GMRTuple t in tupleList) {
                    foreach (List<string> s in root.Enumerate(t)) {
                        yield return CreateTuple(outputVariables, combinedHeader, s);
                    }
                }
            }
        }

        protected GMRTuple CreateTuple(List<string> headerVariables, List<string> combinedHeader, List<string> variables) {
            List<string> result = new List<string>();
            foreach (string headerVariable in headerVariables) {
                TreeNode t = new AlgebraicExpressionParser().Parse(new Tokenizer(new StreamObject(headerVariable)));
                if (t.GetType().Name.Equals("DimensionName") ||
                t.GetType().Name.Equals("RelationAttribute")) {
                    result.Add(t.FindValue(combinedHeader, variables.ToArray()));
                } else if (t.GetType().Name.Equals("Number")) {
                    result.Add(t.FindValue(combinedHeader, variables.ToArray()));
                } else if (t.GetType().Name.Equals("Function")) {
                    result.Add(t.FindValue(combinedHeader, variables.ToArray()));
                } else if (t.GetType().IsSubclassOf(typeof(AlgebraicExpression))) {
                    AlgebraicExpression algebraicExpression = (AlgebraicExpression)t;
                    result.Add(Convert.ToString(algebraicExpression.Compute(combinedHeader, variables.ToArray()).value));
                } else {
                    throw new Exception($"Missed type {t.GetType().Name}, full name: {t.GetType().FullName}");
                }
            }
            return new GMRTuple(result.Count, 1) {
                fields = result.ToArray()
            };
        }

        private List<int> GetIndices(List<string> headerVariables, List<string> combinedHeader) {
            List<int> result = new List<int>();
            foreach (string headerVariable in headerVariables) {
                result.Add(combinedHeader.IndexOf(headerVariable));
            }
            return result;
        }

        private void SaveOutput(List<GMRTuple> outputDataset) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";

            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            using (StreamWriter file = new StreamWriter(@$"{location}\output.txt", false)) {
                file.Write(outputHeader[0]);
                for (int i = 1; i < outputHeader.Count; i++) {
                    file.Write($";{outputHeader[i]}");
                }
                file.WriteLine("");
                foreach (GMRTuple tuple in outputDataset) {
                    file.Write(tuple.fields[0]);
                    for (int i = 1; i < outputHeader.Count; i++) {
                        file.Write($";{tuple.fields[i]}");
                    }
                    file.WriteLine("");
                }
            };

        }

        private void SaveIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";
            using (FileStream fs = new FileStream(@$"{location}\{nodeReduct.name}INDEX.dat", FileMode.Create)) {
                BinaryFormatter bf = new BinaryFormatter();
                nodeReduct.Serialize(bf, fs);
            }
            if (!leafs.Contains(nodeReduct)) {
                InnerNodeReduct n = (InnerNodeReduct)nodeReduct;
                foreach (NodeReduct child in n.children) {
                    SaveIndices(child);
                }
            }
        }

        private List<GMRTuple> LoadUnionData(Dictionary<string, Update> datasetUpdates) {
            List<GMRTuple> result = new List<GMRTuple>();
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\input\";
            foreach (string dataset in unionData) {
                if (datasetUpdates.ContainsKey(dataset)) {
                    result = result.Union(datasetUpdates[dataset].projectedAddedTuples.ToList()).ToList();
                } else {
                    result = result.Union(Utils.CSVToTupleSet(@$"{location}\{dataset}.txt", outputHeader).ToList()).ToList();
                }
            }
            return result;
        }


        private void LoadIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";
            using (FileStream fs = new FileStream(@$"{location}\{nodeReduct.name}INDEX.dat", FileMode.Open)) {
                BinaryFormatter bf = new BinaryFormatter();
                nodeReduct.Deserialize(bf, fs);
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
                if (splitWeekAndYearValues && Utils.HasWeekValue(leaf.variables)) {
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
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null && !node.GetType().Name.Equals("LeafReduct")) node.ProjectUpdate();
                }
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null) {
                        node.ApplyUpdate();
                        if (node.delta != null && node != root) node.ComputeParentUpdate();
                    }
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
                    child.index = NewIndex(n, child, child.orderDimension);
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
            return new Index(r.children[i].index.orderDimension) {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = new List<string>(r.variables),
                eqJoinHeader = r.children[i].index.eqJoinHeader,
                orderDimension = r.children[i].index.orderDimension
            };
        }

        private Index NewIndex(InnerNodeReduct parent, NodeReduct child, string orderDimension) {
            Index index = new Index(orderDimension) {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = new List<string>(child.variables)
            };
            List<string> childVars = child.CopyVars();
            HashSet<string> allEquiVars = Utils.GetEquiVars(parent.predicates);
            //HashSet<string> allInEquiVars = Utils.GetInEquiVars(parent.predicates); //Not possible, because b.week.w and length then are both ordervalues
            allEquiVars.UnionWith(parent.variables);
            if (!index.orderDimension.Equals("")) {
                allEquiVars = Utils.SetMinus(allEquiVars, new HashSet<string> { orderDimension });
            }
            index.eqJoinHeader = Utils.Intersect(childVars, allEquiVars);
            return index;
        }


    }


}
