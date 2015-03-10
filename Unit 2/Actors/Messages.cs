﻿using Akka.Actor;

namespace ChartApp.Actors {
    /// <summary>
    /// Signal used to indicate that it's time to sample all counters
    /// </summary>
    public class GatherMetrics { }

    /// <summary>
    /// Metric data at the time of sample
    /// </summary>
    public class Metric {
        public string Series { get; private set; }
        public float CounterValue { get; private set; }

        public Metric(string series, float counterValue) {
            Series = series;
            CounterValue = counterValue;
        }
    }

    public enum CounterType {
        Cpu,
        Memory,
        Disk
    }

    /// <summary>
    /// Enables a counter and begins publishing values to <see cref="Subscriber"/>.
    /// </summary>
    public class SubscribeCounter
    {
        public SubscribeCounter(CounterType counter, ActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }

        public CounterType Counter { get; private set; }

        public ActorRef Subscriber { get; private set; }
    }

    /// <summary>
    /// Unsubscribes <see cref="Subscriber"/> from receiving updates for a given counter
    /// </summary>
    public class UnsubscribeCounter
    {
        public UnsubscribeCounter(CounterType counter, ActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }

        public CounterType Counter { get; private set; }

        public ActorRef Subscriber { get; private set; }
    }


}