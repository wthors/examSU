namespace BreakoutTests.EntityTests;

using System.Numerics;
using Breakout.Hazards;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using NUnit.Framework;

[TestFixture]
public class HazardTests {
    private Hazard hazard;

    [SetUp]
    public void SetUp() {
        var shape = new DynamicShape(new Vector2(0.5f, 0.5f), new Vector2(0.03f, 0.03f));
        hazard = new Hazard(shape, null, HazardType.LoseLife, 0.005f);
    }


    [Test]
    public void TestHazardInitialization() {
        Assert.AreEqual(HazardType.LoseLife, hazard.Type);
        Assert.IsTrue(hazard.IsAlive);
        Assert.AreEqual(new Vector2(0.0f, -0.005f), hazard.Velocity);
    }

    [Test]
    public void TestHazardMovement() {
        var initialY = hazard.Shape.Position.Y;
        hazard.Update();

        Assert.Less(hazard.Shape.Position.Y, initialY);
        Assert.IsTrue(hazard.IsAlive);
    }

    [Test]
    public void TestHazardOffScreenRemoval() {
        hazard.Shape.Position = new Vector2(0.5f, -0.01f);
        hazard.Update();

        Assert.IsFalse(hazard.IsAlive);
    }

    [Test]
    public void TestHazardSpawnHazard() {
        var spawnedHazard = Hazard.SpawnHazard(new Vector2(0.4f, 0.4f), new Vector2(0.1f, 0.05f));
        Assert.IsNotNull(spawnedHazard);
        Assert.IsTrue(spawnedHazard.Shape.Position.X > 0.4f && spawnedHazard.Shape.Position.X < 0.5f);
        Assert.AreEqual(-0.005f, spawnedHazard.Velocity.Y);
    }
}
