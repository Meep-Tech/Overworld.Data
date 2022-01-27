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
    UxPannel.Tab? _currentPannelTab;
    OrderedDictionary<UxPannel.Tab, UxPannel> _compiledPannels = new();
    List<UxColumn> _currentPannelColumns;
    List<IUxViewElement> _currentColumnEntries;
    Dictionary<string, UxDataField> _fieldsByKey = new ();

    /// <summary>
    /// The current panel tab this builder is working on
    /// </summary>
    public UxPannel.Tab CurrentPannelTab {
      get => _currentPannelTab ??= new UxPannel.Tab(_view.MainTitle) { View = _view };
      private set => _currentPannelTab = value;
    }

    /// <summary>
    /// The current panel tab this builder is working on
    /// </summary>
    public UxTitle CurrentColumnLabel {
      get;
      private set;
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
      // get relevant attributes:
      TooltipAttribute tooltip = fieldInfo?.GetCustomAttribute<TooltipAttribute>();
      SelectableAttribute selectableData = fieldInfo?.GetCustomAttribute<SelectableAttribute>();
      DefaultValueAttribute defaultValue = fieldInfo?.GetCustomAttribute<DefaultValueAttribute>();
      ValidationAttribute validationAttribute = fieldInfo?.GetCustomAttribute<ValidationAttribute>();
      EnableIfAttribute enabledAttribute = fieldInfo?.GetCustomAttribute<EnableIfAttribute>();

      string name = fieldNameOverride ?? null;
      UxDataField.DisplayType? type = null;
      object validation = validationAttribute?._validation ?? null;
      string tooltipText = tooltip?._text;
      object defaultFieldValue = null;
      Func<UxDataField, UxView, bool> enabled = null;

      // Check if the validation points to a local method of some kind that fits what we need
      if(validation is not null && validation is string validationFunctionName) {
        validation = fieldInfo.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == validationFunctionName)
          .Where(method => method.GetParameters().Length == 1)
          .Where(method => method.ReturnType == typeof(bool) || method.ReturnType == typeof((bool, string))).FirstOrDefault() ?? validation;
      }

      // check the is enabled functionality
      if(enabledAttribute is not null) {
        if(fieldInfo.DeclaringType.GetProperty(enabledAttribute._validationFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) is PropertyInfo property) {
          if(property.PropertyType != typeof(bool)) {
            enabled = (uxField, uxPannel) => (bool)property.GetValue(uxField);
          } else if(property.PropertyType != typeof(Func<UxDataField, UxView, bool>)) {
            enabled = (uxField, uxPannel) => ((Func<UxDataField, UxView, bool>)property.GetValue(uxField)).Invoke(uxField, uxPannel);
          } else
            throw new NotSupportedException($"Cannot use the field {property.Name} as an isEnabled determination field for simple ux.");
        } else if(fieldInfo.DeclaringType.GetField(enabledAttribute._validationFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) is FieldInfo field) {
          if(field.FieldType != typeof(bool)) {
            enabled = (uxField, uxPannel) => (bool)field.GetValue(uxField);
          } else if(field.FieldType != typeof(Func<UxDataField, UxView, bool>)) {
            enabled = (uxField, uxPannel) => ((Func<UxDataField, UxView, bool>)field.GetValue(uxField)).Invoke(uxField, uxPannel);
          } else
            throw new NotSupportedException($"Cannot use the field {field.Name} as an isEnabled determination field for simple ux.");
        } else
          throw new NotSupportedException($"Cannot use the field {enabledAttribute?._validationFieldName ?? "NULL"} as an isEnabled determination field for simple ux.");
      }

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
          rows: @default,
          enable: enabled
        );
      } else if(fieldType == typeof(Overworld.Data.Executeable)) {
        // TODO: throw new NotImplementedException($"Executables not yet implimented");
      } else if(typeof(IEnumerable<Data.Executeable>).IsAssignableFrom(fieldType)) {
        // TODO: throw new NotImplementedException($"Executables not yet implimented");
      } else if(fieldType.IsAssignableToGeneric(typeof(IEnumerable<>))) {
        // TODO: throw new NotImplementedException($"Collections not yet implimented");
      }

      /// No match found
      if(!type.HasValue) {
        return null;
      }

      return new UxDataField(
        name: name ?? fieldInfo.Name,
        type: type.Value,
        validation: validation,
        tooltip: tooltipText,
        value: defaultFieldValue ?? defaultValue?.Value ?? null,
        enable: enabled
      );
    }

    /// <summary>
    /// Copy this builder.
    /// </summary>
    public UxViewBuilder Copy() {
      UxView clone = _view.Copy();
      return new() {
        _view = clone,
        _currentPannelTab = _currentPannelTab?.Copy(clone),
        _currentPannelColumns = _currentPannelColumns.Select(entry => entry.Copy(clone)).ToList()
      };
    }

    /// <summary>
    /// reset and empty this builder
    /// </summary>
    public UxViewBuilder Clear(string newMainTitle = null) {
      _currentPannelColumns = null;
      _currentPannelColumns = new List<UxColumn>();
      _currentColumnEntries = new List<IUxViewElement>();
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
        ) {
          View = _view
        }
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
        ) {
          View = _view
        }
      );

      return this;
    }

    /// <summary>
    /// Starts a new column. Label is optional.
    /// </summary>
    public UxViewBuilder StartNewColumn(UxTitle label = null) {
      _buildColumn();
      CurrentColumnLabel = label;
      _currentColumnEntries = new();

      return this;
    }

    /// <summary>
    /// Adds a whole pre-built pannel after the current one (or first if this is starting).
    /// </summary>
    public UxViewBuilder AddPannel(UxPannel.Tab tabData) {
      // if theres pending entries in a column, build them
      if(_currentColumnEntries is not null && _currentColumnEntries.Any()) {
        _buildColumn();
      }


      return this;
    }

    /// <summary>
    /// Starts a new pannel with the given name.
    /// If this isn't called first, everuthing before is put in a default pannel has the main tilte's name.
    /// </summary>
    public UxViewBuilder StartNewPannel(UxPannel.Tab tabData) {
      _compiledPannels.Add(CurrentPannelTab, _buildPannel());
      _colCount = 0;
      CurrentPannelTab = tabData;
      _currentPannelColumns = new();
      tabData.View = _view;

      return this;
    }

    /// <summary>
    /// Sets the current pannel tab's data if there isn't any
    /// </summary>
    public UxViewBuilder SetCurrentPannelTab(UxPannel.Tab tabData) {
      if(_currentPannelTab is null) {
        _currentPannelTab = tabData;
      } else
        throw new Exception($"No current tab yet, use Start New Pannel instead");

      return this;
    }

    /// <summary>
    /// Sets the current columns header if there isn't one already set
    /// </summary>
    public UxViewBuilder SetCurrentColumnHeader(UxTitle label) {
      if(CurrentColumnLabel is null) {
        CurrentColumnLabel = label;
      } else
        throw new Exception($"The current column already has a label: {CurrentColumnLabel?.Text}");

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
      // start a new pannel if we don't have one.
      if(_currentPannelColumns is null) {
        StartNewPannel(CurrentPannelTab);
      }

      // can't add pannels to pannels
      if(element is UxPannel) {
        throw new System.ArgumentException($"Cannot add a pannel to a pannel. You can add pannels to views using the addnextpannel function of the builder");
      }

      // for columns
      if(element is UxColumn column) {
        if(_currentPannelColumns.Count >= 3) {
          throw new System.Exception($"Simple UX Pannel cannot have more than 3 rows.");
        }
        // if theres pending entries in a column, build them
        if(_currentColumnEntries is not null && _currentColumnEntries.Any()) {
          _buildColumn();
        }
        // add the col to the current pannel.
        _currentPannelColumns.Add(column);
      }

      /// start a new column if we don't have one
      if(_currentColumnEntries is null) {
        StartNewColumn();
      }
      _currentColumnEntries.Add(element);

      /// add all the sub entries of the key value set and link them:
      if(element is UxKeyValueSet keyValueSet) {
        // TODO: allow people to apply certain attributes, like the range display one, to a dictionary, and it will apply to the children.
        foreach(KeyValuePair<string, object> entry in keyValueSet.Value as Dictionary<string, object>) {
          UxDataField field = BuildDefaultField(entry.Value.GetType(), fieldNameOverride: keyValueSet.Name + ":" + entry.Key);
          field._controllerField = keyValueSet;
          _currentColumnEntries.Add(field);
        }
      }
      return this;
    }

    UxColumn _buildColumn() {
      _fieldsByKey = _fieldsByKey
        .Merge(_currentColumnEntries._getExpandedFieldsByKey());

      UxColumn col = new(_currentColumnEntries, CurrentColumnLabel) {
        View = _view
      };

      _currentColumnEntries = null;
      CurrentColumnLabel = null;

      return col;
    }

    UxPannel _buildPannel() {
      if(_currentColumnEntries.Any()) {
        _buildColumn();
      }

      UxPannel pannel = new(_currentPannelColumns, CurrentPannelTab) {
        View = _view
      };

      _currentPannelColumns = null;
      _currentPannelTab = null;

      return pannel;
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
