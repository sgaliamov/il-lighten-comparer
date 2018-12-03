﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Config
{
    internal sealed class ConfigurationBuilder
    {
        private readonly ConcurrentDictionary<Type, Configuration> _configurations =
            new ConcurrentDictionary<Type, Configuration>();

        private Configuration _defaultConfiguration = new Configuration(
            new HashSet<string>(),
            false,
            new string[0],
            StringComparison.Ordinal);

        public void DefineConfiguration(Type type, ComparerSettings settings)
        {
            _configurations.AddOrUpdate(
                type,
                _ => _defaultConfiguration.Mutate(settings),
                (_, configuration) => configuration.Mutate(settings));
        }

        public void DefineDefaultConfiguration(ComparerSettings settings)
        {
            _defaultConfiguration = _defaultConfiguration.Mutate(settings);
        }

        public Configuration GetConfiguration(Type type) =>
            _configurations.TryGetValue(type, out var configuration)
                ? configuration
                : _defaultConfiguration;
    }
}