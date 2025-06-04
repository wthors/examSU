namespace Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

/// <summary>
/// Block that requires two hits to be destroyed.
/// </summary>

public class HardenedBlock : BlockBase {
    private readonly IBaseImage _damaged;

    public HardenedBlock(StationaryShape shape, IBaseImage healthy, IBaseImage damaged)
      : base(shape, healthy, health: 2, value: 20) {
        _damaged = damaged;
    }

    public override void DecreaseHealth() {
        base.DecreaseHealth();        // Health-- and DeleteEntity if ≤0
        if (Health == 1) {            // on first hit:
            this.Image = _damaged;      // swap to cracked sprite
        }
    }
}
