namespace BreakoutTests.EntityTests;


using System.Linq;
using System.Numerics;
using Breakout.PowerUps;
using Breakout.States;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using NUnit.Framework;


[TestFixture]
public class PowerUpTests {
    [SetUp]
    public void SetUp() {
        PlayingState.activePowerUps.Clear();
    }

    [Test]
    public void SpawnPowerUp_ShouldAddOnePowerUpAtGivenPosition() {
        // Arrange
        var startPos = new Vector2(0.3f, 0.7f);
        Assert.AreEqual(0, PlayingState.activePowerUps.Count,
            "Expected no powerups before spawning.");

        // Act
        PowerUp.SpawnPowerUp(startPos);

        // Assert
        Assert.AreEqual(1, PlayingState.activePowerUps.Count,
            "spawnPowerUp should only add one powerup.");

        var pu = PlayingState.activePowerUps.Last();
        var dynShape = pu.Shape.AsDynamicShape();
        Assert.AreEqual(startPos, dynShape.Position,
            "PowerUp should be at the given position.");
    }

    [Test]
    public void Move_ShouldApplyFallingSpeedToPositionAndVelocity() {
        // Arrange
        var startPos = new Vector2(0.5f, 0.9f);
        PowerUp.SpawnPowerUp(startPos);
        var pu = PlayingState.activePowerUps.Single();
        var expectedSpeed = pu.fallingSpeed;

        // Act
        pu.Move();

        // Assert:
        var dynShape = pu.Shape.AsDynamicShape();
        Assert.AreEqual(expectedSpeed, dynShape.Velocity,
            "Velocity should be equal to fallingSpeed.");

        // Assert:
        var expectedPos = startPos + expectedSpeed;
        Assert.AreEqual(expectedPos, dynShape.Position,
            "Position should be updated by fallingSpeed.");
    }
}

