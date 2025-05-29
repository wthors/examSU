namespace Breakout.States;

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