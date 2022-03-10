using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using Overworld.Data.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {
  public partial struct Tile {

    /// <summary>
    /// Archetypes for tiles.
    /// </summary>
    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public partial class Type : Archetype<Tile, Tile.Type>.WithDefaultParamBasedModelBuilders, IPortableArchetype, ITaggable {

      #region Archetype Config

      /// <summary>
      /// <inheritdoc/>
      /// </summary>
      protected override bool AllowInitializationsAfterLoaderFinalization
        => true;

      #endregion

      /// <summary>
      /// The default package name for archetyps of this type
      /// </summary>
      public virtual string DefaultPackageKey {
        get;
      } = "_tiles";

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
      public virtual string PackageKey {
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
      public virtual float? DefaultHeight {
        get;
      }

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
      public virtual bool UseDefaultBackgroundAsInWorldTileImage {
        get;
      }

      /// <summary>
      /// If this tile archetype should link itself to a tile when used to make that tile in the world
      /// If you don't want this archetype set as the tile's 'type' then set this to false.
      /// This is used for Background Archetypes for tiles.
      /// </summary>
      public virtual bool LinkArchetypeToTileDataOnSet {
        get;
        internal set;
      } = true;

      /// <summary>
      /// This is used to ignore the type during re-serialization, because another type may already handle importing it via he same resource key.
      /// </summary>
      internal bool _ignoreDuringModReSerialization
        = false;

      /// <summary>
      /// Default tags for this tile type.
      /// </summary>
      public virtual IEnumerable<Tag> DefaultTags {
        get => _DefaultTags ?? new();
      } /**<summary> The backing field used to initialize and override DefaultTags </summary>**/
      protected HashSet<Tag> _DefaultTags {
        get => _defaultTags;
        set => _defaultTags = value;
      } HashSet<Tag> _defaultTags;

      /// <summary>
      /// Can be used to extend this to a new, non-templateable type.
      /// </summary>
      protected Type(
        string name,
        string packageKey,
        string resourceKey,
        Universe universe = null
      ) : base(new Identity(name, packageKey), universe) {
        ResourceKey = resourceKey;
        PackageKey = packageKey;
      }

      /// <summary>
      /// Used to make new tiles via import.
      /// </summary>
      protected Type(
        string name,
        string packageKey,
        string resourceKey,
        JObject config,
        Dictionary<string, object> importOptionsAndObjects,
        Universe universe = null
      ) : this(name, packageKey, resourceKey, universe) {
        LinkArchetypeToTileDataOnSet = 
          importOptionsAndObjects.TryGetValue(nameof(LinkArchetypeToTileDataOnSet), out var foundLinkValue) 
            && (bool)foundLinkValue;
        _ignoreDuringModReSerialization =
          importOptionsAndObjects.TryGetValue(nameof(_ignoreDuringModReSerialization), out var foundIgnoreReserializeValue) 
            && (bool)foundIgnoreReserializeValue;
        Description = config.TryGetValue<string>(Porter.DescriptionConfigKey);
        _defaultTags = config.TryGetValue(Porter.TagsConfigOptionKey, @default: Enumerable.Empty<Tag>()).ToHashSet();
        Description = config.TryGetValue<string>(Porter.DescriptionConfigKey);
        UseDefaultBackgroundAsInWorldTileImage = importOptionsAndObjects.TryGetValue(nameof(UseDefaultBackgroundAsInWorldTileImage), out object useBgAsInWorld)
          ? (bool)useBgAsInWorld
          : config.TryGetValue(Porter.UseDefaultBackgroundAsInWorldTileImageConfigKey, @default: true);
        DefaultBackground = importOptionsAndObjects.TryGetValue(nameof(DefaultBackground), out object backgroundImage)
          ? backgroundImage as UnityEngine.Tilemaps.Tile
          : null;
        if(DefaultBackground is not null) {
          BackgroundImageHashKey = importOptionsAndObjects.TryGetValue(nameof(BackgroundImageHashKey), out object hashKey)
            ? (Hash128)hashKey
            : DefaultBackground.GetTileHash();
        }
        DefaultHeight = config.TryGetValue<float?>(Porter.TileHeightConfigKey) ?? DefaultHeight;
      }

      ///<summary><inheritdoc/></summary>
      public JObject GenerateConfig() {
        JObject config = new() {
          {
            Porter.NameConfigKey,
            JToken.FromObject(Id.Name)
          },
          {
            Porter.PackageNameConfigKey,
            JToken.FromObject(PackageKey)
          },
          {
            Porter.ImportModeConfigKey,
            JToken.FromObject(Porter.BackgroundImageImportMode.Individual)
          }
        };

        if (DefaultHeight is not null) {
          config.Add(Porter.TileHeightConfigKey, DefaultHeight);
        }
        if (Description is not null) {
          config.Add(Porter.DescriptionConfigKey, Description);
        }
        if (UseDefaultBackgroundAsInWorldTileImage is false) {
          config.Add(Porter.UseDefaultBackgroundAsInWorldTileImageConfigKey, false);
        }

        // config image data
        if (DefaultBackground?.sprite.texture != null) {
          config.Add(PorterExtensions.PixelsPerTileConfigKey, JToken.FromObject(DefaultBackground.sprite.pixelsPerUnit));
          config.Add(Porter.ImportModeConfigKey, JToken.FromObject(Porter.BackgroundImageImportMode.Individual));
        }

        config.Add(Porter.TagsConfigOptionKey, JToken.FromObject(DefaultTags));

        return config;
      }

      void IPortableArchetype.Unload()
        => TryToUnload();
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
