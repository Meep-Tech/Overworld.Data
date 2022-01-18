using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Used to build the simple Ux
  /// </summary>
  public class UxViewBuilder {
    int _colCount = 0;
    UxView _view;
    UxPannel.TabData? _currentPannelTab;
    List<IUxViewElement> _currentPannelEntries = new();
    Dictionary<string, UxDataField> _fieldsByKey = new ();
    OrderedDictionary<UxPannel.TabData, UxPannel> _compiledPannels = new();

    /// <summary>
    /// The current panel tab this builder is working on
    /// </summary>
    public UxPannel.TabData CurrentPannelTab {
      get => _currentPannelTab ??= new UxPannel.TabData(_view.MainTitle);
      private set => _currentPannelTab = value;
    }

    /// <summary>
    /// Make a new simple Overworld Ux builder.
    /// </summary>
    public UxViewBuilder(string mainTitle) {
      _view = new(mainTitle);
    } UxViewBuilder() {}

    /// <summary>
    /// Build a default field using the field
    /// </summary>
    public static UxDataField BuildDefaultField(FieldInfo field) 
      => BuildDefaultField(field.FieldType, field);

    /// <summary>
    /// Build a default field using the property
    /// </summary>
    public static UxDataField BuildDefaultField(PropertyInfo prop)
      => BuildDefaultField(prop.PropertyType, prop);

    static UxDataField BuildDefaultField(System.Type fieldType, MemberInfo fieldInfo = null, string fieldNameOverride = null) {
      bool isNumeric = false;
      TooltipAttribute tooltip = fieldInfo?.GetCustomAttribute<TooltipAttribute>();
      SelectableAttribute selectableData = fieldInfo?.GetCustomAttribute<SelectableAttribute>();
      DefaultValueAttribute defaultValue = fieldInfo?.GetCustomAttribute<DefaultValueAttribute>();

      string name = fieldNameOverride ?? null;
      UxDataField.DisplayType? type = null;
      object validation = null;
      string tooltipText = tooltip?._text;
      object defaultFieldValue = null;

      /// Seletctable dropdown fields
      if(selectableData is not null || fieldType.IsEnum) {
        type = (selectableData?._isMultiselect ?? false)
          ? UxDataField.DisplayType.SelectManyDropdown
          : UxDataField.DisplayType.SelectOneOfManyDropdown;
        validation = selectableData?._options ?? Enum.GetValues(fieldType).Cast<object>().Select(e => {
          // TODO: make this static somewhere:
          // Makes capscase into spaced words text.
          return Regex.Replace(e.ToString(), "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
        });
      } else

      /// Potential text input fields
      // Numeric:
      if(fieldType == typeof(int) || fieldType == typeof(double) || fieldType == typeof(float)) {
        RangeSliderAttribute rangeSliderData;
        if((rangeSliderData = fieldInfo?.GetCustomAttribute<RangeSliderAttribute>()) != null) {
          type = UxDataField.DisplayType.RangeSlider;
          validation = (rangeSliderData._min, rangeSliderData._max);
        } else {
          if(fieldType == typeof(int)) {
            validation = (Func<object, bool>)(value => int.TryParse(value as string, out _));
          } else
            validation = (Func<object, bool>)(value => double.TryParse(value as string, out _));
        }

        isNumeric = true;
        defaultFieldValue = 0;
      }//String
      else if(fieldType == typeof(string)) {
        type = UxDataField.DisplayType.Text;
        defaultFieldValue = "";
      } //Char
      else if(fieldType == typeof(char)) {
        validation = (Func<object, bool>)(value => ((value as string)?.Length ?? 0) <= 1);
        type = UxDataField.DisplayType.Text;
        defaultFieldValue = "";
      }
      /// Boolean toggle
      else if(fieldType == typeof(bool)) {
        type = UxDataField.DisplayType.Toggle;
        defaultFieldValue = false;
      } /// Color Selector
      else if(fieldType == typeof(Color)) {
        type = UxDataField.DisplayType.ColorPicker;
        defaultFieldValue = new Color();
      } /// Key value list
      else if(typeof(IDictionary).IsAssignableFrom(fieldType) || fieldType.IsAssignableToGeneric(typeof(IReadOnlyDictionary<,>))) {
        // TODO: dictionarys should be special for executables
        Dictionary<string, object> @default = new();
        if(defaultValue?.Value is not null && defaultValue.Value is object[] defaultItems) {
          for(int entryIndex = 0; entryIndex < defaultItems.Count(); entryIndex++) {
            @default.Add(defaultItems[entryIndex++] as string, defaultItems[entryIndex]);
          }
        }

        return new Ux.Simple.UxKeyValueSet(
          name: fieldInfo?.Name,
          extraEntryValidation: validation as Func<KeyValuePair<string, object>, bool>,
          tooltip: tooltip?._text,
          rows: @default
        );
      } else if(fieldType == typeof(Overworld.Data.Executeable)) {
        // TODO: throw new NotImplementedException($"Executables not yet implimented");
      } else if(typeof(IEnumerable<Data.Executeable>).IsAssignableFrom(fieldType)) {
        // TODO: throw new NotImplementedException($"Executables not yet implimented");
      } else if(fieldType.IsAssignableToGeneric(typeof(IEnumerable<>))) {
        // TODO: throw new NotImplementedException($"Collections not yet implimented");
      }

      ///No match found
      if(!type.HasValue) {
        return null;
      }

      return new UxDataField(
        name: name ?? fieldInfo.Name,
        type: type.Value,
        validation: validation,
        tooltip: tooltipText,
        value: defaultFieldValue ?? defaultValue?.Value ?? null
      );
    }

    /// <summary>
    /// Copy this builder.
    /// </summary>
    public UxViewBuilder Copy() 
      => new () {
        _view = _view,
        _currentPannelTab = _currentPannelTab
      };

    /// <summary>
    /// reset and empty this builder
    /// </summary>
    public UxViewBuilder Clear(string newMainTitle = null) {
      _currentPannelEntries = null;
      _currentPannelEntries = new List<IUxViewElement>();
      _view = new(newMainTitle ?? _view.MainTitle);
      _currentPannelTab = null;
      return this;
    }

    /// <summary>
    /// Add a data field.
    /// </summary>
    public UxViewBuilder AddField(UxDataField field) {
      _addElementToCurrentPannel(field);

      return this;
    }

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items.
    /// </summary>
    public UxViewBuilder AddRow(params UxDataField[] fieldsInRow)
      => AddRow((IEnumerable<UxDataField>)fieldsInRow);

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items. Give it a label to the left:
    /// </summary>
    public UxViewBuilder AddRow(UxTitle label, params UxDataField[] fieldsInRow)
      => AddRow(fieldsInRow, label);

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items.
    /// You can also give it a label to the left:
    /// </summary>
    public UxViewBuilder AddRow(IEnumerable<UxDataField> fieldsInRow, UxTitle label = null) {
      _addElementToCurrentPannel(
        new UxRow(
          fieldsInRow,
          label
        )
      );

      return this;
    }

    /// <summary>
    /// Add a formatted column of controls, inputs, or Ux items.
    /// </summary>
    public UxViewBuilder AddColumn(params IUxViewElement[] fieldsInColumn)
      => AddColumn((IEnumerable<IUxViewElement>)fieldsInColumn);

    /// <summary>
    /// Add a formatted column of controls, inputs, or Ux items. Give it a label to the left:
    /// </summary>
    public UxViewBuilder AddColumn(UxTitle label, params IUxViewElement[] fieldsInColumn)
      => AddColumn(fieldsInColumn, label);

    /// <summary>
    /// Add a formatted column of controls, inputs, or Ux items.
    /// You can also give it a label to the left:
    /// </summary>
    public UxViewBuilder AddColumn(IEnumerable<IUxViewElement> fieldsInColumn, UxTitle label = null) {
      _addElementToCurrentPannel(
        new UxColumn(
          fieldsInColumn,
          label
        )
      );

      return this;
    }

    /// <summary>
    /// Starts a new pannel with the given name.
    /// If this isn't called first, everuthing before is put in a default pannel has the main tilte's name.
    /// </summary>
    public UxViewBuilder StartNewPannel(UxPannel.TabData tabData) {
      _compiledPannels.Add(CurrentPannelTab, _buildPannel());
      _colCount = 0;
      CurrentPannelTab = tabData;

      return this;
    }

    /// <summary>
    /// Build and return the view.
    /// </summary>
    public UxView Build() {
      _view._pannels = _compiledPannels;
      _view._fields = _fieldsByKey;

      return _view;
    }

    /// <summary>
    /// Add an element like a pre-built column or row or field to the current pannel.
    /// </summary>
    internal UxViewBuilder _addElementToCurrentPannel(IUxViewElement element) {
      if(element is UxPannel) {
        throw new System.ArgumentException($"Cannot add a pannel to a pannel. You can add pannels to views using the addnextpannel function of the builder");
      }
      if(element is UxColumn) {
        if(_colCount >= 3) {
          throw new System.Exception($"Simple UX Pannel cannot have more than 3 rows.");
        }
        _colCount++;
      }

      _currentPannelEntries.Add(element);

      /// add all the sub entries of the key value set and link them:
      if(element is UxKeyValueSet keyValueSet) {
        // TODO: allow people to apply certain attributes, like the range display one, to a dictionary, and it will apply to the children.
        foreach(KeyValuePair<string, object> entry in keyValueSet.Value as Dictionary<string, object>) {
          UxDataField field = BuildDefaultField(entry.Value.GetType(), fieldNameOverride: keyValueSet.Name + ":" + entry.Key);
          field._controllerField = keyValueSet;
          _currentPannelEntries.Add(field);
        }
      }
      return this;
    }

    UxPannel _buildPannel() {
      _fieldsByKey = _fieldsByKey
        .Merge(_currentPannelEntries._getExpandedFieldsByKey());

      return new(_currentPannelEntries);
    }
  }

  internal static class UxBuilderExtensions {
    internal static Dictionary<string, UxDataField> _getExpandedFieldsByKey(this IEnumerable<IUxViewElement> elements)
      => elements.SelectMany((IUxViewElement entry)
          => entry is UxColumn column
            ? column.SelectMany(entry =>
              entry is UxRow row
                ? row
                : (IEnumerable<UxDataField>)(new[] { entry }))
            : entry is UxRow row
              ? row
              : (IEnumerable<UxDataField>)(new[] { entry }
          )
        ).ToDictionary(entry => entry.DataKey);
  }
}
