using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldSet : DataField, IIndexedItemsDataField {

    /// <summary>
    /// The type of data this set holds.
    /// </summary>
    public System.Type DataType {
      get;
    }

    internal IEnumerable<Attribute> _childFieldAttributes;
    Func<DataField, KeyValuePair<int, object>, (bool success, string message)>[] _entryValidations;

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="dataType">The type of data the list will accept</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    /// <param name="rowValues">The default/current list values</param>
    public DataFieldSet(
      string name,
      System.Type dataType,
      IEnumerable<Attribute> childFieldAttributes = null,
      string tooltip = null,
      ArrayList rowValues = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      Func<DataField, KeyValuePair<int, object>, (bool success, string message)>[] entryValidations = null,
      Func<DataField, ArrayList, (bool success, string message)>[] fullValidations = null
    ) : base(
      DataFieldSet.DisplayType.FieldList,
      name,
      tooltip,
      rowValues, 
      dataKey,
      isReadOnly,
      enable,
      fullValidations
        ?.Select(func => func.CastMiddleType<ArrayList, object>())
    ) {
      DataType = dataType;
      _childFieldAttributes = childFieldAttributes;
      _entryValidations = entryValidations;
    }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      value.Value = new ArrayList((Value as ArrayList).Cast<object>().ToList());
      value.DefaultValue = withCurrentValuesAsNewDefaults 
        ? new ArrayList(Value as ArrayList) 
        : new ArrayList(DefaultValue as ArrayList);
      (value as DataFieldSet)._childFieldAttributes = _childFieldAttributes;

      return value;
    }

    ///<summary><inheritdoc/></summary>
    public bool TryToUpdateValueAtIndex(object key, object newValue, out string resultMessage) {
      throw new NotImplementedException();
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