namespace Breakout.Managers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using Breakout.Blocks;
using Breakout.Hazards;
using DIKUArcade.GUI;


    public class HazardManager {
        private readonly List<Hazard> _activeHazards = new();
        private readonly Random _rng = new();

        public HazardManager(IEnumerable<IBlock> blocks, double hazardChance = 0.2) {
            foreach (var block in blocks) {
                if (block is not StandardBlock) continue;
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

        public void Update(Action<Hazard> onPlayerCollision) {
            for (int i = _activeHazards.Count - 1; i >= 0; i--) {
                var hz = _activeHazards[i];
                hz.Update();

                if (!hz.IsAlive) {
                    _activeHazards.RemoveAt(i);
                    continue;
                }

                onPlayerCollision?.Invoke(hz);
            }
        }

        public void Render(WindowContext context) {
            foreach (var hz in _activeHazards) {
                hz.RenderEntity(context);
            }
        }
    }


