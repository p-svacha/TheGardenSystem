using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
    public static TKey GetWeightedRandomElement<TKey>(this Dictionary<TKey, int> weightDictionary)
    {
        int probabilitySum = weightDictionary.Sum(x => x.Value);
        int rng = UnityEngine.Random.Range(0, probabilitySum);
        int tmpSum = 0;
        foreach (var kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum)
                return kvp.Key;
        }
        throw new System.Exception("No element selected. Check the dictionary for valid weights.");
    }

    public static TKey GetWeightedRandomElement<TKey>(this Dictionary<TKey, float> weightDictionary)
    {
        float probabilitySum = weightDictionary.Sum(x => x.Value);
        float rng = UnityEngine.Random.Range(0, probabilitySum);
        float tmpSum = 0;
        foreach (var kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum)
                return kvp.Key;
        }
        throw new System.Exception("No element selected. Check the dictionary for valid weights.");
    }

    /// <summary>
    /// Increments the integer value associated with the specified key by a given amount.
    /// If the key does not exist, it is added with the increment amount as its initial value.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to operate on.</param>
    /// <param name="key">The key whose value to increment.</param>
    /// <param name="amount">The amount by which to increment the value (defaults to 1).</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="dictionary"/> or <paramref name="key"/> is null.
    /// </exception>
    public static void Increment<TKey>(this IDictionary<TKey, int> dictionary, TKey key, int amount = 1)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (dictionary.TryGetValue(key, out int current))
        {
            dictionary[key] = current + amount;
        }
        else
        {
            dictionary[key] = amount;
        }
    }

    /// <summary>
    /// Increments the integer values in this dictionary by the corresponding values in another dictionary.
    /// Entries in <paramref name="other"/> that do not exist in this dictionary are added.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionaries.</typeparam>
    /// <param name="dictionary">The dictionary whose values will be updated.</param>
    /// <param name="other">The dictionary providing increment amounts by key.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="dictionary"/> or <paramref name="other"/> is null.
    /// </exception>
    public static void IncrementMultiple<TKey>(this IDictionary<TKey, int> dictionary, IDictionary<TKey, int> other)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        foreach (var kvp in other)
        {
            // Reuse Increment to do the work and null/arg checking
            dictionary.Increment(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Decreases the value of the given key by 1. If the key doesn't exist or is <= 0, it throws an error. If they value is at 1, it removes the key.
    /// </summary>
    public static void Decrement<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int amount = 1)
    {
        if (dictionary.ContainsKey(key))
        {
            if (dictionary[key] < amount) throw new System.Exception($"Key {key} is not allowed to be <{amount} when trying to decrement {amount}, but is {dictionary[key]}");
            else dictionary[key] -= amount;
        }
        else throw new System.Exception($"Key {key} doesn't exist.");
    }

    /// <summary>
    /// Adds a value to a dictionary that stores values grouped by a key.
    /// </summary>
    public static void AddToValueList<TKey, T>(this Dictionary<TKey, List<T>> dictionary, TKey key, T value)
    {
        if (dictionary.ContainsKey(key)) dictionary[key].Add(value);
        else dictionary.Add(key, new List<T>() { value });
    }
}