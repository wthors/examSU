namespace BreakoutTests.EntityTests;

using System;
using System.Numerics;
using Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using NUnit.Framework;

[TestFixture]
public class BlockTests {
    [Test]
    public void BlockHealthDecrementsWhenHit() {
        var shape = new StationaryShape(
            new Vector2(0.0f, 0.1f),
            new Vector2(0.1f, 0.1f)
        );
        //Console.WriteLine(Path.GetFullPath("Assets/Images/blue-block.png"));
        var image = new NoImage(); //new Image("Assets/Images/blue-block.png");
        var dmgimage = new NoImage();
        var block = new HardenedBlock(shape, image, dmgimage);

        block.DecreaseHealth();

        Assert.AreEqual(1, block.Health);
    }

    [Test]
    public void BlockDestroyedAfterTwoHits() {
        var shape = new StationaryShape(
            new Vector2(0.0f, 0.1f),
            new Vector2(0.1f, 0.1f)
        );
        var image = new NoImage();
        var dmgimage = new NoImage();
        var block = new HardenedBlock(shape, image, dmgimage);

        block.DecreaseHealth();
        block.DecreaseHealth();

        Assert.IsFalse(block.IsAlive);
    }
}