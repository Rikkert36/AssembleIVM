using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM {
    [Serializable]
    class GMRTuple {
        public string[] fields;
        public int count;
        public bool alreadyRemoved = false;

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
            for (int i = 0; i < fields.Length; i++) {
                if (!(i == weekIndex)) {
                    fieldList.Add(fields[i]);
                } else {
                    if (!fields[i].Contains(".")) {
                        fieldList.Add("-");
                        fieldList.Add(fields[i]);
                    } else {
                        fieldList.Add(fields[i].Substring(1, 2));
                        fieldList.Add(fields[i].Substring(4, 4));
                    }
                }
            }
            return new GMRTuple(fields.Length + 1, count) {
                fields = fieldList.ToArray()
            };
        }

        public GMRTuple UniteWeekAndYearValues(int firstWeekIndex) {
            List<string> newFields = new List<string>(fields);
            string week = $"W{fields[firstWeekIndex]}.{fields[firstWeekIndex + 1]}";
            newFields[firstWeekIndex] = week;
            newFields.RemoveAt(firstWeekIndex + 1);
            return new GMRTuple(newFields.Count, this.count) {
                fields = newFields.ToArray()
            };
        }
    
        public string GetString() {
            string result = $"[{fields[0]}";
            for (int i = 1; i < fields.Length; i++) {
                result += $", {fields[i]}";
            }
            result += "]";
            return result;
        }
    }

}
