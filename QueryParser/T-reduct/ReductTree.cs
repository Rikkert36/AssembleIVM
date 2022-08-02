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

        public void GetCurrentOutputSet(Dictionary<string, HashSet<string>> oldDatasetMap) {
            Index outputDataset = GetOutputIndex();
            HashSet<string> result = new HashSet<string>();
            foreach (List<GMRTuple> tupleList in outputDataset.tupleMap.Values) {
                foreach (GMRTuple tuple in tupleList) {
                    result.Add(tuple.GetString());
                }
            }
            oldDatasetMap.Add(modelName, result);
        }

        public void UpdateModel(Dictionary<string, Update> datasetUpdates, bool saveTree, bool saveOutput) {
            LoadIndices(root);
            LoadLeafUpdates(datasetUpdates);

            PrintLeafSize();
            PrintLeafDeltaSize();

            TupleCounter.StartUpdating();
            UpdateTree();
            TupleCounter.StopUpdating();
            Console.WriteLine("");
            if (saveTree) SaveIndices(root);


            PrintRootDeltaSize();
            TupleCounter.Push("Enumeration");
            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> delta;
            if (root.delta != null) {
                delta = EnumerateDelta();
            } else {
                delta = new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(new HashSet<GMRTuple>(), new HashSet<GMRTuple>());
            }
            TupleCounter.Pop("Enumeration");

            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> unionDelta = GetUnionData(datasetUpdates);
            if (delta.Item1.Count > 0 || delta.Item2.Count > 0 ||
                unionDelta.Item1.Count > 0 || unionDelta.Item2.Count > 0) {
                Index outputDataset = GetOutputIndex();

                IEnumerable<GMRTuple> addedTuples = delta.Item1.Union(unionDelta.Item1).ToHashSet();
                IEnumerable<GMRTuple> removedTuples = delta.Item2.Union(unionDelta.Item2).ToHashSet();


                Update update = new Update(outputHeader, new List<string> { });
                update.SetAddedTuples(addedTuples);
                update.SetRemovedTuples(removedTuples);

                TupleCounter.Push("Add to output");
                datasetUpdates.Add(modelName, update);
                int outputDataCount = 0;
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetAddedTuples()) {
                    outputDataCount++;
                    AddTuple(outputDataset, tuple);
                }
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetRemovedTuples()) {
                    outputDataCount++;
                    RemoveTuple(outputDataset, tuple);
                }
                Console.WriteLine($"{outputDataCount}");
                TupleCounter.Pop("Add to output");

                if (saveTree) SaveOutputIndex(outputDataset);
                if (saveOutput) SaveOutput(outputDataset);

            } else {
                Console.WriteLine("0 \n0");
            }

        }

        public void RunModel(Dictionary<string, Update> datasetUpdates, bool saveTree, bool saveOutput) {
            InitIndices(root);
            InitLeafUpdates(datasetUpdates);

            UpdateTreeInitial();

            if (saveTree) SaveIndices(root);

            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> delta = EnumerateFullTree();

            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> unionDelta = LoadUnionData(datasetUpdates);
            if (delta.Item1.Count > 0 || delta.Item2.Count > 0 ||
                unionDelta.Item1.Count > 0 || unionDelta.Item2.Count > 0) {
                Index outputDataset = FindOutputDataset();

                IEnumerable<GMRTuple> addedTuples = delta.Item1.Union(unionDelta.Item1).ToHashSet();
                IEnumerable<GMRTuple> removedTuples = delta.Item2.Union(unionDelta.Item2).ToHashSet();

                Update update = new Update(outputHeader, new List<string> { });
                update.SetAddedTuples(addedTuples);
                update.SetRemovedTuples(removedTuples);

                datasetUpdates.Add(modelName, update);
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetAddedTuples()) {
                    AddTuple(outputDataset, tuple);
                }
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetRemovedTuples()) {
                    RemoveTuple(outputDataset, tuple);
                }

                if (saveTree) SaveOutputIndex(outputDataset);
                if (saveOutput) SaveOutput(outputDataset);

            }
        }

        public void RunFullModelWithSomeUpdates(Dictionary<string, Update> datasetUpdates, Dictionary<string, Update> extraUpdates, bool saveTree, bool saveOutput) {
            InitIndices(root);
            InitLeafUpdates(datasetUpdates);
            PrintLeafSize();
            PrintLeafDeltaSize();

            TupleCounter.Push("Update");
            TupleCounter.Push("Apply update");
            ApplyExtraLeafUpdates(extraUpdates);
            TupleCounter.Pop("Apply update");
            TupleCounter.Push("Compute delta");
            UpdateTreeInitial();
            TupleCounter.Pop("Compute delta");
            TupleCounter.Pop("Update");
            Console.WriteLine("");

            if (saveTree) SaveIndices(root);

            PrintRootDeltaSize();
            TupleCounter.Push("Enumeration");
            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> delta = EnumerateFullTree();
            TupleCounter.Pop("Enumeration");

            Console.WriteLine(delta.Item1.Count + delta.Item2.Count);


            Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> unionDelta = LoadUnionData(datasetUpdates);
            if (delta.Item1.Count > 0 || delta.Item2.Count > 0 ||
                unionDelta.Item1.Count > 0 || unionDelta.Item2.Count > 0) {
                Index outputDataset = FindOutputDataset();

                IEnumerable<GMRTuple> addedTuples = delta.Item1.Union(unionDelta.Item1).ToHashSet();
                IEnumerable<GMRTuple> removedTuples = delta.Item2.Union(unionDelta.Item2).ToHashSet();

                Update update = new Update(outputHeader, new List<string> { });
                update.SetAddedTuples(addedTuples);
                update.SetRemovedTuples(removedTuples);
                TupleCounter.Push("Add to output");
                ApplyExtraOutputUpdates(update.projectedAddedTuples, extraUpdates);
                TupleCounter.Pop("Add to output");

                datasetUpdates.Add(modelName, update);
                int outputDataCount = 0;
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetAddedTuples()) {
                    outputDataCount++;
                    AddTuple(outputDataset, tuple);
                }
                foreach (GMRTuple tuple in datasetUpdates[modelName].GetRemovedTuples()) {
                    outputDataCount++;
                    RemoveTuple(outputDataset, tuple);
                }

                if (saveTree) SaveOutputIndex(outputDataset);
                if (saveOutput) SaveOutput(outputDataset);

            }
        }

        private Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> EnumerateFullTree() {
            List<string> combinedHeader = root.RetrieveHeader();
            if (uniteWeekAndYearValues) {
                int firstWeekIndex = Utils.GetWeekIndex(outputVariables);
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    Enumerate(root, combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet(),
                    new HashSet<GMRTuple>());
            } else {
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    Enumerate(root, combinedHeader).ToHashSet(),
                    new HashSet<GMRTuple>());
            }
        }

        virtual protected Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> EnumerateDelta() {
            List<string> combinedHeader = root.RetrieveHeader();
            if (uniteWeekAndYearValues) {
                int firstWeekIndex = Utils.GetWeekIndex(outputVariables);
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    EnumerateAdded(root.delta.GetAddedTuples().ToHashSet(), combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet(),
                    EnumerateRemoved(root.delta.GetRemovedTuples().ToHashSet(), combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet());
            } else {
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    EnumerateAdded(root.delta.GetAddedTuples().ToHashSet(), combinedHeader).ToHashSet(),
                    EnumerateRemoved(root.delta.GetRemovedTuples().ToHashSet(), combinedHeader).ToHashSet());
            }
        }

        virtual protected IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root, List<string> combinedHeader) {
            foreach (List<GMRTuple> tupleList in root.index.tupleMap.Values) {
                foreach (GMRTuple t in tupleList) {
                    foreach (List<string> s in root.Enumerate(t)) {
                        yield return CreateTuple(outputVariables, combinedHeader, s);
                    }
                }
            }
        }

        virtual protected IEnumerable<GMRTuple> EnumerateAdded(HashSet<GMRTuple> tupleList, List<string> combinedHeader) {
            foreach (GMRTuple t in tupleList) {
                foreach (List<string> s in root.EnumerateAdded(t)) {
                    yield return CreateTuple(outputVariables, combinedHeader, s);
                }
            }
        }

        virtual protected IEnumerable<GMRTuple> EnumerateRemoved(HashSet<GMRTuple> tupleList, List<string> combinedHeader) {
            foreach (GMRTuple t in tupleList) {
                foreach (List<string> s in root.EnumerateRemoved(t)) {
                    yield return CreateTuple(outputVariables, combinedHeader, s);
                }
            }
        }

        private Index FindOutputDataset() {
            List<string> eqJoinHeader;

            eqJoinHeader = root.index.eqJoinHeader;

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
                    addDelta.UnionWith(datasetUpdates[dataset].GetAddedTuples());
                    removeDelta.UnionWith(datasetUpdates[dataset].GetRemovedTuples());
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
                    addDelta.UnionWith(datasetUpdates[dataset].GetAddedTuples());
                    removeDelta.UnionWith(datasetUpdates[dataset].GetRemovedTuples());
                }
            }
            return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(addDelta, removeDelta);
        }


        private void InitLeafUpdates(Dictionary<string, Update> datasetUpdates) {
            string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\input\";
            foreach (LeafReduct leaf in leafs) {
                List<GMRTuple> addedTuples;
                List<GMRTuple> removedTuples;
                if (datasetUpdates.ContainsKey(leaf.dataset)) {
                    addedTuples = datasetUpdates[leaf.dataset].GetAddedTuples().ToList();
                    removedTuples = datasetUpdates[leaf.dataset].GetRemovedTuples().ToList();

                } else {
                    addedTuples = Utils.CSVToTupleSet(@$"{location}\{leaf.dataset}.txt", leaf.variables).ToList();
                    removedTuples = new List<GMRTuple>();
                }
                if (splitWeekAndYearValues && Utils.HasWeekValue(leaf.variables)) {
                    SplitWeekAndYearValues(addedTuples, Utils.GetWeekIndex(leaf.variables));
                    SplitWeekAndYearValues(removedTuples, Utils.GetWeekIndex(leaf.variables));
                }
                leaf.delta = new Update(leaf);
                leaf.delta.SetAddedTuples(addedTuples);
                leaf.delta.SetRemovedTuples(removedTuples);

            }
        }

        private void ApplyExtraLeafUpdates(Dictionary<string, Update> extraUpdates) {
            foreach (LeafReduct leaf in leafs) {
                if (extraUpdates.ContainsKey(leaf.dataset)) {
                    Index added = leaf.delta.projectedAddedTuples;
                    foreach (GMRTuple t in extraUpdates[leaf.dataset].GetRemovedTuples()) {
                        RemoveTuple(added, t);
                    }
                    foreach (GMRTuple t in extraUpdates[leaf.dataset].GetAddedTuples()) {
                        AddTuple(added, t);
                    }
                }
            }
        }

        private void ApplyExtraOutputUpdates(Index index, Dictionary<string, Update> extraUpdates) {
            foreach (string dataset in unionData) {
                if (extraUpdates.ContainsKey(dataset)) {
                    foreach (GMRTuple t in extraUpdates[dataset].GetRemovedTuples()) {
                        RemoveTuple(index, t);
                    }
                    foreach (GMRTuple t in extraUpdates[dataset].GetAddedTuples()) {
                        AddTuple(index, t);
                    }
                }
            }
        }

        public void SplitWeekAndYearValues(List<GMRTuple> tupleList, int weekIndex) {
            for (int i = 0; i < tupleList.Count; i++) {
                tupleList[i] = tupleList[i].SplitWeekAndYearValue(weekIndex);
            }
        }


        private void LoadLeafUpdates(Dictionary<string, Update> datasetUpdates) {
            foreach (LeafReduct leaf in leafs) {
                if (datasetUpdates.ContainsKey(leaf.dataset)) {
                    if (splitWeekAndYearValues && Utils.HasWeekValue(leaf.variables)) {
                        leaf.delta = datasetUpdates[leaf.dataset].NewUpdate(leaf, true);
                    } else {
                        leaf.delta = datasetUpdates[leaf.dataset].NewUpdate(leaf, false);
                    }
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
                        TupleCounter.StartApplyingUpdate();
                        node.ApplyUpdate();
                        TupleCounter.StartComputingDelta();
                        if (node.delta != null && node != root) node.ComputeParentUpdate();
                    }
                }
            }
        }

        private void UpdateTreeInitial() {
            for (int i = nodesPerLevel.Count - 1; i > -1; i--) {
                HashSet<NodeReduct> nodes = nodesPerLevel[i];
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null && !node.GetType().Name.Equals("LeafReduct")) node.ProjectUpdate();
                }
                foreach (NodeReduct node in nodes) {
                    if (node.delta != null) {
                        node.ApplyUpdate();
                        if (node.delta != null && node != root) node.ComputeParentUpdateInitial();
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

        private void PrintRootDeltaSize() {
            int count = 0;
            if (root.delta != null) {
                foreach (GMRTuple tuple in root.delta.GetAddedTuples()) {
                    count++;
                }
                foreach (GMRTuple tuple in root.delta.GetRemovedTuples()) {
                    count++;
                }
            }
            Console.WriteLine(count);
        }

        private void PrintLeafSize() {
            int count = 0;
            foreach (LeafReduct leaf in leafs) {
                foreach (List<GMRTuple> tupleList in leaf.index.tupleMap.Values) {
                    count += tupleList.Count;
                }
            }
            Console.WriteLine($"{count}");
        }

        private void PrintLeafDeltaSize() {
            int count = 0;
            foreach (LeafReduct leaf in leafs) {
                if (leaf.delta != null) {
                    foreach (List<GMRTuple> tupleList in leaf.delta.projectedAddedTuples.tupleMap.Values) {
                        count += tupleList.Count;
                    }
                    foreach (List<GMRTuple> tupleList in leaf.delta.projectedRemovedTuples.tupleMap.Values) {
                        count += tupleList.Count;
                    }
                }
            }
            Console.WriteLine($"{count}");
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
            if (t == null) {
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
