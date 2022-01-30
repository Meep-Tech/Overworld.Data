using System;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldKeyValueSet : DataField {
    internal IEnumerable<Attribute> _childFieldAttributes;

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="extraEntryValidation">Add validation other than the built in key validation</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    public DataFieldKeyValueSet(
      string name,
      string tooltip = null,
      Dictionary<string, object> rows = null,
      IEnumerable<Attribute> childFieldAttributes = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      Func<KeyValuePair<string, object>, bool> extraEntryValidation = null
    ) : base(
      DisplayType.KeyValueFieldList,
      name,
      tooltip,
      rows, 
      dataKey,
      isReadOnly,
      enable,
      ((KeyValuePair<string, object> value) => rows.ContainsKey(value.Key)) + (extraEntryValidation ?? ((_) => true))
    ) {
      _childFieldAttributes = childFieldAttributes;
    }

    /// <summary>
    /// Used to update the colletction
    /// </summary>
    /// <param name="pair"></param>
    internal void _update(KeyValuePair<string, object> pair) {
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
    internal void _remove(string key) {
      throw new NotImplementedException();
    }
  }
}