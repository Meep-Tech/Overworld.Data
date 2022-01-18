using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Display data for a component.
  /// </summary>
  public partial class UxPannel : IUxViewElement {

    /// <summary>
    /// The fiels in this model, by key.
    /// </summary>
    public IReadOnlyList<IUxViewElement> Elements {
      get;
      private set;
    }

    internal UxPannel(IList<IUxViewElement> orderedFields) {
      Elements = orderedFields?.ToList();
    }

    /// <summary>
    /// Copy this pannels UI scheme.
    /// </summary>
    /// <returns></returns>
    public UxPannel Copy() {
      Dictionary<string, UxDataField> copiedFields = new();
      return new(null) {
        Elements = Elements
      };
    }

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy()
      => Copy();
  }
}