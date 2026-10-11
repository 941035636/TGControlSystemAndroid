using UnityEditor;
using UnityEngine;

namespace TG.Control.Editor
{
    /// <summary>Stable import settings for the approved 1024x544 module-card artwork.</summary>
    public sealed class OfficialModuleCardImporter : AssetPostprocessor
    {
        private const string ResourceFolder =
            "Assets/Resources/TGExhibitionUI/OfficialModuleCards/";

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ResourceFolder, System.StringComparison.OrdinalIgnoreCase)) return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
        }
    }
}
