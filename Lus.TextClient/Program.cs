using System;
using System.Threading.Tasks;

namespace Lus.TextClient
{
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
}
