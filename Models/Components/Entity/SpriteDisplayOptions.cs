using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// A set of display options an entity sprite can choose from.
  /// </summary>
  public class SpriteDisplayOptions 
    : Archetype.IComponent<SpriteDisplayOptions>, Archetype.ILinkedComponent<SpriteManager>, IComponent.IUseDefaultUniverse {

    Entity.Icon.Type _defaultIconType;
    readonly Lazy<TagedCollection<Tag, Entity.Animation.Type>> _animations
      = new();
    readonly Lazy<TagedCollection<Tag, Entity.Icon.Type>> _icons
      = new();
    readonly Lazy<TagedCollection<Tag, IEntityDisplayableSprite.IArchetype>> _allSpriteOptions
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
    public Entity.Icon.Type DefaultIconType {
      get => _defaultIconType;
      internal set {
        _icons.Value.RemoveTagsForItem(_defaultIconType, DefaultDisplayEntryTag);
        _allSpriteOptions.Value.RemoveTagsForItem(_defaultIconType, DefaultDisplayEntryTag);
        Add(value, DefaultDisplayEntryTag.AsSingleItemEnumerable());
        _defaultIconType = value;
      }
    }

    /// <summary>
    /// Availible sprite animations by tag
    /// </summary>
    public IReadOnlyTagedCollection<Tag, Entity.Animation.Type> AnimationTypes
      => _animations.Value;

    /// <summary>
    /// Availible entity icons
    /// </summary>
    public IReadOnlyTagedCollection<Tag, Entity.Icon.Type> IconTypes
      => _icons.Value;

    /// <summary>
    /// Availible entity icons and animations combined, by tag
    /// </summary>
    public IReadOnlyTagedCollection<Tag, IEntityDisplayableSprite.IArchetype> AllDisplayOptionTypes
      => _allSpriteOptions.Value;

    /// <summary>
    /// Make a new default/empty set of sprite display options quickly
    /// </summary>
    public SpriteDisplayOptions() {}

    /// <summary>
    /// Add a new displayable item (like an icon or animation) to this psrite manager.
    /// </summary>
    public void Add(IEntityDisplayableSprite.IArchetype displayableSpriteTypeArcheytpe, IEnumerable<Tag> tags) {
      if (displayableSpriteTypeArcheytpe is Entity.Icon.Type icon) {
        _icons.Value.Add(tags, icon);
        tags = tags
          .Append(IconPreferenceSearchTag)
          .Append(Tag.Still);
      } else if (displayableSpriteTypeArcheytpe is Entity.Animation.Type animation) {
        _animations.Value.Add(tags, animation);
        tags = tags
          .Append(Tag.Animated);
      }

      _allSpriteOptions.Value.Add(tags, displayableSpriteTypeArcheytpe);
    }

    /// <summary>
    /// Remove an entry type
    /// </summary>
    public void Remove(IEntityDisplayableSprite.IArchetype toRemove) {
      if (toRemove is Entity.Icon.Type icon) {
        _icons.Value.Remove(icon);
      } else if (toRemove is Entity.Animation.Type animation) {
        _animations.Value.Remove(animation);
      }

      _allSpriteOptions.Value.Remove(toRemove);
    }

    #region ILinkedComponent

    SpriteManager Archetype.ILinkedComponent<SpriteManager>.BuildDefaultModelComponent(IModel.Builder parentModelBuilder, Universe universe)
      => new(this);

    #endregion
  }
}
