﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using BEditor.Data;
using BEditor.Data.Property.Easing;

namespace BEditor.Plugin
{
    /// <summary>
    /// Represents a class that initializes the services provided by the <see cref="IPlugin"/>.
    /// </summary>
    public class PluginBuilder
    {
        private readonly Func<IPlugin> _plugin;
        private readonly List<EffectMetadata> _effects = new();
        private readonly List<ObjectMetadata> _objects = new();
        private readonly List<EasingMetadata> _eases = new();
        private (string?, IEnumerable<ICustomMenu>?) _menus;

        private PluginBuilder(Func<IPlugin> create)
        {
            _plugin = create;
        }

        /// <summary>
        ///  Begin configuring an <see cref="IPlugin"/>.
        /// </summary>
        /// <typeparam name="T">Class that implements the <see cref="IPlugin"/> to be configure.</typeparam>
        /// <returns>An <see cref="PluginBuilder"/> instance.</returns>
        public static PluginBuilder Configure<T>() where T : IPlugin, new()
        {
            return new PluginBuilder(() => new T());
        }
        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the effects to be provided.</param>
        /// <returns>An <see cref="PluginBuilder"/> instance.</returns>
        public PluginBuilder With(EffectMetadata metadata)
        {
            _effects.Add(metadata);

            return this;
        }
        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the objects to be provided.</param>
        /// <returns>An <see cref="PluginBuilder"/> instance.</returns>
        public PluginBuilder With(ObjectMetadata metadata)
        {
            _objects.Add(metadata);

            return this;
        }
        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the easings to be provided.</param>
        /// <returns>An <see cref="PluginBuilder"/> instance.</returns>
        public PluginBuilder With(EasingMetadata metadata)
        {
            _eases.Add(metadata);

            return this;
        }
        /// <summary>
        /// Set the menu.
        /// </summary>
        /// <param name="header">The string to display in the menu header.</param>
        /// <param name="menus">Menu to be set.</param>
        /// <returns>An <see cref="PluginBuilder"/> instance.</returns>
        public PluginBuilder SetCustomMenu(string header, IEnumerable<ICustomMenu> menus)
        {
            _menus = (header, menus);

            return this;
        }
        /// <summary>
        /// Register this setting to the specified <see cref="PluginManager"/>.
        /// </summary>
        /// <param name="manager"><see cref="PluginManager"/> to register.</param>
        public void Register(PluginManager manager)
        {
            // Effects
            foreach (var meta in _effects.Where(meta => Attribute.IsDefined(meta.Type, typeof(DataContractAttribute))))
            {
                Serialize.SerializeKnownTypes.Add(meta.Type);

                if (meta.Children is not null)
                    Serialize.SerializeKnownTypes.AddRange(meta.Children.Select(m => m.Type));


                EffectMetadata.LoadedEffects.Add(meta);
            }

            // Objects
            foreach (var meta in _objects.Where(meta => Attribute.IsDefined(meta.Type, typeof(DataContractAttribute))))
            {
                Serialize.SerializeKnownTypes.Add(meta.Type);

                ObjectMetadata.LoadedObjects.Add(meta);
            }

            // Easing
            foreach (var meta in _eases.Where(meta => Attribute.IsDefined(meta.Type, typeof(DataContractAttribute))))
            {
                Serialize.SerializeKnownTypes.Add(meta.Type);

                EasingMetadata.LoadedEasingFunc.Add(meta);
            }

            if (_menus.Item1 is not null && _menus.Item2 is not null)
            {
                manager._menus.Add(_menus!);
            }

            manager._loaded.Add(_plugin());
        }
        /// <summary>
        /// Register this setting to the <see cref="PluginManager.Default"/>.
        /// </summary>
        public void Register()
        {
            Register(PluginManager.Default);
        }
    }
}