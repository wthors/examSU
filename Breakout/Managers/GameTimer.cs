namespace Breakout.Managers;

using DIKUArcade.Timers;

/// <summary>
/// Simple countdown timer used for level time limits.
/// </summary>
public class GameTimer {
    private int _timeRemaining;
    private long _lastTickTime;

    public int Remaining => _timeRemaining;

    public GameTimer(int initialSeconds) {
        _timeRemaining = initialSeconds;
        _lastTickTime = StaticTimer.GetElapsedMilliseconds();
    }

    /// <summary>
    /// Decrease the remaining time by one second when a second has passed.
    /// </summary>
    public void Update() {
        long elapsed = StaticTimer.GetElapsedMilliseconds();
        if (_lastTickTime + 1000 < elapsed) {
            _timeRemaining = System.Math.Max(0, _timeRemaining - 1);
            _lastTickTime += 1000;
        }
    }

    /// <summary>
    /// Add or subtract seconds from the timer. Time never goes below zero.
    /// </summary>
    public void AddTime(int seconds) {
        _timeRemaining = System.Math.Max(0, _timeRemaining + seconds);
    }

    /// <summary>
    /// Reset the internal tick counter, typically when resuming a paused game.
    /// </summary>
    public void Reset() {
        _lastTickTime = StaticTimer.GetElapsedMilliseconds();
    }
}