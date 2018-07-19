using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhilosopherClasses.MainClasses
{
    [Serializable]
    public class Table : IEnumerable, ICloneable
    {
        public Table(int forkNumbers)
        {
            _forks = new Fork[forkNumbers];
            for (int i = 0; i < _forks.Length; i++)
            {
                _forks[i] = new Fork(false);
            }
        }


        public Fork this[int index]
        {
            get
            {
                return _forks[index];
            }
            set
            {
                _forks[index] = value;
            }
        }
        public int Length
        {
            get
            {
                return _forks.Length;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _forks.GetEnumerator();
        }

        public object Clone()
        {
            var table = (Table)MemberwiseClone();
            table._forks = new Fork[table.Length];
            for (int i = 0; i < table.Length; i++)
            {
                table._forks[i] = (Fork)_forks[i].Clone();
            }
            return table;
        }


        private Fork[] _forks;   
    }
}
