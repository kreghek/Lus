using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    internal class GlobeScreenHandler : IScreenHandler
    {
        private GameState _gameState;
        private Label _unitGroupLabel;
        private Label _globeCellDecriptionLabel;

        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            _gameState = gameState;

            Application.Init();
            var top = Application.Top;

            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_Game", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => {
                        Application.RequestStop ();
                    })
                }),
                new MenuBarItem ("_Current Group", new MenuItem [] {
                    new MenuItem ("_Build...", "", () => {
                        var okButton = new Button ("Ok", is_default: true);
                        okButton.Clicked += () => {
                            var building = new Structure(){
                                Name = "Settlers Camp",
                                X = gameState.SelectedUnitGroup.X,
                                Y = gameState.SelectedUnitGroup.Y
                            };

                            gameState.Globe.Structures.Add(building);
                        };
                        var cancelButton = new Button ("Cancel");
                        cancelButton.Clicked += () => { Application.RequestStop (); };

                        var d = new Dialog (
                            "Build something", 50, 20,
                            okButton,
                            cancelButton);

                        var ml2 = new Label (1, 1, "Mouse Debug Line");
                        d.Add (ml2);
                        Application.Run (d);
                    })
                }),
            });

            var battleButton = new Button(1, 1, "Battle!");

            var addUnitButton = new Button(1, 3, "Recruit");

            _unitGroupLabel = new Label(20, 3, $"Fighters: {_gameState.SelectedUnitGroup.Units.Count}");

            var cellInfo = gameState.Globe.Terrain[gameState.SelectedUnitGroup.X, gameState.SelectedUnitGroup.Y].Type;
            _globeCellDecriptionLabel = new Label(20, 5, $"Location: {cellInfo}");

            var globeViewer = new GlobeViewer(1, 5);

            RedrawGlobe(gameState, globeViewer);

            var moveButton = new Button(20, 5, "Test Move");

            moveButton.Clicked += () =>
            {
                gameState.SelectedUnitGroup.X--;
                RedrawGlobe(gameState, globeViewer);
                Application.Refresh();
            };

            top.Add(globeViewer, battleButton, addUnitButton, _unitGroupLabel, _globeCellDecriptionLabel, menu, moveButton);

            battleButton.Clicked += Button_Clicked;
            addUnitButton.Clicked += AddUnitButton_Clicked;

            globeViewer.KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.CursorRight)
                {
                    gameState.SelectedUnitGroup.X--;
                    Application.Refresh();
                }
            };

            Application.Run();

            return Task.FromResult(GameScreen.Battle);
        }

        private static void RedrawGlobe(GameState gameState, GlobeViewer globeViewer)
        {
            var matrix = new Matrix { Items = new int[40, 40] };

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    matrix.Items[i, j] = (int)(gameState.Globe.Terrain[i, j].Type) + 1;
                }
            }

            matrix.Items[gameState.SelectedUnitGroup.X, gameState.SelectedUnitGroup.Y] = 4;

            foreach (var structure in gameState.Globe.Structures)
            {
                matrix.Items[structure.X, structure.Y] = 5;
            }

            globeViewer.Matrix = matrix;
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
