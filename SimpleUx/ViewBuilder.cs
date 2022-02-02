﻿using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using Overworld.Utility;
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
  public class ViewBuilder {
    int _colCount = 0;
    View _view;
    Pannel.Tab? _currentPannelTab;
    OrderedDictionary<Pannel.Tab, Pannel> _compiledPannels = new();
    List<Column> _currentPannelColumns;
    List<IUxViewElement> _currentColumnEntries;
    Dictionary<string, DataField> _fieldsByKey = new ();

    /// <summary>
    /// The current panel tab this builder is working on
    /// </summary>
    public Pannel.Tab CurrentPannelTab {
      get => _currentPannelTab ??= new Pannel.Tab(_view.MainTitle.Text) { View = _view };
      private set => _currentPannelTab = value;
    }

    /// <summary>
    /// The current panel tab this builder is working on
    /// </summary>
    public Title CurrentColumnLabel {
      get;
      private set;
    }

    /// <summary>
    /// Make a new simple Overworld Ux builder.
    /// </summary>
    public ViewBuilder(string mainTitle) {
      _view = new(new(mainTitle));
    } ViewBuilder() {}

    /// <summary>
    /// Make a new simple Overworld Ux builder.
    /// </summary>
    public ViewBuilder(Title mainTitle) {
      _view = new(mainTitle);
    }

    /// <summary>
    /// Build a default field using the field
    /// </summary>
    public static DataField BuildDefaultField(FieldInfo field) 
      => BuildDefaultField(field.FieldType, field);

    /// <summary>
    /// Build a default field using the property
    /// </summary>
    public static DataField BuildDefaultField(PropertyInfo prop)
      => BuildDefaultField(prop.PropertyType, prop);

    static DataField BuildDefaultField(System.Type fieldType, MemberInfo fieldInfo = null, string fieldNameOverride = null, string fieldDataKeyOverride = null) {
      // get relevant attributes:
      TooltipAttribute tooltipAttribute = fieldInfo?.GetCustomAttribute<TooltipAttribute>();
      DropdownAttribute selectableData = fieldInfo?.GetCustomAttribute<DropdownAttribute>();
      DefaultValueAttribute defaultValue = fieldInfo?.GetCustomAttribute<DefaultValueAttribute>();
      ValidationAttribute validationAttribute = fieldInfo?.GetCustomAttribute<ValidationAttribute>();
      EnableIfAttribute enabledAttribute = fieldInfo?.GetCustomAttribute<EnableIfAttribute>();
      RangeSliderAttribute rangeSliderData = null;

      string name = fieldNameOverride ?? null;
      DataField.DisplayType? type = null;
      object validation = validationAttribute?._validation ?? null;
      Func<DataField, object, bool> validationFunction = null;
      string tooltipText = tooltipAttribute?._text;
      object defaultFieldValue = defaultValue?.Value;
      bool isClamped = false;
      Func<DataField, View, bool> enabled = null;

      // Check if the validation points to a local method of some kind that fits what we need
      if(validation is not null && validation is string validationFunctionName) {
        validationFunction = (f, v) => (bool)fieldInfo.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == validationFunctionName)
          .Where(method => method.GetParameters().Length == 1)
          .Where(method => method.ReturnType == typeof(bool) || method.ReturnType == typeof((bool, string))).First().Invoke(null, new[] { f, v });
      }

      // check the is enabled functionality
      if(enabledAttribute is not null) {
        if(fieldInfo?.DeclaringType.GetProperty(enabledAttribute._validationFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) is PropertyInfo property) {
          if(property.PropertyType != typeof(bool)) {
            enabled = (Field, Pannel) => (bool)property.GetValue(Field);
          } else if(property.PropertyType != typeof(Func<DataField, View, bool>)) {
            enabled = (Field, Pannel) => ((Func<DataField, View, bool>)property.GetValue(Field)).Invoke(Field, Pannel);
          } else
            throw new NotSupportedException($"Cannot use the field {property.Name} as an isEnabled determination field for simple ux.");
        } else if(fieldInfo?.DeclaringType.GetField(enabledAttribute._validationFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) is FieldInfo field) {
          if(field.FieldType != typeof(bool)) {
            enabled = (Field, Pannel) => (bool)field.GetValue(Field);
          } else if(field.FieldType != typeof(Func<DataField, View, bool>)) {
            enabled = (Field, Pannel) => ((Func<DataField, View, bool>)field.GetValue(Field)).Invoke(Field, Pannel);
          } else
            throw new NotSupportedException($"Cannot use the field {field.Name} as an isEnabled determination field for simple ux.");
        } else
          throw new NotSupportedException($"Cannot use the field {enabledAttribute?._validationFieldName ?? "NULL"} as an isEnabled determination field for simple ux.");
      }

      /// Seletctable dropdown fields
      if(selectableData is not null || fieldType.IsEnum) {
        selectableData ??= new DropdownAttribute(selectableData?._isMultiselect ?? false);
        type = DataField.DisplayType.Dropdown;
        Dictionary<string, object> options = selectableData?._options;
        if(fieldType.IsEnum) {
          Array enumValues = Enum.GetValues(fieldType);
          selectableData._options ??= new(Enum.GetValues(fieldType).Cast<object>().Zip(
            enumValues.Cast<char>(), (n, v) => new KeyValuePair<string, object>(n.ToString().ToDisplayCase(), v)
          ));
          defaultFieldValue ??= Activator.CreateInstance(fieldType);
        }
      } else
      /// Potential text input fields
      // Numeric:
      if((isClamped = fieldType == typeof(int)) || fieldType == typeof(double) || fieldType == typeof(float)) {
        Func<double, bool> rangeValidation = null;
        bool? isIntClamped = null;
        (float min, float max)? range = validation is (float mi, float ma)
          ? (mi, ma)
          : null;

        // see if there's range validation to be added.
        if(range is not null || ((rangeSliderData = fieldInfo?.GetCustomAttribute<RangeSliderAttribute>()) != null)) {
          type = rangeSliderData is not null 
            ? DataField.DisplayType.RangeSlider
            : type;
          rangeValidation = value
            => value < (rangeSliderData?._min ?? range?.min) && value > (rangeSliderData?._max ?? range?.max);
          isIntClamped = rangeSliderData?._isClampedToInt;
        } else {
          type = DataField.DisplayType.Text;
        }
        isIntClamped ??= fieldType == typeof(int);
        rangeSliderData ??= new RangeSliderAttribute(0, isIntClamped.Value ? 100 : 1, isIntClamped);

        Func<DataField, object, bool> numericValidation = isIntClamped.Value
              ? (field, value) => int.TryParse(value as string, out int val) && (rangeValidation?.Invoke(val) ?? true)
              : (field, value) => double.TryParse(value as string, out double val) && (rangeValidation?.Invoke(val) ?? true);
        validationFunction = validationFunction is not null
          ? numericValidation + validationFunction
          : numericValidation;

        defaultFieldValue ??= 0;
      }//String
      else if(fieldType == typeof(string)) {
        type = DataField.DisplayType.Text;
        defaultFieldValue ??= "";
      } //Char
      else if(fieldType == typeof(char)) {
        validation = (Func<object, bool>)(value => ((value as string)?.Length ?? 0) <= 1);
        type = DataField.DisplayType.Text;
        defaultFieldValue ??= "";
      }
      /// Boolean toggle
      else if(fieldType == typeof(bool)) {
        type = DataField.DisplayType.Toggle;
        defaultFieldValue ??= false;
      } /// Color Selector
      else if(fieldType == typeof(Color)) {
        type = DataField.DisplayType.ColorPicker;
        defaultFieldValue ??= new Color();
      } /// Key value list
      else if(typeof(IDictionary).IsAssignableFrom(fieldType) || fieldType.IsAssignableToGeneric(typeof(IReadOnlyDictionary<,>))) {
        // TODO: dictionarys should be special for executables
        Dictionary<string, object> @default = new();
        if(defaultValue?.Value is not null && defaultValue.Value is object[] defaultItems) {
          for(int entryIndex = 0; entryIndex < defaultItems.Count(); entryIndex++) {
            @default.Add(defaultItems[entryIndex++] as string, defaultItems[entryIndex]);
          }
        }

        defaultFieldValue ??= @default;
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

      return DataField.Make(
        title: name ?? fieldInfo?.Name,
        type: type.Value,
        validation: validationFunction,
        tooltip: tooltipText,
        value: defaultFieldValue,
        enabledIf: enabled,
        attributes: new Attribute[] {
          tooltipAttribute,
          selectableData,
          defaultValue,
          validationAttribute,
          enabledAttribute,
          rangeSliderData
        }.ToDictionary(attribute => attribute.GetType()),
        dataKey: fieldDataKeyOverride
      );
    }

    /// <summary>
    /// Copy this builder.
    /// </summary>
    public ViewBuilder Copy() {
      View clone = _view.Copy();
      return new() {
        _view = clone,
        _currentPannelTab = _currentPannelTab?.Copy(clone),
        _currentPannelColumns = _currentPannelColumns.Select(entry => entry.Copy(clone)).ToList()
      };
    }

    /// <summary>
    /// reset and empty this builder
    /// </summary>
    public ViewBuilder Clear(Title newMainTitle = null) {
      _currentPannelColumns = null;
      _currentPannelColumns = new List<Column>();
      _currentColumnEntries = new List<IUxViewElement>();
      _view = new(newMainTitle ?? _view.MainTitle);
      _currentPannelTab = null;
      return this;
    }

    /// <summary>
    /// Add a data field.
    /// </summary>
    public ViewBuilder AddField(DataField field) {
      _addElementToCurrentPannel(field);

      return this;
    }

    /// <summary>
    /// Add a header inside of a column.
    /// </summary>
    public ViewBuilder AddHeader(Title inColumnHeader) {
      _addElementToCurrentPannel(inColumnHeader);

      return this;
    }

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items.
    /// </summary>
    public ViewBuilder AddRow(params DataField[] fieldsInRow)
      => AddRow((IEnumerable<DataField>)fieldsInRow);

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items. Give it a label to the left:
    /// </summary>
    public ViewBuilder AddRow(Title label, params DataField[] fieldsInRow)
      => AddRow(fieldsInRow, label);

    /// <summary>
    /// Add a formatted row of controls, inputs, or Ux items.
    /// You can also give it a label to the left:
    /// </summary>
    public ViewBuilder AddRow(IEnumerable<DataField> fieldsInRow, Title label = null) {
      _addElementToCurrentPannel(
        new Row(
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
    public ViewBuilder AddColumn(params IUxViewElement[] fieldsInColumn)
      => AddColumn((IEnumerable<IUxViewElement>)fieldsInColumn);

    /// <summary>
    /// Add a formatted column of controls, inputs, or Ux items. Give it a label to the left:
    /// </summary>
    public ViewBuilder AddColumn(Title label, params IUxViewElement[] fieldsInColumn)
      => AddColumn(fieldsInColumn, label);

    /// <summary>
    /// Add a formatted column of controls, inputs, or Ux items.
    /// You can also give it a label to the left:
    /// </summary>
    public ViewBuilder AddColumn(IEnumerable<IUxViewElement> fieldsInColumn, Title label = null) {
      _addElementToCurrentPannel(
        new Column(
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
    public ViewBuilder StartNewColumn(Title label = null) {
      if(_currentColumnEntries is not null) {
        _buildColumn();
      }
      CurrentColumnLabel = label;
      _currentColumnEntries = new();

      return this;
    }

    /// <summary>
    /// Adds a whole pre-built pannel after the current one (or first if this is starting).
    /// </summary>
    public ViewBuilder AddPannel(Pannel.Tab tabData, Pannel pannel) {
      // if theres pending entries in a column, build them
      if(_currentColumnEntries is not null) {
        _buildColumn();
      }

      if(_currentPannelColumns is not null) {
        _buildPannel();
      }

      _compiledPannels.Add(tabData, pannel);
      StartNewPannel(CurrentPannelTab);

      return this;
    }

    /// <summary>
    /// Starts a new pannel with the given name.
    /// If this isn't called first, everuthing before is put in a default pannel has the main tilte's name.
    /// </summary>
    public ViewBuilder StartNewPannel(Pannel.Tab tabData) {
      if(_currentPannelColumns is not null) {
        _buildPannel();
      }
      _colCount = 0;
      CurrentPannelTab = tabData;
      _currentPannelColumns = new();
      tabData.View = _view;

      return this;
    }

    /// <summary>
    /// Sets the current pannel tab's data if there isn't any
    /// </summary>
    public ViewBuilder SetCurrentPannelTab(Pannel.Tab tabData) {
      if(_currentPannelTab is null) {
        _currentPannelTab = tabData;
      } else
        throw new Exception($"No current tab yet, use Start New Pannel instead");

      return this;
    }

    /// <summary>
    /// Sets the current columns header if there isn't one already set
    /// </summary>
    public ViewBuilder SetCurrentColumnHeader(Title label) {
      if(CurrentColumnLabel is null) {
        CurrentColumnLabel = label;
      } else
        throw new Exception($"The current column already has a label: {CurrentColumnLabel?.Text}");

      return this;
    }

    /// <summary>
    /// Build and return the view.
    /// </summary>
    public View Build() {
      /// finish building:
      if(_currentColumnEntries is not null) {
        _buildColumn();
      }
      if(_currentPannelColumns is not null) {
        _buildPannel();
      }

      /// collect and apply data
      _view._tabs = new(); 
      _view._pannels = new();
      foreach((Pannel.Tab tab, Pannel pannel) in _compiledPannels) {
        _view._tabs.Add(tab.Key, tab);
        _view._pannels.Add(tab.Key, pannel);
      }
      _view._fields = _fieldsByKey;

      return _view;
    }

    /// <summary>
    /// Add an element like a pre-built column or row or field to the current pannel.
    /// </summary>
    internal ViewBuilder _addElementToCurrentPannel(IUxViewElement element) {
      // start a new pannel if we don't have one.
      if(_currentPannelColumns is null) {
        StartNewPannel(CurrentPannelTab);
      }

      // can't add pannels to pannels
      if(element is Pannel) {
        throw new System.ArgumentException($"Cannot add a SimpleUx Pannel to another Pannel. You can add pannels to views using the addnextpannel function of the builder");
      }

      // for columns
      if(element is Column column) {
        if(_currentPannelColumns.Count >= 3) {
          throw new System.Exception($"A Simple UX Pannel cannot have more than 3 Columns.");
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

      /// add all the sub entries of the key value set and link them:
      if(element is DataFieldKeyValueSet keyValueSet) {
        // TODO: allow people to apply certain attributes, like the range display one, to a dictionary, and it will apply to the children.
        foreach(KeyValuePair<string, object> entry in keyValueSet.Value as Dictionary<string, object>) {
          DataField field = BuildDefaultField(entry.Value.GetType(), null, fieldNameOverride: "", fieldDataKeyOverride: keyValueSet.Name + "::" + entry.Key);
          field._controllerField = keyValueSet;
          _currentColumnEntries.Add(field);
        }
      } else
        _currentColumnEntries.Add(element);

      return this;
    }

    Column _buildColumn() {
      try {
        _fieldsByKey = _fieldsByKey
          .Merge(_currentColumnEntries._getExpandedFieldsByKey());

        Column col = new(_currentColumnEntries, CurrentColumnLabel) {
          View = _view
        };

        _currentColumnEntries = null;
        CurrentColumnLabel = null;

        _currentPannelColumns.Add(col);
        return col;
      } catch(Exception e) {
        throw new ArgumentException($"Failed to build SimpleUx Column #{_currentPannelColumns?.Count.ToString() ?? "null!"} on Pannel with key: {_currentPannelTab?.Key ?? "null!"}:\n {e}", e.InnerException);
      }
    }

    Pannel _buildPannel() {
      try {
        if(_currentColumnEntries?.Any() ?? false) {
          _buildColumn();
        }

        Pannel pannel = new(_currentPannelColumns, CurrentPannelTab) {
          View = _view
        };

        _compiledPannels.Add(pannel.Key, pannel);
        _currentPannelColumns = null;
        _currentPannelTab = null;
        return pannel;
      } catch (Exception e) {
        throw new ArgumentException($"Failed to build SimpleUx Pannel with key: {_currentPannelTab?.Key ?? "null!"}:\n {e}", e.InnerException);
      }
    }
  }

  internal static class BuilderExtensions {
    internal static Dictionary<string, DataField> _getExpandedFieldsByKey(this IEnumerable<IUxViewElement> elements)
      => elements.SelectMany((IUxViewElement entry)
          => entry is Column column
            ? column.SelectMany(_getExpandedFieldsByKey)
            : entry._getExpandedFieldsByKey()
        ).ToDictionary(entry => entry.DataKey);

    static IEnumerable<DataField> _getExpandedFieldsByKey(this IUxViewElement entry) {
      return entry is Row row
        ? row
        : entry is DataField field && !field.IsReadOnly
          ? new[] { field }
          : Enumerable.Empty<DataField>();
    }
  }
}