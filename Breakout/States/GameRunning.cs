namespace Breakout.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Breakout.Balls;
using Breakout.Blocks;
using Breakout.Hazards;
using Breakout.Level;
using Breakout.Points;
using Breakout.PowerUps;
using Breakout.Managers;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;
using DIKUArcade.Physics;
using DIKUArcade.Timers;

public class GameRunning : IGameState {
    private const double HazardChance = 0.2; // 20% chance to spawn a hazard
    private const int initial_lives = 3;
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

    public int _lives;
    private readonly bool _hasTimer;
    private int _timeRemaining;
    private long _lastTickTime;
    public static List<PowerUp> activePowerUps = new List<PowerUp>();

    private long speedBoostStartTime = 0;
    private readonly int speedBoostDuration = 10000;
    private bool isSpeedBoosted = false;
    private static readonly Random random = new Random();

    public GameRunning(StateMachine stateMachine, int levelNumber)
        : this(stateMachine, levelNumber, new PointTracker()) { }

    public GameRunning(StateMachine stateMachine, int levelNumber, PointTracker tracker) {
        _stateMachine = stateMachine;
        _levelNumber = levelNumber;

        // Initialize lives
        _lives = initial_lives;

        // Initialize point tracker
        pointTracker = tracker;


        // Initialize level service and blocks
        var builder = new LevelBuilder();
        var director = new LevelDirector(builder);
        _levelService = new LevelService(director);
        _blocks = new LevelService(director).GetBlocks(_levelNumber);
        _hazardManager = new HazardManager(_blocks, HazardChance);
        _powerUpManager = new PowerUpManager(_blocks);

        // Setup timer if metadata contains Time key
        var metadata = _levelService.GetMetadata(_levelNumber);
        if (metadata.TryGetValue("Time", out var t) && int.TryParse(t, out var seconds)) {
            _hasTimer = true;
            _timeRemaining = seconds;
            _lastTickTime = StaticTimer.GetElapsedMilliseconds();
        } else {
            _hasTimer = false;
        }


        // Initialize player
        _player = new Player(
            new DynamicShape(new Vector2(0.45f, 0.025f), new Vector2(0.2f, 0.03f)),
            new Image("Breakout.Assets.Images.player.png")
        );
        // Pepare ball image and speed
        _ballImage = new Image("Breakout.Assets.Images.ball.png");
        _ballSpeed = 0.02f;

        // Create ball
        _ball = CreateBall();
        activeBalls.Add(_ball);

    }
    //method for creating ball
    private Ball CreateBall() {
        return new Ball(
            new DynamicShape(new Vector2(0.5f, 0.1f), new Vector2(0.025f, 0.025f)),
            _ballImage,
            _ballSpeed
        );
    }


    public void Render(WindowContext context) {

        // Render player and ball
        _player.RenderEntity(context);
        foreach (var ball in activeBalls.ToList()) {
            ball.RenderEntity(context);
        }
        // Render blocks
        foreach (var block in _blocks) {
            if (block.IsAlive) {
                block.Render(context);
            }
        }
        // Render score
        var scoreText = new Text("Score: " + pointTracker.GetScore(), new Vector2(0.01f, 0.95f), 0.5f);
        scoreText.SetColor(225, 225, 225);
        scoreText.Render(context);

        // Render lives
        var livesText = new Text("Lives: ", new Vector2(0.4f, 0.95f), 0.5f);
        livesText.SetColor(225, 225, 225);
        livesText.Render(context);
        var livesValue = new Text(_lives.ToString(), new Vector2(0.58f, 0.95f), 0.5f);
        livesValue.SetColor(0, 225, 0);
        livesValue.Render(context);

        // Render timer (if applicable)
        if (_hasTimer) {
            var timeText = new Text("Time: " + _timeRemaining, new Vector2(0.69f, 0.95f), 0.5f);
            timeText.SetColor(225, 225, 225);
            timeText.Render(context);
        }

        _hazardManager.Render(context);
        _powerUpManager.Render(context);
    }

