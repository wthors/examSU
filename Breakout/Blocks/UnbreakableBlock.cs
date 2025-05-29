namespace Breakout.Blocks;

using DIKUArcade.Entities;
using DIKUArcade.Graphics;

public class UnbreakableBlock : BlockBase {
    public UnbreakableBlock(StationaryShape shape, IBaseImage healthyImage)
        : base(shape, healthyImage, health: 1, value: 0) { }
    public override void DecreaseHealth() {
        // Unbreakable blocks do not decrease health.
    }
}