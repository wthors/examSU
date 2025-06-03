namespace Breakout.Balls;

using System;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Events;
using DIKUArcade.Graphics;

public class Ball : Entity {
    public Vector2 Velocity {
        get; set;
    }
    private float speed;
    private float speedMultiplier = 1.0f;

    public Ball(DynamicShape shape, IBaseImage image, float initialSpeed)
        : base(shape, image) {
        speed = initialSpeed;
        Velocity = new Vector2(0.0f, speed);
    }

    public void BallLaunch(Vector2 direction) {
        direction = Vector2.Normalize(direction);
        if (direction.Y <= 0) {
            throw new ArgumentException("Ball should be launched upwards.");
        }

        direction = EnsureValidLaunchDirection(direction);
        Velocity = direction * speed;
    }
    public void Move() {
        Shape.AsDynamicShape().Velocity = Velocity * speedMultiplier;
        Shape.Position += Velocity * speedMultiplier;

        if (Shape.Position.Y <= 0) {
            DeleteEntity(); // Deletes ball if it exits the screen at the bottom
        }
    }

    private Vector2 EnsureValidLaunchDirection(Vector2 dir) {
        if (MathF.Abs(dir.X) > 0.75f) {
            dir.X = MathF.Sign(dir.X) * 0.75f;
            dir.Y = MathF.Sqrt(1 - dir.X * dir.X);
        }
        return Vector2.Normalize(dir);
    }
    public void ReflectHorizontal() {
        Velocity = new Vector2(-Velocity.X, Velocity.Y);
    }
    public void ReflectVertical() {
        Velocity = new Vector2(Velocity.X, -Velocity.Y);

    }
    public void PreventTrajectoryLock() {
        if (MathF.Abs(Velocity.X) < 0.01f) {
            Velocity = new Vector2(0.1f, Velocity.Y) * speed;
        }
    }
    public void SetSpeedMultiplier(float multiplier) {
        speedMultiplier = multiplier;
    }
    public void ResetSpeed() {
        SetSpeedMultiplier(1.0f);
    }
    public void SetDirection(Vector2 direction) {
        direction = Vector2.Normalize(direction);
        Velocity = direction * speed;
    }

}