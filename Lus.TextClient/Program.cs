using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    class BattlefieldViewer : View
    {
        int w = 40;
        int h = 40;

        public bool WantCursorPosition { get; set; } = false;

        public BattlefieldViewer(int x, int y) : base(new Rect(x, y, 40, 40))
        {
        }

        public Size GetContentSize()
        {
            return new Size(w, h);
        }

        public void SetCursorPosition(Point pos)
        {
            throw new NotImplementedException();
        }

        public Matrix Matrix { get; set; }

        public override void Redraw(Rect bounds)
        {
            if (Matrix is null)
            {
                return;
            }

            for (var i = 0; i < w; i++)
            {
                for (var j = 0; j < h; j++)
                {
                    Move(i, j);

                    if (Matrix.Items[i, j] == 0)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.DarkGray, Color.Black));
                        Driver.AddStr("░");
                    }
                    else if (Matrix.Items[i, j] == 1)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black));
                        Driver.AddStr("@");
                    }
                    else if (Matrix.Items[i, j] == 2)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.BrightRed, Color.Black));
                        Driver.AddStr("@");
                    }
                }
            }
        }
    }

    public class Matrix
    { 
        public int[,] Items { get; set; }
    }

    

    class Program
    {
        static async Task Main()
        {
            var gameState = new GameState
            {
                CurrentScreen = GameScreen.GlobeGeneration,
            };

            var battleScreenHandler = new BattleScreenHandler();
            var globeScreenHandler = new GlobeScreenHandler();
            var globeGenerationScreenHandler = new GlobeGenerationScreenHandler();

            IScreenHandler screenHandler = globeGenerationScreenHandler;

            do
            {
                var nextScreen = await screenHandler.StartProcessingAsync(gameState).ConfigureAwait(false);

                screenHandler = nextScreen switch
                {
                    GameScreen.Globe => globeScreenHandler,
                    GameScreen.GlobeGeneration => globeGenerationScreenHandler,
                    GameScreen.Battle => battleScreenHandler,
                    _ => throw new InvalidOperationException($"Unsupported screen {nextScreen}.")
                };
            } while (true);
        }
    }

    sealed class UnitStat
    { 
        public int Hp { get; set; }

        public int Damage { get; set; }

        public int Defence { get; set; }

        public string Team { get; set; }
    }

    sealed class Unit
    {
        public int Order { get; set; }

        public int CurrentHp { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public UnitStat Stat { get; set; }

        public bool WasBeAttacked { get; set; }
    }

    sealed class GlobeViewer : View 
    {
        int w = 40;
        int h = 40;

        public bool WantCursorPosition { get; set; } = false;

        public GlobeViewer(int x, int y) : base(new Rect(x, y, 40, 40))
        {
        }

        public Size GetContentSize()
        {
            return new Size(w, h);
        }

        public void SetCursorPosition(Point pos)
        {
            throw new NotImplementedException();
        }

        public Matrix Matrix { get; set; }

        public override void Redraw(Rect bounds)
        {
            if (Matrix is null)
            {
                return;
            }

            for (var i = 0; i < w; i++)
            {
                for (var j = 0; j < h; j++)
                {
                    Move(i, j);

                    if (Matrix.Items[i, j] == 0)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.DarkGray, Color.Black));
                        Driver.AddStr("?");
                    }
                    else if (Matrix.Items[i, j] == 1)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.DarkGray, Color.Black));
                        Driver.AddStr("𓆭");
                    }
                    else if (Matrix.Items[i, j] == 2)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.Gray, Color.Black));
                        Driver.AddStr("∎");
                    }
                    else if (Matrix.Items[i, j] == 3)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.Brown, Color.Black));
                        Driver.AddStr("_");
                    }
                    else if (Matrix.Items[i, j] == 4)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black));
                        Driver.AddStr("@");
                    }
                }
            }
        }
    }

    sealed class Globe
    {
        public Globe()
        {
            UnitGroups = new List<UnitGroup>();
        }

        public List<UnitGroup> UnitGroups { get; }

        public Terrain[,] Terrain { get; set; }
    }

    sealed class UnitGroup
    {
        public UnitGroup()
        {
            Units = new List<UnitStat>();
        }

        public List<UnitStat> Units { get; }
    }

    sealed class Terrain
    {
        public TerrainType Type { get; set; }
    }

    enum TerrainType
    { 
        Lumber,
        Rocks,
        Fields
    }

    sealed class Fraction
    { 
        
    }

    sealed class Resource
    { 

    }

    /// <summary>
    /// Screen handler interface to work with current game state.
    /// Screen == game state.
    /// </summary>
    internal interface IScreenHandler
    {
        /// <summary>
        /// Processing game state while next screen will be transited.
        /// </summary>
        /// <param name="gameState"> Inner common game data. </param>
        /// <returns> Identifier of next game screen. </returns>
        Task<GameScreen> StartProcessingAsync(GameState gameState);
    }

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
    }

    /// <summary>
    /// Identifier of a game screen.
    /// A game screen is a game state.
    /// </summary>
    internal enum GameScreen
    {
        /// <summary>
        /// Undefined screen may be if error in the code.
        /// </summary>
        Undefinded,

        /// <summary>
        /// Globe settings and generation screen.
        /// </summary>
        GlobeGeneration,

        /// <summary>
        /// Global fraction management screen.
        /// </summary>
        Globe,

        /// <summary>
        /// Battle screen.
        /// </summary>
        Battle,
    }

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
                    globe.Terrain[i, j] = new Terrain { Type = (TerrainType)((i * j * 7) % 3) };
                }
            }

            var unitGroup = new UnitGroup();
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

            Application.RequestStop();
        }
    }

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
                    matrix.Items[i, j] = (int)(gameState.Globe.Terrain[i, j].Type + 1);
                }
            }

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

    internal class BattleScreenHandler : IScreenHandler
    {
        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            Application.Init();
            var top = Application.Top;

            var box = new BattlefieldViewer(0, 0);

            top.Add(box);

            var units = new List<Unit>();

            var matrixSize = 40;
            var unitMatrix = new Unit[matrixSize, matrixSize];

            var playerUnits = gameState.SelectedUnitGroup.Units.OrderBy(x=>Guid.NewGuid()).ToArray();
            for (var i = 0; i < playerUnits.Length; i++)
            {
                var unitStat = playerUnits[i];

                var unit = new Unit
                {
                    CurrentHp = unitStat.Hp,
                    Stat = unitStat,
                    X = i % 10,
                    Y = i / 10,
                };
                unitMatrix[unit.X, unit.Y] = unit;
                units.Add(unit);
            }

            for (var i = 0; i < 10; i++)
            {
                var unitStat = new UnitStat()
                {
                    Hp = 70,
                    Damage = 60,
                    Team = "2"
                };

                var unit = new Unit
                {
                    CurrentHp = unitStat.Hp,
                    Stat = unitStat,
                    X = i,
                    Y = matrixSize - 1
                };
                unitMatrix[i, matrixSize - 1] = unit;
                units.Add(unit);
            }

            Task.Run(async () => {
                var counter = 0;

                while (true)
                {
                    var delayTask = Task.Delay(1000);
                    var calculationTask = Task.Run(() => {

                        foreach (var unit in units)
                        {
                            unit.WasBeAttacked = false;
                        }

                        var orderedUnits = units.OrderBy(x => Guid.NewGuid()).ToArray();

                        foreach (var unit in orderedUnits)
                        {
                            if (unit.Stat.Hp <= 0)
                            {
                                continue;
                            }

                            if (unit.WasBeAttacked)
                            {
                                continue;
                            }

                            var moveQ = 0;
                            if (unit.Stat.Team == "1")
                            {
                                moveQ = 1;
                            }
                            else
                            {
                                moveQ = -1;
                            }

                            var targetX = unit.X;
                            var targetY = unit.Y + moveQ;

                            if (targetY >= 0 && targetY <= matrixSize - 1)
                            {
                                var targetUnit = unitMatrix[targetX, targetY];
                                if (targetUnit is null)
                                {
                                    unitMatrix[unit.X, unit.Y] = null;
                                    unitMatrix[targetX, targetY] = unit;
                                    unit.X = targetX;
                                    unit.Y = targetY;
                                }
                                else if (unit.Stat.Team != targetUnit.Stat.Team)
                                {
                                    var damage = unit.Stat.Damage - targetUnit.Stat.Defence;
                                    damage = Math.Max(0, damage);

                                    targetUnit.CurrentHp -= damage;
                                    targetUnit.WasBeAttacked = true;
                                    if (targetUnit.CurrentHp <= 0)
                                    {
                                        units.Remove(targetUnit);
                                        unitMatrix[unit.X, unit.Y] = null;
                                        unitMatrix[targetUnit.X, targetUnit.Y] = unit;
                                        unit.X = targetX;
                                        unit.Y = targetY;
                                    }
                                }
                                else
                                {
                                    // Ally unit blocks the path. Just wait.
                                }
                            }
                        }
                    });

                    await Task.WhenAll(delayTask, calculationTask);

                    var matrix = new Matrix() { Items = new int[matrixSize, matrixSize] };

                    for (var i = 0; i < matrixSize; i++)
                    {
                        for (var j = 0; j < matrixSize; j++)
                        {
                            var unit = unitMatrix[i, j];

                            if (unit is null)
                            {
                                continue;
                            }

                            if (unit.Stat.Team == "1")
                            {
                                matrix.Items[i, j] = 1;
                            }
                            else if (unit.Stat.Team == "2")
                            {
                                matrix.Items[i, j] = 2;
                            }

                        }
                    }

                    counter++;

                    box.Matrix = matrix;
                    Application.Refresh();
                }
            });

            Application.Run();

            return Task.FromResult(GameScreen.Globe);
        }
    }
}
