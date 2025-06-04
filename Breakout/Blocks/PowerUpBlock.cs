namespace Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

/// <summary>
/// Block that spawns a power-up when destroyed.
/// </summary>

public class PowerUpBlock : BlockBase {
    public IBaseImage PowerUpIcon {
        get;
    }
    public PowerUpBlock(StationaryShape shape, IBaseImage healthy, IBaseImage powerUpIcon) :
        base(shape, healthy, health: 1, value: 10) {
        PowerUpIcon = powerUpIcon;
    }

    public override void DecreaseHealth() {
        base.DecreaseHealth();
    }
}
