namespace Breakout;
using Breakout.States;
using DIKUArcade;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;


public class Game : DIKUGame {
    private readonly StateMachine stateMachine;


    public Game(WindowArgs windowArgs) : base(windowArgs) {
        // Initialize state machine (starts in MainMenu)
        stateMachine = new StateMachine();

    }

    public override void Render(WindowContext context) {
        stateMachine.ActiveState.Render(context);
    }

    public override void Update() {
        stateMachine.ActiveState.Update();
    }

    public override void KeyHandler(KeyboardAction action, KeyboardKey key) {
        stateMachine.ActiveState.HandleKeyEvent(action, key);
    }
}