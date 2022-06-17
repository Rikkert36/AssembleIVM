using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM {
    static class Timer {

        static Dictionary<string, long> startTimes = new Dictionary<string, long>();
        static Dictionary<string, long> durations = new Dictionary<string, long>();

        public static void Start(string s) {
            startTimes[s] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            durations[s] = 0;
        }

        public static void Stop(string s) {
            if (startTimes.ContainsKey(s)) {
                long startTime = startTimes[s];
                long durationInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
                durations[s] += durationInMilliseconds;
                startTimes.Remove(s);
            }
            long totalDuration = durations[s];
            double seconds = (double)totalDuration / 1000;
            Console.WriteLine($"{s}: {seconds}");
        }

        public static void Pause(string s) {
            if (durations.ContainsKey(s)) {
                long startTime = startTimes[s];
                long durationInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
                durations[s] += durationInMilliseconds;
                startTimes.Remove(s);
            }
        }

        public static void Resume(string s) {
            if (durations.ContainsKey(s)) {
                startTimes[s] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

    }
}
