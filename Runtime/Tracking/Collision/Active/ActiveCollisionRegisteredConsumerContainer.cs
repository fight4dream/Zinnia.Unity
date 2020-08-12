﻿namespace Zinnia.Tracking.Collision.Active
{
    using Malimbe.BehaviourStateRequirementMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Holds a <see cref="ActiveCollisionConsumer"/> collection of consumers that have successfully been published to by a <see cref="ActiveCollisionPublisher"/>.
    /// </summary>
    public class ActiveCollisionRegisteredConsumerContainer : MonoBehaviour
    {
        /// <summary>
        /// Holds data about a <see cref="ActiveCollisionRegisteredConsumerContainer"/> payload.
        /// </summary>
        [Serializable]
        public class EventData
        {
            /// <summary>
            /// The registered <see cref="ActiveCollisionConsumer"/>.
            /// </summary>
            [Serialized, Cleared]
            [field: DocumentedByXml]
            public ActiveCollisionConsumer Consumer { get; set; }

            /// <summary>
            /// The payload data sent to the <see cref="ActiveCollisionConsumer"/>.
            /// </summary>
            [Serialized, Cleared]
            [field: DocumentedByXml]
            public ActiveCollisionPublisher.PayloadData Payload { get; set; }

            public EventData Set(EventData source)
            {
                return Set(source.Consumer, source.Payload);
            }

            public EventData Set(ActiveCollisionConsumer consumer, ActiveCollisionPublisher.PayloadData payload)
            {
                Consumer = consumer;
                Payload = payload;
                return this;
            }

            public void Clear()
            {
                Set(default, default);
            }
        }

        /// <summary>
        /// Defines the event for the output <see cref="EventData"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<EventData> { }

        /// <summary>
        /// Emitted when each registered consumer payload data is published.
        /// </summary>
        [DocumentedByXml]
        public ActiveCollisionPublisher.UnityEvent Published = new ActiveCollisionPublisher.UnityEvent();
        /// <summary>
        /// Emitted when a consumer is registered.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Registered = new UnityEvent();
        /// <summary>
        /// Emitted when a consumer is unregistered.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Unregistered = new UnityEvent();

        /// <summary>
        /// A collection of registered consumers to ignore when publishing.
        /// </summary>
        public List<ActiveCollisionConsumer> IgnoredRegisteredConsumers { get; set; } = new List<ActiveCollisionConsumer>();
        /// <summary>
        /// The consumers that have successfully consumed the published payload from the <see cref="ActiveCollisionPublisher"/> linked to this.
        /// </summary>
        public Dictionary<ActiveCollisionConsumer, ActiveCollisionPublisher.PayloadData> RegisteredConsumers { get; protected set; } = new Dictionary<ActiveCollisionConsumer, ActiveCollisionPublisher.PayloadData>();

        /// <summary>
        /// The event data emitted when collisions are consumed.
        /// </summary>
        protected readonly EventData eventData = new EventData();

        /// <summary>
        /// Publishes the registered <see cref="ActiveCollisionConsumer"/> components as the component is active and enabled.
        /// Any <see cref="ActiveCollisionConsumer"/> that is in the <see cref="IgnoredRegisteredConsumers"/> will not be published to and the <see cref="IgnoredRegisteredConsumers"/> collection is cleared at the end of the <see cref="Publish"/> operation.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void Publish()
        {
            foreach (ActiveCollisionConsumer registeredConsumer in new List<ActiveCollisionConsumer>(RegisteredConsumers.Keys))
            {
                if (IgnoredRegisteredConsumers.Contains(registeredConsumer))
                {
                    continue;
                }

                if (RegisteredConsumers.TryGetValue(registeredConsumer, out ActiveCollisionPublisher.PayloadData payload))
                {
                    registeredConsumer.Consume(payload, registeredConsumer.ActiveCollision);
                    Published?.Invoke(payload);
                }
            }

            ClearIgnoredRegisteredConsumers();
        }

        /// <summary>
        /// Registers an <see cref="ActiveCollisionConsumer"/>.
        /// </summary>
        /// <param name="consumer">The consumer to register.</param>
        /// <param name="payload">The payload that the consumer successfully consumed.</param>
        [RequiresBehaviourState]
        public virtual void Register(ActiveCollisionConsumer consumer, ActiveCollisionPublisher.PayloadData payload)
        {
            if (consumer == null)
            {
                return;
            }

            RegisteredConsumers[consumer] = payload;
            Registered?.Invoke(eventData.Set(consumer, payload));
        }

        /// <summary>
        /// Unregisters an <see cref="ActiveCollisionConsumer"/>.
        /// </summary>
        /// <param name="consumer">The consumer to unregister.</param>
        public virtual void Unregister(ActiveCollisionConsumer consumer)
        {
            if (consumer == null)
            {
                return;
            }

            RegisteredConsumers.Remove(consumer);
            IgnoredRegisteredConsumers.Remove(consumer);
            Unregistered?.Invoke(eventData.Set(consumer, null));
        }

        /// <summary>
        /// Unregisters all <see cref="ActiveCollisionConsumer"/> components that exist on the given container.
        /// </summary>
        /// <param name="container">The container to unregister the consumers from.</param>
        public virtual void UnregisterConsumersOnContainer(GameObject container)
        {
            foreach (ActiveCollisionConsumer registeredConsumer in new List<ActiveCollisionConsumer>(RegisteredConsumers.Keys))
            {
                if (registeredConsumer.ConsumerContainer == container)
                {
                    Unregister(registeredConsumer);
                }
            }
        }

        /// <summary>
        /// Clears the <see cref="IgnoredRegisteredConsumers"/> collection.
        /// </summary>
        public virtual void ClearIgnoredRegisteredConsumers()
        {
            IgnoredRegisteredConsumers.Clear();
        }
    }
}