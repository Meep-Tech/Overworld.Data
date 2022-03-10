using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Overworld.Data.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {

  public partial struct Tile {

    /// <summary>
    /// Used to import and export tile types.
    /// </summary>
    public class Porter : ArchetypePorter<Tile.Type> {

      /// <summary>
      /// The config specified import mode
      /// </summary>
      [JsonConverter(typeof(StringEnumConverter))]
      public enum BackgroundImageImportMode {
        Individual,
        Sheet,
        Animation // NOT IMPLIMENTED
      }

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public override string SubFolderName {
        get;
      } = "_tiles";

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidConfigOptionKeys
        => base.ValidConfigOptionKeys
          .Append(PorterExtensions.PixelsPerTileConfigKey)
          .Append(DescriptionConfigKey)
          .Append(ImportModeConfigKey)
          .Append(SheetSizeInTilesConfigKey)
          .Append(TagsConfigOptionKey)
          .Append(UseDefaultBackgroundAsInWorldTileImageConfigKey)
          .Append(TileHeightConfigKey);

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidImportOptionKeys
        => base.ValidImportOptionKeys
          .Append(PorterExtensions.PixelsPerTileImportOptionKey)
          .Append(ProvidedSheetDimensionsOption)
          .Append(InPlaceTileCallbackOption);

      /// <summary>
      /// option for dimensions in tiles instead of pixels during import.
      /// Takes an int.
      /// </summary>
      public const string ProvidedSheetDimensionsOption
        = "DimensionsInTiles";

      /// <summary>
      /// Takes an Action[Vector2Int, UnityEngine.Tilemaps.Tile] with the params:
      ///   tile location in it's tilemap
      ///   the generated unity tile
      /// </summary>
      public const string InPlaceTileCallbackOption
        = "TileLocationCallback";

      /// <summary>
      /// Key for the tile height
      /// </summary>
      public const string TileHeightConfigKey
        = "height";

      /// <summary>
      /// Key used to pass in how large the tile sheet is in tiles
      /// </summary>
      public const string SheetSizeInTilesConfigKey
        = "sizeInTiles";

      /// <summary>
      /// Key used to pass in how large the tile sheet is in tiles
      /// </summary>
      public const string UseDefaultBackgroundAsInWorldTileImageConfigKey
        = "useDefaultBackgroundAsInWorldTileImage";

      /// <summary>
      /// The config key for the mode used to import the image.
      /// </summary>
      public const string ImportModeConfigKey
        = "mode";

      /// <summary>
      /// Make a new tile importer. This is made at startup.
      /// TODO: these should be singletons probably.
      /// </summary>
      public Porter(User currentUser)
        : base(() => currentUser.UniqueName) { }

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> BuildLooselyFromConfig(JObject config, IEnumerable<string> assetFiles, Dictionary<string, object> options, out IEnumerable<string> processedFiles) {
        string imageFile = this.GetDefaultImageFromAssets(assetFiles, options, config);
        Texture2D webTexture = null;
        if (this.IsValidImageImportString(imageFile, out ImageImportStringType? type)) {
          if (type == ImageImportStringType.LocalRelative) {
            imageFile = this.ExpandImageImportString(imageFile, Path.GetDirectoryName(assetFiles.First()));
          }
          else if (type == ImageImportStringType.Http) {
            webTexture = this.GetTextureFromHttpImportString(imageFile);
          }
        }

        (string resourceName, string packageName, string resourceKey)
          = ConstructArchetypeKeys(imageFile ?? assetFiles.First(), options, config);

        int? diameter = config.TryGetValue<int?>(PorterExtensions.PixelsPerTileConfigKey);

        Vector2? dimensionsInTiles = config.TryGetValue<Vector2?>(SheetSizeInTilesConfigKey)
          ?? (options.TryGetValue(ProvidedSheetDimensionsOption, out object foundDimensions)
            ? foundDimensions as Vector2?
            : null);

        if (!diameter.HasValue) {
          if (dimensionsInTiles.HasValue) {
            diameter = null;
          }
          else
            diameter = (int)options[PorterExtensions.PixelsPerTileImportOptionKey];
        }

        (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations)
          = webTexture is null
          ? _importUnityTilesFrom(
              imageFile,
              diameter,
              config.TryGetValue(ImportModeConfigKey, StringComparison.OrdinalIgnoreCase, out JToken enumValue)
                && enumValue.Value<BackgroundImageImportMode>() == BackgroundImageImportMode.Individual,
              dimensionsInTiles
            )
          : _importUnityTilesFrom(
              webTexture,
              diameter,
              config.TryGetValue(ImportModeConfigKey, StringComparison.OrdinalIgnoreCase, out JToken enumValue2)
                && enumValue2.Value<BackgroundImageImportMode>() == BackgroundImageImportMode.Individual,
              dimensionsInTiles
            );

        int? index = null;
        if (all.Count > 1) {
          index = 0;
        }

        /// check if there are "special" tile values that need to be added to another type too.
        bool hasHeight = config.HasProperty(TileHeightConfigKey, out string heightKey);
        bool hasSpecialValues = hasHeight;
        JObject specialConfig = hasSpecialValues ? JObject.Parse(config.ToString()) : null;
        if (hasSpecialValues) {
          config.Remove(heightKey);
        }

        // One for the bg
        Dictionary<string, Type> @return = all.ToDictionary(tile => tile.Key.ToString() + (hasSpecialValues ? " (BG)" : ""), tile => {
          Dictionary<string, object> localOptions = new(options ?? new());
          localOptions.Add(nameof(Type.DefaultBackground), tile.Value);
          localOptions.Add(nameof(Type.BackgroundImageHashKey), tile.Key);
          localOptions.Add(nameof(Type.LinkArchetypeToTileDataOnSet), false);
          localOptions.Add(nameof(Type._ignoreDuringModReSerialization), false);
          return BuildArchetypeFromCompiledData(
            resourceName + (hasSpecialValues ? " (BG)" : "") + (index is not null ? $" - {++index}" : ""),
            packageName,
            resourceKey + (index is not null ? $" - {index}" : ""),
            config,
            localOptions,
            Universe
          ).First();
        });

        if (all.Count > 1) {
          index = 0;
        }

        /// configs with special values and a background make more than one archetype.
        // and one with the BG and other linked values.
        if (hasSpecialValues) {
          @return.Merge(all.ToDictionary(tile => tile.Key.ToString(), tile => {
            Dictionary<string, object> localOptions = new(options ?? new());
            localOptions.Add(nameof(Type.DefaultBackground), tile.Value);
            localOptions.Add(nameof(Type.BackgroundImageHashKey), tile.Key);
            localOptions.Add(nameof(Type.LinkArchetypeToTileDataOnSet), true);
            return BuildArchetypeFromCompiledData(
              resourceName + (index is not null ? $" - {++index}" : ""),
              packageName,
              resourceKey + (index is not null ? $" - {index}" : ""),
              specialConfig,
              localOptions,
              Universe
            ).First();
          }));
        }

        if (options.ContainsKey(InPlaceTileCallbackOption)) {
          locations.ForEach(e =>
            ((Action<Vector2Int, Tile.Type>)options[InPlaceTileCallbackOption]).Invoke(
              e.Key,
              @return[e.Value.ToString()]
            )
          );
        }

        processedFiles = new[] {
          imageFile,
          assetFiles.First()
        }.ToHashSet();
        return @return.Values;
      }

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> BuildLooselyFromAssets(IEnumerable<string> assetFiles, Dictionary<string, object> options, out IEnumerable<string> processedFiles) {
        string imageFile = this.GetDefaultImageFromAssets(assetFiles, options)
          ?? throw new ArgumentException($"No default image file found. Tiles with no config require a valid default image");
        Texture2D webTexture = null;
        if (this.IsValidImageImportString(imageFile, out ImageImportStringType? type)) {
          if (type == ImageImportStringType.LocalRelative) {
            imageFile = this.ExpandImageImportString(imageFile, Path.GetDirectoryName(assetFiles.First()));
          }
          else if (type == ImageImportStringType.Http) {
            webTexture = this.GetTextureFromHttpImportString(imageFile);
          }
        }

        (string resourceName, string packageName, string resourceKey)
          = ConstructArchetypeKeys(imageFile, options, null);

        Vector2? dimensionsInTiles = options.TryGetValue(ProvidedSheetDimensionsOption, out object foundDimensions)
          ? foundDimensions as Vector2?
          : null;

        int? diameter;
        if (dimensionsInTiles.HasValue) {
          diameter = null;
        }
        else
          diameter = (int)options[PorterExtensions.PixelsPerTileImportOptionKey];

        (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations)
          = webTexture is null
          ? _importUnityTilesFrom(
              imageFile,
              diameter,
              false,
              dimensionsInTiles
            )
          : _importUnityTilesFrom(
              webTexture,
              diameter,
              false,
              dimensionsInTiles
            );

        int? index = null;
        if (all.Count > 1) {
          index = 0;
        }

        /// build all the tiles
        var emptyConfig = new JObject();
        Dictionary<Hash128, Type> @return = all.ToDictionary(tile => tile.Key, tile => {
          Dictionary<string, object> localOptions = new(options ?? new());
          localOptions.Add(nameof(Type.DefaultBackground), tile.Value);
          localOptions.Add(nameof(Type.BackgroundImageHashKey), tile.Key);
          localOptions.Add(nameof(Type.LinkArchetypeToTileDataOnSet), false);
          return BuildArchetypeFromCompiledData(
            resourceName + (index is not null ? $" - {++index}" : ""),
            packageName,
            resourceKey + (index is not null ? $" - {index}" : ""),
            emptyConfig,
            localOptions,
            Universe
          ).First();
        });

        if (options.ContainsKey(InPlaceTileCallbackOption)) {
          locations.ForEach(e =>
            ((Action<Vector2Int, Tile.Type>)options[InPlaceTileCallbackOption]).Invoke(
              e.Key,
              @return[e.Value]
            )
          );
        }

        processedFiles = new[] {
          imageFile,
          assetFiles.First()
        }.ToHashSet();
        return @return.Values;
      }

      /// <summary>
      /// Saves each tile as it's own image with a config for import
      /// </summary>
      protected override string[] SerializeArchetypeToModFiles(Data.Tile.Type archetype, string packageDirectoryPath) {
        List<string> createdFiles = new();

        // some types are saved along with other types so we ignore them.
        if (!archetype._ignoreDuringModReSerialization) {
          Directory.CreateDirectory(packageDirectoryPath);
          //// tile needs to save the sprite, and the config.json
          /// get the image data
          Texture2D texture = archetype.DefaultBackground?.sprite.texture;
          byte[] imageData = texture?.EncodeToPNG();

          // pixels to png
          string imageFileName;
          if (imageData is not null) {
            imageFileName = Path.Combine(packageDirectoryPath, "texture.png");
            File.WriteAllBytes(imageFileName, imageData);
            createdFiles.Add(imageFileName);
          }

          /// config
          string configFileName = Path.Combine(packageDirectoryPath, DefaultConfigFileName);
          JObject config = archetype.GenerateConfig();
          if (imageData is not null) {
            config.Add("imageFile", "./texture.png");
          }

          // write the config
          File.WriteAllText(configFileName, config.ToString());
          createdFiles.Add(configFileName);
        }

        return createdFiles.ToArray();
      }

      /// <summary>
      /// Import a collection of tiles from an image
      /// </summary>
      (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all,
        Dictionary<Vector2Int, Hash128> locations
      ) _importUnityTilesFrom(Texture2D spriteSheet, int? tileWidthInPixels, bool isIndividual, Vector2? providedTileDimensions) {
        if (!tileWidthInPixels.HasValue) {
          if (!providedTileDimensions.HasValue) {
            tileWidthInPixels = spriteSheet.width;
          }
          else {
            tileWidthInPixels = (int)(spriteSheet.width / providedTileDimensions.Value.x);
          }
        }

        if (isIndividual) {
          UnityEngine.Tilemaps.Tile tile = new() {
            sprite = Sprite.Create(
                spriteSheet,
                new Rect(
                  0, 0, tileWidthInPixels.Value, tileWidthInPixels.Value
                ),
                new Vector2(0.5f, 0.5f),
                tileWidthInPixels.Value
              )
          };

          return (new Dictionary<Hash128, UnityEngine.Tilemaps.Tile> {
            { tile.GetTileHash(), tile }
          }, new Dictionary<Vector2Int, Hash128> {
            {new Vector2Int(0,0),  tile.GetTileHash()}
          });
        }

        (int width, int height) sheetTileDimensions;
        if (providedTileDimensions.HasValue) {
          sheetTileDimensions = (
            (int)providedTileDimensions.Value.x,
            (int)providedTileDimensions.Value.y
          );
        }
        else {
          // get how many sprites are in the sheet in the height and width dimensions
          sheetTileDimensions = (
            spriteSheet.width / tileWidthInPixels.Value,
            spriteSheet.height / tileWidthInPixels.Value
          );

          int trimX = spriteSheet.width % tileWidthInPixels.Value;
          int trimY = spriteSheet.height % tileWidthInPixels.Value;

          if (trimX > 0 || trimY > 0) {
            // WARN the user here too:
          }
        }

        (Dictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations) @return
          = (new(), new());

        for (int x = 0; x < sheetTileDimensions.width; x++) {
          for (int y = 0; y < sheetTileDimensions.height; y++) {
            Texture2D subMap = new(tileWidthInPixels.Value, tileWidthInPixels.Value);
            Color[] colors = spriteSheet.GetPixels(
              x * tileWidthInPixels.Value,
              y * tileWidthInPixels.Value,
              tileWidthInPixels.Value,
              tileWidthInPixels.Value
            );
            subMap.SetPixels(
              colors
            );
            subMap.Apply();

            UnityEngine.Tilemaps.Tile tile
              = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            tile.sprite = Sprite.Create(
              subMap,
              new Rect(0, 0, tileWidthInPixels.Value, tileWidthInPixels.Value),
              new Vector2(0.5f, 0.5f),
              tileWidthInPixels.Value
            );
            Hash128 tileHash = tile.GetTileHash();
            // see if this tile already exists in this world:
            if (@return.all.TryGetValue(tileHash, out _)) {
              ScriptableObject.Destroy(tile);
              //tile = foundLocal;
            }
            else if (@return.all.TryGetValue(tileHash, out _)) {
              ScriptableObject.Destroy(tile);
              //tile = existing;
            }
            else
              @return.all.Add(tileHash, tile);

            @return.locations.Add(new(x, y), tileHash);
          }
        }

        return @return;
      }

      /// <summary>
      /// Import a collection of tiles from an image location
      /// </summary>
      (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all,
        Dictionary<Vector2Int, Hash128> locations
      ) _importUnityTilesFrom(string imageLocation, int? tileWidthInPixels, bool isIndividual, Vector2? providedTileDimensions) {
        Texture2D spriteSheet = new(2, 2);

        var imageFile = new System.IO.FileInfo(imageLocation);
        if (imageFile.Exists) {
          byte[] imageBytes = File.ReadAllBytes(imageFile.FullName);
          spriteSheet.LoadImage(imageBytes);
        }
        else throw new ArgumentException($"Could not find file:{imageLocation}");

        return _importUnityTilesFrom(spriteSheet, tileWidthInPixels, isIndividual, providedTileDimensions);
      }
    }
  }
}

