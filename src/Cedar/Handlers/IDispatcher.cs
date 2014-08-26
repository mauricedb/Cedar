namespace Cedar.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDispatcher
    {
        Task<int> Message<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : class;
    }
}