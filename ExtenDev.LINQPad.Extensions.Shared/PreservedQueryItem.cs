using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class PreservedQueryItem<T>
    {
        private static Dictionary<string, T> alreadyLoaded = new Dictionary<string, T>();

        public static T GetOrCreateValue(Func<T> factory, [CallerMemberName] string name = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var dataKey = $"PreservedQueryItem_{name}";

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
    }
}
