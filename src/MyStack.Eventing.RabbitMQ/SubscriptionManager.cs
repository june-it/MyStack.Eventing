namespace MyStack.Eventing.RabbitMQ
{
    public class SubscriptionManager
    {
        private static readonly Dictionary<Type, List<string>> _subscriptions = [];
        public void Subscribe(Type type, string routingKey)
        {
            if (!_subscriptions.TryGetValue(type, out List<string>? value))
            {
                value = ([]);
                _subscriptions[type] = value;
            }

            value.Add(routingKey);
        }

        public IList<Type>? GetSubscriptions(string messageKey)
        {
            var subscriptions = new List<Type>();
            foreach (var subscription in _subscriptions)
            {
                foreach (var subscribeKey in subscription.Value)
                {
                    if (IsMatch(messageKey, subscribeKey))
                    {
                        subscriptions.Add(subscription.Key);
                        break;
                    }
                }
            }
            return subscriptions;
        }

        public bool IsMatch(string pattern, string input)
        {
            // Convert RabbitMQ wildcards to regular expressions
            pattern = pattern.Replace(".", @"\.")  // Replace . in the pattern with \. to match actual dot characters
                .Replace("*", "[^.]*")  // Replace * in the pattern with [^.]* to match any character except a dot
                .Replace("#", ".*");  // Replace # in the pattern with .* to match any number of any characters
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{pattern}$");
        }
    }
}
