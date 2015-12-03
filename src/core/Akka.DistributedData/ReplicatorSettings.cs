﻿using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.DistributedData
{
    public class ReplicatorSettings : ICloneable
    {
        private readonly string _role;
        private readonly TimeSpan _gossipInterval;
        private readonly TimeSpan _notifySubscribersInterval;
        private readonly int _maxDeltaElements;
        private readonly string _dispatcher;
        private readonly TimeSpan _pruningInterval;
        private readonly TimeSpan _maxPruningDissemination;

        public string Role
        {
            get { return _role; }
        }

        public TimeSpan GossipInterval
        {
            get { return _gossipInterval; }
        }

        public TimeSpan NotifySubscribersInterval
        {
            get { return _notifySubscribersInterval; }
        }

        public int MaxDeltaElements
        {
            get { return _maxDeltaElements; }
        }

        public string Dispatcher
        {
            get { return _dispatcher; }
        }

        public TimeSpan PruningInterval
        {
            get { return _pruningInterval; }
        }

        public TimeSpan MaxPruningDissemination
        {
            get { return _maxPruningDissemination; }
        }

        public ReplicatorSettings(string role,
                                  TimeSpan gossipInterval,
                                  TimeSpan notifySubscribersInterval,
                                  int maxDeltaElements,
                                  string dispatcher,
                                  TimeSpan pruningInterval,
                                  TimeSpan maxPruningDissemination)
        {
            _role = role;
            _gossipInterval = gossipInterval;
            _notifySubscribersInterval = notifySubscribersInterval;
            _maxDeltaElements = maxDeltaElements;
            _dispatcher = dispatcher;
            _pruningInterval = pruningInterval;
            _maxPruningDissemination = maxPruningDissemination;
        }

        public ReplicatorSettings(Config config)
            : this(config.GetString("role", null),
                   config.GetTimeSpan("gossip-interval", TimeSpan.FromSeconds(2.0)),
                   config.GetTimeSpan("notify-subscribers-interval", TimeSpan.FromMilliseconds(500.0)),
                   config.GetInt("max-delta-elements", 1000),
                   config.GetString("use-dispatcher", Dispatchers.DefaultDispatcherId),
                   config.GetTimeSpan("pruning-interval", TimeSpan.FromSeconds(30.0)),
                   config.GetTimeSpan("max-pruning-dissemination", TimeSpan.FromSeconds(60.0)))
        { }

        public ReplicatorSettings()
            : this(ConfigurationFactory.ParseString(""))
        { }

        public ReplicatorSettings(ActorSystem system)
            : this(system.Settings.Config.GetConfig("akka.cluster.distributed-data"))
        { }

        public static Config DefaultConfig()
        {
            var assembly = typeof(IReplicatedData).Assembly;

            using (var stream = assembly.GetManifestResourceStream("Akka.DistributedData.Resources.Reference.conf"))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    return ConfigurationFactory.ParseString(result);
                }
            }
        }

        public object Clone()
        {
            return new ReplicatorSettings(Role, GossipInterval, NotifySubscribersInterval, MaxDeltaElements, Dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithRole(string role)
        {
            return new ReplicatorSettings(role, GossipInterval, NotifySubscribersInterval, MaxDeltaElements, Dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithGossipInterval(TimeSpan gossipInterval)
        {
            return new ReplicatorSettings(Role, gossipInterval, NotifySubscribersInterval, MaxDeltaElements, Dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithNotifySubscribersInterval(TimeSpan notifySubscribersInterval)
        {
            return new ReplicatorSettings(Role, GossipInterval, notifySubscribersInterval, MaxDeltaElements, Dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithMaxDeltaElements(int maxDeltaElements)
        {
            return new ReplicatorSettings(Role, GossipInterval, NotifySubscribersInterval, maxDeltaElements, Dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithDispatcher(string dispatcher)
        {
            if(String.IsNullOrEmpty(dispatcher))
            {
                dispatcher = Dispatchers.DefaultDispatcherId;
            }
            return new ReplicatorSettings(Role, GossipInterval, NotifySubscribersInterval, MaxDeltaElements, dispatcher, PruningInterval, MaxPruningDissemination);
        }

        public ReplicatorSettings WithPruning(TimeSpan pruningInterval, TimeSpan maxPruningDissemination)
        {
            return new ReplicatorSettings(Role, GossipInterval, NotifySubscribersInterval, MaxDeltaElements, Dispatcher, pruningInterval, maxPruningDissemination);
        }
    }
}