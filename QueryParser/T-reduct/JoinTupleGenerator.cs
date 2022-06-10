using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.QueryParser.TreeNodes.Terminals;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class JoinTupleGenerator {

        public List<string> eqJoinHeader;
        public List<string[]> eqJoinTuples;

        public List<string> orderHeader;
        public List<Bound[][]> rangeValueTuples;

        public JoinTupleGenerator(List<string> eqJoinHeader, List<string> orderHeader, List<string> rightTupleHeader,
            GMRTuple rightTuple, TreeNode predicate) {
            this.eqJoinHeader = eqJoinHeader;
            this.orderHeader = orderHeader;
            this.eqJoinTuples = new List<string[]>();
            this.rangeValueTuples = new List<Bound[][]>();
            List<Tuple<HashSet<Comparison>, HashSet<Comparison>>> tupleList =
                new List<Tuple<HashSet<Comparison>, HashSet<Comparison>>>();
            Tuple<HashSet<Comparison>, HashSet<Comparison>> initialTuple =
                new Tuple<HashSet<Comparison>, HashSet<Comparison>>
                (new HashSet<Comparison>(), new HashSet<Comparison>());
            tupleList.Add(initialTuple);
            RecurseTree(predicate, tupleList, initialTuple);
            ConvertJoinTuples(tupleList, rightTupleHeader, rightTuple);
        }

        public bool DoesJoin(Index index) {
            for (int i = 0; i < eqJoinTuples.Count; i++) {
                List<GMRTuple> section = index.Get(eqJoinTuples[i]);
                if (section.Count > 0) {
                    if (rangeValueTuples[i].Length == 0) {
                        return true;
                    } else if (rangeValueTuples[i][0][0] != null && rangeValueTuples[i][0][1] != null) {
                        Bound lowerBound = rangeValueTuples[i][0][0];
                        Bound upperBound = rangeValueTuples[i][0][1];
                        if (lowerBound.ViolatesTuple(section[0], index.header, orderHeader[0], true) ||
                            upperBound.ViolatesTuple(section[section.Count - 1], index.header, orderHeader[0], false)) {
                            continue;
                        }
                        HashSet<int> seenIndices = new HashSet<int>();
                        int j = section.Count / 2;
                        while (true) {
                            if (seenIndices.Contains(j)) {
                                break;
                            }
                            seenIndices.Add(j);
                            if (lowerBound.ViolatesTuple(section[j], index.header, orderHeader[0], true)) {
                                int dif = section.Count - j;
                                int delta = ((section.Count - j) % 2 == 0) ? dif / 2 : dif / 2 + 1;
                                j += delta;
                            } else if (upperBound.ViolatesTuple(section[j], index.header, orderHeader[0], false)) {
                                int delta = (j % 2 == 0) ? j / 2 : j / 2 + 1;
                                j -= delta;
                            } else {
                                return true;
                            }
                        }
                    } else if (rangeValueTuples[i][0][0] != null) { //No upperbound
                        if(!rangeValueTuples[i][0][0]
                            .ViolatesTuple(section[section.Count - 1], index.header, orderHeader[0], true))
                            return true;
                    } else if (rangeValueTuples[i][0][1] != null) {
                        if(!rangeValueTuples[i][0][1]
                            .ViolatesTuple(section[0], index.header, orderHeader[0], false)) return true;
                    } else {
                        throw new Exception("more than 1 range dimension");
                    }
                } 
            }
            return false;
        }

        public List<GMRTuple> RetrieveCorrespondingTuples(Index index) {
            List<GMRTuple> result = new List<GMRTuple>();
            for (int i = 0; i < eqJoinTuples.Count; i++) {
                List<GMRTuple> section = index.Get(eqJoinTuples[i]);
                if (rangeValueTuples[i].Length == 0) {
                    result.AddRange(section);
                } else if (rangeValueTuples[i].Length == 1) {
                    if (rangeValueTuples[i][0][0] != null && rangeValueTuples[i][0][1] != null) {
                        //Do a binary search and get tuples in range
                        Bound lowerBound = rangeValueTuples[i][0][0];
                        Bound upperBound = rangeValueTuples[i][0][1];
                        if (lowerBound.ViolatesTuple(section[0], index.header, orderHeader[0], true) ||
                            upperBound.ViolatesTuple(section[section.Count - 1], index.header, orderHeader[0], false)) {
                            continue;
                        }
                        HashSet<int> seenIndices = new HashSet<int>();
                        bool nothingInRange = false;
                        int j = section.Count / 2;
                        while (true) {
                            if (seenIndices.Contains(j)) {
                                nothingInRange = true;
                                break;
                            }
                            seenIndices.Add(j);
                            if (lowerBound.ViolatesTuple(section[j], index.header, orderHeader[0], true)) {
                                int dif = section.Count - j;
                                int delta = ((section.Count - j) % 2 == 0) ? dif / 2 : dif / 2 + 1;
                                j += delta;
                            } else if (upperBound.ViolatesTuple(section[j], index.header, orderHeader[0], false)) {
                                int delta = (j % 2 == 0) ? j / 2 : j / 2 + 1;
                                j -= delta;
                            } else {
                                break;
                            }
                        }
                        if (nothingInRange) continue;
                        for (int k = j; k > -1 && !lowerBound.ViolatesTuple(section[k], index.header, orderHeader[0], true); k--) {
                            result.Add(section[k]);
                        }
                        for (int l = j + 1; l < section.Count && !upperBound.ViolatesTuple(section[l], index.header, orderHeader[0], false); l--) {
                            result.Add(section[l]);
                        }
                    } else if (rangeValueTuples[i][0][0] != null) {
                        //Get all tuples from end to lower bound
                        Bound lowerBound = rangeValueTuples[i][0][0];
                        for (int j = section.Count - 1; j > -1 && !lowerBound.ViolatesTuple(section[j], index.header, orderHeader[0], true); j--) {
                            result.Add(section[j]);
                        }
                    } else if (rangeValueTuples[i][0][1] != null) {
                        Bound upperBound = rangeValueTuples[i][0][1];
                        for (int j = 0; j < section.Count && !upperBound.ViolatesTuple(section[j], index.header, orderHeader[0], false); j--) {
                            result.Add(section[j]);
                        }
                    } else {
                        throw new Exception("Range without lower-or-upperbound found");
                    }
                } else {
                    throw new Exception("Found predicate with > 1 range value, a corresponding-tuple finding algorithm for this case has yet to be implemented");
                }
            }
            return result;
        }

        private void ConvertJoinTuples(List<Tuple<HashSet<Comparison>, HashSet<Comparison>>> tupleList,
            List<string> rightTupleHeader, GMRTuple rightTuple) {
            foreach (Tuple<HashSet<Comparison>, HashSet<Comparison>> tuple in tupleList) {
                Dictionary<string, TreeNode> eqVarMap = new Dictionary<string, TreeNode>();
                Dictionary<string, Bound[]> rangeVarMap = new Dictionary<string, Bound[]>();
                foreach (Comparison comparison in tuple.Item1) {
                    Tuple<string, string, TreeNode> map = IsolateVariable(comparison, new HashSet<string>(eqJoinHeader));
                    eqVarMap.Add(map.Item1, map.Item3);
                }
                foreach (Comparison comparison in tuple.Item2) {
                    Tuple<string, string, TreeNode> map = IsolateVariable(comparison, new HashSet<string>(orderHeader));
                    Bound[] range;
                    if(!rangeVarMap.ContainsKey(map.Item1)) {
                        range = new Bound[2];
                        rangeVarMap[map.Item1] = range;
                    }  else {
                        range = rangeVarMap[map.Item1];
                    }
                    if (map.Item2.Equals("<")) {
                        range[1] = new Bound(map.Item3, true);
                    } else if (map.Item2.Equals("<=")) {
                        range[1] = new Bound(map.Item3, false);
                    } else if (map.Item2.Equals(">")) {
                        range[0] = new Bound(map.Item3, true);
                    } else { //if (map.Item2.Equals("<="))
                        range[0] = new Bound(map.Item3, false);
                    }
                }
                string[] newEqJoinTuple = new string[eqJoinHeader.Count];
                for (int i = 0; i < eqJoinHeader.Count; i++) {
                    if (!eqVarMap.ContainsKey(eqJoinHeader[i])) {
                        if (rightTupleHeader.Contains(eqJoinHeader[i])) {
                            newEqJoinTuple[i] = rightTuple.fields[rightTupleHeader.IndexOf(eqJoinHeader[i])];
                        } else {
                            throw new Exception("Predicate yields tuples in which not all eqjoin variables are present");
                        }
                    } else {
                        TreeNode treeNode = eqVarMap[eqJoinHeader[i]];
                        if (treeNode.GetType().FullName.Contains("AlgebraicExpression")) {
                            AlgebraicExpression algebraicExpression = (AlgebraicExpression)treeNode;
                            newEqJoinTuple[i] = Convert.ToString(algebraicExpression.Compute(rightTupleHeader, rightTuple.fields).value);
                        } else if (treeNode.GetType().FullName.Contains("DimensionName") ||
                            treeNode.GetType().FullName.Contains("RelationAttribute")) {
                            newEqJoinTuple[i] = treeNode.FindValue(rightTupleHeader, rightTuple.fields); //Has to have a value;
                        } else if (treeNode.GetType().FullName.Contains("StringNode")) {
                            StringNode stringNode = (StringNode)treeNode;
                            newEqJoinTuple[i] = stringNode.value;
                        } else if (treeNode.GetType().FullName.Contains("NumberNode")) {
                            NumberNode numberNode = (NumberNode)treeNode;
                            newEqJoinTuple[i] = numberNode.value;
                        } else {
                            throw new Exception("Missed treenode type");
                        }
                    }
                }
                eqJoinTuples.Add(newEqJoinTuple);
                Bound[][] newRangeTuple = new Bound[orderHeader.Count][];
                for (int i = 0; i < orderHeader.Count; i++) {
                    Bound[] range = rangeVarMap[orderHeader[i]];
                    if (range[0] != null) {
                        range[0].node.FillInNumbers(rightTupleHeader, rightTuple.fields);
                    }
                    if (range[1] != null) {
                        range[1].node.FillInNumbers(rightTupleHeader, rightTuple.fields);
                    }
                    newRangeTuple[i] = range;
                }
                rangeValueTuples.Add(newRangeTuple);
            }
        }

        

        /*
         * Return Tuple<variable, compareOperator, algebraicexpression
         */
        private Tuple<string, string, TreeNode> IsolateVariable(Comparison comparison, HashSet<string> variableSet) {
            Comparison c = (Comparison)comparison.Clone();
            Tuple<bool, TreeNode> find = Utils.FindVariable(c, variableSet);
            if (!find.Item1) throw new Exception("There is a tuple without ordered dimension");
            TreeNode x = find.Item2;
            string name = x.GetString();
            bool inLeft = Utils.FindVariable(c.left, name).Item1;
            while (!(c.left == x || c.right == x)) {
                bool switched = false;
                TreeNode xSubTree;
                TreeNode oSubTree;
                TreeNode newTreeNode = null;
                TreeNode newSmallerTreeNode = null;
                if (inLeft) {
                    xSubTree = c.left;
                    oSubTree = c.right;
                } else {
                    xSubTree = c.right;
                    oSubTree = c.left;
                }
                switch (xSubTree.GetType().Name) {
                    case "Term": {
                            Term term = (Term)xSubTree;
                            TreeNode xxSubTree = Utils.FindVariable(term.left, name).Item1 ? term.left : term.right;
                            TreeNode xoSubTree = xxSubTree == term.left ? term.right : term.left;
                            if (term.termOperator.Equals("+")) {
                                newTreeNode = new Term(oSubTree, xoSubTree, "-");
                                newSmallerTreeNode = xxSubTree;
                            } else {
                                if (Utils.FindVariable(term.left, name).Item1) {
                                    newTreeNode = new Term(xoSubTree, oSubTree, "+");
                                    newSmallerTreeNode = xxSubTree;
                                } else {
                                    newTreeNode = new Term(xxSubTree, oSubTree, "+");
                                    newSmallerTreeNode = xoSubTree;
                                    inLeft = !inLeft;
                                    switched = true;
                                }
                            }
                            break;
                        }
                    case "Factor": {
                            Factor factor = (Factor)xSubTree;
                            TreeNode xxSubTree = Utils.FindVariable(factor.left, name).Item1 ? factor.left : factor.right;
                            TreeNode xoSubTree = xxSubTree == factor.left ? factor.right : factor.left;
                            if (factor.factorOperator.Equals("*")) {
                                newTreeNode = new Factor(oSubTree, xoSubTree, "/");
                                newSmallerTreeNode = xxSubTree;
                            } else {
                                if (Utils.FindVariable(factor.left, name).Item1) {
                                    newTreeNode = new Factor(xoSubTree, oSubTree, "*");
                                    newSmallerTreeNode = xxSubTree;
                                } else {
                                    newTreeNode = new Factor(xxSubTree, oSubTree, "*");
                                    newSmallerTreeNode = xoSubTree;
                                    inLeft = !inLeft;
                                    switched = true;
                                }
                            }
                            break;
                        }

                }
                if ((inLeft && !switched) || (!inLeft && switched)) {
                    c = new Comparison(newSmallerTreeNode, newTreeNode, c.compareOperator);
                } else {
                    c = new Comparison(newTreeNode, newSmallerTreeNode, c.compareOperator);
                }
            }
            TreeNode algebraicExpression = inLeft ? (TreeNode)c.right : (TreeNode)c.left;
            string newCompareOperator = inLeft ? c.compareOperator : Utils.Opposite(c.compareOperator);
            return new Tuple<string, string, TreeNode>(name, newCompareOperator, algebraicExpression);
        }

        private void RecurseTree(
            TreeNode predicate, List<Tuple<HashSet<Comparison>, HashSet<Comparison>>> tupleList,
            Tuple<HashSet<Comparison>, HashSet<Comparison>> currentTuple) {
            if (predicate.GetType().Name.Equals("Comparison")) {
                Comparison comparison = (Comparison)predicate;
                if (comparison.compareOperator.Equals("==")) {
                    currentTuple.Item1.Add(comparison);
                } else {
                    currentTuple.Item2.Add(comparison);
                }
            } else if (predicate.GetType().Name.Equals("And")) {
                And and = (And)predicate;
                RecurseTree(and.left, tupleList, currentTuple);
                RecurseTree(and.right, tupleList, currentTuple);
            } else if (predicate.GetType().Name.Equals("Or")) {
                Or or = (Or)predicate;
                RecurseTree(or.left, tupleList, currentTuple);
                Tuple<HashSet<Comparison>, HashSet<Comparison>> newTuple =
                    new Tuple<HashSet<Comparison>, HashSet<Comparison>>
                    (Clone(currentTuple.Item1), Clone(currentTuple.Item2));
                tupleList.Add(newTuple);
                RecurseTree(or.right, tupleList, newTuple);
            } 
        }


        private HashSet<Comparison> Clone(HashSet<Comparison> set) {
            return set.Select(TreeNode => TreeNode).ToHashSet();
        }


        


    }
}
