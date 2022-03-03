using Simple.Ux.XBam.Components;
using System.Collections.Generic;

namespace Overworld.Data.Entites.Components {
  class EntityRightClickMenuAdditions 
    : IDefaultToggleableEntityComponent<EntityRightClickMenuAdditions>,
    IHasSimpleUxComponentEditMenu<EntityRightClickMenuAdditions>
  {

    string IHasSimpleUxComponentEditMenu.SimpleUxMenuTitle
      => "Right Click Menu Items";

    [DisplayInSimpleUxComponentMenu]
    public Dictionary<string, EntityRightClickMenuItem> ExtraItems {
      get;
      private set;
    }

    bool IToggleableComponent.IsEnabled {
      get;
      set;
    }
  }

  public class EntityRightClickMenuItem {}
}
