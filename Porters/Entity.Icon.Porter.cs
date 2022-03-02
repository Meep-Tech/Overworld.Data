using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
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
      public class Porter : ArchetypePorter<Icon.Type> {

        public const string ImageFileLocationConfigOptionKey
          = "imageFile";

        /// <summary>
        /// Valid image extensions
        /// </summary>
        public readonly static HashSet<string> ValidImageExtensions
          = new() {
            { ".png" }
          };

        public override string DefaultPackageName {
          get;
        } = "_icons";

        public override HashSet<string> ValidConfigOptionKeys
          => base.ValidConfigOptionKeys
            .Append(ImageFileLocationConfigOptionKey);

        public Porter(User currentUser)
          : base(currentUser) { }

        protected override IEnumerable<Type> _importArchetypesFromExternalFile(
          string externalFileLocation,
          string resourceKey,
          string name,
          string packageKey = null,
          Dictionary<string, object> options = null
        ) {
          string extension;
          // just via a config:
          if ((extension = Path.GetExtension(externalFileLocation).ToLower()) == ".json") {
            JObject config = TryToGetConfig(externalFileLocation.AsSingleItemEnumerable(), out _);
            resourceKey = CorrectBaseKeysAndNamesForConfigValues(
              externalFileLocation,
              ref name,
              ref packageKey,
              config
            );

            Sprite sprite = null;
            string imageFile;
            if ((imageFile = config.TryGetValue<string>(ImageFileLocationConfigOptionKey)?.ToLower()) != null) {
              sprite = _getSpriteFromImageImportString(imageFile, new DirectoryInfo(externalFileLocation).FullName, options: options);
              if (sprite is null) {
                throw new ArgumentException($"Could not get a sprite from image location key: {imageFile}, relative to config: {externalFileLocation}");
              }
            } // else try to get a file with the desired name from the same folder as the config.
            else if ((imageFile = new DirectoryInfo(externalFileLocation).GetFiles($"{name}.*")
              .Where(f => ValidImageExtensions.Contains(f.Extension)).FirstOrDefault()?.FullName) != null
            ) {
              sprite = _getSpriteFromImageImportString(imageFile, importStringType: ImageImportStringType.LocalAbsolute, options: options);
            }

            return new Entity.Icon.Type(name, resourceKey, packageKey) {
              Sprite = sprite
            }.AsSingleItemEnumerable();
          }
          // if it's a valid type of image file:
          else if (ValidImageExtensions.Contains(extension)) {
            Sprite sprite = _getSpriteFromImageImportString(externalFileLocation, options: options);
            if (sprite is null) {
              throw new ArgumentException($"Could not get a sprite from image at: {externalFileLocation}");
            }
            return new Entity.Icon.Type(name, resourceKey, packageKey) {
              Sprite = sprite
            }.AsSingleItemEnumerable();
          } else throw new ArgumentException($"{externalFileLocation} is not a valid image type for an Icon. Valid types include: {string.Join(",", ValidImageExtensions)}");
        }

        protected override string[] _serializeArchetypeToModFiles(Type archetype, string packageDirectoryPath) {
          throw new System.NotImplementedException();
        }

        /// <summary>
        /// Types of image import string formats.
        /// </summary>
        public enum ImageImportStringType {
          LocalAbsolute = 0,
          LocalRelative = 1,
          Http = 2
        }

        internal static bool _isValidImageImportString(string imageFileLocationString, out ImageImportStringType? importStringType) {
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
        /// Gets a local file location for a given image import string.
        /// </summary>
        /// <param name="imageFileLocationString">The image path</param>
        /// <param name="accessedFromFolder">The folder this is centered on, in case it's local.</param>
        static Sprite _getSpriteFromImageImportString(string imageFileLocationString, string accessedFromFolder = null, ImageImportStringType? importStringType = null, Dictionary<string, object> options = null) {
          string absoluteImageFilePath = null;
          if (importStringType is not null || _isValidImageImportString(imageFileLocationString, out importStringType)) {
            switch (importStringType) {
              // get the image from the web!
              case ImageImportStringType.Http:
                options[IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting]
                  = false;
                var extension = Path.GetExtension(imageFileLocationString);
                // if it's a valid extension, download it as the image
                if (ValidImageExtensions.Contains(extension)) {
                  using (Stream stream = new WebClient().OpenRead(imageFileLocationString)) {
                    using (var memoryStream = new MemoryStream()) {
                      stream.CopyTo(memoryStream);
                      Texture2D texture = new(2, 2);
                      texture.LoadImage(memoryStream.ToArray());
                      return Sprite.Create(
                        texture,
                        new Rect(
                          0,
                          0,
                          texture.width,
                          texture.height
                        ),
                        new Vector2(0.5f, 0.5f)
                      // TODO: implement pixels per unity adjustment for world size options and config options
                      );
                    }
                  }
                }

                throw new System.ArgumentException($"Invalid file extension: {extension}");

              /// These ones will copy the file to the new desired location.
              //relative to the current folder
              case ImageImportStringType.LocalRelative:
                absoluteImageFilePath = Path.Combine(accessedFromFolder, absoluteImageFilePath);
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
      }
    }
  }
}