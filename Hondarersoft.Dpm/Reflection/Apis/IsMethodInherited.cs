using System;
using System.Reflection;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Reflection
    {
        /// <summary>
        /// メソッドが派生クラスに実装されているかどうかを判定します。
        /// </summary>
        /// <param name="methodName">確認対象のメソッド名。</param>
        /// <returns>メソッドが派生クラスに実装されている場合は <c>true</c>、それ以外は <c>false</c>。</returns>
        public static bool IsMethodInherited(object instance, Type baseType, string methodName)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // 当該メソッドが派生クラスに実装され、かつ、virturl である場合は true
            if ((method.DeclaringType != baseType) && (method.IsVirtual == true))
            {
                return true;
            }

            return false;
        }
    }
}
