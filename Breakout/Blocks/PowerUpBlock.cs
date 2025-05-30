namespace Breakout.Blocks;
using Breakout.PowerUps;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
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
