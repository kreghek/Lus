using System.Collections.Generic;

namespace Lus.TextClient
{
    sealed class Globe
    {
        public Globe()
        {
            UnitGroups = new List<UnitGroup>();
            Structures = new List<Structure>();
        }

        public List<UnitGroup> UnitGroups { get; }

        public Terrain[,] Terrain { get; set; }

        public List<Structure> Structures { get; }
    }
}
