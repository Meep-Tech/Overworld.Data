using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using Overworld.Data.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Overworld.Data {
  public partial class Entity {

    public partial class Icon {

      /// <summary>
      /// Imports entity icons
      /// </summary>
      public class Porter : ArchetypePorter<Icon.Type> {

        ///<summary><inheritdoc/></summary>
        public override string SubFolderName {
          get;
        } = "_icons";

        ///<summary><inheritdoc/></summary>
        public override HashSet<string> ValidConfigOptionKeys
          => base.ValidConfigOptionKeys
            .Append(PorterExtensions.DefaultImageFileLocationConfigOptionKey)
            .Append(PorterExtensions.PixelsPerTileConfigKey)
            .Append(TagsConfigOptionKey);

        ///<summary><inheritdoc/></summary>
        public override HashSet<string> ValidImportOptionKeys 
          => base.ValidImportOptionKeys
            .Append(PorterExtensions.PixelsPerTileImportOptionKey);

        ///<summary><inheritdoc/></summary>
        public Porter(User currentUser)
          : base(() => currentUser.UniqueName) { }

        ///<summary><inheritdoc/></summary>
        protected override IEnumerable<Type> BuildLooselyFromConfig(JObject config, IEnumerable<string> assetFiles, Dictionary<string, object> options, out IEnumerable<string> processedFiles) {
          List<string> allProcessedFiles = new();
          options ??= new();

          options[PorterExtensions.PixelsPerTileImportOptionKey] = config.TryGetValue<int?>(PorterExtensions.PixelsPerTileConfigKey)
            ?? (int)options[PorterExtensions.PixelsPerTileImportOptionKey];

          string imageFile = this.GetDefaultImageFromAssets(assetFiles, options, config);
          if (imageFile is not null) {
            Sprite sprite = this.GetSpriteFromImageImportString(imageFile, new DirectoryInfo(assetFiles.First()).FullName, options: options);
            if (sprite is null) {
              throw new ArgumentException($"Could not get a sprite from image location key: {imageFile}, relative to config: {assetFiles.First()}");
            }

            options[nameof(Type.Sprite)] = sprite;
            allProcessedFiles.Add(imageFile);
          }

          (string resourceName, string packageName, string resourceKey)
            = ConstructArchetypeKeys(imageFile ?? assetFiles.First(), options, config);

          allProcessedFiles.Add(assetFiles.First());

          processedFiles = allProcessedFiles;
          return BuildArchetypeFromCompiledData(resourceName, packageName, resourceKey, config, options, Universe);
        }

        ///<summary><inheritdoc/></summary>
        protected override IEnumerable<Type> BuildLooselyFromAssets(IEnumerable<string> assetFiles, Dictionary<string, object> options, out IEnumerable<string> processedFiles) {
          List<string> allProcessedFiles = new();
          options ??= new();

          options[PorterExtensions.PixelsPerTileImportOptionKey] = 
            (int)options[PorterExtensions.PixelsPerTileImportOptionKey];

          string imageFile = this.GetDefaultImageFromAssets(assetFiles, options);
          if (imageFile != null) {
            options[nameof(Type.Sprite)] = this.GetSpriteFromImageImportString(imageFile, importStringType: ImageImportStringType.LocalAbsolute, options: options);
            allProcessedFiles.Add(imageFile);
          } else throw new ArgumentException($"No files found that match the image types required to make an Entity.");

          (string resourceName, string packageName, string resourceKey)
            = ConstructArchetypeKeys(imageFile ?? assetFiles.First(), options, null);

          allProcessedFiles.Add(assetFiles.First());

          processedFiles = allProcessedFiles;
          return BuildArchetypeFromCompiledData(resourceName, packageName, resourceKey, new JObject(), options, Universe);
        }

        ///<summary><inheritdoc/></summary>
        protected override string[] SerializeArchetypeToModFiles(Type archetype, string packageDirectoryPath) {
          List<string> createdFiles = new();

          Directory.CreateDirectory(packageDirectoryPath);
          // Save the sprite to PNG
          byte[] imageData = null;
          if (archetype.Sprite != null) {
            imageData = archetype.Sprite.texture?.EncodeToPNG();
            if (imageData is not null) {
              string imageFileName = Path.Combine(packageDirectoryPath, "texture.png");
              File.WriteAllBytes(imageFileName, imageData);
              createdFiles.Add(imageFileName);
            }
          }

          // Save the config file:
          string configFileName = Path.Combine(packageDirectoryPath, DefaultConfigFileName);
          JObject config = archetype.GenerateConfig();
          if (imageData is not null) {
            config.Add(PorterExtensions.DefaultImageFileLocationConfigOptionKey, "./texture.png");
          }

          File.WriteAllText(configFileName, config.ToString());
          createdFiles.Add(configFileName);

          return createdFiles.ToArray();
        }
      }
    }
  }
}