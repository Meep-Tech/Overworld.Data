using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {

  /// <summary>
  /// An in game thing that can move around.
  /// </summary>
  public partial class Entity 
    : Model<Entity, Entity.Type>.WithComponents,
    IUnique,
    IWriteableComponentStorage 
  {

    static Dictionary<string, int> _entityUsedNames
      = new Dictionary<string, int>();

    /// <summary>
    /// Unique entity id
    /// </summary>
    public string Id {
      get;
      internal set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    /// <summary>
    /// The display name of an entity
    /// </summary>
    public string Name {
      get;
      set;
    }

    /// <summary>
    /// X Bam Builder
    /// </summary>
    /// <param name="builder"></param>
    Entity(IBuilder<Entity> builder) 
      : this(
        builder?.GetParam(nameof(Name), (builder.Archetype as Type).Id.Name) 
          ?? (builder.Archetype as Type).Id.Name,
        (builder?.GetParam(nameof(IUnique.Id), new System.Guid().ToString()))
    ){}

    /// <summary>
    /// Make a new entity
    /// </summary>
    public Entity(string name, string uniqueId = null) : base() {
      Name = name ?? uniqueId;
      string key = uniqueId ?? Name;
      if(_entityUsedNames.ContainsKey(key ?? "__none_")) {
        _entityUsedNames[uniqueId] = _entityUsedNames[uniqueId] + 1;
        key += _entityUsedNames[uniqueId].ToString();
      }

      Id = key;
    }
  }
}