using System.Collections.Generic;
using System.Threading;

namespace ос3._3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Створення черги з максимальною кількістю елементів 10
            IMessageQueue messageQueue = new BlockingMessageQueue(10);

            // Запуск кількох потоків, які додають та видаляють елементи
            for (int i = 0; i < 5; i++)
            {
                int threadNumber = i;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        messageQueue.Add(new Message($"Thread {threadNumber} - Message {j}"));
                        Console.WriteLine($"Thread {threadNumber} added a message");
                        Thread.Sleep(300);
                    }
                });

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Message message = messageQueue.Poll();
                        Console.WriteLine($"Thread {threadNumber} received: {message.GetData()}");
                        Thread.Sleep(300);
                    }
                });
            }

            // Зачекайте, щоб дати час всім потокам виконати свої завдання
            Thread.Sleep(5000);
        }

        public class Message
        {
            // Опціональні дані для повідомлення
            private readonly string data;

            public Message(string data)
            {
                this.data = data;
            }

            public string GetData()
            {
                return data;
            }
        }

        public interface IMessageQueue
        {
            void Add(Message message);
            Message Poll();
        }

        public class BlockingMessageQueue : IMessageQueue
        {
            private readonly Queue<Message> queue;
            private readonly int capacity;

            public BlockingMessageQueue(int capacity)
            {
                this.queue = new Queue<Message>();
                this.capacity = capacity;
            }

            public void Add(Message message)
            {
                lock (queue)
                {
                    // Чекаємо, поки черга не стане не повною
                    while (queue.Count == capacity)
                    {
                        Monitor.Wait(queue);
                    }

                    // Додаємо повідомлення в чергу
                    queue.Enqueue(message);

                    // Повідомляємо всі потоки, які чекають на можливість додати елемент
                    Monitor.PulseAll(queue);
                }
            }

            public Message Poll()
            {
                lock (queue)
                {
                    // Чекаємо, поки черга не стане не пустою
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(queue);
                    }

                    // Видаляємо та повертаємо перше повідомлення
                    Message message = queue.Dequeue();

                    // Повідомляємо всі потоки, які чекають на можливість взяти елемент
                    Monitor.PulseAll(queue);

                    return message;
                }
            }
        }

    }
}