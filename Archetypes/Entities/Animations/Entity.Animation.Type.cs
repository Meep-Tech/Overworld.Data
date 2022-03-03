using Meep.Tech.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Data {

  public partial class Animation {

    /// <summary>
    /// Types of entity animation.
    /// These can be built by files
    /// </summary>
    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public partial class Type : Archetype<Animation, Animation.Type>, IPortableArchetype {

      #region Config

      ///<summary><inheritdoc/></summary>
      protected override bool AllowInitializationsAfterLoaderFinalization
        => true;

      #endregion

      #region Archetype Data Fields

      /// <summary>
      /// The animation's unique name
      /// </summary>
      public string Name
        => Id.Name;

      /// <summary>
      /// The version of this animation.
      /// If this is different then the name of the compiled *.anim file then it will be recompiles.
      /// </summary>
      public string Version {
        get;
      }

      /// <summary>
      /// The animation's namespace
      /// </summary>
      public string Namespace {
        get;
      }

      /// <summary>
      /// The unity animation clip that is played
      /// </summary>
      public AnimationClip Clip {
        get;
      }

      /// <summary>
      /// The tags that apply to this sprite animation
      /// </summary>
      public IEnumerable<Tag> Tags {
        get;
      }

      /// <summary>
      /// The dimensions of the sprite animation in px
      /// </summary>
      public (int width, int height) Dimensions {
        get;
      }

      /// <summary>
      /// If this animation type was built by an assembly/class rather than though the auto-loader
      /// </summary>
      public bool IsBuildFromAnAssembly {
        get;
      } = true;

      /// <summary>
      /// The key for this resource
      /// </summary>
      public string ResourceKey {
        get;
      }

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string PackageKey {
        get => _packageName ?? DefaultPackageKey;
        protected set => _packageName = value;
      }
      string _packageName;

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string DefaultPackageKey {
        get;
      } = "_entity_animations";

      #endregion

      /// <summary>
      /// XBAM Base Constructors
      /// </summary>
      protected Type(Identity id)
        : base(id) {
      }

      /// <summary>
      /// Import constructor
      /// </summary>
      internal Type(string resourceKey, JObject config, AnimationClip clip, IEnumerable<Tag> tags = null)
        : this(new Identity(
            config["name"].Value<string>(),
            config["namespace"].Value<string>()
        )) {
        ResourceKey = resourceKey;
        Clip = clip;
        IsBuildFromAnAssembly = false;
        Namespace = config["namespace"].Value<string>();
        Version = config["version"].Value<string>();
        Tags = tags ?? config["tags"].ToObject<IEnumerable<Tag>>(
          Models.DefaultUniverse.ModelSerializer.JsonSerializer
        );
        Dimensions = config["namespace"].Value<(int, int)>();
      }

      ///<summary><inheritdoc/></summary>
      public JObject GenerateConfig() {
        throw new System.NotImplementedException();
      }

      /// <summary>
      /// Import a type from a config and data
      /// </summary>
      /*public static Type Import(JObject config, AnimationClip clip, IEnumerable<Tag> tags = null)
        => new Type(config, clip, tags);
      */
    }
  }

  public partial class Entity {
    public partial class Animation {

      /// <summary>
      /// Types of entity animation.
      /// These can be built by files
      /// </summary>
      [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
      public partial class Type : Data.Animation.Type, IEntityDisplayableSprite.IArchetype {

        #region Archetype Data Fields

        /// <summary>
        /// The layer this animation acts on.
        /// Defults to 0 (BaseBody)
        /// </summary>
        public Layer Layer {
          get;
        }

        /// <summary>
        /// The default "scale to fit entity" setting of animations of this type
        /// </summary>
        public bool ShouldScaleToFitEntityByDefault {
          get;
        } = true;

        #endregion

        /// <summary>
        /// XBAM Base Constructors
        /// </summary>
        protected Type(Identity id)
          : base(id) {
        }

        /// <summary>
        /// Import constructor
        /// </summary>
        internal Type(string resourceKey, JObject config, AnimationClip clip, IEnumerable<Tag> tags = null)
          : base(resourceKey, config, clip, tags) {
          ShouldScaleToFitEntityByDefault = config["scaleToFitEntity"].Value<bool>();
        }

        /// <summary>
        /// Build a model from this animation type
        /// </summary>
        protected override Data.Animation ConfigureModel(IBuilder<Data.Animation> builder, Data.Animation model) {
          var entityAnimation = base.ConfigureModel(builder, model) as Animation;
          entityAnimation.ScaleToFitEntity = builder.GetParam(nameof(ScaleToFitEntity), ShouldScaleToFitEntityByDefault);

          return entityAnimation;
        }

        IEntityDisplayableSprite IEntityDisplayableSprite.IArchetype.Make()
          => Make();
      }
    }
  }
}