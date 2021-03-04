using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    internal class GlobeScreenHandler : IScreenHandler
    {
        private GameState _gameState;
        private Label _unitGroupLabel;

        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            _gameState = gameState;

            Application.Init();
            var top = Application.Top;

            var battleButton = new Button(1, 1, "Battle!");

            var addUnitButton = new Button(1, 3, "Recruit");

            _unitGroupLabel = new Label(20, 3, $"Fighters: {_gameState.SelectedUnitGroup.Units.Count}");

            var globeViewer = new GlobeViewer(1, 5);

            var matrix = new Matrix { Items = new int[40, 40] };

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    matrix.Items[i, j] = (int)(gameState.Globe.Terrain[i, j].Type) + 1;
                }
            }

            matrix.Items[gameState.SelectedUnitGroup.X, gameState.SelectedUnitGroup.Y] = 4;

            globeViewer.Matrix = matrix;

            top.Add(globeViewer, battleButton, addUnitButton, _unitGroupLabel);

            battleButton.Clicked += Button_Clicked;
            addUnitButton.Clicked += AddUnitButton_Clicked;

            Application.Run();

            return Task.FromResult(GameScreen.Battle);
        }

        private void AddUnitButton_Clicked()
        {
            var unitStat = new UnitStat()
            {
                Hp = 100,
                Damage = 30,
                Team = "1"
            };

            _gameState.SelectedUnitGroup.Units.Add(unitStat);

            _unitGroupLabel.Text = $"Fighters: {_gameState.SelectedUnitGroup.Units.Count}";
        }

        private void Button_Clicked()
        {
            Application.RequestStop();
        }
    }
}
