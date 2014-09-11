namespace Cedar.ExceptionModels.Client
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Cedar.ContentNegotiation.Client;

    internal static class HttpContentExtensions
    {
        internal static async Task<Exception> ReadException(this ISerializer serializer, HttpContent content, IModelToExceptionConverter modelToExceptionConverter)
        {
            var jsonString = await content.ReadAsStringAsync();

            var modelDryRun = (ExceptionModel)serializer.Deserialize(jsonString, typeof (ExceptionModel));

            Type type = Type.GetType(modelDryRun.TypeName, Assembly.Load, ResolveTypeFromFullName, false, true);

            var model = (ExceptionModel) serializer.Deserialize(jsonString, type);

            return modelToExceptionConverter.Convert(model);
        }

        internal static Type ResolveTypeFromFullName(Assembly _, string typeName, bool ignoreCase)
        {
            var stringComparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes().Where(typeof (ExceptionModel).IsAssignableFrom)
                where type.FullName.Equals(typeName, stringComparison)
                select type).FirstOrDefault() ?? typeof (ExceptionModel);
        }
    }
}