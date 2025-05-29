// StateMachineTests.cs
namespace Breakout.StateTests;

using Breakout.States;
using DIKUArcade.GUI;
using DIKUArcade.Input;
using NUnit.Framework;


// A no-op stub just to stand in for real states:
public class DummyState : IGameState {
    public void Render(WindowContext context) {
    }
    public void Update() {
    }
    public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
    }
}

[TestFixture]
public class StateMachineTests {
    [Test]
    public void Constructor_SetsActiveAndPrevious_ToMainMenu() {
        // Arrange & Act
        var sm = new StateMachine();

        // Assert
        Assert.IsNotNull(sm.ActiveState, "ActiveState should not be null");
        Assert.IsNotNull(sm.PreviousState, "PreviousState should not be null");
        Assert.AreSame(sm.ActiveState, sm.PreviousState,
            "Initially ActiveState and PreviousState should reference the same instance");
        Assert.IsInstanceOf<MainMenu>(sm.ActiveState,
            "The default state should be MainMenu");
    }

    [Test]
    public void SettingActiveState_UpdatesPreviousStateCorrectly() {
        // Arrange
        var sm = new StateMachine();
        var first = new DummyState();
        var second = new DummyState();

        // Act #1: switch to `first`
        sm.ActiveState = first;

        // Assert #1
        Assert.AreSame(first, sm.ActiveState, "ActiveState should be the first dummy");
        Assert.IsInstanceOf<MainMenu>(sm.PreviousState,
            "PreviousState should have been the original MainMenu");

        // Act #2: switch to `second`
        sm.ActiveState = second;

        // Assert #2
        Assert.AreSame(second, sm.ActiveState, "ActiveState should be the second dummy");
        Assert.AreSame(first, sm.PreviousState,
            "PreviousState should now be the first dummy");
    }
}
