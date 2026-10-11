using UnityEditor;
using UnityEngine;

namespace TG.Control.Editor
{
    /// <summary>Preserves the approved 3840x2160 home background without platform recompression.</summary>
    public sealed class HomeBackground4KImporter : AssetPostprocessor
    {
        private const string AssetPath =
            "Assets/Resources/TGExhibitionUI/HomeDarkV2/Backgrounds/HomeTechBackground_4K.png";

        private void OnPreprocessTexture()
        {
            if (!assetPath.Equals(AssetPath, System.StringComparison.OrdinalIgnoreCase)) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Default;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 4096;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;

            var standalone = importer.GetPlatformTextureSettings("Standalone");
            standalone.overridden = true;
            standalone.maxTextureSize = 4096;
            standalone.format = TextureImporterFormat.RGB24;
            standalone.textureCompression = TextureImporterCompression.Uncompressed;
            standalone.crunchedCompression = false;
            standalone.compressionQuality = 100;
            importer.SetPlatformTextureSettings(standalone);
        }
    }
}
