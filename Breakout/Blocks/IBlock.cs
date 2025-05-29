namespace Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;

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