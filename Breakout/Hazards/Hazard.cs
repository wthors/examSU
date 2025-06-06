namespace Breakout.Hazards;

using System.Collections.Generic;
using System.Numerics;
using Breakout.States;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

/// <summary>
/// Possible effects triggered by a hazard.
/// </summary>

public enum HazardType {
    LoseLife, ReduceTime
}

/// <summary>
/// Falling object that causes a negative effect when collected.
/// </summary>

public class Hazard : Entity {
    public HazardType Type {
        get;
    }
    public bool IsAlive { get; set; } = true;
    public Vector2 Velocity {
        get; set;
    }
    private float speed;

    private static readonly Dictionary<HazardType, string> HazardImages = new() {
        { HazardType.LoseLife, "Breakout.Assets.Images.LoseLife.png" },
        { HazardType.ReduceTime, "Breakout.Assets.Images.clock-down.png" }
    };
    private static readonly Random random = new Random();

    public Hazard(DynamicShape shape, IBaseImage image, HazardType type, float speed)
      : base(shape, image) {
        Type = type;
        this.speed = speed;
        Velocity = new Vector2(0.0f, -speed);
    }

    public static Hazard SpawnHazard(Vector2 blockPos, Vector2 blockExtent, float speed = 0.005f) {

        HazardType type = random.NextDouble() < 0.5
            ? HazardType.LoseLife
            : HazardType.ReduceTime;
        // size
        float w = 0.05f, h = 0.05f;

        // spawn position
        var spawnPos = new Vector2(
            blockPos.X + (blockExtent.X - w) / 2,
            blockPos.Y
        );
        var shape = new DynamicShape(spawnPos, new Vector2(w, h));

        // get image from type
        var image = new Image(HazardImages[type]);

        return new Hazard(shape, image, type, speed);
    }

    public void Update() {

        Shape.AsDynamicShape().Velocity = Velocity;
        Shape.Position += Velocity;

        // Check if the hazard has moved off the screen
        if (Shape.Position.Y < 0.0f) {
            IsAlive = false;
        }
    }

    public void Activate(IGameEffectTarget state) {
        switch (Type) {
            case HazardType.LoseLife:
                state.LoseLife();
                break;
            case HazardType.ReduceTime:
                state.AddTime(-10);
                break;
        }
    }

}
