using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
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
            if (root.delta != null) {
                delta = Enumerate();
            } else {
                delta = new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(new HashSet<GMRTuple>(), new HashSet<GMRTuple>());
            }
            
            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> unionDelta = FindUnionData(datasetUpdates, isUpdate);
            if (delta.Item1.Count > 0 || delta.Item2.Count > 0 ||
                unionDelta.Item1.Count > 0 || unionDelta.Item2.Count > 0) {
                Index outputDataset = FindOutputDataset(isUpdate);
                datasetUpdates.Add(modelName, new Update() {
                    projectedAddedTuples = delta.Item1.Union(unionDelta.Item1).ToHashSet(),
                    projectedRemovedTuples = delta.Item2.Union(unionDelta.Item2).ToHashSet()
                });
                foreach (GMRTuple tuple in datasetUpdates[modelName].projectedAddedTuples) {
                    AddTuple(outputDataset, tuple);
                }
                foreach (GMRTuple tuple in datasetUpdates[modelName].projectedRemovedTuples) {
                    RemoveTuple(outputDataset, tuple);
                }

                if (saveTree) SaveOutputIndex(outputDataset);
                if (saveOutput) SaveOutput(outputDataset);
            }
        }

        private Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> Enumerate() {
            List<string> combinedHeader = root.RetrieveHeader();
            if (uniteWeekAndYearValues) {
                int firstWeekIndex = Utils.GetWeekIndex(outputVariables);
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    Enumerate(root.delta.projectedAddedTuples, combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet(),
                    Enumerate(root.delta.projectedRemovedTuples, combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet());
            } else {
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    Enumerate(root.delta.projectedAddedTuples, combinedHeader).ToHashSet(),
                    Enumerate(root.delta.projectedRemovedTuples, combinedHeader).ToHashSet());
            }
        }

        virtual protected IEnumerable<GMRTuple> Enumerate(HashSet<GMRTuple> tupleList, List<string> combinedHeader) {
            foreach (GMRTuple t in tupleList) {
                foreach (List<string> s in root.Enumerate(t)) {
                    yield return CreateTuple(outputVariables, combinedHeader, s);
                }
            }
        }

        private Index FindOutputDataset(bool isUpdate) {
            if (!isUpdate) {
                List<string> eqJoinHeader;
                if (this.GetType().Name.Equals("SpeedAggregateReductTree")) {
                    SpeedAggregateReductTree SART = (SpeedAggregateReductTree)this;
                    eqJoinHeader = new List<string>();
                    foreach (string val in root.index.eqJoinHeader) {
                        if (!val.Equals(SART.r1)) eqJoinHeader.Add(val);
                    }
                } else {
                    eqJoinHeader = root.index.eqJoinHeader;
                }
                string newOrderDimension = root.orderDimension.Equals("") ? "" : outputHeader[outputVariables.IndexOf(root.orderDimension)];
                List<string> newOutputVariables;
                if (uniteWeekAndYearValues) {
                    newOutputVariables = new List<string>();
                    for (int i = 0; i < outputVariables.Count; i++) {
                        if (!outputVariables[i].Contains("week.w")) newOutputVariables.Add(outputVariables[i]);
                    }
                } else {
                    newOutputVariables = outputVariables;
                }
                return new Index(newOrderDimension) {
                    tupleMap = new Dictionary<string, List<GMRTuple>>(),
                    header = outputHeader,
                    eqJoinHeader = eqJoinHeader.Select(val => outputHeader[newOutputVariables.IndexOf(val)]).ToList()
                };
            } else {
                return GetOutputIndex();
            }
        }

        private Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> FindUnionData(Dictionary<string, Update> datasetUpdates, bool isUpdate) {
            if (!isUpdate) {
                return LoadUnionData(datasetUpdates);
            } else {
                return GetUnionData(datasetUpdates);
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

        private void SaveOutput(Index index) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}";

            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            using (StreamWriter file = new StreamWriter(@$"{location}\output.txt", false)) {
                file.Write(outputHeader[0]);
                for (int i = 1; i < outputHeader.Count; i++) {
                    file.Write($";{outputHeader[i]}");
                }
                file.WriteLine("");
                foreach (List<GMRTuple> outputDataset in index.tupleMap.Values) {
                    foreach (GMRTuple tuple in outputDataset) {
                        file.Write(tuple.fields[0]);
                        for (int i = 1; i < outputHeader.Count; i++) {
                            file.Write($";{tuple.fields[i]}");
                        }
                        file.WriteLine("");
                    }
                }
            };

        }

        private Index GetOutputIndex() {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}\index_binaries";
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            using (FileStream fs = new FileStream(@$"{location}\outputINDEX.dat", FileMode.Open)) {
                BinaryFormatter bf = new BinaryFormatter();
                return (Index)bf.Deserialize(fs);

            }
        }

        private void SaveOutputIndex(Index index) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}\index_binaries";
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            using (FileStream fs = new FileStream(@$"{location}\outputINDEX.dat", FileMode.Create)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, index);

            }

        }

        private void SaveIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}\index_binaries";
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
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

        private void LoadIndices(NodeReduct nodeReduct) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\{modelName}\index_binaries";
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
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

        private Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> LoadUnionData(Dictionary<string, Update> datasetUpdates) {
            HashSet<GMRTuple> addDelta = new HashSet<GMRTuple>();
            HashSet<GMRTuple> removeDelta = new HashSet<GMRTuple>();
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\input\";
            foreach (string dataset in unionData) {
                if (datasetUpdates.ContainsKey(dataset)) {
                    addDelta.UnionWith(datasetUpdates[dataset].projectedAddedTuples);
                    removeDelta.UnionWith(datasetUpdates[dataset].projectedRemovedTuples);
                } else {
                    addDelta.UnionWith(Utils.CSVToTupleSet(@$"{location}\{dataset}.txt", outputHeader));
                }
            }
            return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(addDelta, removeDelta);
        }

        private Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> GetUnionData(Dictionary<string, Update> datasetUpdates) {
            HashSet<GMRTuple> addDelta = new HashSet<GMRTuple>();
            HashSet<GMRTuple> removeDelta = new HashSet<GMRTuple>();
            foreach (string dataset in unionData) {
                if (datasetUpdates.ContainsKey(dataset)) {
                    addDelta.UnionWith(datasetUpdates[dataset].projectedAddedTuples);
                    removeDelta.UnionWith(datasetUpdates[dataset].projectedRemovedTuples);
                } 
            }
            return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(addDelta, removeDelta);
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
            Index result = new Index(orderDimension) {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = new List<string>(child.variables)
            };
            List<string> childVars = child.CopyVars();
            HashSet<string> allEquiVars = Utils.GetEquiVars(parent.predicates);
            //HashSet<string> allInEquiVars = Utils.GetInEquiVars(parent.predicates); //Not possible, because b.week.w and length then are both ordervalues
            allEquiVars.UnionWith(parent.variables);
            if (!result.orderDimension.Equals("")) {
                allEquiVars = Utils.SetMinus(allEquiVars, new HashSet<string> { orderDimension });
            }
            result.eqJoinHeader = Utils.Intersect(childVars, allEquiVars);
            return result;
        }

        public void AddTuple(Index index, GMRTuple tuple) {
            List<GMRTuple> section = index.GetOrPlace(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t != null) {
                t.count = t.count; //Don't do anything
            } else {
                if (index.orderDimension.Equals("")) {
                    section.Add(tuple);
                } else {
                    int loc = index.FindLocation(section, tuple);
                    section.Insert(loc, tuple);
                }
            }
        }

        protected void RemoveTuple(Index index, GMRTuple tuple) {
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
