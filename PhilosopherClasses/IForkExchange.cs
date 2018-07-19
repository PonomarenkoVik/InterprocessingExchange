using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhilosopherClasses.MainClasses;

namespace PhilosopherClasses
{
    public interface IForkExchange : IDisposable
    {
        Table Get();
        void Send(Table table);
    }
}
