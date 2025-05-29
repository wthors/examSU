namespace Breakout.Level;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class LevelDefinition {
    public List<string> MapRows { get; set; } = new List<string>();
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<char, string> Legend { get; } = new Dictionary<char, string>();
}

/// <summary>
/// The LevelLoader class is responsible for loading level definitions from embedded resources.
/// It reads the level data from a text file and parses it into a LevelDefinition object.
/// The text file should contain sections for the map, metadata, and legend.
/// </summary>


public static class LevelLoader {
    public static LevelDefinition LoadLevel(string resourceName) {
        var assembly = typeof(LevelLoader).Assembly;
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) {
            throw new FileNotFoundException($"Level resource '{resourceName}' not found.");
        }

        using var reader = new StreamReader(stream);
        var lines = new List<string>();
        while (!reader.EndOfStream) {
            lines.Add(reader.ReadLine() ?? string.Empty);
        }

        return ParseLines(lines);
    }

    private static LevelDefinition ParseLines(List<string> lines) {
        var def = new LevelDefinition();
        int index = 0;

        // Section 1: Map (ASCII grid)
        if (index < lines.Count && lines[index].Trim().Equals("Map:", StringComparison.OrdinalIgnoreCase)) {
            index++;  // skip "Map:" marker
            while (index < lines.Count && !lines[index].Trim().Equals("Map/", StringComparison.OrdinalIgnoreCase)) {
                string line = lines[index];
                if (!string.IsNullOrWhiteSpace(line)) {
                    def.MapRows.Add(line);
                }
                index++;
            }
            // skip the closing "Map/" if present
            if (index < lines.Count && lines[index].Trim().Equals("Map/", StringComparison.OrdinalIgnoreCase)) {
                index++;
            }

            // skip blank lines between sections
            while (index < lines.Count && string.IsNullOrWhiteSpace(lines[index])) {
                index++;
            }
        }

        // Section 2: Metadata (key: value pairs)
        if (index < lines.Count && lines[index].Trim().Equals("Meta:", StringComparison.OrdinalIgnoreCase)) {
            index++;  // skip "Meta:" marker
            while (index < lines.Count && !lines[index].Trim().Equals("Meta/", StringComparison.OrdinalIgnoreCase)) {
                string line = lines[index];
                if (!string.IsNullOrWhiteSpace(line)) {
                    var parts = line.Split(new[] { ':' }, 2);
                    string key = parts[0].Trim();
                    string value = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    if (key.Length > 0) {
                        def.Metadata[key] = value;
                    }
                }
                index++;
            }
            // skip closing "Meta/" if present
            if (index < lines.Count && lines[index].Trim().Equals("Meta/", StringComparison.OrdinalIgnoreCase)) {
                index++;
            }
            // skip blank lines between sections
            while (index < lines.Count && string.IsNullOrWhiteSpace(lines[index])) {
                index++;
            }

        }

        // Section 3: Legend (character = image path mappings)
        if (index < lines.Count && lines[index].Trim().Equals("Legend:", StringComparison.OrdinalIgnoreCase)) {
            index++; // skip 'Legend:'
            while (index < lines.Count && !lines[index].Trim().Equals("Legend/", StringComparison.OrdinalIgnoreCase)) {
                var line = lines[index].Trim();
                if (!string.IsNullOrWhiteSpace(line)) {
                    // Expect format: "<char>) <imagePath>"
                    int sep = line.IndexOf(')');
                    if (sep > 0 && sep < line.Length - 1) {
                        char symbol = line[0];
                        var imagePath = line.Substring(sep + 1).Trim();
                        def.Legend[symbol] = imagePath;
                    }
                }
                index++;
            }
            // skip 'Legend/'
            if (index < lines.Count && lines[index].Trim().Equals("Legend/", StringComparison.OrdinalIgnoreCase)) {
                index++;
            }
        }

        return def;
    }
}
