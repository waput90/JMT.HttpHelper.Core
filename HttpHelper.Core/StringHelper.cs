using Newtonsoft.Json;
using System;

namespace JMT.HttpHelper.Core
{
    internal static class StringHelper
    {
        internal static bool IsNull<TData>(this TData compare1) where TData : new()
        {
            try
            {
                if (compare1 == null)
                    return true;
                else
                {
                    var comparer1 = JsonConvert.SerializeObject(compare1);
                    var comparer2 = JsonConvert.SerializeObject(new TData());
                    return comparer1.Equals(comparer2);
                }
            }
            catch (Exception)
            {

            }
            return default;
        }

        internal static TModel DeserializeTo<TModel>(this string stringValue)
        {
            //wrapper if there are error will just return null instead
            try
            {
                return JsonConvert.DeserializeObject<TModel>(stringValue);
            }
            catch (Exception)
            {
                return default;
            }
        }

    }
}
