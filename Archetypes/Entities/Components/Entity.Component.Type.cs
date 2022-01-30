using Meep.Tech.Data;
using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
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
                = componentArchetype.BuildDefaultField(field, includeAttribute);
              if(builtField is not null) {
                builder.AddField(builtField);
              }
            } else if(member is PropertyInfo prop && (prop.GetMethod.IsPublic || includeAttribute is not null)) {
              DataField builtField 
                =componentArchetype.BuildDefaultField(prop, includeAttribute);
              if(builtField is not null) {
                builder.AddField(builtField);
              }
            }
          }

          return builder;
        };

        /// <summary>
        /// Build a default Ux field for an entity component using the field
        /// </summary>
        public DataField BuildDefaultField(FieldInfo field, Entites.Components.ShowInOverworldEditorAttribute includeAttribute = null) {
          DataField builtField = ViewBuilder.BuildDefaultField(field);
          if(field is null) {
            return null;
          }
          if(includeAttribute is not null) {
            if(includeAttribute.IsReadOnly) {
              builtField.IsReadOnly = true;
            }
          }

          return builtField;
        }

        /// <summary>
        /// Build a default Ux field for an entity component using the property
        /// </summary>
        public DataField BuildDefaultField(PropertyInfo prop, Entites.Components.ShowInOverworldEditorAttribute includeAttribute = null) {
          DataField field = ViewBuilder.BuildDefaultField(prop);
          if(field is null) {
            return null;
          }

          if(includeAttribute is not null) {
            field.IsReadOnly = prop.CanWrite && !includeAttribute.IsReadOnly;
          } else if (field is not null)
            field.IsReadOnly = prop.CanWrite && prop.SetMethod.IsPublic;

          return field;
        }

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