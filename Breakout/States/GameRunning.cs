namespace Breakout.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Breakout.Balls;
using Breakout.Blocks;
using Breakout.Level;
using Breakout.Managers;
using Breakout.Points;
using Breakout.PowerUps;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

/// <summary>
/// Active gameplay state handling all in-game logic.
/// </summary>

public class GameRunning : IGameState, IGameEffectTarget {
    private const double HazardChance = 0.2; // 20% chance to spawn a hazard
    private const int InitialLives = 3;
    public int GetLives() {
        return _lives;
    }

    public void SetLives(int value) {
        _lives = value;
    }

    private readonly StateMachine _stateMachine;
    private readonly int _levelNumber;
    private ILevelService _levelService;
    private readonly Player _player;
    public DynamicShape PlayerShape() {
        return _player.Shape.AsDynamicShape();
    }
    private readonly float _ballSpeed;
    private readonly IBaseImage _ballImage;

    private readonly HazardManager _hazardManager;
    private readonly PowerUpManager _powerUpManager;
    private readonly BallManager _ballManager;

    private readonly List<(PowerUpType type, GameTimer timer)> _activePowerUps = new();

    private readonly PointTracker pointTracker;
    private readonly List<IBlock> _blocks;
    private CollisionManager _collisionManager;

    public int _lives;
    private readonly GameTimer? _timer;

    private static readonly Random random = new Random();

    public GameRunning(StateMachine stateMachine, int levelNumber)
        : this(stateMachine, levelNumber, new PointTracker()) { }

    public GameRunning(StateMachine stateMachine, int levelNumber, PointTracker tracker) {
        _stateMachine = stateMachine;
        _levelNumber = levelNumber;

        // Initialize lives and score tracker
        _lives = InitialLives;
        pointTracker = tracker;

        // Initialize level service, blocks, hazards, and power-ups
        var builder = new LevelBuilder();
        var director = new LevelDirector(builder);
        _levelService = new LevelService(director);
        _blocks = _levelService.GetBlocks(_levelNumber);

        // Setup timer if level has a time limit
        GameTimer? timer = null;
        var metadata = _levelService.GetMetadata(_levelNumber);
        if (metadata.TryGetValue("Time", out var t) && int.TryParse(t, out var seconds)) {
            timer = new GameTimer(seconds);
        }
        _timer = timer;

        // Prepare ball image and speed, then create initial ball manager
        _ballImage = new Image("Breakout.Assets.Images.ball.png");
        _ballSpeed = 0.02f;
        _ballManager = new BallManager(_ballImage, _ballSpeed);
        _ballManager.CreateBall();


        // Initialize player paddle
        _player = new Player(
            new DynamicShape(new Vector2(0.45f, 0.025f), new Vector2(0.2f, 0.03f)),
            new Image("Breakout.Assets.Images.player.png")
        );

        //initialize managers
        _hazardManager = new HazardManager(_blocks, HazardChance);
        _powerUpManager = new PowerUpManager(_blocks);
        _collisionManager = new CollisionManager(_ballManager.Balls, _blocks, _player, pointTracker);
    }


    public void Render(WindowContext context) {
        RenderPlayer(context);
        RenderBalls(context);
        RenderBlocks(context);
        RenderUI(context);
        RenderHazards(context);
        RenderPowerUps(context);
    }

