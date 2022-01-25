using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {

  /// <summary>
  /// A collection of tiles for a board.
  /// </summary>
  public class TileBoard: IEnumerable<(Vector2Int location, Tile data)> {

    /// <summary>
    /// The height/depth for tiles that are just bottomless pits/nothing/etc.
    /// </summary>
    public const float PitDepth = -666.666f;

    /// <summary>
    /// The dimensions of the board, in tiles.
    /// </summary>
    public Vector2Int Dimensions {
      get;
    }

    /// <summary>
    /// The tile board boundaries
    /// </summary>
    public (Vector2Int minBottomLeft, Vector2Int maxTopRight) Bounds {
      get => _bounds ??= _calculateBoardBounds();
    } (Vector2Int minBottomLeft, Vector2Int maxTopRight)? _bounds;

    /// <summary>
    /// Required tile archetypes needed to load this board.
    /// </summary>
    public HashSet<Tile.Type> RequiredTileTypes {
      get => _requiredTileTypes;
    } HashSet<Tile.Type> _requiredTileTypes
      = new();

    /// <summary>
    /// The raw tile data for this board
    /// </summary>
    Dictionary<Vector2Int, Tile> _tiles
      = new();

    /// <summary>
    /// Get a tile via world location from above
    /// </summary>
    public Tile? this[Vector2Int tileWorldLocation] {
      get => _tiles.TryGetValue(tileWorldLocation, out var found)
        ? found
        : null;
      set {
        if(value.HasValue)
          _tiles[tileWorldLocation] = value.Value;
        else
          _tiles.Remove(tileWorldLocation);
      }
    }

    /// <summary>
    /// Get a tile via world location from above
    /// </summary>
    public Tile? this[int x, int y_z] {
      get => _tiles.TryGetValue(new Vector2Int(x, y_z), out var found)
        ? found
        : null;
      set {
        if(value.HasValue)
          _tiles[new Vector2Int(x, y_z)] = value.Value;
        else
          _tiles.Remove(new Vector2Int(x, y_z));
      }
    }

    /// <summary>
    /// Create a new tileboard of the given size.
    /// </summary>
    public TileBoard(Vector2Int dimensions) {
      Dimensions = dimensions;
    }

    public IEnumerator<(Vector2Int location, Tile data)> GetEnumerator()
      => _tiles.Select(e => (e.Key, e.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    (Vector2Int minBottomLeft, Vector2Int maxTopRight) _calculateBoardBounds()
      => (
        // TODO: fix bounds with an un-even X or Y value (subtract 1 from the even dimensions)
        new(
          0 - Dimensions.x / 2,
          0 - Dimensions.y / 2),
        new(
          Dimensions.x / 2 - 1,
          Dimensions.y / 2 - 1
        )
      );
  }
}