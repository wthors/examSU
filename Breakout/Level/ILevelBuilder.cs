namespace Breakout.Level;
using System.Collections.Generic;
using Breakout.Blocks;

/// <summary>
/// The ILevelBuilder interface defines the methods required to build a level.
/// It allows for setting the map rows, legend, metadata, and block size.
/// </summary>


public interface ILevelBuilder {
    ILevelBuilder WithMapRows(List<string> mapRows);
    ILevelBuilder WithLegend(Dictionary<char, string> legend);
    ILevelBuilder WithMetadata(Dictionary<string, string> metadata);
    ILevelBuilder WithBlockSize(float width, float height);
    List<IBlock> Build();
}