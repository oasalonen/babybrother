using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.UnitTest
{
    internal static class Utilities
    {
        public static async Task AssertNextValueIs<T>(this IObservable<T> source, T expected)
        {
            bool isValid = false;
            var tcs = new TaskCompletionSource<bool>();

            source.Subscribe(
                (v) =>
                {
                    isValid = (v.Equals(expected));
                    tcs.TrySetResult(isValid);
                },
                (e) =>
                {
                    tcs.TrySetResult(isValid);
                },
                () =>
                {
                    tcs.TrySetResult(isValid);
                });

            Assert.IsTrue(await tcs.Task);
        }
    }
}
