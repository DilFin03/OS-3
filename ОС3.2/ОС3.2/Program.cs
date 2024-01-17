using System;
using System.Threading;

class Program
{
    static void Main()
    {
        // Створюємо екземпляр класу Once
        IOnce once = new Once();

        // Тест 1: Перша спроба виклику
        Console.WriteLine("Test 1:");
        once.Exec(() => Console.WriteLine("Action executed!"));

        // Тест 2: Друга спроба виклику - має ігноруватися
        Console.WriteLine("\nTest 2:");
        once.Exec(() => Console.WriteLine("Action should not be executed!"));

        // Тест 3: Спроба виклику після виклику методу Exec - має ігноруватися
        Console.WriteLine("\nTest 3:");
        once.Exec(() => Console.WriteLine("Action should not be executed!"));
        
        
        // Тест 4: Використання багато потоків
        Console.WriteLine("\nTest 4: Using multiple threads");

        int numThreads = 5;
        CountdownEvent countdownEvent = new CountdownEvent(numThreads);

        for (int i = 0; i < numThreads; i++)
        {
            Task.Run(() =>
            {
                once.Exec(() => Console.WriteLine("Thread {0} executed!", Thread.CurrentThread.ManagedThreadId));
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();

        Console.ReadLine();
    }
}

public interface IOnce
{
    void Exec(Action action);
}

public class Once : IOnce
{
    private volatile int executed = 0;

    public void Exec(Action action)
    {
        if (executed == 0 && Interlocked.CompareExchange(ref executed, 1, 0) == 0)
        {
            action.Invoke();
        }
    }
}

