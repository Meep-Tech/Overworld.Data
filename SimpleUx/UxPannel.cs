using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Display data for a component.
  /// </summary>
  public partial class UxPannel : IUxViewElement, IEnumerable<UxColumn> {

    /// <summary>
    /// The tab data for this pannel
    /// </summary>
    public UxPannel.Tab Key {
      get;
      internal set;
    }

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public UxView View {
      get;
      internal set;
    }

    /// <summary>
    /// The fiels in this model, by key.
    /// </summary>
    public IReadOnlyList<UxColumn> Elements {
      get;
      private set;
    }

    internal UxPannel(IList<UxColumn> orderedFields, Tab tab) {
      Elements = orderedFields?.ToList();
      Key = tab;
      tab.Pannel = this;
    }

    /// <summary>
    /// Copy this pannels UI scheme.
    /// </summary>
    public UxPannel Copy(UxView toNewView = null) {
      Dictionary<string, UxDataField> copiedFields = new();
      Tab tab = Key;
      UxPannel pannel = new(null, tab) {
        Elements = Elements.Select(element => element.Copy(toNewView)).ToList(),
        View = toNewView,
      };
      pannel.Key = tab;
      tab.Pannel = pannel;

      return pannel;
    }

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(UxView toNewView)
      => Copy(toNewView);

    ///<summary><inheritdoc/></summary>
    public IEnumerator<UxColumn> GetEnumerator()
      => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}