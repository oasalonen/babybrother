using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.UnitTest
{
    internal static class Utilities
    {
        public static async Task AssertEqualsAsync<T>(T expected, Func<T> actual, TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                timeout = TimeSpan.FromSeconds(5);
            }

            T lastValue = default(T);
            var stopwatch = Stopwatch.StartNew();
            do
            {
                lastValue = actual();
                if (expected.Equals(lastValue))
                {
                    break;
                }
                await Task.Delay(10);
            }
            while (stopwatch.Elapsed < timeout.Value);
            Assert.AreEqual(expected, lastValue);
        }

        public static Task AssertNextValueIs<T>(this IObservable<T> source, T expected)
        {
            return source.AssertNextValueIs(expected, () => { });
        }

        public static Task AssertNextValueIs<T>(this IObservable<T> source, T expected, Action trigger)
        {
            return source.AssertNextValueIs(new List<T> { expected }, trigger);
        }

        public static async Task AssertNextValueIs<T>(this IObservable<T> source, IEnumerable<T> expected, Action trigger)
        {
            bool isValid = false;
            string mismatchReason = "{unknown}";
            var originalValues = expected.ToList();
            var expectedQueue = expected.ToList();
            var type = typeof(T);
            var tcs = new TaskCompletionSource<bool>();

            source.Subscribe(
                (v) =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        var nextExpected = expectedQueue.First();
                        expectedQueue.RemoveAt(0);
                        isValid = v.Equals(nextExpected);

                        if (!isValid || expectedQueue.Count == 0)
                        {
                            mismatchReason = v.ToString();
                            tcs.TrySetResult(isValid);
                        }
                    }
                },
                (e) =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        mismatchReason = "{error}";
                        tcs.TrySetResult(isValid && expectedQueue.Count == 0);
                    }
                },
                () =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        mismatchReason = "{endsequence}";
                        tcs.TrySetResult(isValid && expectedQueue.Count == 0);
                    }
                });

            trigger();

            var isOk = await tcs.Task;
            string sequence = "";
            foreach (var v in originalValues)
            {
                if (sequence.Length > 0)
                {
                    sequence += ", ";
                }
                sequence += (v != null ? v.ToString() : "null");
            }
            sequence = "[" + sequence + "]";

            Assert.IsTrue(isOk, "Failed on item " + (originalValues.Count - expectedQueue.Count) + " when verifying sequence " + sequence + ". Got " + mismatchReason);
        }
    }
}
