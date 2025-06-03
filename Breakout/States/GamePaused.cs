namespace Breakout.States;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

public class GamePaused : IGameState {
    private Text[] texts;
    private Text title;
    private int active = 0;
    private StateMachine stateMachine;
    private Image backgroundImage;
    private StationaryShape backgroundShape;

    public GamePaused(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
        //initialize background
        backgroundImage = new Image("Breakout.Assets.Images.SpaceBackground.png");
        backgroundShape = new StationaryShape(0, 0, 1, 1);
        title = new Text("Game Paused", new Vector2(0.05f, 0.90f), 1.2f);
        //initialize menu options
        texts = new Text[] {
            new Text("Continue", new Vector2(0.25f, 0.55f)),
            new Text("Main Menu", new Vector2(0.2f, 0.45f))
        };

    }
    public void Render(WindowContext context) {
        //render background
        backgroundImage.Render(context, backgroundShape);
        title.Render(context);

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
            case 0: // Continue
                if (stateMachine.PreviousState is GameRunning playing) {
                    playing.ResetTimer();
                }
                stateMachine.ActiveState = stateMachine.PreviousState;
                break;
            case 1: // Main Menu
                stateMachine.ActiveState = new MainMenu(stateMachine);
                break;
        }
    }

}