    public void Update() {
        // Move player
        _player.Move();

        // Move ball if launched
        if (_ballLaunched) {
            for (int i = activeBalls.Count - 1; i >= 0; i--) {
                var ball = activeBalls[i];
                ball.Move();
            }
        }

        // Reflect on horizontal/vertical bounds
        foreach (var ball in activeBalls.ToList()) {
            if (ball.Shape.Position.X <= 0.0f ||
                ball.Shape.Position.X + ball.Shape.Extent.X >= 1.0f) {
                ball.ReflectHorizontal();
            }
            if (ball.Shape.Position.Y + ball.Shape.Extent.Y >= 1.0f) {
                ball.ReflectVertical();
            }
        }
        var playerDyn = _player.Shape.AsDynamicShape();

        // Ball and player collision
        foreach (var ball in activeBalls.ToList()) {
            var ballDyn = ball.Shape.AsDynamicShape();
            if (CollisionDetection.Aabb(ballDyn, _player.Shape.AsDynamicShape()).Collision) {
                ball.ReflectVertical();
            }
        }

        // Ball and block collision
        foreach (var ball in activeBalls.ToList()) {
            var ballDyn = ball.Shape.AsDynamicShape();
            foreach (var block in _blocks) {
                if (!block.IsAlive)
                    continue;

                var blockDyn = block.Shape.AsDynamicShape();
                var colData = CollisionDetection.Aabb(ballDyn, blockDyn);
                if (colData.Collision) {
                    if (colData.CollisionDir == CollisionDirection.CollisionDirLeft ||
                        colData.CollisionDir == CollisionDirection.CollisionDirRight) {
                        ball.ReflectHorizontal();
                    } else {
                        ball.ReflectVertical();
                    }

                    block.DecreaseHealth();
                    if (!block.IsAlive) {
                        pointTracker.AddPoints(block.GetValue());
                    }
                    break;
                }
            }
            _hazardManager.Update(hz => {
                var playerDyn = _player.Shape.AsDynamicShape();
                var hzDyn = hz.Shape.AsDynamicShape();
                if (CollisionDetection.Aabb(hzDyn, playerDyn).Collision) {
                    hz.IsAlive = false;
                    ApplyHazardEffect(hz);
                }
            });
            
            _powerUpManager.Update(this);

            //Timer tick
            if (_hasTimer) {
                var elapsed = StaticTimer.GetElapsedMilliseconds();
                if (_lastTickTime + 1000 < elapsed) {
                    _timeRemaining = Math.Max(0, _timeRemaining - 1);
                    _lastTickTime += 1000;
                    if (_timeRemaining == 0) {
                        _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
                        return;
                    }
                }
            }


            //Ball lost
            for (int i = activeBalls.Count - 1; i >= 0; i--) {
                if (activeBalls[i].Shape.Position.Y <= 0.0f) {
                    activeBalls.RemoveAt(i);
                }
            }

            if (activeBalls.Count == 0) {
                _lives = Math.Max(0, _lives - 1);
                if (_lives > 0) {
                    _ballLaunched = false;
                    _ball = CreateBall();
                    activeBalls.Add(_ball);

                } else {

                    // No lives left -> game over
                    _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
                }
            }

            //Check win condition: all blocks destroyed
            bool allBlocksDestroyed = _blocks
                .Where(b => !(b is UnbreakableBlock))
                .All(b => !b.IsAlive);
            if (allBlocksDestroyed) {
                int next = _levelNumber + 1;
                try {
                    _stateMachine.ActiveState = new GameRunning(_stateMachine, next, pointTracker);
                } catch (FileNotFoundException) {
                    // All levels completed -> main menu
                    _stateMachine.ActiveState = new GameWon(_stateMachine, _levelNumber, pointTracker.GetScore());
                }
            }
            for (int i = activePowerUps.Count - 1; i >= 0; i--) {
                var powerUp = activePowerUps[i];
                powerUp.Move();
                var powerupDyn = powerUp.Shape.AsDynamicShape();
                if (powerUp.Shape.Position.Y <= 0.0f) {
                    activePowerUps.RemoveAt(i);
                    continue;
                }

                if (CollisionDetection.Aabb(powerupDyn, playerDyn).Collision) {
                    powerUp.Activate(this);
                    activePowerUps.RemoveAt(i);
                }
            }
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
        isSpeedBoosted = true;
        speedBoostStartTime = StaticTimer.GetElapsedMilliseconds();
    }
    public void DoubleSpeed(float multiplier = 2.0f) {
        _ball.SetSpeedMultiplier(multiplier);
        isSpeedBoosted = true;
        speedBoostStartTime = StaticTimer.GetElapsedMilliseconds();
    }
    public void SplitBalls() {
        var newBalls = new List<Ball>();
        foreach (var ball in activeBalls.ToList()) {
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

    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action == KeyboardAction.KeyPress && key == KeyboardKey.Escape) {
            // Back to main menu
            _stateMachine.ActiveState = new GamePaused(_stateMachine);
        } else if (action == KeyboardAction.KeyPress && key == KeyboardKey.P) {
            //*DEBUG* Skip to next level
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
    
    private void ApplyHazardEffect(Hazard hz) {
    switch (hz.Type) {
        case HazardType.LoseLife:
            _lives = Math.Max(0, _lives - 1);
            if (_lives == 0) {
                _stateMachine.ActiveState = new GameLost(_stateMachine, _levelNumber, pointTracker.GetScore());
            }
            break;
        case HazardType.ReduceTime:
            _timeRemaining = Math.Max(0, _timeRemaining - 5);
            break;
    }
}

}


