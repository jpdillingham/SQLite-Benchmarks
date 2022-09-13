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

            //for (int i = 0; i < 10; i++)
            //{
            //    Test(db, 10000);
            //}
            Insert(db, 5000);

            for (int i = 0; i < 20; i++)
            {
                Search(db, alphabet[rng.Next(alphabet.Length)].ToString(), 1000);
            }

        }

        static void Search(Database db, string term, int count)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                _ = db.Search(term);
            }

            sw.Stop();
            Console.WriteLine($"Searched {count} times in {sw.ElapsedMilliseconds}ms, {count / (sw.ElapsedMilliseconds / 1000d)}ps");
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