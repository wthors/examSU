namespace Breakout.Managers;
using System.Collections.Generic;
using Breakout.Blocks;
using Breakout.PowerUps;
using Breakout.States;
using DIKUArcade.GUI;
using DIKUArcade.Physics;

/// <summary>
/// Controls spawning and lifetime of power-ups and applies their effects.
/// </summary>

public class PowerUpManager {
    private readonly List<PowerUp> _active = new();

    public PowerUpManager(IEnumerable<IBlock> blocks) {
        foreach (var blk in blocks) {
            if (blk is PowerUpBlock pub) {
                blk.OnBlockDestroyed += b => TrySpawn(pub);
            }
        }
    }

    private void TrySpawn(PowerUpBlock blk) {
        var dyn = blk.Shape.AsDynamicShape();
        var pu = PowerUp.SpawnPowerUp(dyn.Position);
        _active.Add(pu);
    }

    public void Update(GameRunning state) {
        for (int i = _active.Count - 1; i >= 0; i--) {
            var pu = _active[i];
            pu.Move();
            var ps = pu.Shape.AsDynamicShape();
            if (ps.Position.Y <= 0f) {
                _active.RemoveAt(i);
                continue;
            }
            if (CollisionDetection.Aabb(ps, state.PlayerShape()).Collision) {
                pu.Activate(state);
                _active.RemoveAt(i);
            }
        }
    }


    public void Render(WindowContext ctx) {
        foreach (var pu in _active)
            pu.RenderEntity(ctx);
    }
}
