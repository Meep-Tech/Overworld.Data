using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A component to place an execution hook on an entity interaction.
  /// </summary>
  public abstract class EntityInteractionHook : EntityHook<EntityInteractionHook> {
    static EntityInteractionHook() {
      Archetypes<Type>._.DisplayName = "Hook: On Interact";
    }
  }
}
