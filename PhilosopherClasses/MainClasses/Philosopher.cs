using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using PhilosopherClasses.ConnectionClasses;
using PhilosopherClasses.Enums;
using PhilosopherClasses.MainClasses;
using PhilosopherClasses.PhilEventArgs;
using Timer = System.Timers.Timer;

namespace PhilosopherClasses
{
    public delegate void ChangeState(object sender, PhilosopherEventArgs args);
    public delegate void Death(object sender, DeathEventArgs args);
    public class Philosopher : IDisposable
    {
        private const int TimeOfEating = 1000;
        private const int TimeToDeath = 100000;//30000;
        private const int TimeOfThinking = 1000;
        public Philosopher(int number, IForkExchange conn)
        {
            string writerMutexName = "WrMutex";
            string tableMutexName = "TableMutex";
            _connection = conn;
            _forksState = ForksState.Empty;
            Number = number;
            Mutex.TryOpenExisting(tableMutexName, out _tableMutex);
            if (_tableMutex == null)
            {
                _tableMutex = new Mutex(false, tableMutexName);
            }
            Mutex.TryOpenExisting(writerMutexName, out _writerMutex);
            if (_writerMutex == null)
            {
                _writerMutex = new Mutex(false, writerMutexName);
            }
            _timerStep = new Timer();
            _timerHealth = new Timer();
            _timerStep.Elapsed += Step;
            _timerHealth.Elapsed += Death;
            _tableMutex.WaitOne();
            GetLeftForkIndex(Number, out _leftForkIndex, Table);
            _tableMutex.ReleaseMutex();

        }

        #region Properties

        public event Death DeathEvent;
        public event ChangeState ChangeStateEvent;
        public Table Table
        {
            get
            {
                return _connection.Get();
            }
            set
            {
                _connection.Send(value);
            }
        }
        public int Number { get; private set; }

        public ForksState ForksState
        {
            get { return _forksState; }
        }
        public PhilosopherState State
        {
            get { return _state; }
        }
        #endregion
        public void Start()
        {
            _timerStep.Interval = TimeOfThinking;
            _timerHealth.Interval = TimeToDeath;
            Step(null, null);
            _timerStep.Start();
            _timerHealth.Start();
            //WriteLog(String.Format("{0}  Philosopher - {1} have just started.", DateTime.Now, Number));
        }

        public void Stop()
        {
            _timerStep.Stop();
            _timerHealth.Stop();
        }

