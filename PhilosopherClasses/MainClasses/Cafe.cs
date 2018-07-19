using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PhilosopherClasses.ConnectionClasses;
using PhilosopherClasses.PhilEventArgs;

namespace PhilosopherClasses.MainClasses
{

    public delegate void TableChangeState(object sender, TableEventArgs args);
    public class Cafe : IDisposable
    {

        public Cafe(int number, IForkExchange conn)
        {
            _connection = conn;
            TableC = new Table(number);
            _connection.Send(TableC);
            string tableMutexName = "TableMutex";

            Mutex.TryOpenExisting(tableMutexName, out _tableMutex);
            if (_tableMutex == null)
            {
                _tableMutex = new Mutex(false, tableMutexName);
            }           
        }

        public TableChangeState StateChanged;

        public void Start()
        {
            for (;;)
            {
                _tableMutex.WaitOne();
                TableC = _connection.Get();
                _connection.Send(TableC);
                StateChanged(this, new TableEventArgs(TableC));
                _tableMutex.ReleaseMutex();
                Thread.Sleep(10);
            }
        }

        public Table TableC { get; set; }



        private readonly IForkExchange _connection;
        private readonly Mutex _tableMutex;

        public void Dispose()
        {
            _connection.Dispose();
        }      
    }
}
