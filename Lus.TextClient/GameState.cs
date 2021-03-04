namespace Lus.TextClient
{
    /// <summary>
    /// The common system text game data.
    /// </summary>
    internal class GameState
    {
        /// <summary>
        /// Current game screen.
        /// </summary>
        public GameScreen CurrentScreen { get; set; }

        public Globe Globe { get; set; }

        public UnitGroup SelectedUnitGroup { get; set; }

        public int Money { get; set; } = 1000;
    }
}
