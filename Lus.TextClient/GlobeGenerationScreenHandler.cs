using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    internal class GlobeGenerationScreenHandler : IScreenHandler
    {
        private GameState _gameState;

        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            _gameState = gameState;

            Application.Init();
            var top = Application.Top;

            var button = new Button(1, 1, "Generate!");

            top.Add(button);

            button.Clicked += Button_Clicked;

            Application.Run();

            return Task.FromResult(GameScreen.Globe);
        }

        private void Button_Clicked()
        {
            var globeSize = 10;

            var globe = new Globe()
            {
                Terrain = new Terrain[globeSize, globeSize]
            };

            for (var i = 0; i < globeSize; i++)
            {
                for (var j = 0; j < globeSize; j++)
                {
                    globe.Terrain[i, j] = new Terrain { Type = (TerrainType)((i * j * 11) % 3) };
                }
            }

            var unitGroup = new UnitGroup() { X = 5, Y = 5 };
            for (var i = 0; i < 10; i++)
            {
                var unitStat = new UnitStat()
                {
                    Hp = 100,
                    Damage = 30,
                    Defence = 1,
                    Team = "1"
                };

                unitGroup.Units.Add(unitStat);
            }

            _gameState.Globe = globe;
            _gameState.SelectedUnitGroup = unitGroup;
            _gameState.Globe.UnitGroups.Add(unitGroup);

            Application.RequestStop();
        }
    }
}
