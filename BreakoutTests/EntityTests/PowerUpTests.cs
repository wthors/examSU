namespace BreakoutTests.EntityTests;


using System;
using System.Numerics;
using System.Reflection;
using Breakout;
using Breakout.Balls;
using Breakout.Managers;
using Breakout.PowerUps;
using Breakout.States;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using NUnit.Framework;



[TestFixture]
public class PowerUpTests {
    private FieldInfo timerField;
    private FieldInfo playerField;
    private FieldInfo playerSpeedMultField;
    private FieldInfo ballManagerField;
    private FieldInfo ballSpeedMultField;
    private FieldInfo activeBallsField;


    [SetUp]
    public void SetUp() {
        timerField = typeof(GameRunning).GetField("_timer", BindingFlags.NonPublic | BindingFlags.Instance);
        playerField = typeof(GameRunning).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance);
        playerSpeedMultField = typeof(Player).GetField("speedMultiplier", BindingFlags.NonPublic | BindingFlags.Instance);
        ballManagerField = typeof(GameRunning).GetField("_ballManager", BindingFlags.NonPublic | BindingFlags.Instance);
        ballSpeedMultField = typeof(Ball).GetField("speedMultiplier", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [Test]
    public void Move_ShouldApplyFallingSpeedToPositionAndVelocity() {
        var startPos = new Vector2(0.5f, 0.9f);
        var pu = PowerUp.SpawnPowerUp(startPos);
        var expected = pu.fallingSpeed;

        // Act
        pu.Move();

        // Assert
        var dyn = pu.Shape.AsDynamicShape();
        Assert.AreEqual(expected, dyn.Velocity,
            "Move() must set the shape’s Velocity to fallingSpeed");
        Assert.AreEqual(startPos + expected, dyn.Position,
            "Move() must offset position by fallingSpeed");
    }

    [Test]
    public void SpawnPowerUp_ShouldReturnPowerUpAtGivenPosition() {
        var startPos = new Vector2(0.3f, 0.7f);
        // Act
        PowerUp pu = PowerUp.SpawnPowerUp(startPos);
        // Assert
        Assert.IsNotNull(pu, "SpawnPowerUp must return a PowerUp instance");
        var dyn = pu.Shape.AsDynamicShape();
        Assert.AreEqual(startPos, dyn.Position,
            "Returned PowerUp should have its shape positioned at the spawn coordinates");
    }

    [Test]
    public void Activate_GainLife_IncreasesLivesByOne() {
        // Arrange: create game state and a GainLife power-up
        var state = new GameRunning(null, levelNumber: 1);
        state.SetLives(2); // set lives below max (3) to test increment
        var gainLifePU = new PowerUp(
            new DynamicShape(new Vector2(0.1f, 0.1f), new Vector2(0.05f, 0.05f)),
            null,
            PowerUpType.GainLife
        );
        int initialLives = state.GetLives();

        // Act: simulate collecting the power-up
        gainLifePU.Activate(state);

        // Assert: lives should have increased by 1 (up to a maximum of 3)
        int expectedLives = Math.Min(initialLives + 1, 3);
        Assert.AreEqual(expectedLives, state.GetLives(),
            "GainLife should increase the player's lives by one (capped at 3).");
    }

    [Test]
    public void Activate_MoreTime_AddsTenSeconds() {
        // Arrange: create game state and a MoreTime power-up
        var state = new GameRunning(null, levelNumber: 1);
        var moreTimePU = new PowerUp(
            new DynamicShape(new Vector2(0.2f, 0.2f), new Vector2(0.05f, 0.05f)),
            null,
            PowerUpType.MoreTime
        );
        // Capture the initial remaining time (could be 0 if no timer, or level metadata value)
        var timer = (GameTimer) timerField.GetValue(state);
        int initialTime = timer?.Remaining ?? 0;

        // Act: activate MoreTime power-up
        moreTimePU.Activate(state);

        // Assert: time remaining should be initial + 10 seconds
        timer = (GameTimer) timerField.GetValue(state);
        int updatedTime = timer?.Remaining ?? 0;
        Assert.AreEqual(initialTime + 10, updatedTime,
            "MoreTime should add 10 seconds to the remaining time.");
    }

    [Test]
    public void Activate_PlayerSpeed_SetsPlayerSpeedMultiplier() {
        // Arrange: create game state and a PlayerSpeed power-up
        var state = new GameRunning(null, levelNumber: 1);
        var playerSpeedPU = new PowerUp(
            new DynamicShape(new Vector2(0.3f, 0.3f), new Vector2(0.05f, 0.05f)),
            null,
            PowerUpType.PlayerSpeed
        );
        // Get player's current speed multiplier (should start at 1.0)
        var player = (Player) playerField.GetValue(state);
        float initialMultiplier = (float) playerSpeedMultField.GetValue(player);
        Assert.AreEqual(1.0f, initialMultiplier, "Initial player speed multiplier should be 1.0.");

        // Act: activate PlayerSpeed power-up
        playerSpeedPU.Activate(state);

        // Assert: player's speed multiplier should be increased (default multiplier is 1.5x)
        float boostedMultiplier = (float) playerSpeedMultField.GetValue(player);
        Assert.AreEqual(1.5f, boostedMultiplier,
            "PlayerSpeed should set the player's speed multiplier to 1.5 (i.e., 150% of normal).");
    }

    [Test]
    public void Activate_DoubleSpeed_DoublesBallSpeed() {
        // Arrange: create game state and a DoubleSpeed power-up
        var state = new GameRunning(null, levelNumber: 1);
        var doubleSpeedPU = new PowerUp(
            new DynamicShape(new Vector2(0.4f, 0.4f), new Vector2(0.05f, 0.05f)),
            null,
            PowerUpType.DoubleSpeed
        );
        // Get ball's current speed multiplier (should start at 1.0)
        var BallManager = (BallManager) ballManagerField.GetValue(state);
        var ball = BallManager.Balls[0]; // Get the first ball

        float initialBallMult = (float) ballSpeedMultField.GetValue(ball);
        Assert.AreEqual(1.0f, initialBallMult, "Initial ball speed multiplier should be 1.0.");

        // Act: activate DoubleSpeed power-up
        doubleSpeedPU.Activate(state);

        // Assert: ball's speed multiplier should be doubled to 2.0
        float boostedBallMult = (float) ballSpeedMultField.GetValue(ball);
        Assert.AreEqual(2.0f, boostedBallMult,
            "DoubleSpeed should set the ball's speed multiplier to 2.0 (double the normal speed).");
    }

    [Test]
    public void Activate_SplitBalls_IncreasesActiveBallsCount() {
        // Arrange: create game state and a SplitBalls power-up
        var state = new GameRunning(null, levelNumber: 1);
        var splitBallsPU = new PowerUp(
            new DynamicShape(new Vector2(0.5f, 0.5f), new Vector2(0.05f, 0.05f)),
            null,
            PowerUpType.SplitBalls
        );
        // Check initial number of active balls (should start with 1 ball in play)
        var ballManager = (BallManager) ballManagerField.GetValue(state);
        var ballsList = ballManager.Balls;
        int initialBallCount = ballsList.Count;
        Assert.GreaterOrEqual(initialBallCount, 1, "There should be at least one ball active initially.");

        // Act: activate SplitBalls power-up
        splitBallsPU.Activate(state);

        // Assert: the number of active balls should have increased (for one ball, expect three balls total)
        int newBallCount = ballsList.Count;
        int expectedCount = initialBallCount * 3;
        Assert.AreEqual(expectedCount, newBallCount,
            "SplitBalls should increase the number of active balls (e.g., 1 ball becomes 3 balls).");
    }

    [Test]
    public void DoubleSpeed_ExpiresAndResetsBallSpeed() {
        var state = new GameRunning(null, levelNumber: 1);
        state.DoubleSpeed(multiplier: 2.0f, duration: 1);

        var ballManager = (BallManager) ballManagerField.GetValue(state);
        var ball = ballManager.Balls[0];

        Assert.AreEqual(2.0f, (float) ballSpeedMultField.GetValue(ball));

        var listField = typeof(GameRunning).GetField("_activePowerUps", BindingFlags.NonPublic | BindingFlags.Instance);
        var list = (System.Collections.IList) listField.GetValue(state);
        var tuple = (ValueTuple<PowerUpType, GameTimer>) list[0];
        tuple.Item2.AddTime(-1); // expire immediately

        state.Update();

        Assert.AreEqual(1.0f, (float) ballSpeedMultField.GetValue(ball),
            "Ball speed should reset when DoubleSpeed duration expires.");
    }
}

