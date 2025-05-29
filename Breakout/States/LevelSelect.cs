namespace Breakout.States;

using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

public class LevelSelect : IGameState {
    private Text[] texts;
    private Text title;
    private int active = 0;
    private StateMachine stateMachine;
    private Image backgroundImage;
    private StationaryShape backgroundShape;

    public LevelSelect(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
        //initialize background
        backgroundImage = new Image("Breakout.Assets.Images.SpaceBackground.png");
        backgroundShape = new StationaryShape(0, 0, 1, 1);
        //initialize title
        title = new Text("Select Level", new Vector2(0.1f, 0.90f), 1.2f);
        //initialize menu options
        texts = new Text[] {
            new Text("Level 1", new Vector2(0.13f, 0.73f), 0.7f ),
            new Text("Level 2", new Vector2(0.13f, 0.63f), 0.7f),
            new Text("Level 3", new Vector2(0.13f, 0.53f), 0.7f),
            new Text("Level 4", new Vector2(0.13f, 0.43f), 0.7f),
            new Text("Back to Main Menu", new Vector2(0.13f, 0.33f), 0.7f)
        };
    }

    public void Render(WindowContext context) {
        //render background
        backgroundImage.Render(context, backgroundShape);
        //render title
        title.SetColor(255, 255, 255, 255);
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
            case 0: // Level 1
                stateMachine.ActiveState = new GameRunning(stateMachine, 1);
                break;
            case 1: // Level Select
                stateMachine.ActiveState = new GameRunning(stateMachine, 2);
                break;
            case 2: // Level Select
                stateMachine.ActiveState = new GameRunning(stateMachine, 3);
                break;
            case 3: // Level Select
                stateMachine.ActiveState = new GameRunning(stateMachine, 4);
                break;
            case 4: // Back to Main Menu
                stateMachine.ActiveState = new MainMenu(stateMachine);
                break;
        }
    }

}

