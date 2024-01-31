using System.Collections.Generic;
using System.Linq;
using Graphics;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Utilities.PostProcess
{
    public class SpritePostProcessor : AssetPostprocessor
    {
        private const string SpriteRelativePath = "Assets/Art/Sprites";
        
        private const string UiLoadableAtlasPath = "Assets/Art/Atlases/UILoadable.spriteatlas";
 
        private void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter)assetImporter;
        
            if (!importer.assetPath.Contains(SpriteRelativePath))
            {
                return;
            }

            if (importer.assetPath.Contains("IgnorePostprocess"))
            {
                return;
            }
        
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        
            // NOTE: вычищаем тег, легаси атлас уже не поддерживается
            importer.spritePackingTag = "";
            
            SetPlatformSettings(importer, 2048, TextureResizeAlgorithm.Bilinear, TextureImporterFormat.ASTC_6x6, TextureCompressionQuality.Best);

            if (!importer.assetPath.Contains("NotAtlased"))
            {
                return;
            }
        
            if (importer.assetPath.Contains("Compressed"))
            {
                SetPlatformSettings(importer, 512, TextureResizeAlgorithm.Bilinear, TextureImporterFormat.RGB24, TextureCompressionQuality.Normal);
            }
            else
            {
                SetPlatformSettings(importer, 2048, TextureResizeAlgorithm.Bilinear, TextureImporterFormat.ASTC_6x6, TextureCompressionQuality.Best);
            }
        }

        private void SetPlatformSettings(TextureImporter importer, int maxSize,
            TextureResizeAlgorithm resizeAlgorithm, TextureImporterFormat format, TextureCompressionQuality compressionQuality)
        {
            // NOTE: вычищаем платформенные настройки, если вдруг случайно затесались.
            importer.ClearPlatformTextureSettings("Android");
            importer.ClearPlatformTextureSettings("iPhone");

            var settings = new TextureImporterPlatformSettings
            {
                name = "iPhone",
                overridden = true,
                maxTextureSize = maxSize,
                resizeAlgorithm = resizeAlgorithm,
                format = format,
                compressionQuality = (int)compressionQuality,
            };

            importer.SetPlatformTextureSettings(settings);

            settings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                maxTextureSize = maxSize,
                resizeAlgorithm = resizeAlgorithm,
                format = format,
                compressionQuality = (int)compressionQuality
            };

            importer.SetPlatformTextureSettings(settings);
        }
    
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!importedAssets.Any(assetPath => assetPath.Contains(SpriteRelativePath)) &&
                !deletedAssets.Any(assetPath => assetPath.Contains(SpriteRelativePath)) &&
                !movedAssets.Any(assetPath => assetPath.Contains(SpriteRelativePath)))
            {
                return;
            }
        
            var loadableSprites = new List<Sprite>();
        
            foreach (string spriteGuid in AssetDatabase.FindAssets("t:Sprite", new string[] { SpriteRelativePath }))
            {
                string path = AssetDatabase.GUIDToAssetPath(spriteGuid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(spriteGuid));
            
                if (path.ToLower().Contains("loadable"))
                {
                    loadableSprites.Add(sprite);
                }
            }
        
            Debug.Log("Postprocessing ui atlases...");

            //UpdateAtlass(CompressedAtlasPath, compressedSprites);
            UpdateAtlas(UiLoadableAtlasPath);
        
        
            Debug.LogFormat($"Postprocessing {nameof(IconsData)}. Found {loadableSprites.Count} sprites");
            loadableSprites.Sort(CompareSprites);

            // NOTE: поиск по типу почему-то не работает ("t:IconManager")
            bool isUpdated = false;
            foreach (string prefabGuid in AssetDatabase.FindAssets(nameof(IconsData)))
            {
                // NOTE: сюда попадает не только префаб, но и его скрипт
                var prefab = AssetDatabase.LoadAssetAtPath<IconsData>(AssetDatabase.GUIDToAssetPath(prefabGuid));
                if (prefab == null)
                    continue;

                prefab.ReplaceIcons(loadableSprites.ToArray());
                EditorUtility.SetDirty(prefab);

                isUpdated = true;
            }

            if (!isUpdated)
            {
                Debug.LogError($"{nameof(IconsData)} prefab was not found");
            }
            else
            {
                Debug.LogFormat($"Postprocessing {nameof(IconsData)} done");
            }
        
            AssetDatabase.SaveAssets();
        }

        private static void UpdateAtlas(string path,
            TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
            TextureImporterFormat format = TextureImporterFormat.ASTC_6x6,
            TextureCompressionQuality compressionQuality = TextureCompressionQuality.Best)
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (atlas == null)
            {
                Debug.LogError($"Atlas was not found at path {path}");
                return;
            }

            var settings = new TextureImporterPlatformSettings
            {
                name = "iPhone",
                overridden = true,
                resizeAlgorithm = resizeAlgorithm,
                format = format,
                compressionQuality = (int)compressionQuality
            };

            atlas.SetPlatformSettings(settings);

            settings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                resizeAlgorithm = resizeAlgorithm,
                format = format,
                compressionQuality = (int)compressionQuality
            };

            atlas.SetPlatformSettings(settings);
        
            EditorUtility.SetDirty(atlas);
        }

        private static int CompareSprites(Sprite sprite1, Sprite sprite2)
        {
            int result = sprite1.name.CompareTo(sprite2.name);
            if (result == 0 && sprite1 != sprite2)
            {
                result = sprite1.bounds.extents.magnitude.CompareTo(sprite2.bounds.extents.magnitude);

                if (result == 0)
                {
                    string path1 = AssetDatabase.GetAssetPath(sprite1);
                    string path2 = AssetDatabase.GetAssetPath(sprite2);

                    Debug.LogWarningFormat("Sprites with same name & size detected: {0}\n{1}\n{2}", sprite1.name, path1, path2);

                    result = path1.CompareTo(path2);
                }
            }
            return result;
        }
    }
}