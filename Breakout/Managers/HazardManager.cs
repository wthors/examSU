namespace Breakout.Managers;

using System;
using System.Collections.Generic;
using Breakout.Blocks;
using Breakout.Hazards;
using Breakout.States;
using DIKUArcade.Entities;
using DIKUArcade.GUI;
using DIKUArcade.Physics;


public class HazardManager {
    private readonly List<Hazard> _activeHazards = new();
    private readonly Random _rng = new();

    public HazardManager(IEnumerable<IBlock> blocks, double hazardChance = 0.2) {
        foreach (var block in blocks) {
            if (block is not StandardBlock)
                continue;
            // Only spawn hazards for StandardBlock instances
            block.OnBlockDestroyed += b => TrySpawn(b, hazardChance);
        }
    }

    private void TrySpawn(IBlock block, double chance) {

        if (_rng.NextDouble() < chance) {
            var dyn = block.Shape.AsDynamicShape();
            var hazard = Hazard.SpawnHazard(dyn.Position, dyn.Extent);
            _activeHazards.Add(hazard);
        }
    }

    public void Update(GameRunning state) {
        for (int i = _activeHazards.Count - 1; i >= 0; i--) {
            var hz = _activeHazards[i];
            hz.Update();  // move the hazard
            if (!hz.IsAlive) {
                _activeHazards.RemoveAt(i);
                continue;
            }
            // Check collision with player paddle
            if (CollisionDetection.Aabb(hz.Shape.AsDynamicShape(), state.PlayerShape()).Collision) {
                hz.Activate(state);
                _activeHazards.RemoveAt(i);
            }
        }
    }

    public void Render(WindowContext context) {
        foreach (var hz in _activeHazards) {
            hz.RenderEntity(context);
        }
    }
}


