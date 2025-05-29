namespace Breakout.Tests;
using Breakout.Points;
using NUnit.Framework;

[TestFixture]
public class PointTrackerTests {
    [Test]
    public void TestAddPoints() {
        var points = new PointTracker();
        points.AddPoints(10);
        Assert.AreEqual(10, points.GetScore());
    }

    [Test]
    public void TestAddZeroPoints() {
        var points = new PointTracker();
        points.AddPoints(0);
        Assert.AreEqual(0, points.GetScore());
    }

    [Test]
    public void TestAddNegativePoints() {
        var points = new PointTracker();
        points.AddPoints(-5);
        Assert.AreEqual(0, points.GetScore(), "Negative points should not reduce score.");
    }

    [Test]
    public void TestResetPoints() {
        var points = new PointTracker();
        points.AddPoints(15);
        points.Reset();
        Assert.AreEqual(0, points.GetScore());
    }
}
