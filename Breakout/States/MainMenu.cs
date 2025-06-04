namespace Breakout.States;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

public class MainMenu : MenuState {

    public MainMenu(StateMachine stateMachine) : base(stateMachine) {
        //initialize background
        backgroundImage = new Image("Breakout.Assets.Images.BreakoutTitleScreen.png");
        backgroundShape = new StationaryShape(0, 0, 1, 1);
        //initialize menu options
        texts = new Text[] {
            new Text("New Game", new Vector2(0.13f, 0.7f), 0.7f ),
            new Text("Level Select", new Vector2(0.13f, 0.64f), 0.7f),
            new Text("Quit", new Vector2(0.13f, 0.555f), 0.7f)
        };
    }

    protected override void PerformAction() {
        switch (active) {
            case 0: // New Game
                stateMachine.ActiveState = new GameRunning(stateMachine, 1);
                break;
            case 1: // Level Select
                stateMachine.ActiveState = new LevelSelect(stateMachine);
                break;
            case 2: // Quit
                QuitGame();
                break;
        }
    }

    private void QuitGame() {
        System.Environment.Exit(1);
    }
}
