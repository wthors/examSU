namespace Breakout.States;

/// <summary>
/// Keeps track of the current and previous game states.
/// </summary>

public class StateMachine {
    public IGameState PreviousState {
        get; private set;
    }
    private IGameState activeState;
    public IGameState ActiveState {
        get => activeState;
        set {
            PreviousState = activeState;
            activeState = value;
        }
    }

    public StateMachine() {
        PreviousState = new MainMenu(this);
        activeState = PreviousState;
    }
}