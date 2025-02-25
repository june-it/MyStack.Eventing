namespace MyStack.Eventing.RabbitMQ
{
    /// <summary>
    /// Represents an interface for subscription registration.
    /// </summary>
    internal interface ISubscriptionRegistrar
    {
        /// <summary>
        /// Subscribes to a collection of message types.
        /// </summary>
        /// <param name="messagetypes"></param>
        void Subscribe(params Type[] messagetypes);
    }
}
