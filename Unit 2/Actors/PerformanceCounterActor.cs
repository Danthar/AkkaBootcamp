using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;

namespace ChartApp.Actors {
    /// <summary>
    /// Actor responsible for monitoring a specific <see cref="PerformanceCounter"/>
    /// </summary>
    public class PerformanceCounterActor : UntypedActor {

        private readonly string seriesName;
        private readonly Func<PerformanceCounter> performanceCounterGenerator;
        private PerformanceCounter counter;

        private readonly HashSet<ActorRef> subscriptions;
        private readonly CancellationTokenSource cancelPublishing;

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator) {
            this.seriesName = seriesName;
            this.performanceCounterGenerator = performanceCounterGenerator;
            subscriptions = new HashSet<ActorRef>();
            cancelPublishing = new CancellationTokenSource();
        }

        protected override void PreStart() {
            counter = performanceCounterGenerator();
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), Self,
                                              new GatherMetrics(), cancelPublishing.Token);


        }

        protected override void PostStop()
        {
            try
            {
                // terminate the scheduled task
                cancelPublishing.Cancel(false);
                counter.Dispose();
            }
            catch
            {
                // we don't care about additional "ObjectDisposed" exceptions
            }
            finally
            {
                base.PostStop();
            }
        }

        protected override void OnReceive(object message)
        {
            if (message is GatherMetrics)
            {
                // publish latest counter value to all subscribers
                var metric = new Metric(seriesName, counter.NextValue());
                foreach (var sub in subscriptions)
                    sub.Tell(metric);
            }
            else if (message is SubscribeCounter)
            {
                // add a subscription for this counter
                // (it's parent's job to filter by counter types)
                var sc = message as SubscribeCounter;
                subscriptions.Add(sc.Subscriber);
            }
            else if (message is UnsubscribeCounter)
            {
                // remove a subscription from this counter
                var uc = message as UnsubscribeCounter;
                subscriptions.Remove(uc.Subscriber);
            }
        }

    }
}