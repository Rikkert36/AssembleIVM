using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM {
    static class TupleCounter {

        private static Stack<int> countStack = new Stack<int>();
        private static Stack<string> nameStack = new Stack<string>();

        private static int computeDelta = 0;
        private static int applyUpdate = 0;
        private static bool updating = false;
        private static bool computingDelta = false;

        public static void Push(string name) {
            countStack.Push(0);
            nameStack.Push(name);
        }


        public static void Pop(string name) {
            /*string tabs = "";
            *//*for (int i = 1; i < countStack.Count; i++) {
                tabs += "\t";
            }*//*
            int count = countStack.Pop();
            string lastName = nameStack.Pop();
            if (lastName.Equals(name)) {
                Console.WriteLine($"{count}");
                //Console.WriteLine($"{tabs}{lastName}: {String.Format("{0:n0}", count).Replace('.', '\'')}");
                if (countStack.Count != 0) {
                    int groupCount = countStack.Pop();
                    countStack.Push(count + groupCount);
                }
            } else {
                throw new Exception($"You tried to pop {name}, whereas {lastName} is on top of the stack");
            }*/

        }

        public static void StartUpdating() {
            updating = true;
            Push("Update");
        }

        public static void StopUpdating() {
            updating = false;
            string tabs = "";
            /*for (int i = 0; i < countStack.Count; i++) {
                tabs += "\t";
            }*/
            Console.WriteLine($"{applyUpdate}");
            Console.WriteLine($"{computeDelta}");
           /* Console.WriteLine($"{tabs}Apply update: {String.Format("{0:n0}", applyUpdate).Replace('.', '\'')}");
            Console.WriteLine($"{tabs}Compute delta: {String.Format("{0:n0}", computeDelta).Replace('.', '\'')}");*/
            Increment(applyUpdate);
            Increment(computeDelta);
            applyUpdate = 0;
            computeDelta = 0;
            Pop("Update");
        }

        public static void StartComputingDelta() {
            computingDelta = true;
        }
        public static void StartApplyingUpdate() {
            computingDelta = false;
        }


        public static void Increment(int i) {
            /*int count = countStack.Pop();
            countStack.Push(count + i);*/
        }

        public static void Increment() {
            /*if (!updating) {
                int count = countStack.Pop();
                countStack.Push(++count);
            } else if (computingDelta) {
                computeDelta++;
            } else {
                applyUpdate++;
            }*/
        }
    }
}
