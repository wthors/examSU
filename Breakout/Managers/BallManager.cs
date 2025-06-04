namespace Breakout.Managers;

using System;
using System.Collections.Generic;
using System.Numerics;
using Breakout.Balls;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

public class BallManager {
    private readonly List<Ball> _balls = new();
    private bool _ballLaunched;
    private readonly IBaseImage _ballImage;
    private readonly float _ballSpeed;

    public List<Ball> Balls => _balls;
    public bool BallLaunched => _ballLaunched;

    public BallManager(IBaseImage ballImage, float ballSpeed) {
        _ballImage = ballImage;
        _ballSpeed = ballSpeed;
        _ballLaunched = false;
    }

    public Ball CreateBall() {
        var ball = new Ball(
            new DynamicShape(new Vector2(0.5f, 0.1f), new Vector2(0.025f, 0.025f)),
            _ballImage,
            _ballSpeed
        );
        _balls.Add(ball);
        return ball;
    }

    public void LaunchBall(Vector2 direction) {
        if (_ballLaunched) return;
        foreach (var ball in _balls) {
            ball.BallLaunch(direction);
        }
        _ballLaunched = true;
    }

    public void MoveBalls() {
        if (!_ballLaunched) return;
        for (int i = _balls.Count - 1; i >= 0; i--) {
            _balls[i].Move();
        }
    }

    public void RemoveOffscreenBalls() {
        for (int i = _balls.Count - 1; i >= 0; i--) {
            if (_balls[i].Shape.Position.Y <= 0.0f) {
                _balls.RemoveAt(i);
            }
        }
    }

    public void SplitBalls() {
        var newBalls = new List<Ball>();
        foreach (var ball in _balls) {
            var pos = ball.Shape.Position;
            var velocity = ball.Velocity;
            var angles = new float[] { 0, 30f, -30f };
            foreach (var angleDeg in angles) {
                var rad = MathF.PI * angleDeg / 180f;
                var cos = MathF.Cos(rad);
                var sin = MathF.Sin(rad);
                var newVelX = velocity.X * cos - velocity.Y * sin;
                var newVelY = velocity.X * sin + velocity.Y * cos;
                var newBall = new Ball(new DynamicShape(pos, ball.Shape.Extent), _ballImage, _ballSpeed);
                newBall.SetDirection(Vector2.Normalize(new Vector2(newVelX, newVelY)));
                newBalls.Add(newBall);
            }
        }
        _balls.Clear();
        _balls.AddRange(newBalls);
    }

    public void ResetLaunch() {
        _ballLaunched = false;
    }
}