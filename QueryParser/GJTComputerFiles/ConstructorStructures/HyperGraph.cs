using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles.ConstructorStructures {
    class HyperGraph {
        public readonly List<GJTNode> hyperEdges;
        public readonly HashSet<HashSet<string>> predicates;
        HashSet<string> projectedVariables;

        public HyperGraph(List<GJTNode> hyperEdges, HashSet<HashSet<string>> predicates,
        HashSet<string> projectedVariables) {
            this.hyperEdges = hyperEdges;
            this.predicates = predicates;
            this.projectedVariables = projectedVariables;
        }

        public List<GJTNode> GetNodes() {
            return hyperEdges;
        }

        public void EmptyProjectedVariables() {
            this.projectedVariables = new HashSet<string> { };
        }

        public HashSet<string> GetProjectedVariables() {
            return this.projectedVariables;
        }

        public bool IsEmpty() {
            return hyperEdges.Count == 0 && predicates.Count == 0; 
        }

        public Tuple<HyperGraph, bool> AttemptReduction() {
            Tuple<HyperGraph, bool> FIL = AttemptFLT();
            if (FIL.Item2) return FIL;
            Tuple<HyperGraph, bool> CSE = AttemptCSE();
            if (CSE.Item2) return CSE;
            Tuple<HyperGraph, bool> ISO = AttemptISO();
            if (ISO.Item2) return ISO;         
            return new Tuple<HyperGraph, bool>(this, false);
        }

        private Tuple<HyperGraph, bool> AttemptFLT() {
            HashSet<HashSet<string>> newPredicates = new HashSet<HashSet<string>>();
            bool hasFiltered = false;
            foreach (HashSet<string> predicate in predicates) {
                bool isSubset = false;
                foreach (GJTNode node in hyperEdges) {
                    if (IsSubset(predicate, new HashSet<string>(node.variables))) {
                        isSubset = true;
                        hasFiltered = true;
                    }
                }
                if (!isSubset) newPredicates.Add(predicate);
            }
            if (hasFiltered) {
                return new Tuple<HyperGraph, bool>(
                    new HyperGraph(hyperEdges, newPredicates, CopyProVars()), true);
            }   else {
                return new Tuple<HyperGraph, bool>(null, false);
            }
        }

        private Tuple<HyperGraph, bool> AttemptCSE() {
            /*foreach (GJTNode node1 in hyperEdges) {
                foreach (GJTNode node2 in hyperEdges) {
                    if (node1 != node2 && IsConditionalSubset(node1.variables, node2.variables)) {
                        List<GJTNode> newHyperNodes = new List<GJTNode>();
                        HashSet<HashSet<string>> newPredicates = new HashSet<HashSet<string>>();
                        foreach (GJTNode node in hyperEdges) {
                            if (node == node1) {
                                GJTInnerNode newNode = new GJTInnerNode(
                                    node2.CopyVars(),
                                    new List<GJTNode> { node1, node2 }
                                    , null
                                    );
                                newHyperNodes.Add(newNode);
                                HashSet<HashSet<string>> usedPredicates = new HashSet<HashSet<string>>();
                                foreach (HashSet<string> predicate in predicates) {
                                    foreach (string var in SetMinus(node1.variables, node2.variables)) {
                                        if (predicate.Contains(var) && !usedPredicates.Contains(predicate)) usedPredicates.Add(predicate);
                                    }
                                }
                                newNode.predicates = usedPredicates;
                                foreach (HashSet<string> predicate in predicates) {
                                    if (!usedPredicates.Contains(predicate)) newPredicates.Add(predicate);
                                }
                                //remove predicate and add to node
                            } else if (node == node2) {
                                //don't do anything
                            } else {
                                newHyperNodes.Add(node);
                            }
                        }
                        return new Tuple<HyperGraph, bool>(
                            new HyperGraph(newHyperNodes, newPredicates, projectedVariables), true);
                    }
                }
            }*/
            return new Tuple<HyperGraph, bool>(null, false);
        }

        private Tuple<HyperGraph, bool> AttemptISO() {
            /*foreach (GJTNode node in hyperEdges) {
                HashSet<string> isolatedVariables = GetIsolatedVariables(node.variables);
                if(isolatedVariables.Count > 0) {
                    List<GJTNode> newHyperNodes = new List<GJTNode>();
                    foreach (GJTNode oldNode in this.hyperEdges) {
                        if(node == oldNode) {
                            GJTInnerNode newNode = new GJTInnerNode(
                                SetMinus(oldNode.variables, isolatedVariables),
                                new HashSet<GJTNode> { oldNode }
                                );
                            newHyperNodes.Add(newNode);
                        } else {
                            newHyperNodes.Add(oldNode);
                        }
                    }
                    return new Tuple<HyperGraph, bool>(
                        new HyperGraph(newHyperNodes, this.predicates, this.projectedVariables), true);
                }
            }*/
            return new Tuple<HyperGraph, bool>(null, false);
        }

        private bool IsConditionalSubset(HashSet<string> subsetCan, HashSet<string> supersetCan) {
            return IsSubset(GetEquiJoin(subsetCan), supersetCan) &&
                IsSubset(GetExtended(SetMinus(subsetCan, supersetCan)), supersetCan);
        }

        private bool IsSubset(HashSet<string> SubsetCan, HashSet<string> SupersetCan) {
            foreach(string subVar in SubsetCan) {
                if (!SupersetCan.Contains(subVar)) return false;
            }
            return true;
        }

        private HashSet<string> SetMinus(HashSet<string> s1, HashSet<string> s2) {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in s1) {
                if (!s2.Contains(s)) result.Add(s);
            }
            return result;
        }

        private HashSet<string> GetIsolatedVariables(HashSet<string> edge) {
            HashSet<string> result = new HashSet<string>();
            foreach (string var in edge) {
                if (IsIsolated(var)) result.Add(var);
            }
            return result;
        }

        private bool IsIsolated(string var) {
            if(OccursInAPredicate(var) || IsEquiJoin(var)) {
                return false;
            }
            return true;
        }

        private bool OccursInAPredicate(string var) {
            foreach(HashSet<string> predicate in predicates) {
                if (predicate.Contains(var)) {
                    return true;
                }
            }
            return false;
        }

        private HashSet<string> GetEquiJoin(HashSet<string> edge) {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in edge) {
                if (IsEquiJoin(s)) result.Add(s);
            }
            return result;
        }

        private bool IsEquiJoin(string var) {
            if (projectedVariables.Contains(var) || In2HyperEdges(var)) return true;
            return false;
        }

        private bool In2HyperEdges(string var) {
            bool seen = false;
            foreach (GJTNode node in hyperEdges) {
                if(seen) {
                    if (node.variables.IndexOf(var) >= 0) return true;
                } else {
                    if (node.variables.IndexOf(var) >= 0) seen = true;
                }
            }
            return false;
        }

        private HashSet<string> GetExtended(HashSet<string> edge) {
            HashSet<string> unionSet = new HashSet<string>();
            foreach(HashSet<string> predicate in predicates) {
                foreach(string s in edge) {
                    if (predicate.Contains(s)) unionSet.UnionWith(predicate);
                }
            }
            return SetMinus(unionSet, edge);
        }

        private HashSet<string> CopyProVars() {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in projectedVariables) {
                result.Add(s);
            }
            return result;
        }
    }
}
