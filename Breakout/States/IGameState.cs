namespace Breakout.States;

using DIKUArcade.GUI;
using DIKUArcade.Input;

public interface IGameState {
    void Update();
    void Render(WindowContext context);
    void HandleKeyEvent(KeyboardAction action, KeyboardKey key);
}