using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    class GJTComputer {

        public GJTNode ConstructTree(string name) {
            HyperGraph current = new ManualLeafs().GetLeafs(name);
            bool newGraph = true;
            while(newGraph) {
                Tuple<HyperGraph, bool> attempt = current.AttemptReduction();
                current = attempt.Item1;
                newGraph = attempt.Item2;
            }
            foreach(GJTNode node in current.GetNodes()) {
                node.inFrontier = true;
            }
            CheckConnexFree(current, name);
            current.EmptyProjectedVariables();
            newGraph = true;
            while(newGraph) {
                Tuple<HyperGraph, bool> attempt = current.AttemptReduction();
                current = attempt.Item1;
                newGraph = attempt.Item2;
            }
            if (!IsEmpty(current)) throw new Exception($"{name} is cyclic, no compatible GJT");
            return current.hyperEdges[0];
        }

        public GeneralJoinTree ConstructTree(TreeNode query) {
            throw new NotImplementedException();
        }

        private bool IsEmpty(HyperGraph hyperGraph) {
            return hyperGraph.hyperEdges.Count == 1 &&
                hyperGraph.hyperEdges[0].variables.Count == 0 &&
                hyperGraph.predicates.Count == 0;
        }

        private void CheckConnexFree(HyperGraph hyperGraph, string name) {
            HashSet<string> frontier = new HashSet<string>();
            foreach (GJTNode node in hyperGraph.hyperEdges) {
                frontier.UnionWith(node.variables);
            }
            foreach (string s in frontier) {
                if (!hyperGraph.GetProjectedVariables().Contains(s)) {
                    Console.WriteLine($"Not free-connex, need to add {s} to the projected variables");
                }
            }
        }


        /*private List<GJTNode> GetLeafs(TreeNode node, string name) {
            if (node.GetType().Name.Equals("SimpRelation")) {
                List<Variable> variables = GetVariables((SimpRelation)node);
                return new List<GJTNode>{
                    new GJTLeaf(variables)
                };
            } else {
                List<GJTNode> result = new List<GJTNode>();
                string nodeType = node.GetType().Name;
                if (nodeType.Equals("Query")) {
                    Query query = (Query)node;
                    result = GetLeafs(query.OuterBody, name);
                } else if (nodeType.Equals("CombinedRelation")) {
                    CombinedRelation cr = (CombinedRelation)node;
                    result = result
                        .Concat(GetLeafs(cr.left, name))
                        .Concat(GetLeafs(cr.right, name))
                        .ToList();
                }
                return result;
            }
        }

        private List<Variable> GetVariables(SimpRelation node) {
            throw new NotImplementedException();
        }*/




    }
}
