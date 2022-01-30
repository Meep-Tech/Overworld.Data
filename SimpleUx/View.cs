using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A simple ux view, with controls, and potentially with multiple pannels that contain content.
  /// </summary>
  public class View : IEnumerable<KeyValuePair<Pannel.Tab, Pannel>> {
    internal OrderedDictionary<string, Pannel> _pannels
      = new();
    internal OrderedDictionary<string, Pannel.Tab> _tabs
      = new();
    internal IReadOnlyDictionary<string, DataField> _fields;

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
    public Title MainTitle {
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

    internal View(Title mainTitle) {
      MainTitle = mainTitle;
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel GetPannel(Pannel.Tab tab)
      => _pannels[tab.Key];

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel GetPannel(string tabKey, out Pannel.Tab tab) {
      tab = GetTab(tabKey);
      return GetPannel(tab);
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel GetPannel(int tabIndex, out Pannel.Tab tab) {
      tab = GetTab(tabIndex);
      return GetPannel(tab);
    }

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel GetPannel(int tabIndex)
      => _pannels[tabIndex];

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel.Tab GetTab(int tabIndex)
      => _tabs[tabIndex];

    /// <summary>
    /// Get the pannel at the given tab
    /// </summary>
    public Pannel.Tab GetTab(string key)
      => _tabs[key];

    /// <summary>
    /// Get a field by key
    /// </summary>
    public DataField GetField(string key)
      => _fields[key];

    /// <summary>
    /// Get a field value by key
    /// </summary>
    public object GetFieldValue(string key)
      => _fields[key].Value;

    /// <summary>
    /// Get a field value by key
    /// </summary>
    public TValue GetFieldValue<TValue>(string key)
      => (TValue)_fields[key].Value;

    /// <summary>
    /// Copy this view layout and current values.
    /// </summary>
    public View Copy() {
      OrderedDictionary<string, Pannel> newPannelDic = new();
      OrderedDictionary<string, Pannel.Tab> newTabDic = new();
      View newView = new(MainTitle) {
        _pannels = newPannelDic,
        _tabs = newTabDic
      };

      // copy the layout.
      _pannels.ForEach(pannel => newPannelDic.Add(pannel.Key, pannel.Value.Copy(newView)));
      _tabs.ForEach(tab => newTabDic.Add(tab.Key, tab.Value.Copy(newView)));
      // find the new fields.
      Dictionary<string, DataField> copiedFields = new();
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
    public IEnumerator<KeyValuePair<Pannel.Tab, Pannel>> GetEnumerator()
      => _pannels.Select(_pannelData => new KeyValuePair<Pannel.Tab, Pannel>(_tabs[_pannelData.Key], _pannelData.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}