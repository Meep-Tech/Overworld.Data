using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A simple ux view, with controls, and potentially with multiple pannels that contain content.
  /// </summary>
  public class UxView : IEnumerable<KeyValuePair<UxPannel.Tab, UxPannel>> {
    internal OrderedDictionary<UxPannel.Tab, UxPannel> _pannels
      = new();
    internal IReadOnlyDictionary<string, UxDataField> _fields;

    /// <summary>
    /// Extra context you can provide to the component.
    /// </summary>
    public IReadOnlyDictionary<string, object> Context {
      get;
      private set;
    }

    /// <summary>
    /// The main title of this view.
    /// </summary>
    public string MainTitle {
      get;
      private set;
    }

    /// <summary>
    /// If this view has more than one pannel.
    /// </summary>
    public bool HasMultiplePannels
      => _pannels.Count > 1;

    /// <summary>
    /// The number of tabs/pannels
    /// </summary>
    public int NumberOfTabs
      => _pannels.Count;

    internal UxView(string mainTitle) {
      MainTitle = mainTitle;
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public UxPannel GetPannel(UxPannel.Tab tab)
      => _pannels[tab];

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public UxPannel GetPannel(string tabKey, out UxPannel.Tab tab) {
      tab = _pannels.Keys.First(key => key.Key == tabKey);
      return GetPannel(tab);
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public UxPannel GetPannel(int tabIndex, out UxPannel.Tab tab) {
      tab = _pannels.Keys.Skip(tabIndex).FirstOrDefault();
      return GetPannel(tab);
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public UxPannel GetPannel(int tabIndex)
      => _pannels[tabIndex];

    /// <summary>
    /// Copy this view layout and current values.
    /// </summary>
    public UxView Copy() {
      UxView newView = new(MainTitle);
      // copy the layout.
      OrderedDictionary<UxPannel.Tab, UxPannel> newPannelDic = new();
      _pannels.ForEach(pannel => newPannelDic.Add(pannel.Key, pannel.Value.Copy(newView)));
      // find the new fields.
      Dictionary<string, UxDataField> copiedFields = new();
      _fields = newPannelDic.Values
        .SelectMany(pannel => pannel.Elements._getExpandedFieldsByKey())
        .ToDictionary(
          e => e.Key,
          e => e.Value
        );

      // set up and return the copy
      newView.Context = Context;
      newView._pannels = newPannelDic;
      return newView;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<KeyValuePair<UxPannel.Tab, UxPannel>> GetEnumerator()
      => _pannels.ToList().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}