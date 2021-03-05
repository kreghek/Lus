using System;
using System.Collections.Generic;

namespace Lus.TextClient
{
    class StructureScheme
    {
        public StructureScheme()
        {
            RequiredStructures = new HashSet<string>();
            Production = new HashSet<Production>();
        }

        public string Sid { get; set; }

        public string Name { get; set; }
        public int Cost { get; set; }
        public IReadOnlySet<string> RequiredStructures { get; set; }

        public IReadOnlySet<Production> Production { get; set; }

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
            yield return new StructureScheme
            {
                Name = "Settlers Camp",
                Cost = 1000,
                Sid = "settlers-camp",
                Production = new HashSet<Production> 
                {
                    new Production{ Terrain = TerrainType.Fields, Resource = ResourceType.Food, Count= 1 },
                    new Production{ Terrain = TerrainType.Lumber, Resource = ResourceType.Manufactoring, Count= 1 },
                    new Production{ Terrain = TerrainType.Rocks, Resource = ResourceType.Money, Count= 1 },
                }
            };

            yield return new StructureScheme
            {
                Name = "Ore Mine",
                Cost = 5,
                Sid = "ore-mine",
                RequiredStructures = new HashSet<string> { "settlers-camp" },
                Production = new HashSet<Production> {
                    new Production{ Terrain = TerrainType.Rocks, Resource = ResourceType.Manufactoring, Count= 1 },
                }
            };

            yield return new StructureScheme
            {
                Name = "Farm",
                Cost = 5,
                Sid = "farm",
                RequiredStructures = new HashSet<string> { "settlers-camp" },
                Production = new HashSet<Production> {
                    new Production{ Terrain = TerrainType.Fields, Resource = ResourceType.Food, Count= 1 },
                }
            };

            yield return new StructureScheme
            {
                Name = "Sawmill",
                Cost = 5,
                Sid = "sawmill",
                RequiredStructures = new HashSet<string> { "settlers-camp" },
                Production = new HashSet<Production> {
                    new Production{ Terrain = TerrainType.Lumber, Resource = ResourceType.Manufactoring, Count= 1 },
                }
            };
        }

        public static IEnumerable<StructureScheme> All => _all;
    }
}
