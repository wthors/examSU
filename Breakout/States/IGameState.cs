namespace Breakout.States;

using DIKUArcade.GUI;
using DIKUArcade.Input;

/// <summary>
/// Common interface for all game states.
/// </summary>

public interface IGameState {
    void Update();
    void Render(WindowContext context);
    void HandleKeyEvent(KeyboardAction action, KeyboardKey key);
}