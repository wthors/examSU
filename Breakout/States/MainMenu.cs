namespace Breakout.States;
using System.Numerics;
using Breakout.Blocks;
using Breakout.Level;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

public class MainMenu : IGameState {
    private Text[] texts;
    private int active = 0;
    private StateMachine stateMachine;
    private Image backgroundImage;
    private StationaryShape backgroundShape;

    public MainMenu(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
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

    public void Render(WindowContext context) {
        //render background
        backgroundImage.Render(context, backgroundShape);
        //render menu options
        for (int i = 0; i < texts.Length; i++) {
            if (i == active) {
                texts[i].SetColor(255, 255, 0, 255);
            } else {
                texts[i].SetColor(255, 255, 255, 255);
            }
            texts[i].Render(context);
        }

    }

    public void Update() {
        //no update for a static menu
    }

    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action == KeyboardAction.KeyPress) {
            if (key == KeyboardKey.Up) {
                active = (active > 0) ? active - 1 : texts.Length - 1;
            } else if (key == KeyboardKey.Down) {
                active = (active < texts.Length - 1) ? active + 1 : 0;
            } else if (key == KeyboardKey.Enter) {
                PerformAction();
            }
        }

    }
    private void PerformAction() {
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
