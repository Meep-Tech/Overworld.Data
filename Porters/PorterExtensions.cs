using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Overworld.Data.IO {

  /// <summary>
  /// Types of image import string formats.
  /// </summary>
  public enum ImageImportStringType {
    /// <summary>
    /// The file string is absolute in the local filesystem
    /// </summary>
    LocalAbsolute = 0,
    /// <summary>
    /// The file string is relative to the config file in the local filesystem
    /// </summary>
    LocalRelative = 1,
    /// <summary>
    /// The file string is an http location on the net.
    /// </summary>
    Http = 2
  }

  /// <summary>
  /// Extra values and functions for Archetype Porters
  /// </summary>
  public static class PorterExtensions {

    /// <summary>
    /// Json config key for the image file to use for this icon.
    /// </summary>
    public const string DefaultImageFileLocationConfigOptionKey
      = "defaultImageFile";

    /// <summary>
    /// json config Key for the tile diameter in pixels per tile for size calculation 
    /// </summary>
    public const string PixelsPerTileConfigKey
      = "pixelsPerTileDiameter";

    /// <summary>
    /// import options Key for the tile diameter in pixels per tile for size calculation 
    /// </summary>
    public const string PixelsPerTileImportOptionKey
      = "PixelsPerTileDiameter";

    /// <summary>
    /// Valid image extensions
    /// </summary>
    public readonly static HashSet<string> ValidImageExtensions
      = new() {
        { ".png" }
      };

    /// <summary>
    /// Helper to the default image from the assets based on the config, options, and valid image extension values.
    /// </summary>
    public static string GetDefaultImageFromAssets(this ArchetypePorter porter, IEnumerable<string> assetFiles, Dictionary<string, object> options, JObject config = null) {
      string imageFile;

      if (config == null) {
        if ((imageFile = assetFiles.Where(f => ValidImageExtensions.Contains(Path.GetExtension(f))).FirstOrDefault()) != null) {
          return imageFile;
        }
        else return null;
      }
      else {
        if ((imageFile = config.TryGetValue<string>(DefaultImageFileLocationConfigOptionKey)?.ToLower()) != null) {
          return imageFile;
        } // else try to get a file from the provided assets with the correct name.
        else if (config.ContainsKey(ArchetypePorter.NameConfigKey) && (imageFile = assetFiles.Where(f => f.StartsWith($"{config.GetValue<string>(ArchetypePorter.NameConfigKey)}."))
          .Where(f => ValidImageExtensions.Contains(Path.GetExtension(f))).FirstOrDefault()) != null
        ) {
          return imageFile;
        } // else try to get a file with the same name as the config itself.
        else if ((imageFile = _tryToGetAssetWithTheSameNameAsConfig(assetFiles.First(), assetFiles)) != null) {
          return imageFile;
        } // else try to get a file from the provided assets with the correct extension only.
        else if ((imageFile = assetFiles.Where(f => ValidImageExtensions.Contains(Path.GetExtension(f))).FirstOrDefault()) != null) {
          return imageFile;
        }
      }
      
      return null;
    }

    static string _tryToGetAssetWithTheSameNameAsConfig(string configFile, IEnumerable<string> assetFiles) {
      string configFileName = Path.GetFileNameWithoutExtension(configFile);
      foreach (string file in assetFiles) {
        if (Path.GetFileName(file).StartsWith(configFileName + ".")) {
          if (ValidImageExtensions.Contains(Path.GetExtension(file))) {
            return file;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Check if an image import string is valid
    /// </summary>
    /// <param name="imageFileLocationString"></param>
    /// <param name="importStringType"></param>
    /// <returns></returns>
    public static bool IsValidImageImportString(this ArchetypePorter porter, string imageFileLocationString, out ImageImportStringType? importStringType) {
      // get the image from the web!
      if (imageFileLocationString.StartsWith("https:") || imageFileLocationString.StartsWith("http:")) {
        importStringType = ImageImportStringType.Http;
        return true;
      } //relative to the current folder
      else if (imageFileLocationString.StartsWith("./")) {
        importStringType = ImageImportStringType.LocalRelative;
        return true;
      } // relative to a parent folder
      else if (imageFileLocationString.StartsWith("../")) {
        importStringType = ImageImportStringType.LocalRelative;
        return true;
      } // try to get the absolute file path
      else if (imageFileLocationString[0..6].Contains(":") || imageFileLocationString.StartsWith("/")) {
        importStringType = ImageImportStringType.LocalAbsolute;
        return true;
      }

      importStringType = null;
      return false;
    }

    /// <summary>
    /// Expad an image import string to a full file location.
    /// </summary>
    public static string ExpandImageImportString(this ArchetypePorter porter, string imageFileLocationString, string accessedFromFolder = null, ImageImportStringType? importStringType = null) {
      if (importStringType is not null || porter.IsValidImageImportString(imageFileLocationString, out importStringType)) {
        return importStringType switch {
          // get the image from the web or absolute
          ImageImportStringType.Http or ImageImportStringType.LocalAbsolute => imageFileLocationString,
          // relative to the current folder
          ImageImportStringType.LocalRelative => Path.GetFullPath(Path.Combine(accessedFromFolder, imageFileLocationString)),
          _ => throw new Exception(),
        };
      } else throw new Exception();
    }

    /// <summary>
    /// Gets a local file location for a given image import string.
    /// </summary>
    /// <param name="imageFileLocationString">The image path</param>
    /// <param name="accessedFromFolder">The folder this is centered on, in case it's local.</param>
    public static Sprite GetSpriteFromImageImportString(this ArchetypePorter porter, string imageFileLocationString, string accessedFromFolder = null, ImageImportStringType? importStringType = null, Dictionary<string, object> options = null) {
      string absoluteImageFilePath = null;
      if (importStringType is not null || porter.IsValidImageImportString(imageFileLocationString, out importStringType)) {
        switch (importStringType) {
          // get the image from the web!
          case ImageImportStringType.Http:
            var texture = porter.GetTextureFromHttpImportString(imageFileLocationString);
            return Sprite.Create(
              texture,
              new Rect(
                0,
                0,
                texture.width,
                texture.height
              ),
              new Vector2(0.5f, 0.5f),
              (int)options[PorterExtensions.PixelsPerTileImportOptionKey]
            );

          /// These ones will copy the file to the new desired location.
          //relative to the current folder
          case ImageImportStringType.LocalRelative:
            absoluteImageFilePath = Path.GetFullPath(Path.Combine(accessedFromFolder, absoluteImageFilePath));
            break;

          // try to get the absolute file path
          case ImageImportStringType.LocalAbsolute:
            absoluteImageFilePath = imageFileLocationString;
            break;
        }

        Texture2D iconTexture = new(2, 2);
        iconTexture.LoadImage(File.ReadAllBytes(absoluteImageFilePath));
        return Sprite.Create(
          iconTexture,
          new Rect(
            0,
            0,
            iconTexture.width,
            iconTexture.height
          ),
          new Vector2(0.5f, 0.5f)
        // TODO: implement pixels per unity adjustment for world size options and config options
        );
      }

      return null;
    }

    /// <summary>
    /// Get a unity texture 2d from an import string of type http.
    /// </summary>
    public static Texture2D GetTextureFromHttpImportString(this ArchetypePorter porter, string imageFileLocationString) {
      var extension = Path.GetExtension(imageFileLocationString);
      // if it's a valid extension, download it as the image
      if (PorterExtensions.ValidImageExtensions.Contains(extension)) {
        using (Stream stream = new WebClient().OpenRead(imageFileLocationString)) {
          using (var memoryStream = new MemoryStream()) {
            stream.CopyTo(memoryStream);
            Texture2D texture = new(2, 2);
            texture.LoadImage(memoryStream.ToArray());

            return texture;
          }
        }
      }

      throw new System.ArgumentException($"Invalid file extension: {extension}");
    }
  }
}
