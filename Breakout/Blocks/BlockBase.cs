namespace Breakout.Blocks;

using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;


public abstract class BlockBase : Entity, IBlock {
    public int Health {
        get; protected set;
    }
    public int Value {
        get; protected set;
    }

    protected BlockBase(StationaryShape shape, IBaseImage image, int health, int value)
        : base(shape, image) {
        Health = health;
        Value = value;
    }
    public virtual void DecreaseHealth() {
        this.Health--;
        if (Health <= 0) {
            this.DeleteEntity();
            OnBlockDestroyed(this);
        }
    }

    public void Render(WindowContext context) {
        base.RenderEntity(context);
    }
    public int GetValue() {
        return Value;
    }

    public bool IsAlive => Health > 0;

    public event Action<IBlock> OnBlockDestroyed = delegate { };
}