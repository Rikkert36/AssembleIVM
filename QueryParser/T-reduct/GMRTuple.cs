using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM {
    [Serializable]
    class GMRTuple {
        public string[] fields;
        public int count;

        public GMRTuple(int headerLength, int count) {
            fields = new string[headerLength];
            this.count = count;
        }

        public override string ToString() {
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

        public bool Equals(GMRTuple t) {
            if (fields.Length != t.fields.Length) return false;
            for (int i = 0; i < fields.Length; i++) {
                if (!fields[i].Equals(t.fields[i])) return false;
            }
            return true;
        }

        public GMRTuple SplitWeekAndYearValue(int weekIndex) {
            List<string> fieldList = new List<string>();
            for (int i = 0; i < fields.Length + 1; i++) {
                if (!(i == weekIndex)) {
                    fieldList.Add(fields[i]);
                } else {
                    if (int.TryParse(fields[i], out _)) {
                        fieldList.Append("-");
                        fieldList.Append(fields[i]);
                    } else {
                        fields.Append(fields[i].Substring(1, 2));
                        fields.Append(fields[i].Substring(4, 4));
                    }
                }
            }
            return new GMRTuple(fields.Length + 1, count) {
                fields = fieldList.ToArray()
            };
        }
    }
}
