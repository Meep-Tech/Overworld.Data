using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Data {

  /// <summary>
  /// A Game world, consisting of multuiple tile boards
  /// </summary>
  public partial class World {

    /// <summary>
    /// The 4 cardinal directions.
    /// </summary>
    public enum CardinalDirection {
      North,
      East,
      South,
      West
    }

    /// <summary>
    /// The 4 cardinal corner directions.
    /// </summary>
    public enum CardinalCorner {
      NorthWest,
      NorthEast,
      SouthEast,
      SouthWest
    }

    /// <summary>
    /// The world origin of tileboards in (XZ) space
    /// </summary>
    public static Vector2Int Origin
      = Vector2Int.zero;

    /// <summary>
    /// The 4 cardinal directions offsets
    /// </summary>
    public readonly static Dictionary<CardinalDirection, Vector2Int> CardinalOffsets
      = new() {
        {
          CardinalDirection.North,
          new Vector2Int(0, 1)
        },
        {
          CardinalDirection.East,
          new Vector2Int(1, 0)
        },
        {
          CardinalDirection.South,
          new Vector2Int(0, -1)
        },
        {
          CardinalDirection.West,
          new Vector2Int(-1, 0)
        }
      };

    /// <summary>
    /// The owner-set options for this world.
    /// </summary>
    public Settings Options {
      get;
    } = new();

    /// <summary>
    /// All of the entities in the current world by id
    /// </summary>
    public IReadOnlyDictionary<string, Entity> Entities
      => _entities;
    readonly Dictionary<string, Entity> _entities
      = new();

    /// <summary>
    /// The editor specific data.
    /// This should be lazy loaded when the editor is opened for a world.
    /// </summary>
    public Dictionary<string, TileBoard> Boards {
      get;
    } = new();

    /// <summary>
    /// The world boundaries
    /// </summary>
    public (Vector2Int minBottomLeft, Vector2Int maxTopRight) Bounds {
      get => _bounds ??= _calculateWorldBounds();
    }
    (Vector2Int minBottomLeft, Vector2Int maxTopRight)? _bounds;

    (Vector2Int minBottomLeft, Vector2Int maxTopRight) _calculateWorldBounds()
      => (
        new(
          Origin.x - Options.Dimensions.x / 2,
          Origin.y - Options.Dimensions.y / 2),
        new(
          Origin.x + Options.Dimensions.x / 2,
          Origin.y + Options.Dimensions.y / 2
        )
      );
  }
}