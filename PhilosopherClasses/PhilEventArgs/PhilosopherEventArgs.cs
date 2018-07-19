using System;
using PhilosopherClasses.Enums;

namespace PhilosopherClasses.PhilEventArgs
{
    public class PhilosopherEventArgs : EventArgs
    {
        public PhilosopherEventArgs(ForksState forksState, PhilosopherState state, int number)
        {
            ForksState = forksState;
            Number = number;
            State = state;
        }
        public ForksState ForksState { get; private set; }
        public int Number { get; private set; }
        public PhilosopherState State { get; private set; }
    }
}
