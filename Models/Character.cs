using Meep.Tech.Data;
using System;

namespace Overworld.Data {

  /// <summary>
  /// A player controlled entity
  /// </summary>
  public partial class Character : Entity {

    /// <summary>
    /// The unique, human readable name of a character. Like their username
    /// </summary>
    public string UniqueName {
      get;
      internal set;
    }

    /// <summary>
    /// X Bam Builder
    /// </summary>
    /// <param name="builder"></param>
    Character(IBuilder<Entity> builder)
     : this(
        builder?.GetParam(nameof(Name), (builder?.Archetype as Type).Id.Name)
          ?? (builder?.Archetype as Type)?.Id.Name,
       builder?.GetAndValidateParamAs<string>(nameof(UniqueName))
         ?? new Guid().ToString()
    ) {}
    
    /// <summary>
    /// Make a new character
    /// </summary>
    public Character(string name, string uniqueName) : base(name, null) {
      Name = name;
      UniqueName = uniqueName;
      Id = new Guid().ToString();
    }
  }
}