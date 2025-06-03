namespace Breakout.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using DIKUArcade.Timers;

public class GameRunning : IGameState {
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
    private Ball _ball;
    private readonly float _ballSpeed;
    private readonly IBaseImage _ballImage;
    private bool _ballLaunched = false;

    private readonly HazardManager _hazardManager;
    private readonly PowerUpManager _powerUpManager;

    private readonly PointTracker pointTracker;
    private readonly List<IBlock> _blocks;
    private List<Ball> activeBalls = new List<Ball>();
    private CollisionManager _collisionManager;

    public int _lives;
    private readonly bool _hasTimer;
    private int _timeRemaining;
    private long _lastTickTime;
    public static List<PowerUp> activePowerUps = new List<PowerUp>();

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
        var metadata = _levelService.GetMetadata(_levelNumber);
        if (metadata.TryGetValue("Time", out var t) && int.TryParse(t, out var seconds)) {
            _hasTimer = true;
            _timeRemaining = seconds;
            _lastTickTime = StaticTimer.GetElapsedMilliseconds();
        } else {
            _hasTimer = false;
        }

        // Prepare ball image and speed, then create initial ball
        _ballImage = new Image("Breakout.Assets.Images.ball.png");
        _ballSpeed = 0.02f;
        _ball = CreateBall();
        activeBalls.Add(_ball);

        // Initialize player paddle
        _player = new Player(
            new DynamicShape(new Vector2(0.45f, 0.025f), new Vector2(0.2f, 0.03f)),
            new Image("Breakout.Assets.Images.player.png")
        );

        //initialize managers
        _hazardManager = new HazardManager(_blocks, HazardChance);
        _powerUpManager = new PowerUpManager(_blocks);
        _collisionManager = new CollisionManager(activeBalls, _blocks, _player, pointTracker);
    }


    public void Render(WindowContext context) {
        // Render player paddle and balls
        _player.RenderEntity(context);
        foreach (var ball in activeBalls) {
            ball.RenderEntity(context);
        }
        // Render remaining blocks
        foreach (var block in _blocks) {
            if (block.IsAlive) {
                block.Render(context);
            }
        }
        // Render score and lives
        var scoreText = new Text("Score: " + pointTracker.GetScore(), new Vector2(0.01f, 0.95f), 0.5f);
        scoreText.SetColor(225, 225, 225);
        scoreText.Render(context);
        var livesText = new Text("Lives: ", new Vector2(0.4f, 0.95f), 0.5f);
        livesText.SetColor(225, 225, 225);
        livesText.Render(context);
        var livesValue = new Text(_lives.ToString(), new Vector2(0.58f, 0.95f), 0.5f);
        livesValue.SetColor(0, 225, 0);
        livesValue.Render(context);
        // Render timer if applicable
        if (_hasTimer) {
            var timeText = new Text("Time: " + _timeRemaining, new Vector2(0.69f, 0.95f), 0.5f);
            timeText.SetColor(225, 225, 225);
            timeText.Render(context);
        }
        // Render hazards and power-ups
        _hazardManager.Render(context);
        _powerUpManager.Render(context);
    }

    public void Update() {
        // Move player paddle
        _player.Move();
        // Move balls if launched
        if (_ballLaunched) {
            for (int i = activeBalls.Count - 1; i >= 0; i--) {
                activeBalls[i].Move();
            }
        }

        // Check collisions for balls, player, and blocks
        _collisionManager.Update();

        // Update hazards and check for hazard-player collisions
        _hazardManager.Update(this);
        if (_lives == 0)
            return;
        // Update power-up and check for power-up-player collisions
        _powerUpManager.Update(this);


        // Countdown timer if active
        if (_hasTimer) {
            long elapsed = StaticTimer.GetElapsedMilliseconds();
            if (_lastTickTime + 1000 < elapsed) {
                _timeRemaining = Math.Max(0, _timeRemaining - 1);
                _lastTickTime += 1000;
                if (_timeRemaining == 0) {
                    _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
                    return;
                }
            }
        }
        // Remove balls that fell below the screen
        for (int i = activeBalls.Count - 1; i >= 0; i--) {
            if (activeBalls[i].Shape.Position.Y <= 0.0f) {
                activeBalls.RemoveAt(i);
            }
        }
        // If no balls remain, lose a life or end game
        if (activeBalls.Count == 0) {
            _lives = Math.Max(0, _lives - 1);
            if (_lives > 0) {
                _ballLaunched = false;
                _ball = CreateBall();
                activeBalls.Add(_ball);
            } else {
                // No lives left -> game over
                _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
                return;
            }
        }
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
            return;
        }

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
        } else if (action == KeyboardAction.KeyPress && key == KeyboardKey.Space && !_ballLaunched) {
            float rx = (float) (random.NextDouble() * 1.0 - 0.5);
            _ball.BallLaunch(new Vector2(rx, 1.0f));
            _ballLaunched = true;
        } else {
            _player.KeyHandler(action, key);
        }
    }

    public void ResetTimer() {
        if (_hasTimer) {
            _lastTickTime = StaticTimer.GetElapsedMilliseconds();
        }
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

    public void AddPowerUp(PowerUp powerUp) {
        activePowerUps.Add(powerUp);
    }
    public void AddTime(int seconds) {
        _timeRemaining += seconds;
    }
    public void IncreasePaddleSpeed(float multiplier = 1.5f) {
        _player.SetSpeedMultiplier(multiplier);
    }
    public void DoubleSpeed(float multiplier = 2.0f) {
        _ball.SetSpeedMultiplier(multiplier);
    }

    public Ball CreateBall() {
        return new Ball(
            new DynamicShape(new Vector2(0.5f, 0.1f), new Vector2(0.025f, 0.025f)),
            _ballImage,
            _ballSpeed
        );
    }

    public void SplitBalls() {
        var newBalls = new List<Ball>();
        foreach (var ball in activeBalls) {
            var pos = _ball.Shape.Position;
            var velocity = _ball.Velocity;
            var angles = new float[] { 0, 30f, -30f };
            foreach (var angleDeg in angles) {
                var rad = MathF.PI * angleDeg / 180f;
                var cos = MathF.Cos(rad);
                var sin = MathF.Sin(rad);
                var newVelX = velocity.X * cos - velocity.Y * sin;
                var newVelY = velocity.X * sin + velocity.Y * cos;
                var newVelocity = new Vector2(newVelX, newVelY);
                var newBall = new Ball(new DynamicShape(pos, _ball.Shape.Extent), _ball.Image, 0.02f);
                newBall.SetDirection(Vector2.Normalize(newVelocity));
                newBalls.Add(newBall);
            }
        }
        activeBalls.Clear();
        activeBalls.AddRange(newBalls);
    }
}
