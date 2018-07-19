using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhilosopherClasses.MainClasses
{
    [Serializable]
    public class Fork : ICloneable
    {
        public Fork(bool isUsing)
        {
            IsUse = isUsing;
        }

        public bool IsUse { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
