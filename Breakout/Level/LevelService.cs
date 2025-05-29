namespace Breakout.Level;
using System.Collections.Generic;
using Breakout.Blocks;
using Breakout.Level;

public interface ILevelService {
    List<IBlock> GetBlocks(int levelNumber);
    Dictionary<string, string> GetMetadata(int levelNumber);
}

/// <summary>
/// The LevelService class is responsible for loading and constructing levels.
/// It uses the LevelDirector to build the level based on a level definition.
/// </summary>
public class LevelService : ILevelService {
    private readonly LevelDirector _director;
    public LevelService(LevelDirector director) {
        _director = director;
    }
    public List<IBlock> GetBlocks(int levelNumber) {
        string resName = $"Breakout.Assets.Levels.level{levelNumber}.txt";
        var def = LevelLoader.LoadLevel(resName);
        return _director.Construct(def);
    }

    public Dictionary<string, string> GetMetadata(int levelNumber) {
        string resName = $"Breakout.Assets.Levels.level{levelNumber}.txt";
        var def = LevelLoader.LoadLevel(resName);
        return def.Metadata;
    }

}