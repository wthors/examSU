namespace Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

/// <summary>
/// Basic block destroyed with a single hit.
/// </summary>

public class StandardBlock : BlockBase {
    public StandardBlock(StationaryShape shape, IBaseImage healthyImage)
        : base(shape, healthyImage, health: 1, value: 10) { }
    public override void DecreaseHealth() {
        base.DecreaseHealth();
    }
}