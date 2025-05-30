namespace Breakout.PowerUps;
using System;
using System.Numerics;
using Breakout.States;
using DIKUArcade.Entities;
using DIKUArcade.Events;
using DIKUArcade.Graphics;
public enum PowerUpType {
    GainLife,
    MoreTime,
    PlayerSpeed,
    DoubleSpeed,
    SplitBalls
}

public class PowerUp : Entity {
    private static readonly PowerUpType[] AllPowerUpTypes = (PowerUpType[]) Enum.GetValues(typeof(PowerUpType));
    private static readonly Random random = new Random();
    private readonly PowerUpType type;
    private static readonly Dictionary<PowerUpType, string> PowerUpImages = new() {
        { PowerUpType.GainLife, "Breakout.Assets.Images.LifePickUp.png" },
        { PowerUpType.MoreTime, "Breakout.Assets.Images.clock-down.png" },
        { PowerUpType.PlayerSpeed, "Breakout.Assets.Images.SpeedPickUp.png" },
        { PowerUpType.DoubleSpeed, "Breakout.Assets.Images.DoubleSpeedPowerUp.png"},
        { PowerUpType.SplitBalls, "Breakout.Assets.Images.SplitPowerUp.png"}
    };

    public readonly Vector2 fallingSpeed = new Vector2(0.0f, -0.008f);
    public PowerUp(DynamicShape shape, IBaseImage image, PowerUpType type)
        : base(shape, image) {
        this.type = type;
    }
    public static PowerUp SpawnPowerUp(Vector2 position) {
        PowerUpType randomType = AllPowerUpTypes[random.Next(AllPowerUpTypes.Length)];
        var size = new Vector2(0.05f, 0.05f);  // example powerup size, adjust as needed
        var shape = new DynamicShape(position, size);

        string imagePath = PowerUpImages[randomType];
        var image = new Image(imagePath);
        var powerUp = new PowerUp(shape, image, randomType);
        return powerUp;
    }


    public void Move() {
        Shape.AsDynamicShape().Velocity = fallingSpeed;
        Shape.Position += fallingSpeed;
    }
    public void Activate(GameRunning state) {
        switch (type) {
            case PowerUpType.GainLife:
                state.GainLife();
                break;
            case PowerUpType.MoreTime:
                state.AddTime(10);
                break;
            case PowerUpType.PlayerSpeed:
                state.IncreasePaddleSpeed();
                break;
            case PowerUpType.DoubleSpeed:
                state.DoubleSpeed();
                break;
            case PowerUpType.SplitBalls:
                state.SplitBalls();
                break;
        }
    }
}