    public void Update() {
        MovePlayer();
        MoveBalls();
        HandleCollisions();
        HandleHazards();
        if (_lives == 0)
            return;
        HandlePowerUps();
        UpdateActivePowerUps();
        UpdateTimer();
        if (_lives == 0)
            return;
        HandleBallLoss();
        if (_lives == 0)
            return;
        CheckWinCondition();
    }

    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action == KeyboardAction.KeyPress && key == KeyboardKey.Escape) {
            // Back to main menu
            _stateMachine.ActiveState = new GamePaused(_stateMachine);
        } else if (action == KeyboardAction.KeyPress && key == KeyboardKey.P) {
            // DEBUG: Skip to next level
            if (_levelNumber < 4) {
                _stateMachine.ActiveState = new GameRunning(_stateMachine, _levelNumber + 1, pointTracker);
            } else {
                _stateMachine.ActiveState = new GameWon(_stateMachine, _levelNumber, pointTracker.GetScore());
            }
        } else if (action == KeyboardAction.KeyPress && key == KeyboardKey.Space && !_ballManager.BallLaunched) {
            float rx = (float) (random.NextDouble() * 1.0 - 0.5);
            _ballManager.LaunchBall(new Vector2(rx, 1.0f));
        } else {
            _player.KeyHandler(action, key);
        }
    }


    private void MovePlayer() {
        // Move player paddle based on input
        _player.Move();
    }

    private void MoveBalls() {
        // Move balls if launched
        _ballManager.MoveBalls();
    }


    private void HandleCollisions() {
        // Check collisions between balls and blocks, player, and hazards
        _collisionManager.Update();
    }

    private void HandleHazards() {
        // Update hazards and check for hazard-ball collisions
        _hazardManager.Update(this);
    }

    private void HandlePowerUps() {
        // Update power-up and check for power-up-player collisions
        _powerUpManager.Update(this);
    }

    private void UpdateActivePowerUps() {
        for (int i = _activePowerUps.Count - 1; i >= 0; i--) {
            var (type, timer) = _activePowerUps[i];
            timer.Update();
            if (timer.Remaining <= 0) {
                switch (type) {
                    case PowerUpType.DoubleSpeed:
                        foreach (var ball in _ballManager.Balls) {
                            ball.ResetSpeed();
                        }
                        break;
                    case PowerUpType.PlayerSpeed:
                        _player.ResetSpeed();
                        break;
                }
                _activePowerUps.RemoveAt(i);
            }
        }
    }

    private void UpdateTimer() {
        // Countdown timer if active
        if (_timer != null) {
            _timer.Update();
            if (_timer.Remaining <= 0) {
                _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
            }
        }
    }
    private void RemoveOffscreenBalls() {
        // Remove balls that have fallen off the bottom of the screen
        _ballManager.RemoveOffscreenBalls();
    }

    private void HandleBallLoss() {
        RemoveOffscreenBalls();
        if (_ballManager.Balls.Count == 0) {
            LoseLife();
            if (_lives > 0) {
                _ballManager.ResetLaunch();
                _ballManager.CreateBall();
            }
        }
    }

    private void CheckWinCondition() {
        // Check win condition (all breakable blocks destroyed)
        bool allBlocksDestroyed = _blocks
                .Where(b => !(b is UnbreakableBlock))
                .All(b => !b.IsAlive);
        if (allBlocksDestroyed) {
            int nextLevel = _levelNumber + 1;
            try {
                _stateMachine.ActiveState = new GameRunning(_stateMachine, nextLevel, pointTracker);
            } catch (FileNotFoundException) {
                // All levels completed -> main menu
                _stateMachine.ActiveState = new GameWon(_stateMachine, _levelNumber, pointTracker.GetScore());
            }
        }
    }

    private void RenderPlayer(WindowContext context) {
        _player.RenderEntity(context);
    }
    private void RenderBalls(WindowContext context) {
        foreach (var ball in _ballManager.Balls) {
            ball.RenderEntity(context);
        }
    }
    private void RenderBlocks(WindowContext context) {
        foreach (var block in _blocks) {
            if (block.IsAlive) {
                block.Render(context);
            }
        }
    }

    private void RenderScore(WindowContext context) {
        var scoreText = new Text("Score: " + pointTracker.GetScore(), new Vector2(0.01f, 0.95f), 0.5f);
        scoreText.SetColor(225, 225, 225);
        scoreText.Render(context);
    }

    private void RenderLives(WindowContext context) {
        var livesText = new Text("Lives: ", new Vector2(0.4f, 0.95f), 0.5f);
        livesText.SetColor(225, 225, 225);
        livesText.Render(context);
        var livesValue = new Text(_lives.ToString(), new Vector2(0.58f, 0.95f), 0.5f);
        livesValue.SetColor(0, 225, 0);
        livesValue.Render(context);
    }

    private void RenderTimer(WindowContext context) {
        if (_timer != null) {
            var timeText = new Text("Time: " + _timer.Remaining, new Vector2(0.69f, 0.95f), 0.5f);
            timeText.SetColor(225, 225, 225);
            timeText.Render(context);
        }
    }

    private void RenderUI(WindowContext context) {
        RenderScore(context);
        RenderLives(context);
        RenderTimer(context);
    }

    private void RenderHazards(WindowContext context) {
        _hazardManager.Render(context);
    }

    private void RenderPowerUps(WindowContext context) {
        _powerUpManager.Render(context);
    }

    public void ResetTimer() {
        _timer?.Reset();
    }

    public void GainLife() {
        if (_lives < InitialLives) {
            _lives++;
        }
    }

    public void LoseLife() {
        _lives = Math.Max(0, _lives - 1);
        if (_lives == 0) {
            _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
        }
    }

    public void AddTime(int seconds) {
        _timer?.AddTime(seconds);
    }
    public void IncreasePaddleSpeed(float multiplier = 1.5f, int duration = 10) {
        _player.SetSpeedMultiplier(multiplier);
        _activePowerUps.Add((PowerUpType.PlayerSpeed, new GameTimer(duration)));
    }
    public void DoubleSpeed(float multiplier = 2.0f, int duration = 10) {
        foreach (var ball in _ballManager.Balls) {
            ball.SetSpeedMultiplier(multiplier);
        }
        _activePowerUps.Add((PowerUpType.DoubleSpeed, new GameTimer(duration)));
    }

    public Ball CreateBall() {
        return new Ball(
            new DynamicShape(new Vector2(0.5f, 0.1f), new Vector2(0.025f, 0.025f)),
            _ballImage,
            _ballSpeed
        );
    }

    public void SplitBalls() {
        _ballManager.SplitBalls();
    }
}
