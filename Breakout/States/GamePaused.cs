namespace Breakout.States;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;

/// <summary>
/// Menu displayed when the player pauses the game.
/// </summary>

public class GamePaused : MenuState {
    private Text title;

    public GamePaused(StateMachine stateMachine) : base(stateMachine) {
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
    public override void Render(WindowContext context) {
        backgroundImage!.Render(context, backgroundShape!);
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
    protected override void PerformAction() {
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