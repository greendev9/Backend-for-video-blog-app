using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Helpers
{
  public static class CollectionExtensions
  {

    #region Extension Methods

    public static int SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
      return source.Sum(selector) ?? 0;
    }
    public static decimal SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
    {
      return source.Sum(selector) ?? 0;

    }
    public static decimal MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
    {
      return source.Max(selector) ?? 0;

    }
    public static decimal MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
    {
      return source.Min(selector) ?? 0;

    }
    public static decimal AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
    {
      return source.Average(selector) ?? 0;

    }

    public static IEnumerable<T> CombineAll<T>(this IEnumerable<IEnumerable<T>> list)
    {
      return list.Aggregate((current, next) => current.Concat(next));
    }
    public static List<T> SequenceClone<T>(this List<T> list) where T : ICloneable
    {
      return new List<T>(list.Select(item => ((T)item.Clone())));
    }

    public static Dictionary<TKey, TValue> SequenceClone1<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        where TKey : struct
        where TValue : class, ICloneable   //Todo Ori. Fix. Fix clone.
    {
      Dictionary<TKey, TValue> retList = new Dictionary<TKey, TValue>();
      foreach (var kp in dictionary)
      {
        retList.Add(kp.Key, (TValue)kp.Value.Clone());
      }

      return retList;
    }

    public static Dictionary<TKey, TValue> SequenceClone2<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        where TKey : struct
        where TValue : struct   //Todo Ori. Fix. Fix clone.
    {
      Dictionary<TKey, TValue> retList = new Dictionary<TKey, TValue>();
      foreach (var kp in dictionary)
      {
        retList.Add(kp.Key, kp.Value);
      }

      return retList;
    }

    public static Dictionary<TKey, TValue> SequenceClone3<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        where TKey : class, ICloneable   //Todo Ori. Fix. Fix clone.
        where TValue : struct
    {
      Dictionary<TKey, TValue> retList = new Dictionary<TKey, TValue>();
      foreach (var kp in dictionary)
      {
        retList.Add((TKey)kp.Key.Clone(), kp.Value);
      }

      return retList;
    }


    #region DictionaryList Methods


    /// <summary>
    /// Converts the current key value pair list to a conventional dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
    {
      return list.ToDictionary(kp => kp.Key, kp => kp.Value);
    }

    /// <summary>
    /// Returns the value of the given key in the current dictionary or the default value if none exists.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
      TValue value;
      bool isExists = dict.TryGetValue(key, out value);
      if (!isExists)
        return default(TValue);

      return value;
    }

    /// <summary>
    /// Converts the current list to a dictionary list.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DictionaryList<TKey, TElement> ToDictionaryList<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
      List<KeyValuePair<TKey, TElement>> list = source.Select(item => new KeyValuePair<TKey, TElement>(keySelector.Invoke(item), elementSelector.Invoke(item))).ToList();  //.ToDictionary(keySelector, elementSelector).ToList();
      return new DictionaryList<TKey, TElement>(list);
    }

    /// <summary>
    /// Converts the current dictionary to a dictionary list.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static DictionaryList<TKey, TValue> ToDictionaryList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
      return ToDictionaryList(dictionary, kp => kp.Key, kp => kp.Value);
    }

    /// <summary>
    /// Converts the current dictionary list to a conventional dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DictionaryList<TKey, TValue> list)
    {
      return list.ToDictionary(kp => kp.Key, kp => kp.Value);
    }

    /// <summary>
    /// Casts all values of the current dictionary to the given interface type.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="ITValueNew"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static Dictionary<TKey, ITValueNew> CastDictionary<TKey, TValue, ITValueNew>(this Dictionary<TKey, TValue> dictionary)
        where TValue : ITValueNew
    {
      return dictionary.ToDictionary(kp => kp.Key, kp => (ITValueNew)kp.Value);
    }

    /// <summary>
    /// Converts the current dictionary list to a conventional dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Dictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> list)
    {
      int idx = 0;
      return list.ToDictionary(kp => idx++, kp => kp);
    }

    /// <summary>
    /// Swiches the keys of the current dictionary with its values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Dictionary<TValue, TKey> Switch<TKey, TValue>(this Dictionary<TKey, TValue> list)
    {
      return list.ToDictionary(kp => kp.Value, kp => kp.Key);
    }
    #endregion

    /// <summary>
    /// Returns all indices of the elements the match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static IEnumerable<int> FindAllIndices<T>(this List<T> list, Predicate<T> match)
    {
      var indexedDic = list.ToIndexedDictionary();
      return indexedDic.Where(kp => match.Invoke(kp.Value)).Select(kp => kp.Key).ToList();
    }

    /// <summary>
    /// Returns a value for whether the given list does not contain the specified value.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool NotContains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
      return !source.Contains(value);
    }

    /// <summary>
    /// Returns a value for whether any item in the given list satisfies the specified predicate.
    /// also returns the first item that had satisfied that condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="foundItem"></param>
    /// <returns></returns>
    public static bool GetAny<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, out TSource firstMatchedItem)
    {
      firstMatchedItem = source.FirstOrDefault(predicate);
      if (firstMatchedItem != null)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns a value for whether the given list is empty.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
    {
      return !source.Any();
    }

    /// <summary>
    /// Returns a value for whether the given list is not empty.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
    {
      return !source.IsEmpty();
    }

    /// <summary>
    /// Returns a value for whether the given list is empty.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool None<TSource>(this IEnumerable<TSource> source)
    {
      return !source.Any();
    }

    /// <summary>
    /// Returns a value for whether NONE of the elements in the given list satisfy the specified condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      return !source.Any(predicate);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form, and returns the resultant list.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static List<TResult> ToSelectedList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      return source.Select(selector).ToList();
    }
    #endregion


    /// <summary>
    /// Attempts to get the value of the specified key in the current string-to-string dictionary, returns String.Empty otherwise.
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string TryGet(this Dictionary<string, string> dict, string key)
    {
      return TryGet(dict, key, String.Empty);
    }

    /// <summary>
    /// Attempts to get the value of the given key in the current string-to-string dictionary, returns the specified defaultValue otherwise.
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string TryGet(this Dictionary<string, string> dict, string key, string defaultValue)
    {
      string value;
      bool isOK = dict.TryGetValue(key, out value);
      if (isOK)
      {
        return value;
      }
      else
      {
        return defaultValue;
      }
    }

  }

  public interface IConsistencyListItem
  {

    #region Properties

    int ListConsistencyId { get; set; }
    int ConsistentAfter { get; set; }
    #endregion
  }
}