using System;
using System.Collections.Generic;

namespace Domain.Helpers
{
  /// <summary>
  /// Represents a dictionary list, that is a list of key value pair values.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  [Serializable]
  public class DictionaryList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
  {
    public DictionaryList()
    {

    }
    public DictionaryList(IEnumerable<KeyValuePair<TKey, TValue>> list)
        : base(list)
    {
    }

    /// <summary>
    /// Adds the given key and value to the current dictionary list.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
      this.Add(new KeyValuePair<TKey, TValue>(key, value));
    }
  }

}
