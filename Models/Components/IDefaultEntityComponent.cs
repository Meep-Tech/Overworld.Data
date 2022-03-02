using Meep.Tech.Data;

namespace Overworld.Data {

  /// <summary>
  /// An interface for helping to make default entity components.
  /// Entity components don't explicityly need this interface.
  /// </summary>
  /// <typeparam name="TComponentBase">Only one component of each base type can be added to a model.</typeparam>
  public partial interface IDefaultEntityComponent<TComponentBase>
    : Simple.Ux.XBam.Components.IHasSimpleUxComponentEditMenu<TComponentBase>,
      IModel.IComponent<TComponentBase>,
      IComponent.IUseDefaultUniverse
    where TComponentBase : IModel.IComponent<TComponentBase> { }

  /// <summary>
  /// An interface for helping to make default entity components.
  /// Entity components don't explicityly need this interface.
  /// This one is toggleable.
  /// </summary>
  /// <typeparam name="TComponentBase">Only one component of each base type can be added to a model.</typeparam>
  public partial interface IDefaultToggleableEntityComponent<TComponentBase>
    : IToggleableComponent,
      IDefaultEntityComponent<TComponentBase>
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