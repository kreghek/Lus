using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    internal class GlobeScreenHandler : IScreenHandler
    {
        private GameState _gameState;
        private Label _unitGroupLabel;
        private Label _globeCellDecriptionLabel;
        private Label _resourcesLabel;
        private Label _timeLabel;

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

                        var list = GetAvailableStructuresList(gameState);
                        var buildingList = new ListView(list){ X = 0, Y = 0, Width = 50, Height = 15};
                        var errorLabel = new Label(0, 16, string.Empty);

                        var okButton = new Button ("Ok", is_default: true);

                        buildingList.SelectedItemChanged += (e) =>
                        {
                            var structure = e.Value as StructureScheme;
                            if (structure.Cost <= gameState.Resources[ResourceType.Money])
                            {
                                errorLabel.Text = string.Empty;
                                Application.Refresh();
                            }
                            else
                            {
                                errorLabel.Text = "Has not enought Money.";
                                Application.Refresh();
                            }
                        };

                        okButton.Clicked += () => {

                            var selectedBuilding = list[buildingList.SelectedItem];

                            if (selectedBuilding.Cost <= gameState.Resources[ResourceType.Money])
                            {
                                var building = new Structure(){
                                    Scheme = selectedBuilding,
                                    X = gameState.SelectedUnitGroup.X,
                                    Y = gameState.SelectedUnitGroup.Y
                                };

                                gameState.Globe.Structures.Add(building);

                                gameState.Resources[ResourceType.Money]-=selectedBuilding.Cost;

                                UpdateResourceLabel();

                                Application.RequestStop ();
                            }
                            else
                            {
                                errorLabel.Text = "Has not enought Money.";
                                Application.Refresh();
                            }
                        };
                        var cancelButton = new Button ("Cancel");
                        cancelButton.Clicked += () => { Application.RequestStop (); };

                        var d = new Dialog (
                            "Build something", 50, 20,
                            okButton,
                            cancelButton);

                        d.Add(buildingList, errorLabel);
                        Application.Run(d);
                    }),

                    new MenuItem("_Recruit...", "", ()=>{

                        if (gameState.Resources[ResourceType.Money] >= 1000 && gameState.Resources[ResourceType.Food] >= 1000)
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
                    })
                }),
            });

            var battleButton = new Button(1, 1, "Next day!");
            _timeLabel = new Label(1, 2, "1 day of Spring, 1 year");

            _unitGroupLabel = new Label(20, 1, $"Fighters: {_gameState.SelectedUnitGroup.Units.Count}");
            _resourcesLabel = new Label(20, 2, "$1000 E1000 T1000 F1000");

            var cellInfo = gameState.Globe.Terrain[gameState.SelectedUnitGroup.X, gameState.SelectedUnitGroup.Y].Type;
            _globeCellDecriptionLabel = new Label(20, 3, $"Location: {cellInfo}");

            var globeViewer = new GlobeViewer(1, 5);
            globeViewer.SetFocus();

            RedrawGlobe(gameState, globeViewer);

            top.Add(globeViewer, battleButton, _unitGroupLabel, _globeCellDecriptionLabel, menu, _resourcesLabel);

            battleButton.Clicked += ()=> { CalculateNextDay(gameState); UpdateResourceLabel(); };

            globeViewer.KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.CursorRight)
                {
                    gameState.SelectedUnitGroup.X++;
                }
                else if (e.KeyEvent.Key == Key.CursorLeft)
                {
                    gameState.SelectedUnitGroup.X--;
                }
                else if (e.KeyEvent.Key == Key.CursorUp)
                {
                    gameState.SelectedUnitGroup.Y--;
                }
                else if (e.KeyEvent.Key == Key.CursorDown)
                {
                    gameState.SelectedUnitGroup.Y++;
                }

                RedrawGlobe(gameState, globeViewer);
                Application.Refresh();

                e.Handled = true;
            };

            UpdateResourceLabel();

            Application.Run();

            return Task.FromResult(GameScreen.Battle);
        }

        private static void CalculateNextDay(GameState gameState)
        {
            foreach (var structure in gameState.Globe.Structures)
            {
                if (HasWorkingPlayerGroup(structure, gameState.Globe.UnitGroups))
                {
                    var connectedTerrains = GetConnectedTerrains(structure, gameState.Globe.Terrain).ToList();

                    foreach (var production in structure.Scheme.Production)
                    {
                        var terrainsOfType = connectedTerrains.Where(x => x.Type == production.Terrain);
                        if (terrainsOfType.Any())
                        {
                            gameState.Resources[production.Resource] += production.Count * terrainsOfType.Count();
                        }
                    }
                }
            }
        }

        private static IEnumerable<Terrain> GetConnectedTerrains(Structure structure, Terrain[,] terrain)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var targetX = structure.X + i;
                    var targetY = structure.Y + j;

                    if (targetX < 0)
                    {
                        continue;
                    }

                    if (targetY < 0)
                    {
                        continue;
                    }

                    if (targetX >= terrain.GetUpperBound(0))
                    {
                        continue;
                    }

                    if (targetY >= terrain.GetUpperBound(1))
                    {
                        continue;
                    }

                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    yield return terrain[structure.X + i, structure.Y + j];
                }
            }
        }

        private static bool HasWorkingPlayerGroup(Structure structure, List<UnitGroup> unitGroups)
        {
            return unitGroups.Where(x => x.X == structure.X && x.Y == structure.Y).Sum(x => x.Units.Count()) > 0;
        }

        private static StructureScheme[] GetAvailableStructuresList(GameState gameState)
        {
            return Structures.All.Where(x=>CheckRequiredStructures(x, gameState)).ToArray();
        }

        private static bool CheckRequiredStructures(StructureScheme targetStructure, GameState gameState)
        {
            var allPlayerStructuresSids = gameState.Globe.Structures.Select(x => x.Scheme.Sid).Distinct().ToArray();
            return !targetStructure.RequiredStructures.Except(allPlayerStructuresSids).Any();
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

        private void UpdateResourceLabel()
        {
            _resourcesLabel.Text = $"${_gameState.Resources[ResourceType.Money]}" + " "
                + $"E{_gameState.Resources[ResourceType.Energy]}" + " "
                + $"T{_gameState.Resources[ResourceType.Manufactoring]}" + " "
                + $"F{_gameState.Resources[ResourceType.Food]}";
        }
    }
}
