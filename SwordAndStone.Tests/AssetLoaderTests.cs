using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using SwordAndStone.Common;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for AssetLoader to verify texture loading functionality
    /// </summary>
    [TestFixture]
    public class AssetLoaderTests
    {
        private string testDataPath;
        
        [SetUp]
        public void Setup()
        {
            // Create a temporary test directory structure
            testDataPath = Path.Combine(Path.GetTempPath(), "SwordAndStoneTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDataPath);
            
            // Create subdirectories
            Directory.CreateDirectory(Path.Combine(testDataPath, "gui"));
            Directory.CreateDirectory(Path.Combine(testDataPath, "gui", "wow"));
            
            // Create dummy files
            File.WriteAllText(Path.Combine(testDataPath, "test.png"), "test content");
            File.WriteAllText(Path.Combine(testDataPath, "gui", "button.png"), "button content");
            File.WriteAllText(Path.Combine(testDataPath, "gui", "wow", "actionbar_bg.png"), "actionbar content");
        }
        
        [TearDown]
        public void Cleanup()
        {
            // Clean up test directory
            if (Directory.Exists(testDataPath))
            {
                Directory.Delete(testDataPath, true);
            }
        }
        
        [Test]
        public void TestAssetLoader_LoadsFilesWithRelativePaths()
        {
            // Arrange
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert
            Assert.Greater(assetList.count, 0, "Should load at least one asset");
            Assert.AreEqual(1.0f, progress.value, "Progress should be complete");
            
            // Check that assets include both relative paths and filenames
            var assetNames = assetList.items.Take(assetList.count).Select(a => a.name).ToList();
            
            // Should have full paths
            Assert.IsTrue(assetNames.Contains("gui/wow/actionbar_bg.png"), 
                "Should contain full path 'gui/wow/actionbar_bg.png'");
            Assert.IsTrue(assetNames.Contains("gui/button.png"), 
                "Should contain full path 'gui/button.png'");
            
            // Should also have filenames for backward compatibility
            Assert.IsTrue(assetNames.Contains("actionbar_bg.png"), 
                "Should contain filename 'actionbar_bg.png' for backward compatibility");
            Assert.IsTrue(assetNames.Contains("button.png"), 
                "Should contain filename 'button.png' for backward compatibility");
        }
        
        [Test]
        public void TestAssetLoader_NormalizesPathSeparators()
        {
            // Arrange
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert - all paths should use forward slashes
            var assetNames = assetList.items.Take(assetList.count).Select(a => a.name).ToList();
            foreach (var name in assetNames)
            {
                Assert.IsFalse(name.Contains("\\"), 
                    $"Asset name '{name}' should not contain backslashes");
            }
        }
        
        [Test]
        public void TestAssetLoader_ConvertsToLowercase()
        {
            // Arrange
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert - all asset names should be lowercase
            var assetNames = assetList.items.Take(assetList.count).Select(a => a.name).ToList();
            foreach (var name in assetNames)
            {
                Assert.AreEqual(name.ToLowerInvariant(), name, 
                    $"Asset name '{name}' should be lowercase");
            }
        }
        
        [Test]
        public void TestAssetLoader_BackwardCompatibility()
        {
            // Arrange
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert - old code referencing just "actionbar_bg.png" should still work
            var actionbarAsset = assetList.items.Take(assetList.count)
                .FirstOrDefault(a => a.name == "actionbar_bg.png");
            
            Assert.IsNotNull(actionbarAsset, 
                "Should find asset by filename for backward compatibility");
            Assert.IsNotNull(actionbarAsset.data, 
                "Asset data should be loaded");
            Assert.Greater(actionbarAsset.dataLength, 0, 
                "Asset should have content");
        }
        
        [Test]
        public void TestAssetLoader_NewCodeWithPaths()
        {
            // Arrange
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert - new code referencing "gui/wow/actionbar_bg.png" should work
            var actionbarAsset = assetList.items.Take(assetList.count)
                .FirstOrDefault(a => a.name == "gui/wow/actionbar_bg.png");
            
            Assert.IsNotNull(actionbarAsset, 
                "Should find asset by full path");
            Assert.IsNotNull(actionbarAsset.data, 
                "Asset data should be loaded");
            Assert.Greater(actionbarAsset.dataLength, 0, 
                "Asset should have content");
        }
        
        [Test]
        public void TestAssetLoader_IgnoresThumbsDb()
        {
            // Arrange
            File.WriteAllText(Path.Combine(testDataPath, "Thumbs.db"), "thumbs content");
            AssetLoader loader = new AssetLoader(new string[] { testDataPath });
            AssetList assetList = new AssetList();
            assetList.items = new Asset[2048];
            FloatRef progress = new FloatRef();
            
            // Act
            loader.LoadAssetsAsync(assetList, progress);
            
            // Assert
            var assetNames = assetList.items.Take(assetList.count).Select(a => a.name).ToList();
            Assert.IsFalse(assetNames.Any(n => n.ToLower().Contains("thumbs.db")), 
                "Should not load Thumbs.db files");
        }
    }
}
