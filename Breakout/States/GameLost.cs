namespace Breakout.States;
using System.Numerics;
using Breakout.Points;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

public class GameLost : IGameState {
    private readonly StateMachine _stateMachine;
    private readonly int _levelNumber;

    private readonly Image _backgroundImage;
    private readonly StationaryShape _backgroundShape;
    private readonly Text _title;
    private readonly Text[] _options;
    private int _activeOption;
    private int _finalScore;
    private readonly Text _scoreText;

    public GameLost(StateMachine stateMachine, int levelNumber, int finalScore) {
        _stateMachine = stateMachine;
        _levelNumber = levelNumber;
        _finalScore = finalScore;

        // Background
        _backgroundImage = new Image("Breakout.Assets.Images.SpaceBackground.png");
        _backgroundShape = new StationaryShape(0f, 0f, 1f, 1f);

        // Title
        _title = new Text("Game Over", new Vector2(0.15f, 0.8f), 1.2f);

        // Score display
        _scoreText = new Text($"Score: {_finalScore}", new Vector2(0.2f, 0.7f));

        // Menu options
        _options = new Text[] {
    new Text("Retry Level", new Vector2(0.18f, 0.5f)),
    new Text("Main Menu", new Vector2(0.2f, 0.4f))
  };
        _activeOption = 0;
    }

    public void Render(WindowContext context) {
        // Render background and title
        _backgroundImage.Render(context, _backgroundShape);
        _title.Render(context);
        _scoreText.Render(context);

        // Render options, highlighting the active one
        for (int i = 0; i < _options.Length; i++) {
            if (i == _activeOption) {
                _options[i].SetColor(255, 255, 0, 255);
            } else {
                _options[i].SetColor(255, 255, 255, 255);
            }
            _options[i].Render(context);
        }
    }

    public void Update() {
        // No dynamic updates needed for this menu state
    }

    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action != KeyboardAction.KeyPress) {
            return;
        }

        switch (key) {
            case KeyboardKey.Up:
                _activeOption = (_activeOption > 0)
                  ? _activeOption - 1
                  : _options.Length - 1;
                break;

            case KeyboardKey.Down:
                _activeOption = (_activeOption < _options.Length - 1)
                  ? _activeOption + 1
                  : 0;
                break;

            case KeyboardKey.Enter:
                PerformAction();
                break;
        }
    }

    private void PerformAction() {
        switch (_activeOption) {
            case 0:
                // Retry current level
                _stateMachine.ActiveState = new GameRunning(_stateMachine, _levelNumber);
                break;

            case 1:
                // Back to main menu
                _stateMachine.ActiveState = new MainMenu(_stateMachine);
                break;
        }
    }
}


