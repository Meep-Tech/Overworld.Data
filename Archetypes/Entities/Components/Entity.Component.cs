using Meep.Tech.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A component that can be added to an entity.
    /// </summary>
    [Serializable]
    public abstract class Component
      : IModel.IComponent,
        IComponent.IUseDefaultUniverse {

      /// <summary>
      /// There can only be one component per type attached to an entity.
      /// </summary>
      public interface IType 
        : IComponent.IBuilderFactory {}
    }

    /// <summary>
    /// A component that can be added to an entity.
    /// </summary>
    /// <typeparam name="TEntityComponentBaseType">Only one component of each base type can be added to a model.</typeparam>
    public abstract partial class Component<TEntityComponentBaseType>
      : Component,
      IModel.IComponent<Entity.Component<TEntityComponentBaseType>>
    {

      /// <summary>
      /// If this component is enabled.
      /// </summary>
      public bool IsEnabled {
        get;
        set;
      }

      /// <summary>
      /// For making a new type of component.
      /// </summary>
      protected Component() {}

      /// <summary>
      /// Update from a ux.
      /// </summary>
      /// <param name="ux">The ux for this component</param>
      /// <param name="updatedFieldKey">(optional) a field that was changed.</param>
      protected void UpdateFromUxFieldChange(Ux.Simple.UxView ux, string updatedFieldKey = null) {
        if(updatedFieldKey is not null && ux._fields.TryGetValue(updatedFieldKey, out var found)) {
          if(updatedFieldKey.Contains(':')) {
            var parts = updatedFieldKey.Split(':');
            ((this.GetType().GetMember(parts[0]).First() as PropertyInfo).GetValue(this) as IDictionary)[parts[0]]
              = ux._fields[updatedFieldKey].Value;
          } else
            (this.GetType().GetMember(updatedFieldKey).First() as PropertyInfo)
              .SetValue(this, ux._fields[updatedFieldKey].Value);
        }
      } 
    }
  }
}