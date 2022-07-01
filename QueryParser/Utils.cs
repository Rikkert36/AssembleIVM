using AssembleIVM.QueryParser.TreeNodes.Terminals;
using AssembleIVM.T_reduct;
using Microsoft.VisualBasic.FileIO;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM {
    static class Utils {
        public static HashSet<GMRTuple> CSVToTupleSet(string fileLocation, List<string> variables) {
            using (TextFieldParser parser = new TextFieldParser(fileLocation)) { 
                HashSet<GMRTuple> resultTable = new HashSet<GMRTuple>();
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                string[] header = parser.ReadFields();
                List<int> headerIntMap = new List<int>();
                if (header.Length != variables.Count) 
                    throw new Exception($"File {fileLocation} does not have as many dimensionvalues as {variables}");
                for (int i = 0; i < header.Length; i++) {
                    for (int j = 0; j < variables.Count; j++) {
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

        public static List<string> SetMinus(List<string> left, List<string> right) {
            List<string> result = new List<string>();
            foreach(string s in left) {
                if(!right.Contains(s)) {
                    result.Add(s);
                }
            }
            return result;
        }

        public static HashSet<string> SetMinus(HashSet<string> left, HashSet<string> right) {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in left) {
                if (!right.Contains(s)) {
                    result.Add(s);
                }
            }
            return result;
        }

        public static List<string> Union(List<string> t1, List<string> t2) {
            List<string> result = new List<string>();
            foreach (string s in t1) result.Add(s);
            foreach (string s in t2) result.Add(s);
            return result;
        }

        public static List<string> Union(string[] t1, string[] t2) {
            List<string> result = new List<string>();
            foreach (string s in t1) result.Add(s);
            foreach (string s in t2) result.Add(s);
            return result;
        }

        public static HashSet<GMRTuple> Union(IEnumerable<GMRTuple> l1, IEnumerable<GMRTuple> l2) {
            Dictionary<string, GMRTuple> unionMap = new Dictionary<string, GMRTuple>();
            foreach (GMRTuple t in l1) {
                if (t.sum != null) {
                    unionMap.Add(t.GetString(), new GMRTuple(t.fields.Length, t.count) 
                    { fields = t.fields, sum = new Number(t.sum.value) });
                } else {
                    unionMap.Add(t.GetString(), new GMRTuple(t.fields.Length, t.count) { fields = t.fields });
                }
            }
            foreach (GMRTuple t in l2) {
                if (unionMap.ContainsKey(t.GetString())) {
                    GMRTuple sameTuple = unionMap[t.GetString()];
                    sameTuple.count -= t.count;
                    if (t.sum != null)sameTuple.sum.value -= t.sum.value;
                } else {
                    if (t.sum != null) {
                    unionMap.Add(t.GetString(), new GMRTuple(t.fields.Length, -t.count) 
                    { fields = t.fields, sum = new Number(-t.sum.value) });
                } else {
                    unionMap.Add(t.GetString(), new GMRTuple(t.fields.Length, -t.count) { fields = t.fields });
                }
                }
            }
            return unionMap.Values.ToHashSet();      
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

        public static HashSet<string> GetInEquiVars(List<TreeNode> predicates) {
            HashSet<string> result = new HashSet<string>();
            foreach (TreeNode predicate in predicates) {
                result.UnionWith(GetInEquiVars(predicate));
            }
            return result;
        }

        public static bool HasWeekValue(List<string> variables) {
            for (int i = 0; i < variables.Count; i++) {
                string var = variables[i];
                string[] slices = var.Split(".");
                if (Array.IndexOf(slices, "week") >= 0) return true;
            }
            return false;
        }

        public static int GetWeekIndex(List<string> variables) {
            for (int i = 0; i < variables.Count; i++) {
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

        public static string ToString(List<string> fields) {
            string result = "";
            foreach (string f in fields) {
                if (int.TryParse(f, out _) || double.TryParse(f, out _)) {
                    result += $"number({f})";
                } else {
                    result += f;
                }
            }
            return result;
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
                case "NumberNode": {
                        return new HashSet<string> {  };
                    }
                case "StringNode": {
                        return new HashSet<string> { };
                    }

            }
            throw new Exception($"You missed predicate type: {n.GetType().Name}");
        }

        private static HashSet<string> GetInEquiVars(TreeNode n) {
            if (n == null || n.GetType().Name.Equals("CartesianProduct")) return new HashSet<string>();
            switch (n.GetType().Name) {
                case "And": {
                        And andN = (And)n;
                        HashSet<string> result = GetInEquiVars(andN.left);
                        result.UnionWith(GetInEquiVars(andN.right));
                        return result;
                    }
                case "Or": {
                        Or orN = (Or)n;
                        HashSet<string> result = GetInEquiVars(orN.left);
                        result.UnionWith(GetInEquiVars(orN.right));
                        return result;
                    }
                case "Comparison": {
                        Comparison cN = (Comparison)n;
                        if (!cN.compareOperator.Equals("==")) {
                            HashSet<string> result = GetInEquiVars(cN.left);
                            result.UnionWith(GetInEquiVars(cN.right));
                            return result;
                        } else {
                            return new HashSet<string>();
                        }
                    }
                case "Term": {
                        Term t = (Term)n;
                        HashSet<string> result = GetInEquiVars(t.left);
                        result.UnionWith(GetInEquiVars(t.right));
                        return result;
                    }
                case "Factor": {
                        Factor f = (Factor)n;
                        HashSet<string> result = GetInEquiVars(f.left);
                        result.UnionWith(GetInEquiVars(f.right));
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
                case "NumberNode": {
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
