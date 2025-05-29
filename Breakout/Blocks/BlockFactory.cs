namespace Breakout.Blocks;
using System;
using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using Microsoft.VisualBasic;

public static class BlockFactory {
    private static readonly Dictionary<string, Func<StationaryShape, IBaseImage, IBaseImage, IBaseImage, IBlock>>
    _registry = new Dictionary<string, Func<StationaryShape, IBaseImage, IBaseImage, IBaseImage, IBlock>>();

    static BlockFactory() {
        Register("Standard", (shape, healthy, damaged, powerupIcon) => new StandardBlock(shape, healthy));
        Register("Hardened", (shape, healthy, damaged, powerupIcon) => new HardenedBlock(shape, healthy, damaged));
        Register("Unbreakable", (shape, healthy, damaged, powerupIcon) => new UnbreakableBlock(shape, healthy));
        Register("PowerUp", (shape, healthy, damaged, powerupIcon) => new PowerUpBlock(shape, healthy, powerupIcon));
    }

    public static void Register(string type, Func<StationaryShape, IBaseImage, IBaseImage, IBaseImage, IBlock> creator) {
        _registry[type] = creator;
    }

    public static IBlock Create(string type, StationaryShape shape, IBaseImage healthyImage, IBaseImage damagedImage, IBaseImage powerUpIcon) {
        if (!_registry.TryGetValue(type, out var ctor)) {
            throw new ArgumentException($"Unknown block type: {type}", nameof(type));
        }
        return ctor(shape, healthyImage, damagedImage, powerUpIcon);
    }
}

