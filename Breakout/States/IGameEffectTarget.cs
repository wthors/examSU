namespace Breakout.States;

/// <summary>
/// Interface for applying game effects produced by hazards or power-ups.
/// </summary>

public interface IGameEffectTarget {
    void GainLife();
    void LoseLife();
    void AddTime(int seconds);
    void IncreasePaddleSpeed(float multiplier = 1.5f, int duration = 10);
    void DoubleSpeed(float multiplier = 2.0f, int duration = 10);
    void SplitBalls();
}