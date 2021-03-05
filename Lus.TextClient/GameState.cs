using System.Collections.Generic;

namespace Lus.TextClient
{
    /// <summary>
    /// The common system text game data.
    /// </summary>
    internal class GameState
    {
        public GameState()
        {
            Resources = new Dictionary<ResourceType, int> {
                { ResourceType.Money, 1000 },
                { ResourceType.Manufactoring, 0 },
                { ResourceType.Food, 0 },
                { ResourceType.Energy, 0 },
            };
        }

        /// <summary>
        /// Current game screen.
        /// </summary>
        public GameScreen CurrentScreen { get; set; }

        public Globe Globe { get; set; }

        public UnitGroup SelectedUnitGroup { get; set; }

        public int TimeCounter { get; set; } = 1;

        public Dictionary<ResourceType, int> Resources { get; }
    }
}
