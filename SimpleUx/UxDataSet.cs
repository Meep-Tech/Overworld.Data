using System;
using System.Collections;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class UxDataSet : UxDataField {

    /// <summary>
    /// The type of data this set holds.
    /// </summary>
    public System.Type DataType {
      get;
    }

    internal IEnumerable<Attribute> _childFieldAttributes;

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="dataType">The type of data the list will accept</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    /// <param name="rowValues">The default/current list values</param>
    public UxDataSet(
      string name,
      System.Type dataType,
      IEnumerable<Attribute> childFieldAttributes = null,
      string tooltip = null,
      ArrayList rowValues = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<UxDataField, UxPannel, bool> enable = null,
      Func<int, object, bool> validation = null
    ) : base(
      UxDataSet.DisplayType.FieldList,
      name,
      tooltip,
      rowValues, 
      dataKey,
      isReadOnly,
      enable,
      validation
    ) {
      DataType = dataType;
      _childFieldAttributes = childFieldAttributes;
    }

    /// <summary>
    /// Used to update the colletction
    /// </summary>
    internal void _update(KeyValuePair<int, object> itemAtIndex) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// remove the last item from the collection
    /// </summary>
    internal void _removeLast() {
      throw new NotImplementedException();
    }

    /// <summary>
    /// remove the item at the collection index
    /// </summary>
    internal void _remove(int key) {
      throw new NotImplementedException();
    }
  }
}