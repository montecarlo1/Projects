using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Task2
{
    //public static class ParallelEnumerable
    //{
    //    public static ParallelQuery<TSource> AsParallel<TSource>(this IEnumerable<TSource> source);

    //    public static IEnumerable<TSource> AsSequential<TSource>(this ParallelQuery<TSource> source);
    //}
    public static class EnumerableEx
    {
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext);
    }

    public static class ParallelEnumerable
    {
        public static void ForAll<TSource>(this ParallelQuery<TSource> source, Action<TSource> action);
    }
    internal static partial class QueryMethods
    {
        //reflection
        private static readonly Assembly CoreLibrary = typeof(object).GetTypeInfo().Assembly;

        internal static void OptInOutParallel()
        {
            IEnumerable<string> obsoleteTypes = CoreLibrary.GetExportedTypes() // Return IEnumerable<Type>.
                .AsParallel() // Return ParallelQuery<Type>.
                .Where(type => type.GetTypeInfo().GetCustomAttribute<ObsoleteAttribute>() != null) // ParallelEnumerable.Where.
                .Select(type => type.FullName) // ParallelEnumerable.Select.
                .AsSequential() // Return IEnumerable<Type>.
                .OrderBy(name => name); // Enumerable.OrderBy.
            obsoleteTypes.ForEach(name => name.WriteLine());
        }
        internal static void QueryExpression()
        {
            IEnumerable<string> obsoleteTypes =
                from name in
                    (from type in CoreLibrary.GetExportedTypes().AsParallel()
                     where type.GetTypeInfo().GetCustomAttribute<ObsoleteAttribute>() != null
                     select type.FullName).AsSequential()
                orderby name
                select name;
            obsoleteTypes.ForEach(name => name.WriteLine());
        }
        internal static void ForEachForAll()
        {
            Enumerable
                .Range(0, Environment.ProcessorCount * 2)
                .ForEach(value => value.WriteLine()); // 0 1 2 3 4 5 6 7

            ParallelEnumerable
                .Range(0, Environment.ProcessorCount * 2)
                .ForAll(value => value.WriteLine()); // 2 6 4 0 5 3 7 1
        }
    }
    class Program
    {
        static async Task<string> DoTaskAsync(string name, int timeout)
        {
            var start = DateTime.Now;
            Console.WriteLine("Enter {0}, {1}", name, timeout);
            await Task.Delay(timeout);
            Console.WriteLine("Exit {0}, {1}", name, (DateTime.Now - start).TotalMilliseconds);
            return name;
        }

        static async Task DoWork1()
        {
            var t1 = DoTaskAsync("t1.1", 1000);
            var t2 = DoTaskAsync("t1.2", 1000);
            var t3 = DoTaskAsync("t1.3", 1000);

            //await t1; await t2; await t3;
            await Task.WhenAll(t1, t2, t3);
            Console.WriteLine("DoWork1 results: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result));
        }

        static async Task DoWork2()
        {
            var t1 = DoTaskAsync("t2.1", 3000);
            var t2 = DoTaskAsync("t2.2", 2000);
            var t3 = DoTaskAsync("t2.3", 1000);

            await Task.WhenAll(t1, t2, t3);

            Console.WriteLine("DoWork2 results: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result));
        }
        /// <summary>
        /// Do something asyn.
        /// </summary>
        /// <returns></returns>
        public Task DoWork3()
        {
            var t1 = DoTaskAsync("t3.1", 3000);
            var t2 = DoTaskAsync("t3.2", 2000);
            var t3 = DoTaskAsync("t3.3", 1000);
            var t4 = DoTaskAsync("t3.4", 3000);
            var t5 = DoTaskAsync("t3.5", 2000);
            //await Task.WhenAll(t1, t2, t3);

            Console.WriteLine("DoWork3 results: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result, t4.Result, t5.Result));
            return Task.WhenAll(t1, t2, t3, t4, t5);
        }

        internal static partial class LinqToXml
        {
            internal static async Task QueryExpression()
            {
                using (HttpClient httpClient = new HttpClient())
                using (Stream downloadStream = await httpClient.GetStreamAsync("https://weblogs.asp.net/dixin/rss"))
                {
                    XDocument feed = XDocument.Load(downloadStream);
                    IEnumerable<XElement> source = feed.Descendants("item"); // Get source.
                    IEnumerable<string> query = from item in source
                                                where (bool)item.Element("guid").Attribute("isPermaLink")
                                                orderby (DateTime)item.Element("pubDate")
                                                select (string)item.Element("title"); // Define query.
                    foreach (string result in query) // Execute query.
                    {
                        Trace.WriteLine(result);
                    }
                }
            }
        }
        internal static void CreateAndSerialize()
        {
            XNamespace @namespace = "https://weblogs.asp.net/dixin";
            XElement rss = new XElement(
                "rss",
                new XAttribute("version", "2.0"),
                new XAttribute(XNamespace.Xmlns + "dixin", @namespace),
                new XElement(
                    "channel",
                    new XElement(
                        "item",            // Implicitly converted to XName.
                        new XElement("title", "LINQ via C#"),
                        new XElement("link", "https://weblogs.asp.net/dixin/linq-via-csharp"),
                        new XElement(
                            "description",
                            XElement.Parse("<p>This is a tutorial of LINQ and functional programming. Hope it helps.</p>")),
                        new XElement("pubDate", new DateTime(2009, 9, 7).ToString("r")),
                        new XElement(
                            "guid",
                            new XAttribute("isPermaLink", "true"), // "isPermaLink" is implicitly converted to XName.
                            "https://weblogs.asp.net/dixin/linq-via-csharp"),
                        new XElement("category", "C#"),
                        new XElement("category", "LINQ"),
                        new XComment("Comment."),
                        new XElement(
                            @namespace + "source",
                            https://github.com/Dixin/CodeSnippets/tree/master/Dixin/Linq 
                            ))));
            rss.ToString(); // Serialize XDocument to string.
        }
        static void Main(string[] args)
        {

            Task.WhenAll(DoWork1(), DoWork2()).Wait();

            new Program().DoWork3();
            var tasks = new Task<long>[10];
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                int delayInterval = 18 * ctr;
                tasks[ctr - 1] = Task.Run(async () => {
                    long total = 0;
                    await Task.Delay(delayInterval);
                    var rnd = new Random();
                    // Generate 1,000 random numbers.
                    for (int n = 1; n <= 1000; n++)
                        total += rnd.Next(0, 1000);

                    return total;
                });
            }
            var continuation = Task.WhenAll(tasks);
            try
            {
                continuation.Wait();
            }
            catch (AggregateException)
            { }

            if (continuation.Status == TaskStatus.RanToCompletion)
            {
                long grandTotal = 0;
                foreach (var res in continuation.Result)
                {
                    grandTotal += res;
                    Console.WriteLine("Mean: {0:N2}, n = 1,000", res / 1000.0);
                }

                Console.WriteLine("\nMean of Means: {0:N2}, n = 10,000",
                                  grandTotal / 10000);
            }
            // Display information on faulted tasks.
            else
            {
                foreach (var t in tasks)
                    Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
            }


            // The Three Parts of a LINQ Query:
            //  1. Data source.
            int[] numbers = new int[7] { 0, 1, 2, 3, 4, 5, 6 };

            // 2. Query creation.
            // numQuery is an IEnumerable<int>
            var numQuery =
                from num in numbers
                where (num % 2) == 0
                select num;
            List<int> result = numQuery.ToList();
            int count = numQuery.Count();
            // 3. Query execution.
            foreach (int num in numQuery)
            {
                Console.Write("{0,1} ", num);
            }
            Console.WriteLine("======================");
            //or choose another query
            for (int num = 0; num < count; num++)
            {
                Console.WriteLine(result[num]);
            }
                      

        }
    }
}
