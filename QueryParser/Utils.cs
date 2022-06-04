using AssembleIVM.QueryParser.TreeNodes.Terminals;
using Microsoft.VisualBasic.FileIO;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM {
    static class Utils {
        public static HashSet<GMRTuple> CSVToTupleSet(string fileLocation, string[] variables) {
            using (TextFieldParser parser = new TextFieldParser(fileLocation)) { 
                HashSet<GMRTuple> resultTable = new HashSet<GMRTuple>();
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                string[] header = parser.ReadFields();
                List<int> headerIntMap = new List<int>();
                if (header.Length != variables.Length) 
                    throw new Exception($"File {fileLocation} does not have as many dimensionvalues as {variables}");
                for (int i = 0; i < header.Length; i++) {
                    for (int j = 0; j < variables.Length; j++) {
                        if (SimpleDimension(header[i]).Equals(SimpleDimension(variables[j]))) {
                            headerIntMap.Add(j);
                            break;
                        }
                    }
                }
                if (headerIntMap.Count != header.Length)
                    throw new Exception($"The dimensionnames from {fileLocation} are not compatible with {variables}");
                while (!parser.EndOfData) {
                    //Process row
                    string[] fields = parser.ReadFields();
                    GMRTuple t = new GMRTuple(header.Length, 1);
                    resultTable.Add(t);
                    for (int i = 0; i < header.Length; i++) {
                        t.fields[headerIntMap[i]] = fields[i];
                    }
               
                }
                return resultTable;
            }
        }

        private static string SimpleDimension(string s) {
            if (s.Contains(".")) {
                int dotIndex = s.LastIndexOf(".");
                return s.Substring(dotIndex + 1).ToLower();
            }
            return s.ToLower();
        }

        public static List<string> Intersect(List<string> list, HashSet<string> set) {
            List<string> result = new List<string>();
            foreach(string s in list) {
                if (set.Contains(s)) result.Add(s);
            }
            return result;
        }
        public static HashSet<string> GetEquiVars(List<TreeNode> predicates) {
            HashSet<string> result = new HashSet<string>();
            foreach (TreeNode predicate in predicates) {
                result.UnionWith(GetEquiVars(predicate));
            }
            return result;
        }

        public static int GetWeekIndex(string[] variables) {
            for (int i = 0; i < variables.Length; i++) {
                string var = variables[i];
                string[] slices = var.Split(".");
                if (Array.IndexOf(slices, "week") >= 0) return i;
            }
            throw new Exception("header does not have a week value");
        }

        public static bool DimensionNameEquals(TreeNode treeNode, string s) {
            if (treeNode.GetType().Equals("DimensionName")) {
                DimensionName dimensionName = (DimensionName)treeNode;
                return dimensionName.value.Equals(s);
            } else if (treeNode.GetType().Equals("RelationAttribute")) {
                RelationAttribute relationAttribute = (RelationAttribute)treeNode;
                return relationAttribute.GetString().Equals(s);
            }
            return false;
        }

        public static string Opposite(string s) {
            switch (s) {
                case ">=": return "<=";
                case ">": return "<";
                case "<": return ">";
                case "<=": return ">=";
            }
            return s;
        }

        private static HashSet<string> GetEquiVars(TreeNode n) {
            if (n == null || n.GetType().Name.Equals("CartesianProduct")) return new HashSet<string>();
            switch (n.GetType().Name) {
                case "And": {
                        And andN = (And)n;
                        HashSet<string> result = GetEquiVars(andN.left);
                        result.UnionWith(GetEquiVars(andN.right));
                        return result;
                    }
                case "Or": {
                        Or orN = (Or)n;
                        HashSet<string> result = GetEquiVars(orN.left);
                        result.UnionWith(GetEquiVars(orN.right));
                        return result;
                    }
                case "Comparison": {
                        Comparison cN = (Comparison)n;
                        if (cN.compareOperator.Equals("==")) {
                            HashSet<string> result = GetEquiVars(cN.left);
                            result.UnionWith(GetEquiVars(cN.right));
                            return result;
                        } else {
                            return new HashSet<string>();
                        }
                    }
                case "Term": {
                        Term t = (Term)n;
                        HashSet<string> result = GetEquiVars(t.left);
                        result.UnionWith(GetEquiVars(t.right));
                        return result;
                    }
                case "Factor": {
                        Factor f = (Factor)n;
                        HashSet<string> result = GetEquiVars(f.left);
                        result.UnionWith(GetEquiVars(f.right));
                        return result;
                    }
                case "DimensionName": {
                        DimensionName dN = (DimensionName)n;
                        return new HashSet<string> { dN.value };
                    }
                case "RelationAttribute": {
                        RelationAttribute raN = (RelationAttribute)n;
                        return new HashSet<string> { raN.GetString() };
                    }
                case "Number": {
                        NumberNode nN = (NumberNode)n;
                        return new HashSet<string> { nN.value };
                    }
                case "StringNode": {
                        StringNode sN = (StringNode)n;
                        return new HashSet<string> { sN.value };
                    }

            }
            throw new Exception($"You missed predicate type: {n.GetType().Name}");
        }

        public static Tuple<bool, TreeNode> FindVariable(TreeNode treeNode, HashSet<string> variableSet) {
            switch (treeNode.GetType().Name) {
                case "DimensionName": {
                        DimensionName dimensionName = (DimensionName)treeNode;
                        if (variableSet.Contains(dimensionName.value)) return new Tuple<bool, TreeNode>(true, dimensionName);
                        break;
                    }
                case "RelationAttribute": {
                        RelationAttribute relationAttribute = (RelationAttribute)treeNode;
                        if (variableSet.Contains(relationAttribute.GetString())) return new Tuple<bool, TreeNode>(true, relationAttribute);
                        break;
                    }
                case "Number": {
                        break;
                    }
                case "StringNode": {
                        break;
                    }
                case "Factor": {
                        Factor factor = (Factor)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(factor.left, variableSet);
                        Tuple<bool, TreeNode> right = FindVariable(factor.right, variableSet);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
                case "Term": {
                        Term term = (Term)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(term.left, variableSet);
                        Tuple<bool, TreeNode> right = FindVariable(term.right, variableSet);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
                case "Comparison": {
                        Comparison comparison = (Comparison)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(comparison.left, variableSet);
                        Tuple<bool, TreeNode> right = FindVariable(comparison.right, variableSet);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
            }
            return new Tuple<bool, TreeNode>(false, null);
        }

        public static Tuple<bool, TreeNode> FindVariable(TreeNode treeNode, string variable) {
            switch (treeNode.GetType().Name) {
                case "DimensionName": {
                        DimensionName dimensionName = (DimensionName)treeNode;
                        if (variable == dimensionName.value) return new Tuple<bool, TreeNode>(true, dimensionName);
                        break;
                    }
                case "RelationAttribute": {
                        RelationAttribute relationAttribute = (RelationAttribute)treeNode;
                        if (variable == relationAttribute.GetString()) return new Tuple<bool, TreeNode>(true, relationAttribute);
                        break;
                    }
                case "Number": {
                        break;
                    }
                case "StringNode": {
                        break;
                    }
                case "Factor": {
                        Factor factor = (Factor)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(factor.left, variable);
                        Tuple<bool, TreeNode> right = FindVariable(factor.right, variable);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
                case "Term": {
                        Term term = (Term)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(term.left, variable);
                        Tuple<bool, TreeNode> right = FindVariable(term.right, variable);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
                case "Comparison": {
                        Comparison comparison = (Comparison)treeNode;
                        Tuple<bool, TreeNode> left = FindVariable(comparison.left, variable);
                        Tuple<bool, TreeNode> right = FindVariable(comparison.right, variable);
                        if (left.Item1) return left;
                        if (right.Item1) return right;
                        break;
                    }
            }
            return new Tuple<bool, TreeNode>(false, null);
        }


    }
}
