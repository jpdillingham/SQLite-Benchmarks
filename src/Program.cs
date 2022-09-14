using System.Diagnostics;

namespace Benchmarks
{
    internal class Program
    {
        private static readonly string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

        static void Main(string[] args)
        {
            var rng = new Random();
            var db = new Database();
            var results = new List<double>();

            //for (int i = 0; i < 10; i++)
            //{
            //    Test(db, 10000);
            //}
            Insert(db, 5000);

            for (int i = 0; i < 1000; i++)
            {
                results.Add(Search(db, alphabet[rng.Next(alphabet.Length)].ToString(), 250));
                Console.WriteLine($"{i}/{1000}");
            }

            Console.WriteLine($"Average: {results.Average()}, Min: {results.Min()}, Max: {results.Max()}");
        }

        static double Search(Database db, string term, int count)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                _ = db.Search(term);
            }

            sw.Stop();
            var result = count / (sw.ElapsedMilliseconds / 1000d);
            Console.WriteLine($"Searched {count} times in {sw.ElapsedMilliseconds}ms, {result}ps");
            return result;
        }

        static void Insert(Database db, int count)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                db.Insert();
            }

            sw.Stop();
            Console.WriteLine($"Inserted {count} records in {sw.ElapsedMilliseconds}ms, {count / (sw.ElapsedMilliseconds / 1000d)}ps");
        }
    }
}