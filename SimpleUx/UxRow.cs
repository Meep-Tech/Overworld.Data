using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A Row of UX items.
  /// Can't contain columns or other rows.
  /// Can have a label.
  /// </summary>
  public class UxRow : IUxViewElement, IEnumerable<UxDataField> {
    List<UxDataField> _elements;

    /// <summary>
    /// The label for this row.
    /// </summary>
    public UxTitle Label { 
      get;
    }

    /// <summary>
    /// Info tooltip for the row label
    /// </summary>
    public virtual string LabelTooltip {
      get;
    } = null;

    internal UxRow(IEnumerable<UxDataField> elements, UxTitle label) {
      _elements = elements.ToList();
      Label = label;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<UxDataField> GetEnumerator() {
      return ((IEnumerable<UxDataField>)_elements).GetEnumerator();
    }

    ///<summary><inheritdoc/></summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)_elements).GetEnumerator();
    }

    /// <summary>
    /// Copy this row and it's contents
    /// </summary>
    public UxRow Copy()
      => new(_elements.Select(element => element.Copy()), Label);

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy()
      => Copy();
  }
}
