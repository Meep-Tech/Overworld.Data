using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make height map tiles.
/// </summary>
[Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
public class BasicHeightMapTile : Overworld.Data.Tile.Type {

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  public static Collection.Branch<BasicHeightMapTile> Types {
    get;
  } = new();

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  public static Dictionary<float, BasicHeightMapTile> TypesByHeight {
    get;
  } = new();

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  public override bool UseDefaultBackgroundAsInWorldTileImage
    => false;

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  public override bool LinkArchetypeToTileDataOnSet
    => false;

  /// <summary>
  /// Make a base height tile for a new height value.
  /// This will throw if you try to create type for a height that already has a type.
  /// </summary>
  public BasicHeightMapTile(
    float heightValue,
    UnityEngine.Tilemaps.Tile backgroundImage
  ) : base(
    $"{(heightValue != 0 ? Mathf.Abs(heightValue).ToString() + "t " : "Sea Level")} {(heightValue > 0 ? "Above Sea Level" : heightValue == 0 ? "" : heightValue < 0 ? "Below Sea Level" : "")}",
    $"BasicHeightSetter-{heightValue}",
    $"_base",
    Newtonsoft.Json.Linq.JObject.FromObject(new Dictionary<string, object> {
      {Overworld.Data.Tile.Porter.TileHeightConfigKey, heightValue }
    }),
    new Dictionary<string, object> {
      {nameof(Overworld.Data.Tile.Type.DefaultBackground), backgroundImage },
      {nameof(Overworld.Data.Tile.Type.BackgroundImageHashKey), backgroundImage?.sprite.texture.imageContentsHash },
    }
  ) {
    Description = $"Used to set just a tile's height to {Id.Name}; {heightValue}t.";
    TypesByHeight.Add(heightValue, this);
    Types.Add(this);
  }
}