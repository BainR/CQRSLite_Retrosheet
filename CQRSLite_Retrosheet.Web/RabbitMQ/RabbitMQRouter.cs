using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Routing;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Web.RabbitMQ
{
    // https://raw.githubusercontent.com/gautema/CQRSlite/master/Framework/CQRSlite/Routing/Router.cs
    // https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html

    public class RabbitMQRouter : ICommandSender, IEventPublisher, IHandlerRegistrar
    {
        private const string ExchangeName = "retrosheet";
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private string queueName;
        private EventingBasicConsumer consumer;

        private readonly Dictionary<Type, List<Func<IMessage, CancellationToken, Task>>> _routes = new Dictionary<Type, List<Func<IMessage, CancellationToken, Task>>>();
        private readonly Dictionary<string, Type> _typeMaps = new Dictionary<string, Type>();

        public RabbitMQRouter()
        {
            factory = new ConnectionFactory() { HostName = "localhost", UseBackgroundThreadsForIO = true };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: ExchangeName, type: "topic");
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "#");
            consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                IMessage msg = JsonConvert.DeserializeObject(message, _typeMaps[routingKey]) as IMessage;

                if (!_routes.TryGetValue(msg.GetType(), out var handlers))
                    throw new InvalidOperationException("No handler registered");
                if (handlers.Count != 1 && typeof(ICommand).IsAssignableFrom(msg.GetType()))
                    throw new InvalidOperationException("Cannot send to more than one handler");
                handlers[0](msg, default(CancellationToken));
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public Task Send<T>(T command, CancellationToken cancellationToken = default(CancellationToken)) where T : class, ICommand
        {
            string message = JsonConvert.SerializeObject(command);
            string routingKey = command.GetType().Name;
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: ExchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            return Task.CompletedTask;
        }

        public void RegisterHandler<T>(Func<T, CancellationToken, Task> handler) where T : class, IMessage
        {
            _typeMaps[typeof(T).Name] = typeof(T);

            if (!_routes.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Func<IMessage, CancellationToken, Task>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add((message, token) => handler((T)message, token));
        }

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEvent
        {
            if (!_routes.TryGetValue(@event.GetType(), out var handlers))
                return Task.FromResult(0);

            string message = JsonConvert.SerializeObject(@event);
            string routingKey = @event.GetType().Name;
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: ExchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            return Task.CompletedTask;
        }
    }
}
