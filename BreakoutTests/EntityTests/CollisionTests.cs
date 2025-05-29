namespace BreakoutTests.EntityTests;

using System.Numerics;
using Breakout;
using Breakout.Balls;
using Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using NUnit.Framework;

public class CollisionTests {
    public Ball testBall;
    public Player testPlayer;
    public StandardBlock testBlock;


    [Test]
    public void BallBlockCollision() {
        var ball = new DynamicShape(new Vector2(0.5f, 0.5f), new Vector2(0.1f, 0.1f), new Vector2(0.0f, -0.1f));
        var block = new StationaryShape(new Vector2(0.5f, 0.3f), new Vector2(0.2f, 0.2f));

        var collision = CollisionDetection.Aabb(ball, block);

        Assert.IsTrue(collision.Collision);
    }

    [Test]
    public void BallPlayerCollision() {
        var ball = new DynamicShape(new Vector2(0.5f, 0.5f), new Vector2(0.1f, 0.1f), new Vector2(0.0f, -0.1f));
        var player = new DynamicShape(new Vector2(0.5f, 0.3f), new Vector2(0.1f, 0.1f), new Vector2(0.0f, 0.0f));

        var collision = CollisionDetection.Aabb(ball, player);

        Assert.IsTrue(collision.Collision);
    }
}