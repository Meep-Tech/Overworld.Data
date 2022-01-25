using Meep.Tech.Data.Utility;
using System;

/// <summary>
/// An ordered collection of delegates
/// </summary>
/// <typeparam name="TAction"></typeparam>
public class DelegateCollection<TAction>
  : OrderedDictionary<string, TAction> where TAction
  : Delegate {
}