namespace Breakout.States;

using System;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

public class GameWon : IGameState {
    private readonly StateMachine _stateMachine;
    private readonly int _levelNumber;
    private readonly int _finalScore;
    private readonly Image _backgroundImage;
    private readonly StationaryShape _backgroundShape;
    private readonly Text _title;
    private readonly Text _options;
    private readonly Text _scoreText;


    public GameWon(StateMachine stateMachine, int levelNumber, int finalScore) {
        _stateMachine = stateMachine;
        _levelNumber = levelNumber;
        _finalScore = finalScore;

        _backgroundImage = new Image("Breakout.Assets.Images.SpaceBackground.png");
        _backgroundShape = new StationaryShape(0f, 0f, 1f, 1f);

        _title = new Text("You Win!", new Vector2(0.2f, 0.8f), 1.2f);
        _title.SetColor(0, 255, 0);

        _options = new Text("Press Enter to return", new Vector2(0.17f, 0.5f), 0.5f);

        _scoreText = new Text("Final Score: " + _finalScore, new System.Numerics.Vector2(0.20f, 0.7f), 0.7f);
    }

    public void Render(WindowContext context) {
        // Render background
        _backgroundImage.Render(context, _backgroundShape);

        // Render title and score
        _title.Render(context);
        _scoreText.SetColor(225, 225, 225);
        _scoreText.Render(context);

        // Render instructions
        _options.SetColor(200, 200, 200);
        _options.Render(context);
    }

    public void Update() {
        // Nothing dynamic for this menu state
    }

    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action == KeyboardAction.KeyPress && key == KeyboardKey.Enter) {
            _stateMachine.ActiveState = new MainMenu(_stateMachine);
        }
    }
}
