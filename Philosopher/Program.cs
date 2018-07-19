using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PhilosopherClasses;
using PhilosopherClasses.ConnectionClasses;
using PhilosopherClasses.PhilEventArgs;


namespace Philosopher
{
    class Program
    {
        static void Main(string[] args)
        {
            int number;
            int.TryParse(args[0], out number);

            //IForkExchange conn = new Connection2("Memory1");
            //IForkExchange conn = new Connection("Pipe")
            using (IForkExchange conn = new Connection2("Memory1"))
            {               
                var philosopher = new PhilosopherClasses.Philosopher(number, conn);
                philosopher.ChangeStateEvent += Show;
                philosopher.DeathEvent += Death;
                var thread = new Thread(philosopher.Start);
                thread.Start();
                thread.Join();
            }
            
            Console.ReadKey();
        }

        private static void Show(object sender, PhilosopherEventArgs args)
        {
            Console.Clear();
            switch (args.State)
            {
                case PhilosopherClasses.Enums.PhilosopherState.ThinkingWithoutForks:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case PhilosopherClasses.Enums.PhilosopherState.ThinkingWithLeftFork:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case PhilosopherClasses.Enums.PhilosopherState.Eating:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;              
            }
            Console.WriteLine("Philosopher № {0}  ForkState - {1}  State - {2}", args.Number, args.ForksState, args.State);
        }

        private static void Death(object sender, DeathEventArgs args)
        {
            Console.WriteLine("Philosopher №{0} has just dead");
        }
    }
}
