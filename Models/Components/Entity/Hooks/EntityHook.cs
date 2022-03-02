using Simple.Ux.XBam.Components;
using System.Collections.Generic;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A type of component that uses a hook to execute some code from the perspective of an entity.
  /// </summary>
  public abstract class EntityHook<TEntityHookBase> :
      IDefaultEntityComponent<TEntityHookBase>,
      IHasSimpleUxComponentEditMenu<TEntityHookBase>,
      IEntityHookComponent 
    where TEntityHookBase : EntityHook<TEntityHookBase> 
  {

    /// <summary>
    /// The title used for this component in it's simpleux menu
    /// </summary>
    public abstract string SimpleUxMenuTitle {
      get;
    }

    string IHasSimpleUxComponentEditMenu.SimpleUxMenuTitle
      => SimpleUxMenuTitle;

    ///<summary><inheritdoc/></summary>
    [DisplayInSimpleUxComponentMenu]
    public IEnumerable<Executeable> Executeables {
      get;
      private set;
    }

    bool IToggleableComponent.IsEnabled {
      get;
      set; 
    }
  }
}