using Meep.Tech.Data;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Overworld.Data {
  public partial class Entity {
    public abstract partial class Component<TEntityComponentBaseType> {

      /// <summary>
      ///  on init, set the builder factory for each subtype:
      /// </summary>
      static Component() {
        Components<TEntityComponentBaseType>.BuilderFactory
          = new Type() {
            ModelBaseType = typeof(TEntityComponentBaseType)
          };
      }

      /// <summary>
      /// There can only be one component per type attached to an entity.
      /// </summary>
      [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
      public class Type : IComponent<TEntityComponentBaseType>.BuilderFactory, IType {
        View _compiledEditorUx;
        Dictionary<string, MemberInfo> _autoMembers;

        /// <summary>
        /// The display name of this component.
        /// </summary>
        public string DisplayName {
          get;
          internal set;
        } = Regex.Replace(typeof(TEntityComponentBaseType).Name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");

        /// <summary>
        /// The display name of this component.
        /// </summary>
        public Func<ViewBuilder, Type, ViewBuilder> BuildEditorUx {
          get;
          internal set;
        } = (builder, componentArchetype) => {
          System.Type ComponentBase = typeof(TEntityComponentBaseType);
          foreach(MemberInfo member in ComponentBase.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Cast<MemberInfo>().Concat(ComponentBase.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          ) {
            // ignore attribute
            if(member.GetCustomAttribute<Entites.Components.IgnoreInOverworldEditorAttribute>() != null) {
              continue;
            }

            /// Check for fields we should include by default:
            Entites.Components.ShowInOverworldEditorAttribute includeAttribute
              = member.GetCustomAttribute<Entites.Components.ShowInOverworldEditorAttribute>();
         
            if(member is FieldInfo field && (field.IsPublic || includeAttribute is not null)) {
              DataField builtField 
                = ViewBuilder.BuildDefaultField(field, includeAttribute is not null ? new Dictionary<System.Type, Attribute> {
                  { typeof(ReadOnlyAttribute),  new ReadOnlyAttribute(includeAttribute.IsReadOnly) },
                  { typeof(DisplayNameAttribute),  new DisplayNameAttribute(includeAttribute.Name ?? field.Name) }
                } : null);
              if(builtField is not null) {
                builder.AddField(builtField);
              }
            } else if(member is PropertyInfo prop && (prop.GetMethod.IsPublic || includeAttribute is not null)) {
              DataField builtField
                = ViewBuilder.BuildDefaultField(prop, includeAttribute is not null ? new Dictionary<System.Type, Attribute> {
                  { typeof(ReadOnlyAttribute),  new ReadOnlyAttribute(includeAttribute.IsReadOnly) },
                  { typeof(DisplayNameAttribute),  new DisplayNameAttribute(includeAttribute.Name ?? prop.Name) }
                } : null);
              if(builtField is not null) {
                builder.AddField(builtField);
              }
            }
          }

          return builder;
        };

        /// <summary>
        /// Get the base editor UX for this type of component.
        /// </summary>
        /// <returns></returns>
        public View GetEmptyEditorUx()
          => _compiledEditorUx
            ??= BuildEditorUx(new ViewBuilder(DisplayName), this)
                .Build();

        internal Type()
          : base(new Identity(typeof(TEntityComponentBaseType).FullName)) {
          GetEmptyEditorUx();
        }
      }
    }
  }
}