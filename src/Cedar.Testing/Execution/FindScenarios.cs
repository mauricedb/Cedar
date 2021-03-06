namespace Cedar.Testing.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public static class FindScenarios
    {
        public static IEnumerable<Func<KeyValuePair<string, Task<ScenarioResult>>>> InAssemblies(params Assembly[] assemblies)
        {
            return from assembly in assemblies
                from type in assembly.GetTypes()
                from result in InType(type)
                select result;
        }

        private static IEnumerable<Func<KeyValuePair<string, Task<ScenarioResult>>>> InType(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            
            if(constructor == null)
            {
                return Enumerable.Empty<Func<KeyValuePair<string, Task<ScenarioResult>>>>();
            }

            var singles = from method in type.GetMethods()
                where method.ReturnType == typeof(Task<ScenarioResult>)
                select FromMethodInfo(method, constructor);

            var enumerables = from method in type.GetMethods()
                where typeof(IEnumerable<Task<ScenarioResult>>).IsAssignableFrom(method.ReturnType)
                from result in FromEnumerableMethodInfo(method, constructor)
                select result;

            return singles.Concat(enumerables);
        }

        private static Func<KeyValuePair<string, Task<ScenarioResult>>> FromMethodInfo(MethodInfo method, ConstructorInfo constructor)
        {
            var suiteName = constructor.DeclaringType.FullName;

            var instance = constructor.Invoke(new object[0]);

            var result = ((Task<ScenarioResult>)method.Invoke(instance, new object[0]));

            return () => new KeyValuePair<string, Task<ScenarioResult>>(suiteName,
                result.ContinueWith(task =>
                {
                    DisposeIfNecessary(instance);

                    return task.Result;
                }));
        }

        private static IEnumerable<Func<KeyValuePair<string, Task<ScenarioResult>>>> FromEnumerableMethodInfo(
            MethodInfo method, ConstructorInfo constructor)
        {
            var suiteName = constructor.DeclaringType.FullName;

            var instance = constructor.Invoke(new object[0]);

            var results = (IEnumerable<Task<ScenarioResult>>)method.Invoke(instance, new object[0]);

            return results.Select(result => new Func<KeyValuePair<string, Task<ScenarioResult>>>(
                () => new KeyValuePair<string, Task<ScenarioResult>>(suiteName, result)));
        }


        private static void DisposeIfNecessary(object instance)
        {
            if(instance is IDisposable)
            {
                ((IDisposable) instance).Dispose();
            }
        }
    }
}