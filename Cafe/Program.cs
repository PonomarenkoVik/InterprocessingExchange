using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PhilosopherClasses;
using PhilosopherClasses.ConnectionClasses;
using PhilosopherClasses.PhilEventArgs;
using PhilosopherClasses.MainClasses;

namespace Cafe
{
    class Program
    {
        static void Main(string[] args)
        {
            int number;
            int.TryParse(args[0], out number);
            //IForkExchange conn = new Connection( "Pipe")
            using (IForkExchange conn = new Connection2("Memory1"))
            {
                var cafe = new PhilosopherClasses.MainClasses.Cafe(number, conn);
                cafe.StateChanged += Show;
                var thread = new Thread(cafe.Start);
                thread.Start();
                thread.Join(); 
            }
            
            
            Console.ReadKey();
        }

        private static void Show(object sender, TableEventArgs args)
        {
            
            bool print = false;
            if (_table == null)
            {
                _table = args.TableC;
                Print(args);
            }
            for (int i = 0; i < args.TableC.Length; i++)
			{
                if (_table[i].IsUse != args.TableC[i].IsUse)
                {
                    print = true;
                    break;
                }
			}

            if (print)
            {
                Print(args);
            }
            
        }

        private static void Print(TableEventArgs args)
        {
            Console.Clear();
            for (int i = 0; i < args.TableC.Length; i++)
            {
                if (args.TableC[i].IsUse)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("Fork № {0} IsUse = {1}", i, args.TableC[i].IsUse);
            }
        }

       private static Table _table;
    }
}
