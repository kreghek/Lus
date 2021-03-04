using System.Collections.Generic;

namespace Lus.TextClient
{
    sealed class UnitGroup
    {
        public UnitGroup()
        {
            Units = new List<UnitStat>();
        }

        public List<UnitStat> Units { get; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
