using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Data.Entites.Components {
  class EntityRightClickMenuAdditions : Entity.Component<EntityRightClickMenuAdditions> {
    static EntityRightClickMenuAdditions() {
      Archetypes<Type>._.DisplayName = "Right Click Menu Items";
    }

    [ShowInOverworldEditor]
    public Dictionary<string, EntityRightClickMenuItem> ExtraItems {
      get;
      private set;
    }
  }

  public class EntityRightClickMenuItem {}
}
