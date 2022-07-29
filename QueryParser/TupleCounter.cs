using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM {
    static class TupleCounter {

        private static Stack<int> countStack = new Stack<int>();
        private static Stack<string> nameStack = new Stack<string>();

        public static void Push(string name) {
            countStack.Push(0);
            nameStack.Push(name);
        }

        public static void Pop(string name) {
            string tabs = "";
            for (int i = 1; i < countStack.Count; i++) {
                tabs += "\t";
            }
            int count = countStack.Pop();
            string lastName = nameStack.Pop();
            if(lastName.Equals(name)) {
                Console.WriteLine($"{tabs}{lastName}: {String.Format("{0:n0}",count).Replace('.', '\'')}");
                if (countStack.Count != 0) {
                    int groupCount = countStack.Pop();
                    countStack.Push(count + groupCount);
                }
            } else {
                throw new Exception($"You tried to pop {name}, whereas {lastName} is on top of the stack");
            }
        }

        public static void Increment(int i) {
            int count = countStack.Pop();
            countStack.Push(count + i);
        }

        public static void Increment() {
            int count = countStack.Pop();
            countStack.Push(++count);
        }
    }
}
