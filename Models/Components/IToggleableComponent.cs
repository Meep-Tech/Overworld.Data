using Meep.Tech.Data;

namespace Overworld.Data {
  /// <summary>
  /// A component that can be added to an entity.
  /// </summary>
  public interface IToggleableComponent
    : IModel.IComponent,
      IModel.IComponent.IUseDefaultUniverse {

    /// <summary>
    /// There can only be one component per type attached to an entity.
    /// </summary>
    public interface IType
      : IComponent.IBuilderFactory { }

    /// <summary>
    /// If this component is enabled.
    /// Also used to enable and disable.
    /// </summary>
    public bool IsEnabled {
      get;
      internal set;
    }

    /// <summary>
    /// Toggle if this component is enabled or disabled.
    /// </summary>
    void ToggleEnabled(bool? toEnabled = null)
      => EntityComponentExtensionMethods.ToggleEnabled(this, toEnabled);

    /// <summary>
    /// Callback for on-deacivated/disabled
    /// </summary>
    void OnDisabled() { }

    /// <summary>
    /// Callback for on-acivated/enabled
    /// </summary>
    void OnEnabled() { }
  }
}