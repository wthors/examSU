namespace Breakout;

using System;
using System.Numerics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;


public class Player : Entity {

    private float moveLeft = 0.0f;
    private float moveRight = 0.0f;
    private float baseSpeed = 0.015f;
    private float speedMultiplier = 1.0f;
    public Player(DynamicShape shape, IBaseImage image) : base(shape, image) {

    }

    public void Move() {
        float nextX = Shape.AsDynamicShape().Position.X + Shape.AsDynamicShape().Velocity.X;
        if (nextX < 0.0f) {
            nextX = 0.0f;
        }
        if (nextX > 1.0f - Shape.Extent.X) {
            nextX = 1.0f - Shape.Extent.X;
        }
        Shape.Position = new System.Numerics.Vector2(nextX, Shape.Position.Y);
    }

    public void SetMoveLeft(bool val) {
        if (val == true) {
            moveLeft = -baseSpeed * speedMultiplier;
            UpdateVelocity();
        } else {
            moveLeft = 0;
            UpdateVelocity();
        }
    }

    public void SetMoveRight(bool val) {
        if (val == true) {
            moveRight = baseSpeed * speedMultiplier;
            UpdateVelocity();
        } else {
            moveRight = 0;
            UpdateVelocity();
        }
    }
    private void UpdateVelocity() {
        Shape.AsDynamicShape().Velocity.X = moveLeft + moveRight;
    }
    public void KeyHandler(KeyboardAction action, KeyboardKey key) {
        if (action == KeyboardAction.KeyPress) {
            if (key == KeyboardKey.Left) {
                SetMoveLeft(true);
            }
            if (key == KeyboardKey.Right) {
                SetMoveRight(true);
            }
        } else if (action == KeyboardAction.KeyRelease) {
            if (key == KeyboardKey.Left) {
                SetMoveLeft(false);
            }
            if (key == KeyboardKey.Right) {
                SetMoveRight(false);
            }
        }

    }
    public Vector2 GetPosition() {
        return Shape.Position;
    }
    public void SetSpeedMultiplier(float multiplier) {
        speedMultiplier = multiplier;
    }
    public void ResetSpeed() {
        SetSpeedMultiplier(1.0f);
    }
}