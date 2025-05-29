namespace BreakoutTests.EntityTests;

using System.Numerics;
using Breakout;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using NUnit.Framework;

public class TestsPlayer {
    public Player testPlayer;

    [SetUp]
    public void Setup() {
        testPlayer = new Player(
           new DynamicShape(new Vector2(0.45f, 0.1f), new Vector2(0.2f, 0.03f)),
           new Image("BreakoutTests.Assets.Images.player.png"));
    }

    // Tests the player's initial position
    [Test]
    public void TestPlayerInitialPosition() {
        Assert.AreEqual(0.45f, testPlayer.GetPosition().X);
        Assert.AreEqual(0.1f, testPlayer.GetPosition().Y);
    }

    // Tests if the player moves left
    [Test]
    public void TestPlayerMovesLeft() {
        testPlayer.SetMoveLeft(true);
        testPlayer.Move();
        Assert.Less(testPlayer.GetPosition().X, 0.45f);
    }

    // Tests if the player moves right
    [Test]
    public void TestPlayerMovesRight() {
        testPlayer.SetMoveRight(true);
        testPlayer.Move();
        Assert.Greater(testPlayer.GetPosition().X, 0.45f);
    }

    // Tests if the player stops moving
    [Test]
    public void TestPlayerStopsMoving() {
        testPlayer.SetMoveRight(true);
        testPlayer.Move();
        float prevX = testPlayer.GetPosition().X;
        testPlayer.SetMoveRight(false);
        testPlayer.Move();
        Assert.AreEqual(prevX, testPlayer.GetPosition().X);
    }

    // Tests if the left bound is working
    [Test]
    public void TestPlayerBoundsLeft() {
        testPlayer.SetMoveLeft(true);
        for (int i = 0; i < 100; i++) {
            testPlayer.Move();
        }
        Assert.AreEqual(0.0f, testPlayer.GetPosition().X);
    }

    // Tests if the right bound is working
    [Test]
    public void TestPlayerBoundsRight() {
        testPlayer.SetMoveRight(true);
        for (int i = 0; i < 100; i++) {
            testPlayer.Move();
        }
        Assert.AreEqual(1.0f - testPlayer.Shape.Extent.X, testPlayer.GetPosition().X);
    }


}