﻿using Meep.Tech.Data;

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

  /// <summary>
  /// A component that can be added to an entity.
  /// </summary>
  /// <typeparam name="TComponentBase">Only one component of each base type can be added to a model.</typeparam>
  public partial interface IToggleableComponent<TComponentBase>
    : IToggleableComponent,
      IModel.IComponent<TComponentBase>
    where TComponentBase : IModel.IComponent<TComponentBase> { }

  public static class EntityComponentExtensionMethods {

    /// <summary>
    /// Toggle if this component is enabled or disabled.
    /// </summary>
    public static void ToggleEnabled<EC>(this EC component, bool? toEnabled = null)
      where EC : IToggleableComponent 
    {
      toEnabled ??= !component.IsEnabled;
      if (component.IsEnabled != toEnabled) {
        if (toEnabled.Value) {
          component.OnEnabled();
        } else {
          component.OnDisabled();
        }

        component.IsEnabled = toEnabled.Value;
      }
    }
  }
}