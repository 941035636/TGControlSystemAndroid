using System.Collections.Generic;

public static class DictionaryExtension
{
	public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
	{
		dict.TryGetValue(key, out var value);
		return value;
	}
}
