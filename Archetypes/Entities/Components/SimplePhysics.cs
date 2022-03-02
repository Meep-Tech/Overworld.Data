using Meep.Tech.Data;
using Simple.Ux.Data;

namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// When attached to an entity in world, it means this entity will spawn at this location when the world starts.
  /// </summary>
  public class SimplePhysics : IDefaultEntityComponent<SimplePhysics> {

    /// <summary>
    /// The parent of this component
    /// </summary>
    [Simple.Ux.XBam.Components.IgnoreForSimpleUxComponentMenu]
    public Entity Parent {
      get;
      private set;
    }

    /// <summary>
    /// If gravity should be applied to this Entity.
    /// </summary>
    [Tooltip("If gravity should be applied to this Entity.")]
    public bool ApplyGravity {
      get;
      set;
    }

    /// <summary>
    /// If this entity is solid.
    /// </summary>
    [Tooltip("If this entity blocks and pushes other entities.")]
    public bool IsSolid {
      get;
      set;
    }

    /// <summary>
    /// If this entity can be pushed around by other entities
    /// </summary>
    [Tooltip("If this entity can be pushed around by other solid entities.")]
    [EnableIf("IsSolid")]
    public bool IsPushable {
      get;
      set;
    }

    bool IToggleableComponent.IsEnabled {
      get;
      set;
    }

    /// <summary>
    /// Default configuration
    /// </summary>
    IModel IModel.Configure(IBuilder builder) {
      Parent = builder.Parent as Entity;
      return this;
    } SimplePhysics() { }
  }
}
