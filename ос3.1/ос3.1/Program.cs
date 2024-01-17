

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace os3._1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Warehouse warehouse = new Warehouse();
            List<string> titles = new List<string>
        {
            "Harry Potter and the Philosopher's Stone",
            "Harry Potter and the Chamber of Secrets",
            "Harry Potter and the Prisoner of Azkaban",
            "Harry Potter and the Goblet of Fire",
            "Harry Potter and the Half-Blood Prince",
            "Harry Potter and the Deathly Hallows"
        };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            DateTime timeBeforeFirstFetch = DateTime.Now;

            // Створюємо список завдань асинхронного отримання статусу книги
            List<Task<(string Title, Warehouse.ItemStatus Status)>> tasks = new List<Task<(string, Warehouse.ItemStatus)>>();

            foreach (string title in titles)
            {
                tasks.Add(FetchBookStatusAsync(warehouse, title));
            }

            // Очікуємо завершення всіх асинхронних завдань
            await Task.WhenAll(tasks);

            // Виводимо результати
            foreach (var task in tasks)
            {
                var result = task.Result;
                Console.WriteLine($"{result.Title} - {result.Status}");
            }

            stopwatch.Stop();
            Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Time elapsed: {DateTime.Now - timeBeforeFirstFetch}");

        }

        // Асинхронний метод для отримання статусу книги
        private static async Task<(string, Warehouse.ItemStatus)> FetchBookStatusAsync(Warehouse warehouse, string title)
        {
            return (title, await Task.Run(() => warehouse.FetchBookStatus(title)));
        }
    }

    public class Warehouse
    {
        public enum ItemStatus { AVAILABLE, PRE_ORDER, UNAVAILABLE }

        public ItemStatus FetchBookStatus(string bookTitle)
        {
            // Імітація затримки на 2 секунди
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Генерація випадкового індексу для статусу товару
            Random random = new Random();
            int idx = random.Next(0, Enum.GetValues(typeof(ItemStatus)).Length);

            return (ItemStatus)Enum.GetValues(typeof(ItemStatus)).GetValue(idx);
        }
    }
}
