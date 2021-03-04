using System;
using System.Collections.Generic;

namespace Lus.TextClient
{
    class StructureScheme
    {
        public StructureScheme()
        {
            RequiredStructures = new HashSet<string>();
        }

        public string Sid { get; set; }

        public string Name { get; set; }
        public int Cost { get; set; }
        public IReadOnlySet<string> RequiredStructures { get; set; }


        public override string ToString()
        {
            return $"{Name} ${Cost}";
        }
    }

    class Production
    {
        public TerrainType Terrain { get; set; }
        public ResourceType Resource { get; set; }
        public int Count { get; set; }
    }

    class Structure
    {
        public StructureScheme Scheme { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    static class Structures
    {
        private static IEnumerable<StructureScheme> _all;

        static Structures()
        {
            _all = Initialize();
        }

        private static IEnumerable<StructureScheme> Initialize()
        {
            yield return new StructureScheme { Name = "Settlers Camp", Cost = 1000, Sid = "settlers-camp" };
            yield return new StructureScheme { Name = "Ore Mine", Cost = 5, Sid = "ore-mine", RequiredStructures = new HashSet<string> { "settlers-camp" } };
        }

        public static IEnumerable<StructureScheme> All => _all;
    }
}
