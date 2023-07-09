using DemoBed.Base.EventBus.Abtraction;
using NetCoreEventBus.Infra.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBed.Base.EventBus.InMemorySubsriptionImplementation
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        #region Fields
        private readonly Dictionary<string, List<Subscription>> handlers = new Dictionary<string, List<Subscription>>();
        private readonly List<Type> eventTypes = new List<Type>();
        #endregion

        #region Event Handlers
        public event EventHandler<string>? OnEventRemoved;
        #endregion

        #region Events info
        public string GetEventIdentifier<TEvent>() => typeof(TEvent).Name;

        public Type GetEventTypeByName(string eventName) => eventTypes.SingleOrDefault(t => t.Name == eventName) ?? throw new ArgumentNullException();

        public IEnumerable<Subscription> GetHandlersForEvent(string eventName) => handlers[eventName];

        public Dictionary<string, List<Subscription>> GetAllSubscriptions() => new Dictionary<string, List<Subscription>>(handlers);
        #endregion

        #region Subscriptions management
        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventIdentifier<TEvent>();

            DoAddSubscription(typeof(TEvent), typeof(TEventHandler), eventName);

            if (!eventTypes.Contains(typeof(TEvent)))
            {
                eventTypes.Add(typeof(TEvent));
            }
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : Event
        {
            var handlerToRemove = FindSubscriptionToRemove<TEvent, TEventHandler>();
            var eventName = GetEventIdentifier<TEvent>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void Clear()
        {
            handlers.Clear();
            eventTypes.Clear();
        }
        #endregion

        #region Status
        public bool IsEmpty => !handlers.Keys.Any();

        public bool HasSubscriptionsForEvent(string eventName) => handlers.ContainsKey(eventName);
        #endregion

        #region Private methods
        private void DoAddSubscription(Type eventType, Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                handlers.Add(eventName, new List<Subscription>());
            }

            if (handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            handlers[eventName].Add(new Subscription(eventType, handlerType));
        }

        private void DoRemoveHandler(string eventName, Subscription subscriptionToRemove)
        {
            if (subscriptionToRemove == null)
            {
                return;
            }

            handlers[eventName].Remove(subscriptionToRemove);
            if (handlers[eventName].Any())
            {
                return;
            }

            handlers.Remove(eventName);
            var eventType = eventTypes.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null)
            {
                eventTypes.Remove(eventType);
            }

            RaiseOnEventRemoved(eventName);
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        private Subscription FindSubscriptionToRemove<TEvent, TEventHandler>()
             where TEvent : Event
             where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventIdentifier<TEvent>();
            return DoFindSubscriptionToRemove(eventName, typeof(TEventHandler)) ?? throw new ArgumentNullException();
        }

        private Subscription? DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);

        }
        #endregion
    }
}