        public void WriteLog(string log)
        {
            _writerMutex.WaitOne();
            using (Writer writer = new Writer("log.txt"))
            {
                writer.SaveLog(log + String.Format("{0}", Environment.NewLine));
            }  
            _writerMutex.ReleaseMutex();
        }
        private void Step(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _tableMutex.WaitOne();
            Table table = Table;
            switch (_state)
            {
                case PhilosopherState.ThinkingWithoutForks:
                    TryGetFork(Number, ref _forksState, table);
                    if (_forksState == ForksState.Left)
                    {
                        _state = PhilosopherState.ThinkingWithLeftFork;

                        WriteLog(String.Format("{0}  Philosopher - {1} have just taken a left fork  - {2}.", DateTime.Now, Number, _leftForkIndex));
                        WriteLog(String.Format("{0}  Philosopher - {1} is {2} .", DateTime.Now, Number, _state));
                        UpdateState();
                    }
                    break;
                case PhilosopherState.ThinkingWithLeftFork:
                    TryGetFork(Number, ref _forksState, table);
                    if (_forksState == ForksState.LeftRight)
                    {
                        WriteLog(String.Format("{0}  Philosopher - {1} have just taken a right fork - {2}.", DateTime.Now, Number, Number));
                        WriteLog(String.Format("{0}  Philosopher - {1} is {2} .", DateTime.Now, Number, _state));
                        _state = PhilosopherState.Eating;
                        _timerStep.Interval = TimeOfEating;
                        UpdateState();
                    }
                    break;
                case PhilosopherState.Eating:
                    PutLeftFork(Number, table);
                    WriteLog(String.Format("{0}  Philosopher - {1} have just put the left fork.", DateTime.Now, Number));                                      
                    _forksState = ForksState.Right;
                    UpdateState();
                    PutRightFork(Number, table);
                    _state = PhilosopherState.ThinkingWithoutForks;
                    WriteLog(String.Format("{0}  Philosopher - {1} have just put a right fork.", DateTime.Now, Number));
                    WriteLog(String.Format("{0}  Philosopher - {1} is {2} .", DateTime.Now, Number, _state));                  
                    _forksState = ForksState.Empty;
                    _timerStep.Interval = TimeOfThinking;
                    _timerHealth.Stop();
                    _timerHealth.Start();
                    UpdateState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _tableMutex.ReleaseMutex();
        }

        internal void TryGetFork(int philosopherNumber, ref ForksState philosophForksState, Table table)
        {
            
            if (philosophForksState == ForksState.Empty)
            {
                if (GetPermissionToUseForks(table))
                {
                    if (!table[_leftForkIndex].IsUse)
                    {
                        table[_leftForkIndex].IsUse = true;
                        philosophForksState = ForksState.Left;
                        Table = table;
                        //Debug.WriteLine("{0}  Philosopher № {1} Get Left Fork", DateTime.Now, philosopherNumber);
                    }
                }
            }
            else
            {
                if (philosophForksState == ForksState.Left)
                {
                    TryGetRightFork(philosopherNumber, ref philosophForksState, table);
                }
            }
        }

        internal void PutLeftFork(int numberPh, Table table)
        {
            PutFork(_leftForkIndex, table);
            //Debug.WriteLine("{0}  Philosopher № {1} Put Left Fork № {2}", DateTime.Now, numberPh, indexLeftFork);
        }

        internal void PutRightFork(int numberPh, Table table)
        {
            PutFork(numberPh, table);
            //Debug.WriteLine("{0}  Philosopher № {1} Put Right Fork № {2}", DateTime.Now, numberPh, numberPh);
        }

        private void PutFork(int indexFork, Table table)
        {

            table[indexFork].IsUse = false;
            Table = table;


        }

        private void TryGetRightFork(int numPhil, ref ForksState forkState, Table table)
        {
            if (!table[numPhil].IsUse)
            {
                table[numPhil].IsUse = true;
                forkState = ForksState.LeftRight;
                Table = table;
                //Debug.WriteLine("{0}  Philosopher № {1} Get Right Fork", DateTime.Now, numPhil);
            }
        }

        private void GetLeftForkIndex(int philosopherNumber, out int leftForkIndex, Table table)
        {
            if (philosopherNumber == 0)
            {
                leftForkIndex = table.Length - 1;
            }
            else
            {
                leftForkIndex = philosopherNumber - 1;
            }
        }

        private bool GetPermissionToUseForks(Table table)
        {
            bool result = false;
            int numberOfUsingForks = 0;
            foreach (Fork fork in table)
            {
                if (fork.IsUse)
                {
                    numberOfUsingForks++;
                }
            }
            if (numberOfUsingForks < table.Length - 1)
            {
                result = true;
            }
            return result;
        }


        private void UpdateState()
        {
            //Debug.WriteLine("{0}  Philosopher № {1} {2}", DateTime.Now, Number, _state);
            if (ChangeStateEvent != null)
            {
                ChangeStateEvent(this, new PhilosopherEventArgs(_forksState, _state, Number));
            }
        }

        private void Death(object sender, ElapsedEventArgs e)
        {
            if (DeathEvent != null)
            {
                DeathEvent(this, new DeathEventArgs(Number));
                Stop();
            }
        }
        public void Dispose()
        {
            if (_timerStep != null) _timerStep.Dispose();
            if (_timerHealth != null) _timerHealth.Dispose();
            _connection.Dispose();
        }

       
        private ForksState _forksState;
        private readonly int _leftForkIndex;
        private PhilosopherState _state;
        private readonly Timer _timerStep;
        private readonly Timer _timerHealth;
        private readonly IForkExchange _connection;
        private readonly Mutex _writerMutex;
        private readonly Mutex _tableMutex;

    }
}
