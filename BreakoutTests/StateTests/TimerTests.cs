namespace BreakoutTests.StateTests;

using System.Reflection;
using Breakout.States;
using NUnit.Framework;

[TestFixture]
public class TimerTests {
    private FieldInfo? timeField;

    [SetUp]
    public void SetUp() {
        timeField = typeof(GameRunning).GetField("_timeRemaining", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [Test]
    public void AddTime_NeverGoesNegative() {
        var state = new GameRunning(null, levelNumber: 1);
        // Reduce time by more than the current remaining to ensure clamping
        state.AddTime(-1000);
        int remaining = (int)timeField!.GetValue(state)!;
        Assert.That(remaining, Is.GreaterThanOrEqualTo(0));
        Assert.AreEqual(0, remaining, "AddTime should clamp time to zero when subtracting more than remaining");
    }
}