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
    private readonly Dictionary<int, LevelDefinition> _levelCache = new();
    public LevelService(LevelDirector director) {
        _director = director;
    }
    private LevelDefinition GetDefinition(int levelNumber) {
        if (!_levelCache.TryGetValue(levelNumber, out var def)) {
            string resName = $"Breakout.Assets.Levels.level{levelNumber}.txt";
            def = LevelLoader.LoadLevel(resName);
            _levelCache[levelNumber] = def;
        }
        return def;
    }


    public List<IBlock> GetBlocks(int levelNumber) {
        var def = GetDefinition(levelNumber);
        return _director.Construct(def);
    }

    public Dictionary<string, string> GetMetadata(int levelNumber) {
        var def = GetDefinition(levelNumber);
        return def.Metadata;
    }

}