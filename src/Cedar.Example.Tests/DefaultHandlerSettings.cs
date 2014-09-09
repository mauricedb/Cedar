namespace Cedar.Example.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cedar.Annotations;
    using Cedar.Commands;
    using Cedar.ContentNegotiation;
    using Cedar.Handlers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal class DefaultHandlerSettings : HandlerSettings
    {
        private readonly JsonSerializer _jsonSerializer;

        internal DefaultHandlerSettings(
            [NotNull] HandlerModule handlerModule,
            [NotNull] IContentTypeMapper contentTypeMapper,
            IExceptionToModelConverter exceptionToModelConverter = null)
            : this(Enumerable.Repeat(handlerModule, 1), contentTypeMapper, exceptionToModelConverter)
        {}

        internal DefaultHandlerSettings(
            [NotNull] IEnumerable<HandlerModule> handlerModules,
            [NotNull] IContentTypeMapper contentTypeMapper,
            IExceptionToModelConverter exceptionToModelConverter = null)
            : base(handlerModules, contentTypeMapper, exceptionToModelConverter)
        {
        }
    }
}