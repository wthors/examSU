namespace Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;

/// <summary>
/// Exposes common behaviour and properties for all block types.
/// </summary>

public interface IBlock {
    int Health {
        get;
    }
    int Value {
        get;
    }
    Shape Shape {
        get;
    }
    void DecreaseHealth();
    void Render(WindowContext context);
    bool IsAlive {
        get;
    }

    int GetValue();

    event Action<IBlock> OnBlockDestroyed;
}