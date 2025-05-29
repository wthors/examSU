namespace Breakout.Level;
using System.Collections.Generic;
using System.Numerics;
using Breakout.Blocks;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

/// <summary>
/// The AdvancedLevelBuilder class implements the ILevelBuilder interface.
/// It constructs a level based on the provided map rows, legend, metadata, and block size.
/// </summary>

public class LevelBuilder : ILevelBuilder {
    private List<string> _mapRows;
    private Dictionary<char, string> _legend;
    private Dictionary<string, string> _metadata;
    private float _blockWidth;
    private float _blockHeight;

    public ILevelBuilder WithMapRows(List<string> mapRows) {
        _mapRows = mapRows;
        return this;
    }
    public ILevelBuilder WithLegend(Dictionary<char, string> legend) {
        _legend = legend;
        return this;
    }
    public ILevelBuilder WithMetadata(Dictionary<string, string> metadata) {
        _metadata = metadata;
        return this;
    }
    public ILevelBuilder WithBlockSize(float width, float height) {
        _blockWidth = width;
        _blockHeight = height;
        return this;
    }
    public List<IBlock> Build() {
        var blocks = new List<IBlock>();

        HashSet<char> powerUpSymbols = new();
        if (_metadata.TryGetValue("PowerUp", out var powerUpValue)) {
            powerUpSymbols = new HashSet<char>(powerUpValue);
        }

        for (int i = 0; i < _mapRows.Count; i++) {
            var line = _mapRows[i];
            for (int j = 0; j < line.Length; j++) {
                char sym = line[j];
                if (sym == '-' || !_legend.ContainsKey(sym))
                    continue;

                float x = j * _blockWidth;
                float y = 1.0f - (i + 1) * _blockHeight;
                var shape = new StationaryShape(
                    new Vector2(x, y),
                    new Vector2(_blockWidth, _blockHeight)
                );

                string baseName = _legend[sym];
                var healthyImage = new Image($"Breakout.Assets.Images.{baseName}");
                string damagedName = Path.GetFileNameWithoutExtension(baseName) + "-damaged";
                var damagedImage = new Image($"Breakout.Assets.Images.{damagedName}.png");
                var powerUpIcon = new Image($"Breakout.Assets.Images.DoubleSpeedPowerUp.png");


                // new: detect unbreakable first
                bool isUnbreakable = _metadata.TryGetValue("Unbreakable", out var unbreakSymbol)
                                     && unbreakSymbol == sym.ToString();
                bool isHardened = _metadata.TryGetValue("Hardened", out var hardSymbol)
                                     && hardSymbol == sym.ToString();
                bool isPowerUp = _metadata.TryGetValue("PowerUp", out var powerUpSymbol)
                                     && powerUpSymbol == sym.ToString();

                // choose type in order: Unbreakable → Hardened → PowerUp → Standard
                string type = isUnbreakable
                              ? "Unbreakable"
                              : isHardened
                                ? "Hardened"
                                : isPowerUp
                                    ? "PowerUp"
                                    : "Standard";

                blocks.Add(BlockFactory.Create(type, shape, healthyImage, damagedImage, powerUpIcon));
            }
        }
        return blocks;
    }
}