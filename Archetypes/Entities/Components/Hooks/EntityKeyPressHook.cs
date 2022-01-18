using Meep.Tech.Data;
using System;
using System.Text;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// This type of hook is exexuted on a key press.
  /// </summary>
  public abstract class EntityKeyPressHook : EntityHook<EntityKeyPressHook> {
    static EntityKeyPressHook() {
      Archetypes<Type>._.DisplayName = "Hook: Key Presses";
    }
  }
}
