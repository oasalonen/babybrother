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
        public static Task AssertEqualsAsync<T>(T expected, Func<T> actual, TimeSpan? timeout = null)
        {
            T lastValue = default(T);

            return AssertAsync(() =>
            {
                lastValue = actual();
                return expected.Equals(lastValue);
            },
            () => 
            {
                Assert.AreEqual(expected, lastValue);
            }, timeout);
        }

        public static Task AssertIsApproximateTimeAsync(DateTimeOffset expected, Func<DateTimeOffset> actual, TimeSpan? delta = null, TimeSpan? timeout = null)
        {
            if (delta == null)
            {
                delta = TimeSpan.FromSeconds(10);
            }
            var lastValue = DateTimeOffset.MinValue;

            Func<DateTimeOffset, DateTimeOffset, bool> isApproximatelyEqual = (d1, d2) => 
            {
                var diff = d1 - d2;
                if (diff.Ticks < 0)
                {
                    diff = diff.Negate();
                }
                return diff.CompareTo(delta.Value) <= 1;
            };

            return AssertAsync(() =>
            {
                lastValue = actual();
                return isApproximatelyEqual(lastValue, expected);
            },
            () =>
            {
                var message = "DateTime equality mismatch. Expected time: " + expected.ToString() + ", Actual time: " + lastValue.ToString() + ", Max delta: " + delta.Value.ToString();
                Assert.IsTrue(isApproximatelyEqual(lastValue, expected), message);
            }, timeout);
        }

        private static async Task AssertAsync(Func<bool> stopCondition, Action assert, TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                timeout = TimeSpan.FromSeconds(5);
            }

            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (stopCondition())
                {
                    break;
                }
                await Task.Delay(10);
            }
            while (stopwatch.Elapsed < timeout.Value);
            assert();
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
