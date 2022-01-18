using System.Collections.Generic;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A type of component that uses a hook to execute some code from the perspective of an entity.
  /// </summary>
  public abstract class EntityHook<TEntityHookBase> : Entity.Component<TEntityHookBase>, IEntityHookComponent 
    where TEntityHookBase : EntityHook<TEntityHookBase> {

    ///<summary><inheritdoc/></summary>
    [ShowInOverworldEditor]
    public IEnumerable<Executeable> Executeables {
      get;
      private set;
    }
  }
}