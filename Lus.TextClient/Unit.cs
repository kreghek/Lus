namespace Lus.TextClient
{
    sealed class Unit
    {
        public int Order { get; set; }

        public int CurrentHp { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public UnitStat Stat { get; set; }

        public bool WasBeAttacked { get; set; }
    }
}
