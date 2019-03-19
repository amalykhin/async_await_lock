using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace async_await_lock
{
    class Program
    {
		static readonly object lockObject = new object();
		static int asyncSum;
        static void Main(string[] args)
        {
            Console.WriteLine("Lock keyword demo. It will evaluate sum of a random array:");
			var rand = new Random();
			var numbers = new List<int>();
			for (int i = 0; i < 10000000; i++) {
				var num = rand.Next(50);
				numbers.Add(num);
			}
			Console.WriteLine();
			Int64 nonAsyncSum = numbers.Sum();
			Console.WriteLine("Non-async sum: " + nonAsyncSum);

			var listOfLists = toLists(numbers, 100);
			var taskList = new List<Task>();
			foreach (List<int> list in listOfLists) {
				taskList.Add(computeAsyncSum(list));
			}
			Task.WhenAll(taskList)
				.ContinueWith((t) => Console.WriteLine("Async sum: " + asyncSum));
        }

		static async Task computeAsyncSum(List<int> list)
		{
			await Task.Run(() => {
				var sum = list.Sum();
				lock (lockObject) {
					asyncSum += sum;
				}
			});
		}

		public static List<List<int>> toLists(List<int> originalList, int sublistSize=10)
		{
			var output = new List<List<int>>();
			for (int i = 0; i < originalList.Count; i += sublistSize)
				output.Add(originalList.GetRange(i, Math.Min(sublistSize, originalList.Count - i)));

			return output;
		}
    }

}
