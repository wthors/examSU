namespace Breakout.Level;
using System.Collections.Generic;
using Breakout.Blocks;

/// <summary>
/// Orchestrates the building of levels using a supplied builder.
/// </summary>

public class LevelDirector {
    private readonly ILevelBuilder _builder;
    public LevelDirector(ILevelBuilder builder) {
        _builder = builder;
    }

    /// <summary>
    /// Constructs a level based on the provided LevelDefinition.
    /// </summary>
    public List<IBlock> Construct(LevelDefinition def) {
        int rows = def.MapRows.Count;
        int cols = def.MapRows[0].Length;
        float blockW = 1.0f / cols;
        float blockH = 0.05f;
        return _builder
            .WithMapRows(def.MapRows)
            .WithLegend(def.Legend)
            .WithMetadata(def.Metadata)
            .WithBlockSize(blockW, blockH)
            .Build();
    }
}