using Meep.Tech.Data;
using UnityEngine;

namespace Overworld.Data {
  public partial struct Tile {

    /// <summary>
    /// Archetypes for tiles.
    /// </summary>
    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public partial class Type : Archetype<Tile, Tile.Type>, IPortable {

      #region Archetype Config

      /// <summary>
      /// <inheritdoc/>
      /// </summary>
      public override bool AllowInitializationsAfterLoaderFinalization
        => true;

      #endregion

      /// <summary>
      /// The default package name for archetyps of this type
      /// </summary>
      public virtual string DefaultPackageName {
        get;
      } = "-Tiles";

      /// <summary>
      /// The unique resource key of this type
      /// </summary>
      public virtual string ResourceKey {
        get;
      }

      /// <summary>
      /// The hash key of the image
      /// </summary>
      public Hash128 BackgroundImageHashKey {
        get;
      }

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string PackageName {
        get;
        protected set;
      }

      /// <summary>
      /// The background tile this tile is using
      /// </summary>
      public virtual UnityEngine.Tilemaps.Tile DefaultBackground {
        get;
      }

      /// <summary>
      /// The tile height
      /// </summary>
      public virtual float DefaultHeight {
        get;
      } = 0;

      /// <summary>
      /// The tile height
      /// </summary>
      public virtual string Description {
        get;
        protected set;
      }

      /// <summary>
      /// If the default background should be used as the tile image in world.
      /// If false, the DefaultBackground image is just for use in the editor ui.
      /// </summary>
      public virtual bool UseDefaultBackgroundAsInWorldTileImage
        => true;

      /// <summary>
      /// If this tile archetype should link itself to a tile when used to make that tile in the world
      /// If you don't want this archetype set as the tile's 'type' then set this to false.
      /// </summary>
      public virtual bool LinkArchetypeToTileDataOnSet
        => true;

      /// <summary>
      /// Used to make new tiles via import.
      /// </summary>
      protected internal Type(
        string name,
        string resourceKey,
        string packageName = null,
        UnityEngine.Tilemaps.Tile backgroundImage = null,
        Hash128? imageHashKey = null,
        float? defaultHeight = null
      ) : base(new Identity(
        name,
        packageName
      )) {
        ResourceKey = resourceKey;
        PackageName = packageName;
        DefaultBackground = backgroundImage;
        if(DefaultBackground is not null) {
          BackgroundImageHashKey = imageHashKey
            ?? DefaultBackground.GetTileHash();
        }
        DefaultHeight = defaultHeight ?? DefaultHeight;
      }
    }
  }

  /// <summary>
  /// Extensions for Unity Tiles
  /// </summary>
  public static class TileExtensions {

    /// <summary>
    /// Gets a tile's hash code from it's image
    /// </summary>
    public static Hash128 GetTileHash(this UnityEngine.Tilemaps.Tile tile)
      => tile.sprite.texture.imageContentsHash;
  }
}
