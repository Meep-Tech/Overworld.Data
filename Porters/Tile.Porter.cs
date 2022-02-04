using Meep.Tech.Collections;
using Meep.Tech.Data;
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
      public override string DefaultPackageName {
        get;
      } = "_tiles";

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidConfigOptionKeys
        => base.ValidConfigOptionKeys
          .Append(PixelsPerTileConfigKey)
          .Append(ImportModeConfigKey)
          .Append(SheetSizeInTilesConfigKey)
          .Append(TileHeightConfigKey);

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidImportOptionKeys 
        => base.ValidImportOptionKeys
          .Append(PixelsPerTileOption)
          .Append(ProvidedSheetDimensionsOption)
          .Append(InPlaceTileCallbackOption);

      /// <summary>
      /// Valid image extensions
      /// </summary>
      public readonly static HashSet<string> ValidImageExtensions
        = new() {
          {".png"}
        };

      /// <summary>
      /// option for pixels per tile.
      /// Takes an int.
      /// </summary>
      public const string PixelsPerTileOption
        = "PixelsPerTile";

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
      /// Key for the tile diameter in pixels value in the config
      /// </summary>
      public const string PixelsPerTileConfigKey 
        = "diameter";

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
      /// The config key for the mode used to import the image.
      /// </summary>
      public const string ImportModeConfigKey 
        = "mode";

      /// <summary>
      /// Make a new tile importer. This is made at startup.
      /// TODO: these should be singletons probably.
      /// </summary>
      public Porter(User currentUser) 
        : base(currentUser) {}

      /// <summary>
      /// Imports the archetyps, assuming the one file is an image or config.json
      /// TODO: implement just config.json upload
      /// TODO: this should return two archetypes, the background and special.
      /// 
      /// </summary>
      /// <param name="options">
      /// - PixelsPerTileOption: the pixel diameter of imported tiles.
      /// - (optional) InPlaceTileCallbackOption: Action[Vector2Int&#44; Tile.Type] executed on the imported tile, given it's location in it's image.
      /// </param>
      /// <returns></returns>
      protected override IEnumerable<Type> _importArchetypesFromExternalFile(
        string externalFileLocation,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        // if we got no image file, but got a config json:
        if(externalFileLocation.ToLower().EndsWith(".json")) {
          JObject config = TryToGetConfig(externalFileLocation.AsSingleItemEnumerable(), out _);

          resourceKey = CorrectBaseKeysAndNamesForConfigValues(
            externalFileLocation,
            ref name,
            ref packageKey,
            config
          );

          return new[]{new Tile.Type(
            name,
            packageKey,
            resourceKey,
            null,
            null,
            config.TryGetValue(TileHeightConfigKey, out JToken value)
              ? value.Value<float>()
              : null
          )};

        } // if it's just an image file:
        else if(ValidImageExtensions.Contains(Path.GetExtension(externalFileLocation).ToLower())) {
          (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations)
            = _importUnityTilesFrom(
              externalFileLocation,
              (int)options[PixelsPerTileOption],
              false,
              options.TryGetValue(ProvidedSheetDimensionsOption, out object foundDimensions)
                ? foundDimensions as Vector2Int?
                : null
            );

          int? index = null;
          if(all.Count > 0) {
            index = 0;
          }

          Dictionary<Hash128, Tile.Type> types
          = new();

          all.ForEach(tile => {
            string currentName = $"{name}{(index is not null ? $"-{++index}" : "")}";
            string currentKey = $"{resourceKey}{(index is not null ? $"-{index}" : "")}";
            types.Add(
              tile.Key,
              new Tile.Type(
                currentName,
                currentKey,
                packageKey,
                tile.Value,
                tile.Key
              ) {
                LinkArchetypeToTileDataOnSet = false
              }
            );
          });

          if(options.ContainsKey(InPlaceTileCallbackOption)) {
            locations.ForEach(e =>
              ((Action<Vector2Int, Tile.Type>)options[InPlaceTileCallbackOption]).Invoke(
                e.Key,
                types[e.Value]
              )
            );
          }

          return types.Values;
        } else throw new ArgumentException($"Invalid file type for tile upload: {Path.GetExtension(externalFileLocation)}");
      }

      /// <summary>
      /// Imports the archetyps, assuming at least one of the tiles is an image and one may be an config.json
      /// </summary>
      /// <param name="options">
      /// - PixelsPerTileOption: the pixel diameter of imported tiles.
      /// - ProvidedSheetDimensionsOption (optional): if the image is a sprite sheet, you can provide a custom number of tiles to pull from it
      /// - InPlaceTileCallbackOption (optional): Action[Vector2Int&#44; Tile.Type] executed on the imported tile, given it's location in it's image.
      /// </param>
      /// <returns></returns>
      protected override IEnumerable<Type> _importArchetypesFromExternalFiles(
        string[] externalFileLocations,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        JObject config = TryToGetConfig(externalFileLocations, out _);

        if(!config.HasValues) {
          throw new NotSupportedException($"Multi-File imports for Tiles must have a config. Animations may be implimented this way in the future.");
        }

        string tileMap = externalFileLocations.OrderBy(fileName => fileName)
          .First(fileName => ValidImageExtensions.Contains(Path.GetExtension(fileName).ToLower()));

        if(tileMap is null) {
          throw new System.ArgumentException($"Could not find a valid image file for Tile type creation in directory: {Path.GetDirectoryName((externalFileLocations.First()))}.\n Full path: {externalFileLocations.First()}");
        }

        if(config?.HasValues ?? false) {
          CorrectBaseKeysAndNamesForConfigValues(tileMap, ref packageKey, ref name, config);
        }

        int? diameter = config.TryGetValue(PixelsPerTileConfigKey, out JToken value)
          ? value.Value<int>()
          : null;

        Vector2? dimensionsInTiles = config.TryGetValue(SheetSizeInTilesConfigKey, out JToken sizeInTiles)
          ? sizeInTiles.Value<Vector2>()
          : options.TryGetValue(ProvidedSheetDimensionsOption, out object foundDimensions)
            ? foundDimensions as Vector2?
            : null;

        if(!diameter.HasValue) {
          if(dimensionsInTiles.HasValue) {
            diameter = null;
          } else
            diameter = (int)options[PixelsPerTileOption];
        }

        (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations)
          = _importUnityTilesFrom(
            tileMap,
            diameter,
            config.TryGetValue(ImportModeConfigKey, out JToken enumValue)
              && enumValue.Value<BackgroundImageImportMode>() == BackgroundImageImportMode.Individual,
            dimensionsInTiles
          );

        bool hasHeight = config.TryGetValue(TileHeightConfigKey, out JToken heightValue);

        bool hasSpecialValues = hasHeight;
        var @return = all.Select(tile =>
          new Tile.Type(
            name + (hasSpecialValues ? " (BG)" : ""),
            resourceKey,
            packageKey,
            tile.Value,
            tile.Key
          ) {
            LinkArchetypeToTileDataOnSet = false,
            _ignoreDuringModReSerialization = hasSpecialValues
          }
        );

        /// configs with special values and a background make more than one archetype.
        // One for the BG and one with the BG and other linked values.
        if(hasSpecialValues) {
          @return.Concat(all.Select(tile =>
            new Tile.Type(
              name,
              resourceKey,
              packageKey,
              tile.Value,
              tile.Key,
              heightValue?.Value<float>()
            ) {
            }
        ));
        }


        if(options.ContainsKey(InPlaceTileCallbackOption)) {
          locations.ForEach(e =>
            ((Action<Vector2Int, Tile.Type>)options[InPlaceTileCallbackOption]).Invoke(
              e.Key,
              @return.First()
            )
          );
        }

        return @return;
      }

      /// <summary>
      /// Saves each tile as it's own image with a config for import
      /// </summary>
      protected override string[] _serializeArchetypeToModFiles(Data.Tile.Type archetype, string packageDirectoryPath) {
        List<string> createdFiles = new();

        // some types are saved along with other types so we ignore them.
        if(!archetype._ignoreDuringModReSerialization) {
          //// tile needs to save the sprite, and the config.json
          /// get the image data
          Texture2D texture = archetype.DefaultBackground?.sprite.texture;
          byte[] imageData = texture?.EncodeToPNG();

          // pixels to png
          if(imageData is not null) {
            string imageFileName = Path.Combine(packageDirectoryPath, "_texture.png");
            createdFiles.Add(imageFileName);
            File.WriteAllBytes(imageFileName, imageData);
          }

          /// config
          string configFileName = Path.Combine(packageDirectoryPath, IArchetypePorter.ConfigFileName);
          createdFiles.Add(configFileName);
          JObject config = new() {
            {
              NameConfigKey,
              JToken.FromObject(archetype.Id.Name)
            }
          };

          // config image data
          if(imageData is not null) {
            config.Add(PixelsPerTileConfigKey, JToken.FromObject(archetype.DefaultBackground.sprite.pixelsPerUnit));
            config.Add(ImportModeConfigKey, JToken.FromObject(Porter.BackgroundImageImportMode.Individual));
          }

          // write the config
          File.WriteAllText(configFileName, config.ToString());
        }

        return createdFiles.ToArray();
      }

      /// <summary>
      /// Import a collection of tiles from an image location
      /// </summary>
      (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all,
        Dictionary<Vector2Int, Hash128> locations
      ) _importUnityTilesFrom(string imageLocation, int? tileWidthInPixels, bool isIndividual, Vector2? providedTileDimensions) {
        Texture2D spriteSheet = new(2, 2);
        Dictionary<Hash128, UnityEngine.Tilemaps.Tile> tileTypes
          = new();
        spriteSheet.LoadImage(File.ReadAllBytes(imageLocation));

        if(!tileWidthInPixels.HasValue) {
          if(!providedTileDimensions.HasValue) {
            tileWidthInPixels = spriteSheet.width;
          } else {
            tileWidthInPixels = (int)(spriteSheet.width / providedTileDimensions.Value.x);
          }
        }

        if(isIndividual) {
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
        if(providedTileDimensions.HasValue) {
          sheetTileDimensions = (
            (int)providedTileDimensions.Value.x, 
            (int)providedTileDimensions.Value.y
          );
        } else {
          // get how many sprites are in the sheet in the height and width dimensions
          sheetTileDimensions = (
            spriteSheet.width / tileWidthInPixels.Value,
            spriteSheet.height / tileWidthInPixels.Value
          );

          int trimX = spriteSheet.width % tileWidthInPixels.Value;
          int trimY = spriteSheet.height % tileWidthInPixels.Value;

          if(trimX > 0 || trimY > 0) {
            // WARN the user here too:
          }
        }

        (Dictionary<Hash128, UnityEngine.Tilemaps.Tile> all, Dictionary<Vector2Int, Hash128> locations) @return
          = (new(), new());

        for(int x = 0; x < sheetTileDimensions.width; x++) {
          for(int y = 0; y < sheetTileDimensions.height; y++) {
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
              new Rect(0,0, tileWidthInPixels.Value, tileWidthInPixels.Value),
              new Vector2(0.5f, 0.5f),
              tileWidthInPixels.Value
            );
            Hash128 tileHash = tile.GetTileHash();
            // see if this tile already exists in this world:
            if(@return.all.TryGetValue(tileHash, out _)) {
              ScriptableObject.Destroy(tile);
              //tile = foundLocal;
            } else if(tileTypes.TryGetValue(tileHash, out _)) {
              ScriptableObject.Destroy(tile);
              //tile = existing;
            } else 
              @return.all.Add(tileHash, tile);

            @return.locations.Add(new(x, y), tileHash);
          }
        }

        return @return;
      }
    }
  }
}

