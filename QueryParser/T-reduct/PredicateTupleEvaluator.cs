using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.QueryParser.TreeNodes.Terminals;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class PredicateTupleEvaluator {

        public bool Evaluate(List<string> header, GMRTuple tuple, TreeNode predicate) {
            Tuple<HashSet<Comparison>, HashSet<Comparison>> initialTuple =
                new Tuple<HashSet<Comparison>, HashSet<Comparison>>
                (new HashSet<Comparison>(), new HashSet<Comparison>());
            List<Tuple<HashSet<Comparison>, HashSet<Comparison>>> tupleList =
                new List<Tuple<HashSet<Comparison>, HashSet<Comparison>>> { initialTuple };
            RetrieveComparisons(predicate, tupleList, initialTuple);
            foreach (Tuple<HashSet<Comparison>, HashSet<Comparison>> compareSet in tupleList) {
                if (IsTrueComparisons(header, tuple, compareSet)) return true;
            }
            return false;
        }

        private bool IsTrueComparisons(List<string> header, GMRTuple tuple, Tuple<HashSet<Comparison>, 
            HashSet<Comparison>> compareSet) {
            foreach(Comparison comparison in compareSet.Item1) {
                if (!ComparisonIsTrue(header, tuple, comparison)) return false;
            }
            return true;
        }

        private bool ComparisonIsTrue(List<string> header, GMRTuple tuple, Comparison comparison) {
            string left = Solve(header, tuple, comparison.left);
            string right = Solve(header, tuple, comparison.right);
            switch(comparison.compareOperator) {
                case("=="):
                    return left == right;
                case (">"):
                    return double.Parse(left, CultureInfo.InvariantCulture) > double.Parse(right, CultureInfo.InvariantCulture);
                case (">="):
                    return double.Parse(left, CultureInfo.InvariantCulture) >= double.Parse(right, CultureInfo.InvariantCulture);
                case ("<"):
                    return double.Parse(left, CultureInfo.InvariantCulture) < double.Parse(right, CultureInfo.InvariantCulture);
                case ("<="):
                    return double.Parse(left, CultureInfo.InvariantCulture) <= double.Parse(right, CultureInfo.InvariantCulture);
                case ("!="):
                    return double.Parse(left, CultureInfo.InvariantCulture) != double.Parse(right, CultureInfo.InvariantCulture);
                default:
                    throw new Exception($"Unknown compare operator: {comparison.compareOperator}");
            }

        }

        private string Solve(List<string> header, GMRTuple tuple, TreeNode treeNode) {;
            if (treeNode.GetType().FullName.Contains("AlgebraicExpression")) {
                AlgebraicExpression algebraicExpression = (AlgebraicExpression)treeNode;
                return Convert.ToString(algebraicExpression.Compute(header, tuple.fields).value);
            } else if (treeNode.GetType().FullName.Contains("DimensionName") ||
                treeNode.GetType().FullName.Contains("RelationAttribute")) {
                return treeNode.FindValue(header, tuple.fields); //Has to have a value;
            } else if (treeNode.GetType().FullName.Contains("StringNode")) {
                StringNode stringNode = (StringNode)treeNode;
                return stringNode.value;
            } else if (treeNode.GetType().FullName.Contains("NumberNode")) {
                NumberNode numberNode = (NumberNode)treeNode;
                return numberNode.value;
            } else {
                throw new Exception("Missed treenode type");
            }
        }

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

        private void RetrieveComparisons(TreeNode predicate, List<Tuple<HashSet<Comparison>,
            HashSet<Comparison>>> tupleList, Tuple<HashSet<Comparison>, HashSet<Comparison>> currentTuple) {
            if (predicate.GetType().Name.Equals("Comparison")) {
                Comparison comparison = (Comparison)predicate;
                if (comparison.compareOperator.Equals("==")) {
                    currentTuple.Item1.Add(comparison);
                } else {
                    currentTuple.Item2.Add(comparison);
                }
            } else if (predicate.GetType().Name.Equals("And")) {
                And and = (And)predicate;
                RetrieveComparisons(and.left, tupleList, currentTuple);
                RetrieveComparisons(and.right, tupleList, currentTuple);
            } else if (predicate.GetType().Name.Equals("Or")) {
                Or or = (Or)predicate;
                RetrieveComparisons(or.left, tupleList, currentTuple);
                Tuple<HashSet<Comparison>, HashSet<Comparison>> newTuple =
                    new Tuple<HashSet<Comparison>, HashSet<Comparison>>
                    (Clone(currentTuple.Item1), Clone(currentTuple.Item2));
                tupleList.Add(newTuple);
                RetrieveComparisons(or.right, tupleList, newTuple);
            }
        }
        private HashSet<Comparison> Clone(HashSet<Comparison> set) {
            return set.Select(TreeNode => TreeNode).ToHashSet();
        }
    }
}
