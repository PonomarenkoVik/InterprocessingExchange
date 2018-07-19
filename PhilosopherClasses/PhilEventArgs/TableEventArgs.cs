using System;
using PhilosopherClasses.MainClasses;

namespace PhilosopherClasses.PhilEventArgs
{
    public class TableEventArgs : EventArgs
    {
        public TableEventArgs(Table table)
        {
            TableC = (Table)table.Clone();
        }
        public Table TableC { get; private set; }
    }
}
