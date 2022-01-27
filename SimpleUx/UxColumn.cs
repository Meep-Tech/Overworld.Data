using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A column in a simple Ux.
  /// A pannel can have up to 3 columns, but 3 is the reccomended.
  /// Columns can have labels at the top.
  /// Columns cannot contain other columns, but Columns can contain rows.
  /// </summary>
  public class UxColumn : IUxViewElement, IEnumerable<IUxViewElement> {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public UxView View {
      get;
      internal set;
    }

    List<IUxViewElement> _elements;

    /// <summary>
    /// The label for this row.
    /// </summary>
    public UxTitle Title {
      get;
    }

    internal UxColumn(IEnumerable<IUxViewElement> elements, UxTitle label) {
      _elements = elements.Select(element => {
        return element is UxColumn
          ? throw new System.Exception($"Cannot place a Simple Ux Column inside another column.")
          : element;
      }).ToList();
      Title = label;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<IUxViewElement> GetEnumerator() {
      return ((IEnumerable<IUxViewElement>)_elements).GetEnumerator();
    }

    ///<summary><inheritdoc/></summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)_elements).GetEnumerator();
    }

    /// <summary>
    /// Copy this column and it's contents
    /// </summary>
    public UxColumn Copy(UxView toNewView = null) 
      => new UxColumn(_elements.Select(element => element.Copy(toNewView)), Title);

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(UxView toNewView)
      => Copy(toNewView);
  }
}
