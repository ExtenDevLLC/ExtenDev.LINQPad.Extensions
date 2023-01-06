using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class ReflectionExtensions
    {

        // TODO: Is this used anywhere? Is it helpful? Does it work?
        public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type)
        {
            var attributeType = typeof(T);
            return type.GetCustomAttributes(attributeType, true)
                .Union(
                    type.GetInterfaces()
                        .SelectMany(interfaceType => interfaceType.GetCustomAttributes(attributeType, true))
                )
                .Distinct()
                .Cast<T>();
        }

        public static MethodInfo GetImplementingMethod(this MethodInfo interfaceMethod, Type classType)
        {
            #region Parameter Validation

            if (interfaceMethod == null)
                throw new ArgumentNullException(nameof(interfaceMethod));
            if (classType == null)
                throw new ArgumentNullException(nameof(classType));
            if (interfaceMethod.DeclaringType == null)
                throw new ArgumentException(nameof(interfaceMethod), $"{nameof(interfaceMethod)} is not defined by a declaring type.");
            if (!interfaceMethod.DeclaringType.IsInterface)
                throw new ArgumentException($"{nameof(interfaceMethod)} is not defined by an interface", nameof(interfaceMethod));
            if (!interfaceMethod.DeclaringType.IsAssignableFrom(classType))
                throw new ArgumentException($"{nameof(classType)} '{classType.FullName}' does not implement {nameof(interfaceMethod)} '{interfaceMethod.DeclaringType.FullName}'", nameof(classType));

            #endregion

            InterfaceMapping map = classType.GetInterfaceMap(interfaceMethod.DeclaringType);
            MethodInfo? result = null;

            for (int index = 0; index < map.InterfaceMethods.Length; index++)
            {
                if (map.InterfaceMethods[index] == interfaceMethod)
                    result = map.TargetMethods[index];
            }

            Debug.Assert(result != null, "Unable to locate MethodInfo for implementing method");

            return result!;
        }
    }
}
