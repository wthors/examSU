namespace Breakout.Points;

/// <summary>
/// Tracks the player's score across levels.
/// </summary>
public class PointTracker {
    private int score;

    public PointTracker() {
        score = 0;
    }

    public void AddPoints(int amount) {
        if (amount > 0) {
            score += amount;
        }
    }

    public void Reset() {
        score = 0;
    }

    public int GetScore() {
        return score;
    }
}
