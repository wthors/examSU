namespace Breakout;

using DIKUArcade.GUI;

/// <summary>
/// Application entry point for launching the Breakout game.
/// </summary>

class Program {
    static void Main(string[] args) {
        var windowArgs = new WindowArgs() { Title = "Breakout" };
        var game = new Game(windowArgs);
        game.Run();
    }
}