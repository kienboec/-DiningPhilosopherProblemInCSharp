using System;
using System.Threading;

namespace DiningPhilosopherProblemInCSharp
{
    class Program
    {
        // https://www.geeksforgeeks.org/dining-philosopher-problem-using-semaphores/
        private const int NUMBER_OF_PHILOSOPHERS = 5;
        const int NUMBER_OF_ROUNDS = 5;

        private static SemaphoreSlim[] chopsticks;
        private static Thread[] philosophers;

        static void Main(string[] args)
        {
            chopsticks = new SemaphoreSlim[NUMBER_OF_PHILOSOPHERS];
            philosophers = new Thread[NUMBER_OF_PHILOSOPHERS];
            for (int i = 0; i < NUMBER_OF_PHILOSOPHERS; i++)
            {
                chopsticks[i] = new SemaphoreSlim(1, 1);
            }

            for (int i = 0; i < NUMBER_OF_PHILOSOPHERS; i++)
            {
                philosophers[i] = new Thread(PhilosopherMain);
                philosophers[i].Start(i);
                Console.WriteLine($"Starting philosopher {i} on {philosophers[i].ManagedThreadId}");
            }

            for (int i = 0; i < NUMBER_OF_PHILOSOPHERS; i++)
            {
                philosophers[i].Join();
                Console.WriteLine($"Waiting for philosopher {i}");
            }

            Console.WriteLine("done");
        }

        static void PhilosopherMain(object parameter)
        {
            int philosopherNumber = (int)parameter;
            for (int round = 0; round < NUMBER_OF_ROUNDS; round++)
            {
                Console.WriteLine($"Philosopher {philosopherNumber} starts round {round}");
                Think(philosopherNumber);
                PickUp(philosopherNumber);
                Eat(philosopherNumber);
                PutDown(philosopherNumber);
            }

            Thread.Sleep(3_000); // seconds

            Console.WriteLine("Philosopher {0} done on thread: {1}",
                philosopherNumber,
                Thread.CurrentThread.ManagedThreadId);
        }

        static void Think(int philosopherNumber)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int sleepTime = (random.Next() % 3 + 1) * 100;
            Console.WriteLine("Philosopher {0} will think for {1} micro-seconds",
                philosopherNumber,
                sleepTime);
            Thread.Sleep(sleepTime);
        }

        static void PickUp(int philosopherNumber)
        {
            int right = (philosopherNumber + 1) % NUMBER_OF_PHILOSOPHERS;
            int left = (philosopherNumber + NUMBER_OF_PHILOSOPHERS) % NUMBER_OF_PHILOSOPHERS;

            int first = philosopherNumber % 2 == 0 ? right : left;
            int second = philosopherNumber % 2 == 0 ? left : right;

            Console.WriteLine("Philosopher {0} is waiting to pick up chopstick {1}", philosopherNumber, first);

            chopsticks[first].Wait();
            Console.WriteLine("Philosopher {0} picked up chopstick {1}", philosopherNumber, first);

            Console.WriteLine("Philosopher {0} is waiting to pick up chopstick {1}", philosopherNumber, second);
            chopsticks[second].Wait();
            Console.WriteLine("Philosopher {0} picked up chopstick {1}", philosopherNumber, second);
        }

        static void Eat(int philosopherNumber)
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            int eatTime = (random.Next() % 3 + 1) * 100;
            Console.WriteLine("Philosopher {0} will eat for {1} micro-seconds", philosopherNumber, eatTime);
            Thread.Sleep(eatTime);
        }

        static void PutDown(int philosopherNumber)
        {
            Console.WriteLine("Philosopher {0} will will put down her chopsticks", philosopherNumber);
            chopsticks[(philosopherNumber + 1) % NUMBER_OF_PHILOSOPHERS].Release();
            chopsticks[(philosopherNumber + NUMBER_OF_PHILOSOPHERS) % NUMBER_OF_PHILOSOPHERS].Release();
        }
    }
}
