using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    class ManualLeafs {

        public HyperGraph GetLeafs(string name) {
            switch (name) {
                case "2computenetavail":
                    return Computenetavail();
                case "3rollupdc001toyear":
                    return RollupDC001ToYear();
                case "8movingtotal":
                    return MovingTotalV2();
            }
            throw new Exception($"name {name} is not a file");
        }

        private HyperGraph Computenetavail() {
            /*GJTNode dc001_a = new GJTLeaf(new HashSet<string> { "a.fact", "employee", "week", "a.hours" });
            GJTNode dc001_b = new GJTLeaf(new HashSet<string> { "b.fact", "employee", "week", "b.hours" });
            GJTNode dc001_c = new GJTLeaf(new HashSet<string> { "c.fact", "employee", "week", "c.hours" });
            GJTNode factnet = new GJTLeaf(new HashSet<string> { "d.fact" });
            List<GJTNode> nodes = new List<GJTNode> { dc001_a, dc001_b, dc001_c, factnet };
            HashSet<HashSet<string>> predicates = new HashSet<HashSet<string>> { };
            HashSet<string> projectedVariables = new HashSet<string> 
                {"employee", "week", "d.fact", "a.hours", "b.hours", "c.hours"};
            return new HyperGraph(nodes, predicates, projectedVariables);  */
            return null;
        }

        private HyperGraph RollupDC001ToYear() {
            /*GJTNode dc001 = new GJTLeaf(new HashSet<string> {"employee", "week.x", "week.y", "fact", "hours"});
            List<GJTNode> nodes = new List<GJTNode> { dc001 };
            HashSet<HashSet<string>> predicates = new HashSet<HashSet<string>> { };
            HashSet<string> projectedVariables = new HashSet<string> 
                {"employee", "week.y", "fact", "hours" };
            return new HyperGraph(nodes, predicates, projectedVariables);*/
            return null;
        }

        private HyperGraph MovingTotal() {
            /*GJTNode dc002_a = new GJTLeaf(new HashSet<string> { "fact", "team", "a.week.w", "a.week.y", "a.hours" });
            GJTNode dc002_b = new GJTLeaf(new HashSet<string> { "fact", "team", "b.week.w", "b.week.y", "b.hours" });
            GJTNode yearLength = new GJTLeaf(new HashSet<string> { "b.week.y", "length" });
            List<GJTNode> nodes = new List<GJTNode> { dc002_a, dc002_b, yearLength };
            HashSet<HashSet<string>> predicates = new HashSet<HashSet<string>> { 
                new HashSet<string>{"a.week.w", "b.week.w"}, 
                new HashSet<string>{"a.week.y, b.week.y" },
                new HashSet<string>{ "a.week.w", "b.week.w", "length"}
            };
            HashSet<string> projectedVariables = new HashSet<string> { "fact", "team", "a.week.w", "a.week.y", "b.hours" };
            HashSet<string> extraProVars = new HashSet<string> { "b.week.w", "b.week.y", "length" };
            projectedVariables.UnionWith(extraProVars);
            return new HyperGraph(nodes, predicates, projectedVariables);*/
            return null;
        }

        private HyperGraph MovingTotalV2() {
            /*GJTNode dc002_a = new GJTLeaf(new HashSet<string> { "fact", "team", "a.week.w", "a.week.y", "a.hours" });
            GJTNode dc002_btimesYL = new GJTLeaf(new HashSet<string> { "fact", "team", "b.week.w", "b.week.y", "b.hours", "length" });
            List<GJTNode> nodes = new List<GJTNode> { dc002_a, dc002_btimesYL };
            HashSet<HashSet<string>> predicates = new HashSet<HashSet<string>> {
                new HashSet<string>{"a.week.w", "b.week.w"},
                new HashSet<string>{"a.week.y", "b.week.y" },
                new HashSet<string>{ "a.week.w", "b.week.w", "length"}
            };
            HashSet<string> projectedVariables = new HashSet<string> { "fact", "team", "a.week.w", "a.week.y", "b.hours" };
            return new HyperGraph(nodes, predicates, projectedVariables);*/
            return null;
        }
    }
}
