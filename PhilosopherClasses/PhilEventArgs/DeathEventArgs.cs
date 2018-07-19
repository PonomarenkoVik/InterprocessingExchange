using System;

namespace PhilosopherClasses.PhilEventArgs
{
    public class DeathEventArgs : EventArgs
    {
        public DeathEventArgs(int number)
        {
            Number = number;
        }
        public int Number { get; private set; }
    }
}
