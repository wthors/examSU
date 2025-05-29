namespace BreakoutTests.LevelLoadingTests;

using Breakout.Level;  // Namespace where LevelLoader and LevelDefinition are defined
using NUnit.Framework;

[TestFixture]
public class LevelLoaderTests {
    // Helper to get the manifest resource name for test level files
    private string TestLevelResource(string name) {
        return $"Breakout.Assets.TestLevels.{name}.txt";
    }

    [Test]
    public void LoadLevel_ValidFile_ParsesAllSections() {
        // Arrange
        string resourceName = TestLevelResource("valid");
        // Act
        LevelDefinition def = LevelLoader.LoadLevel(resourceName);
        // Assert - Map section
        Assert.IsNotNull(def.MapRows, "MapRows should not be null");
        Assert.AreEqual(2, def.MapRows.Count, "Should have 2 map rows.");
        Assert.AreEqual("##--##", def.MapRows[0], "First map row incorrect");
        Assert.AreEqual("##--##", def.MapRows[1], "Second map row incorrect");
        // Assert - Metadata section
        Assert.IsNotNull(def.Metadata, "Metadata should not be null");
        Assert.IsTrue(def.Metadata.ContainsKey("Name"), "Name key is missing in metadata");
        Assert.AreEqual("Test Level", def.Metadata["Name"], "Name metadata value is incorrect");
        Assert.IsTrue(def.Metadata.ContainsKey("Time"), "Time key is missing in metadata");
        Assert.AreEqual("180", def.Metadata["Time"], "Time metadata value is incorrect");
        // Assert - Legend section
        Assert.IsNotNull(def.Legend, "Legend should not be null");
        Assert.AreEqual(2, def.Legend.Count, "Should have 2 legend entries");
        Assert.IsTrue(def.Legend.ContainsKey('#'), "Legend missing entry for '#'");
        Assert.AreEqual("green-block.png", def.Legend['#'], "Legend value for '#' is incorrect");
        Assert.IsTrue(def.Legend.ContainsKey('-'), "Legend missing entry for '-'");
        Assert.AreEqual("teal-block.png", def.Legend['-'], "Legend value for '-' is incorrect");
    }

    [Test]
    public void LoadLevel_MetadataOnlyFile_ParsesMetadataAndSkipsOthers() {
        string resourceName = TestLevelResource("metadata_only");
        LevelDefinition def = LevelLoader.LoadLevel(resourceName);
        // Map section should be empty (no Map: in file)
        Assert.IsNotNull(def.MapRows);
        Assert.AreEqual(0, def.MapRows.Count, "MapRows should be empty for a metadata-only file");
        // Metadata section should contain the one key-value pair
        Assert.IsTrue(def.Metadata.ContainsKey("Name"), "Metadata 'Name' key not found");
        Assert.AreEqual("Metadata Level", def.Metadata["Name"], "Metadata 'Name' value is incorrect");
        Assert.IsTrue(def.Metadata.ContainsKey("Time"), "Time key is missing in metadata");
        Assert.AreEqual("180", def.Metadata["Time"], "Time metadata value is incorrect");
        Assert.AreEqual(2, def.Metadata.Count, "Metadata dictionary should have exactly 2 entries");
        // Legend section should be empty (no Legend: in file)
        Assert.IsNotNull(def.Legend);
        Assert.AreEqual(0, def.Legend.Count, "Legend should be empty for a metadata-only file");
    }

    [Test]
    public void LoadLevel_LegendOnlyFile_ParsesLegendAndSkipsOthers() {
        string resourceName = TestLevelResource("legend_only");
        LevelDefinition def = LevelLoader.LoadLevel(resourceName);
        // Map section should be empty (no Map: in file)
        Assert.IsNotNull(def.MapRows);
        Assert.AreEqual(0, def.MapRows.Count, "MapRows should be empty for a legend-only file");
        // Metadata section should be empty (no Meta: in file)
        Assert.IsNotNull(def.Metadata);
        Assert.AreEqual(0, def.Metadata.Count, "Metadata should be empty for a legend-only file");
        // Legend section should contain the defined mappings
        Assert.IsTrue(def.Legend.ContainsKey('#'), "Legend '#' entry not found");
        Assert.AreEqual("red-block.png", def.Legend['#'], "Legend value for '#' is incorrect");
        Assert.IsTrue(def.Legend.ContainsKey('%'), "Legend '%' entry not found");
        Assert.AreEqual("orange-block.png", def.Legend['%'], "Legend value for '%' is incorrect");
        Assert.AreEqual(2, def.Legend.Count, "Legend dictionary should have 2 entries for a legend-only file");
    }

    [Test]
    public void LoadLevel_EmptyFile_ReturnsEmptyLevelDefinition() {
        string resourceName = TestLevelResource("empty");
        LevelDefinition def = LevelLoader.LoadLevel(resourceName);
        // All sections should be present but empty
        Assert.IsNotNull(def.MapRows);
        Assert.AreEqual(0, def.MapRows.Count, "MapRows should be empty for an empty file");
        Assert.IsNotNull(def.Metadata);
        Assert.AreEqual(0, def.Metadata.Count, "Metadata should be empty for an empty file");
        Assert.IsNotNull(def.Legend);
        Assert.AreEqual(0, def.Legend.Count, "Legend should be empty for an empty file");
    }
}
