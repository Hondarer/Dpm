using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Reflection
{
    public static class Delegates
    {
        private static ConcurrentDictionary<Type, Func<object>> InstanceDelegateCache = new ConcurrentDictionary<Type, Func<object>>();
        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> GetDelegateCache = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        private static ConcurrentDictionary<Tuple<Type, string>, Action<object, object>> SetDelegateCache = new ConcurrentDictionary<Tuple<Type, string>, Action<object, object>>();
        private static ConcurrentDictionary<Tuple<Type, string, Type[]>, Action<object, object[]>> ActionDelegateCache = new ConcurrentDictionary<Tuple<Type, string, Type[]>, Action<object, object[]>>();
        private static ConcurrentDictionary<Tuple<Type, string, Type[]>, Func<object, object[], object>> FuncDelegateCache = new ConcurrentDictionary<Tuple<Type, string, Type[]>, Func<object, object[], object>>();

        /// <summary>
        /// 式木を使って、<param name="targetType"/> クラスの新しいインスタンスを生成するデリゲートを生成します。
        /// </summary>
        /// <param name="targetType">対象となる型。</param>
        /// <returns>生成されたデリゲート。</returns>
        private static Func<object> CreateCreateInstanceDelegate(Type targetType)
        {
            // new T()

            Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(Expression.New(targetType));

            return lambda.Compile();
        }

        public static object CreateInstance(Type targetType)
        {
            if (InstanceDelegateCache.ContainsKey(targetType) == false)
            {
                InstanceDelegateCache.TryAdd(targetType, CreateCreateInstanceDelegate(targetType));
            }

            return InstanceDelegateCache[targetType]();
        }

        /// <summary>
        /// 式木を使って、<param name="targetType"/> クラスの
        /// <param name="memberName"/> メンバーの値を取得するデリゲートを生成します。
        /// </summary>
        /// <param name="targetType">対象となる型。</param>
        /// <param name="memberName">対象となるメンバー名。</param>
        /// <returns>生成されたデリゲート。</returns>
        private static Func<object, object> CreateGetDelegate(Type targetType, string memberName)
        {
            // (object target) => (object)((T)target).memberName

            ParameterExpression target = Expression.Parameter(typeof(object), "target");

            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(target, targetType), memberName)
                    , typeof(object))
                , target);

            return lambda.Compile();
        }

        public static object Get(object target, string memberName)
        {
            Tuple<Type, string> key = new Tuple<Type, string>(target.GetType(), memberName);

            if (GetDelegateCache.ContainsKey(key) == false)
            {
                GetDelegateCache.TryAdd(key, CreateGetDelegate(target.GetType(), memberName));
            }

            return GetDelegateCache[key](target);
        }

        /// <summary>
        /// 式木を使って、<param name="targetType"/> クラスの
        /// <param name="memberName"/> メンバーの値を設定するデリゲートを生成します。
        /// </summary>
        /// <param name="targetType">対象となる型。</param>
        /// <param name="memberName">対象となるメンバー名。</param>
        /// <returns>生成されたデリゲート。</returns>
        private static Action<object, object> CreateSetDelegate(Type targetType, string memberName)
        {
            // (object target, object value) => ((T)target).memberName = (U)value

            ParameterExpression target = Expression.Parameter(typeof(object), "target");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            MemberExpression left =
                Expression.PropertyOrField(
                    Expression.Convert(target, targetType), memberName);

            UnaryExpression right = Expression.Convert(value, left.Type);

            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                Expression.Assign(left, right),
                target, value);

            return lambda.Compile();
        }

        public static void Set(object target, string memberName, object value)
        {
            Tuple<Type, string> key = new Tuple<Type, string>(target.GetType(), memberName);

            if (SetDelegateCache.ContainsKey(key) == false)
            {
                SetDelegateCache.TryAdd(key, CreateSetDelegate(target.GetType(), memberName));
            }

            SetDelegateCache[key](target, value);
        }

        /// <summary>
        /// 式木を使って、<param name="targetType"/> クラスの
        /// <param name="methodName"/> メンバーの戻り値を持たないメソッドを実行するデリゲートを生成します。
        /// </summary>
        /// <param name="targetType">対象となる型。</param>
        /// <param name="methodName">対象となるメソッド名。</param>
        /// <param name="types">対象となるメソッドの型の配列。省略可能です。既定値は <c>null</c> です。</param>
        /// <returns>生成されたデリゲート。</returns>
        private static Action<object, object[]> CreateActionDelegate(Type targetType, string methodName, Type[] types = null)
        {
            // <Action[object, object[]]>(object $target, object[] $args)
            // {
            //     ((targetType)$target).MethodName((MethodType0)$args[0])
            // }

            MethodInfo methodInfo;

            if (types == null)
            {
                methodInfo = targetType.GetMethod(methodName);
            }
            else
            {
                methodInfo = targetType.GetMethod(methodName, types);
            }

            ParameterExpression target = Expression.Parameter(typeof(object), "target");
            ParameterExpression args = Expression.Parameter(typeof(object[]), "args");

            // メソッドに渡す引数はオブジェクト配列をインデクサでアクセス + キャスト => (cast)args[index]
            UnaryExpression[] methodParms = methodInfo.GetParameters()
                .Select((x, index) =>
                    Expression.Convert(
                        Expression.ArrayIndex(args, Expression.Constant(index)),
                    x.ParameterType))
                .ToArray();

            ParameterExpression[] parameters = new ParameterExpression[] { target, args };

            Expression<Action<object, object[]>> lambda =
                Expression.Lambda<Action<object, object[]>>(
                    Expression.Call(Expression.Convert(target, targetType), methodInfo, methodParms)
                , parameters);

            return lambda.Compile();
        }

        public static void Action(object target, string methodName, object[] args, Type[] types = null)
        {
            Tuple<Type, string, Type[]> key = new Tuple<Type, string, Type[]>(target.GetType(), methodName, types);

            if (ActionDelegateCache.ContainsKey(key) == false)
            {
                ActionDelegateCache.TryAdd(key, CreateActionDelegate(target.GetType(), methodName));
            }

            ActionDelegateCache[key](target, args);
        }

        public static void Action(object target, string methodName, object arg = null)
        {
            if (arg == null)
            {
                Action(target, methodName, null, null);
            }
            else
            {
                Action(target, methodName, new object[] { arg }, null);
            }
        }

        public static void Action(object target, string methodName, params object[] args)
        {
            Action(target, methodName, args, null);
        }

        /// <summary>
        /// 式木を使って、<param name="targetType"/> クラスの
        /// <param name="methodName"/> メンバーの戻り値を返すメソッドを実行するデリゲートを生成します。
        /// </summary>
        /// <param name="targetType">対象となる型。</param>
        /// <param name="methodName">対象となるメソッド名。</param>
        /// <param name="types">対象となるメソッドの型の配列。省略可能です。既定値は <c>null</c> です。</param>
        /// <returns>生成されたデリゲート。</returns>
        private static Func<object, object[], object> CreateFuncDelegate(Type targetType, string methodName, Type[] types = null)
        {
            // <Func[object, object[], object]>(object $target, object[] $args)
            // {
            //     (object)((targetType)$target).MethodName((MethodType0)$args[0])
            // }

            MethodInfo methodInfo;

            if (types == null)
            {
                methodInfo = targetType.GetMethod(methodName);
            }
            else
            {
                methodInfo = targetType.GetMethod(methodName, types);
            }

            ParameterExpression target = Expression.Parameter(typeof(object), "target");
            ParameterExpression args = Expression.Parameter(typeof(object[]), "args");

            // メソッドに渡す引数はオブジェクト配列をインデクサでアクセス + キャスト => (cast)args[index]
            UnaryExpression[] methodParms = methodInfo.GetParameters()
                .Select((x, index) =>
                    Expression.Convert(
                        Expression.ArrayIndex(args, Expression.Constant(index)),
                    x.ParameterType))
                .ToArray();

            ParameterExpression[] parameters = new ParameterExpression[] { target, args };

            Expression<Func<object, object[], object>> lambda =
                Expression.Lambda<Func<object, object[], object>>(
                    Expression.Convert(
                        Expression.Call(Expression.Convert(target, targetType), methodInfo, methodParms)
                    , typeof(object))
                , parameters);

            return lambda.Compile();
        }

        public static object Func(object target, string methodName, object[] args, Type[] types = null)
        {
            Tuple<Type, string, Type[]> key = new Tuple<Type, string, Type[]>(target.GetType(), methodName, types);

            if (FuncDelegateCache.ContainsKey(key) == false)
            {
                FuncDelegateCache.TryAdd(key, CreateFuncDelegate(target.GetType(), methodName));
            }

            return FuncDelegateCache[key](target, args);
        }

        public static object Func(object target, string methodName, object arg = null)
        {
            if (arg == null)
            {
                return Func(target, methodName, null, null);
            }
            else
            {
                return Func(target, methodName, new object[] { arg });
            }
        }

        public static object Func(object target, string methodName, params object[] args)
        {
            return Func(target, methodName, args, null);
        }
    }
}
