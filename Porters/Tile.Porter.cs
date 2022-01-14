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
      /// Takes an Action<Vector2Int, UnityEngine.Tilemaps.Tile> with the params:
      ///   tile location in it's tilemap
      ///   the generated unity tile
      /// </summary>
      public const string InPlaceTileCallbackOption
        = "TileLocationCallback";

      /// <summary>
      /// Key for the name value in the config
      /// </summary>
      private const string NameConfigKey = "name";

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public override string DefaultPackageName {
        get;
      } = "-Tiles";

      public Porter(User currentUser) 
        : base(currentUser) {}

      protected override IEnumerable<Type> _importArchetypesFromExternalFile(
        string externalFileLocation,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, UnityEngine.Tilemaps.Tile[,] inPlace)
          = _importUnityTilesFrom(
            externalFileLocation,
            (int)options[PixelsPerTileOption],
            false,
            options.TryGetValue(ProvidedSheetDimensionsOption, out object foundDimensions)
              ? foundDimensions as Vector2?
              : null
          );

        if(options.ContainsKey(InPlaceTileCallbackOption)) {
          for(int x = 0; x < inPlace.GetLength(0); x++) {
            for(int y = 0; y < inPlace.GetLength(1); y++) {
              ((Action<Vector2Int, UnityEngine.Tilemaps.Tile>)options[InPlaceTileCallbackOption]).Invoke(
                new Vector2Int(x, y),
                inPlace[x, y]
              );
            }
          }
        }

        int? index = null;
        if(all.Count > 0) {
          index = 0;
        }

        return all.Select(tile => {
          string currentName = $"{name}{(index is not null ? $"-{++index}" : "")}";
          string currentKey = $"{resourceKey}{(index is not null ? $"-{index}" : "")}";
          return new Tile.Type(
            currentName,
            currentKey,
            packageKey,
            tile.Value,
            tile.Key
          );
        });
      }

      protected override IEnumerable<Type> _importArchetypesFromExternalFiles(
        string[] externalFileLocations,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        string configFile = externalFileLocations.FirstOrDefault(fileName => fileName == IPorter.ConfigFileName);
        if(configFile is null) {
          throw new NotSupportedException($"Multi-File imports for Tiles must have a config. Animations may be implimented this way in the future.");
        }

        JObject config = JObject.Parse(
          File.ReadAllText(configFile)
        );

        string tileMap = externalFileLocations.OrderBy(fileName => fileName)
          .First(fileName => fileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase));

        int? diameter = config.TryGetValue("diameter", out JToken value)
          ? value.Value<int>()
          : null;

        Vector2? dimensionsInTiles = config.TryGetValue("sizeInTiles", out JToken sizeInTiles)
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

        (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all, UnityEngine.Tilemaps.Tile[,] inPlace)
          = _importUnityTilesFrom(
            tileMap,
            diameter,
            config.TryGetValue("mode", out JToken enumValue)
              && enumValue.Value<BackgroundImageImportMode>() == BackgroundImageImportMode.Individual,
            dimensionsInTiles
          );

        if(options.ContainsKey(InPlaceTileCallbackOption)) {
          for(int x = 0; x < inPlace.GetLength(0); x++) {
            for(int y = 0; y < inPlace.GetLength(1); y++) {
              ((Action<Vector2Int, UnityEngine.Tilemaps.Tile>)options[InPlaceTileCallbackOption]).Invoke(
                new Vector2Int(x, y),
                inPlace[x, y]
              );
            }
          }
        }

        return all.Select(tile =>
          new Tile.Type(
            config.TryGetValue(NameConfigKey, out JToken value)
              ? value.Value<string>()
              : name,
            resourceKey,
            packageKey,
            tile.Value,
            tile.Key
          )
        );
      }

      protected override string[] _serializeArchetypeToModFiles(Data.Tile.Type archetype, string packageDirectoryPath) {
        // tile needs to save the sprite, and the config.json
        List<string> createdFiles = new List<string>();
        Texture2D texture = archetype.DefaultBackground?.sprite.texture;
        byte[] imageData = texture?.EncodeToPNG();

        if(imageData is not null) {
          string imageFileName = Path.Combine(packageDirectoryPath, "_texture.png");
          createdFiles.Add(imageFileName);
          File.WriteAllBytes(imageFileName, imageData);
        }

        string configFileName = Path.Combine(packageDirectoryPath, IPorter.ConfigFileName);
        createdFiles.Add(configFileName);
        JObject config = new() {
          { NameConfigKey, JToken.FromObject(archetype.Id.Name) }
        };

        if(imageData is not null) {
          config.Add($"diameter", JToken.FromObject(archetype.DefaultBackground.sprite.pixelsPerUnit));
          config.Add($"mode", JToken.FromObject(Porter.BackgroundImageImportMode.Individual));
        }

        return createdFiles.ToArray();
      }

      /// <summary>
      /// Import a collection of tiles from an image location
      /// </summary>
      (IReadOnlyDictionary<Hash128, UnityEngine.Tilemaps.Tile> all,
        UnityEngine.Tilemaps.Tile[,] inPlace
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
          }, new UnityEngine.Tilemaps.Tile[,] { 
            { tile} 
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

        (Dictionary<Hash128, UnityEngine.Tilemaps.Tile> all, UnityEngine.Tilemaps.Tile[,] inPlace) @return
          = (new(), new UnityEngine.Tilemaps.Tile[sheetTileDimensions.width,sheetTileDimensions.height]);

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
            if(@return.all.TryGetValue(tileHash, out UnityEngine.Tilemaps.Tile foundLocal)) {
              //ScriptableObject.Destroy(tile);
              tile = foundLocal;
            } else if(tileTypes.TryGetValue(tileHash, out UnityEngine.Tilemaps.Tile existing)) {
              //ScriptableObject.Destroy(tile);
              tile = existing;
            } else 
              @return.all.Add(tileHash, tile);

            @return.inPlace[x, y] = tile;
          }
        }

        return @return;
      }
    }
  }
}

