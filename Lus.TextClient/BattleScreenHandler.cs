using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Lus.TextClient
{
    internal class BattleScreenHandler : IScreenHandler
    {
        private List<Unit> _units;
        private GameState _gameState;

        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            _gameState = gameState;

            Application.Init();
            var top = Application.Top;

            var box = new BattlefieldViewer(0, 0);

            var quitButton = new Button(41, 1, "Ends");

            quitButton.Clicked += QuitButton_Clicked;

            top.Add(box, quitButton);

            _units = new List<Unit>();

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
                _units.Add(unit);
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
                _units.Add(unit);
            }

            Task.Run(async () => {
                var counter = 0;

                while (true)
                {
                    var delayTask = Task.Delay(1000);
                    var calculationTask = Task.Run(() => {

                        foreach (var unit in _units)
                        {
                            unit.WasBeAttacked = false;
                        }

                        var orderedUnits = _units.OrderBy(x => Guid.NewGuid()).ToArray();

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
                                        _units.Remove(targetUnit);
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

        private void QuitButton_Clicked()
        {
            var alivePlayerUnits = _units.Where(x => x.Stat.Hp > 0 && x.Stat.Team == "1").Select(x => x.Stat);
            var deadUnitsFromState = _gameState.SelectedUnitGroup.Units.Except(alivePlayerUnits).ToArray();

            foreach (var unit in deadUnitsFromState)
            {
                _gameState.SelectedUnitGroup.Units.Remove(unit);
            }

            Application.RequestStop();
        }
    }
}
