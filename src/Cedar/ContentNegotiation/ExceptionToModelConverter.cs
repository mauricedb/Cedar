namespace Cedar.ContentNegotiation
{
    using System;
    using Cedar.ContentNegotiation.Client;
    using Cedar.ExceptionModels.Client;

    public class ExceptionToModelConverter : IExceptionToModelConverter
    {
        public virtual ExceptionModel Convert(Exception exception)
        {
            ExceptionModel model = null;

            TypeSwitch.On(exception)
                .Case<HttpStatusException>(ex =>
                {
                    exception = ex.InnerException;
                    model = Convert(exception);
                })
                .Case<ArgumentException>(ex => model = new ArgumentExceptionModel
                {
                    ParamName = ex.ParamName,
                })
                .Case<NotSupportedException>(ex => model = new NotSupportedExceptionModel())
                .Case<InvalidOperationException>(ex => model = new InvalidOperationExceptionModel())
                .Default(() => model = new ExceptionModel
                {
                    Message = string.Format("[No exception serializer found for {0}].{1}", exception.GetType(), Environment.NewLine)
                });

            model.Message = exception.Message;
            model.StackTrace = exception.StackTrace;
            return model;
        }
    }
}