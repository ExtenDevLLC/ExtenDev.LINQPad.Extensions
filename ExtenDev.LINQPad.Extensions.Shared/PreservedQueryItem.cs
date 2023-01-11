using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class PreservedQueryItem<T>
    {
        private static Dictionary<string, T> alreadyLoaded = new Dictionary<string, T>();

        private static string GetKey(string? name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return $"PreservedQueryItem_{name}";
        }

        public static T GetOrCreateValue(Func<T> factory, [CallerMemberName] string? name = null)
        {
            var dataKey = GetKey(name);

            T ret;

            if (!alreadyLoaded.TryGetValue(dataKey, out ret))
            {

                var data = AppDomain.CurrentDomain.GetData(dataKey);
                if (data is T)
                {
                    ret = (T)data;
                }
                else
                {
                    ret = factory();
                    AppDomain.CurrentDomain.SetData(dataKey, ret);
                }

                alreadyLoaded.Add(dataKey, ret);
            }

            return ret;
        }

        public static void SetValue(T value, [CallerMemberName] string name = null)
        {
            var dataKey = GetKey(name);

            AppDomain.CurrentDomain.SetData(dataKey, value);
            alreadyLoaded[dataKey] = value;
        }
    }
}
