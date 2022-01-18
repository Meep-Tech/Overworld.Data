using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {
  public partial class Animation {

    /// <summary>
    /// A collection of tags that can apply to an animation
    /// </summary>
    public class Tags
      : HashSet<Tag>,
        IModel.IComponent<Tags>,
        IComponent.IUseDefaultUniverse {
    }
  }
}