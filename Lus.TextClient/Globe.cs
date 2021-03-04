using System.Collections.Generic;

namespace Lus.TextClient
{
    sealed class Globe
    {
        public Globe()
        {
            UnitGroups = new List<UnitGroup>();
        }

        public List<UnitGroup> UnitGroups { get; }

        public Terrain[,] Terrain { get; set; }
    }
}
