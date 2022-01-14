using Meep.Tech.Data;

namespace Overworld.Data {
  public partial struct Tile : IModel<Tile, Tile.Type>, IModel.IUseDefaultUniverse {

    /// <summary>
    /// The archetype originally used to make this tile.
    /// A tile can be modified around it, and then reset to it as well.
    /// </summary>
    public Type Archetype {
      get => _archetype;
      set => _initializeFor(value);
    } Type _archetype;

    /// <summary>
    /// The background tile this tile is using
    /// </summary>
    public UnityEngine.Tilemaps.Tile Background {
      get;
      private set;
    }

    /// <summary>
    /// The tile height
    /// </summary>
    public float Height {
      get;
      set;
    }

    /// <summary>
    /// can be used to reference a type who's background should be used instead.
    /// This is to avoid duplicating tiles.
    /// </summary>
    Type _backgroundOverride;

    Tile(IBuilder<Tile> builder) : this() {
      _initializeFor((Type)builder.Archetype);
      Background = builder?.GetParam<UnityEngine.Tilemaps.Tile>(nameof(Background)) ?? Background;
      Height = builder?.GetParam<float?>(nameof(Height), null) ?? Height;
    }

    /// <summary>
    /// Resets this tile to it's current archetype's setting
    /// </summary>
    public void ResetForCurrentArchetype()
      => _initializeFor(_archetype);

    /// <summary>
    /// Override the background to another type's background image
    /// </summary>
    public void OverrideBackgroundTo(Type archetype) {
      Background = _backgroundOverride.DefaultBackground;
      if(Background is not null) {
        _backgroundOverride = archetype;
      }
    }

    /// <summary>
    /// Initialize this for a new archetype
    /// </summary>
    /// <param name="archetype"></param>
    void _initializeFor(Type archetype) {
      _backgroundOverride = null;
      if(archetype.LinkArchetypeToTileDataOnSet) {
        _archetype = archetype;
        Background = archetype.DefaultBackground;
      } // if we're not linking, and it has a background, we need to hide it in the override.
      else {
        if(archetype.DefaultBackground != null) {
          Background = _archetype.DefaultBackground;
          _backgroundOverride = _archetype;
        }
      }

      Height = archetype.DefaultHeight;
    }
  }
}
