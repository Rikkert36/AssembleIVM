using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.GJTComputerFiles.ConstructorStructures.nodes;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.TreeNodes.Predicates;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Computepercentagefacts11 : ManualGJT {
        public override GeneralJoinTree Construct() {
            List<string> dc004VarsA = new List<string> { "a.fact", "a.team", "a.week", "a.hours" };
            List<string> dc004VarsB = new List<string> { "b.fact", "b.team", "b.week", "b.hours" };
            List<string> teamWeek = new List<string> { "a.team", "a.week" };
            GJTLeaf dc004_01 = new GJTLeaf(
                "dc004_01",
                dc004VarsA,
                "movingtotal8",
                null
                );
            GJTLeaf dc004_02 = new GJTLeaf(
                "dc004_02",
                dc004VarsA,
                "movingtotal8",
                null
                );
            GJTLeaf dc004_03 = new GJTLeaf(
                "dc004_03",
                dc004VarsB,
                "movingtotal8",
                null
                );
            GJTLeaf PFHoliday = new GJTLeaf(
                "PFHoliday",
                new List<string> { "percentage_fact"},
                "PFactHoliday",
                new FrontierEnumerator()
                );
            GJTLeaf PFEducation = new GJTLeaf(
                "PFEducation",
                new List<string> { "percentage_fact" },
                "PFactEducation",
                new FrontierEnumerator()
                );
            PFEducation.inFrontier = true;
            PFHoliday.inFrontier = true;

            GJTInnerNode selHol = new GJTSelectNode(
                "selHol",
                dc004VarsA,
                new List<GJTNode> { dc004_01 },
                new List<TreeNode> { ParsePredicate("a.fact == \"holiday\"") },
                new FrontierEnumerator()
                );
            GJTInnerNode selEdu = new GJTSelectNode(
                "selEdu",
                dc004VarsA,
                new List<GJTNode> { dc004_02 },
                new List<TreeNode> { ParsePredicate("a.fact == \"education\"") },
                new FrontierEnumerator()
                );
            GJTInnerNode selGross = new GJTSelectNode(
                "selGross",
                dc004VarsB,
                new List<GJTNode> { dc004_03 },
                new List<TreeNode> { ParsePredicate("b.fact == \"gross availability\"") },
                new FrontierEnumerator()
                );
            selHol.inFrontier = true;
            selEdu.inFrontier = true;
            selGross.inFrontier = true;
            GJTInnerNode joinHol = new GJTSingleJoinNode(
                "joinHol",
                teamWeek,
                new List<GJTNode> { selHol, PFHoliday},
                new List<TreeNode> { null, new CartesianProduct()},
                new SingleJoinEnumerator()
                );
            GJTInnerNode joinEdu = new GJTSingleJoinNode(
                "joinEdu",
                teamWeek,
                new List<GJTNode> { selEdu, PFEducation },
                new List<TreeNode> { null, new CartesianProduct() },
                new SingleJoinEnumerator()
                );
            GJTInnerNode union = new GJTUnionNode(
                "union",
                teamWeek,
                new List<GJTNode> { joinHol, joinEdu},
                new List<TreeNode> { null, null },
                new UnionEnumerator()
                );
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                teamWeek,
                new List<GJTNode> { union, selGross},
                new List<TreeNode> { null,
                    ParsePredicate("a.team == b.team and a.week == b.week")
                },
                new SingleJoinEnumerator()
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { PFEducation, PFHoliday, selHol, selEdu, selGross },
                new HashSet<GJTLeaf> { dc004_01, dc004_02, dc004_03, PFEducation, PFHoliday },
                new List<string> { "Percentage_fact", "Team", "Week", "Percentage" },
                new List<string> { "percentage_fact", "a.team", "a.week", "a.hours/b.hours" },
                new List<string> { }
                );

        }

        public override string GetName() {
            return "computepercentagefacts11";
        }
    }
}
