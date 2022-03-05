using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// Manages sprites and helps retrieve desired sprite animations and icons.
  /// </summary>
  [Meep.Tech.Data.Configuration.Loader.Settings.BuildAllDeclaredEnumValuesOnInitialLoad]
  public class SpriteManager : IDefaultEntityComponent<SpriteManager> {

    Entity.Icon _defaultIcon;
    readonly Lazy<TagedCollection<Tag, Animation>> _animations
      = new();
    readonly Lazy<TagedCollection<Tag, Entity.Icon>> _icons
      = new();
    readonly Lazy<TagedCollection<Tag, IEntityDisplayableSprite>> _allSpriteOptions
      = new();
    readonly Lazy<Dictionary<string, IEntityDisplayableSprite>> _allSpriteOptionsById
      = new();

    /// <summary>
    /// Tag used to put single frame icon-based results first in searches over animated results.
    /// </summary>
    public static Tag IconPreferenceSearchTag {
      get;
    } = new Tag("Icon");

    /// <summary>
    /// Tag used to identify the default icons and animations used by this entity.
    /// </summary>
    public static Tag DefaultDisplayEntryTag {
      get;
    } = new Tag("Default");

    /// <summary>
    /// The default icon used for this entity.
    /// </summary>
    public Entity.Icon DefaultIcon {
      get => _defaultIcon;
      internal set {
        _icons.Value.RemoveTagsForItem(_defaultIcon, DefaultDisplayEntryTag);
        _allSpriteOptions.Value.RemoveTagsForItem(_defaultIcon, DefaultDisplayEntryTag);
        Add(value, DefaultDisplayEntryTag.AsSingleItemEnumerable());
        _defaultIcon = value;
      }
    }

    /// <summary>
    /// Availible sprite animations by tag
    /// </summary>
    public IReadOnlyTagedCollection<Tag, Animation> Animations
      => _animations.Value;

    /// <summary>
    /// Availible entity icons
    /// </summary>
    public IReadOnlyTagedCollection<Tag, Entity.Icon> Icons
      => _icons.Value;

    /// <summary>
    /// Availible entity icons and animations combined, by tag
    /// </summary>
    public IReadOnlyTagedCollection<Tag, IEntityDisplayableSprite> AllDisplayOptions
      => _allSpriteOptions.Value;

    /// <summary>
    /// Get all display options by their unique ids.
    /// </summary>
    public IReadOnlyDictionary<string, IEntityDisplayableSprite> AllDisplayOptionsById
      => _allSpriteOptionsById.Value;

    /// <summary>
    /// Make a new default/empty sprite manager based on a set of options.
    /// </summary>
    internal SpriteManager(SpriteDisplayOptions options) { 
      if (options.DefaultIconType is not null) {
        DefaultIcon = (Entity.Icon)(options.DefaultIconType as IEntityDisplayableSprite.IArchetype).Make();
      }
      foreach(var entry in options.AllDisplayOptionTypes) {
        Add(entry.Value.Make(), entry.Key);
      }
    }

    /// <summary>
    /// Add a new displayable item (like an icon or animation) to this psrite manager.
    /// </summary>
    public void Add(IEntityDisplayableSprite displayableSpriteAnimationOrIcon, IEnumerable<Tag> tags) {
      if (displayableSpriteAnimationOrIcon is Entity.Icon icon) {
        _icons.Value.Add(tags, icon);
        tags = tags
          .Append(IconPreferenceSearchTag)
          .Append(Entity.Animation.BuiltInTag.Still);
      } else if (displayableSpriteAnimationOrIcon is Animation animation) {
        _animations.Value.Add(tags, animation);
        tags = tags
          .Append(Entity.Animation.BuiltInTag.Animated);
      }

      _allSpriteOptions.Value.Add(tags, displayableSpriteAnimationOrIcon);
      _allSpriteOptionsById.Value.Add(displayableSpriteAnimationOrIcon.Id, displayableSpriteAnimationOrIcon);
    }

    /// <summary>
    /// Remove an entry by it's unique id.
    /// </summary>
    public void Remove(string entryUniqueId) {
      if (_allSpriteOptionsById.Value.TryGetValue(entryUniqueId, out IEntityDisplayableSprite entry)) {
        if (entry is Entity.Icon icon) {
          _icons.Value.Remove(icon);
        } else if (entry is Animation animation) {
          _animations.Value.Remove(animation);
        }

        _allSpriteOptions.Value.Remove(entry);
        _allSpriteOptionsById.Value.Remove(entryUniqueId);
      }
    }
  }
}
