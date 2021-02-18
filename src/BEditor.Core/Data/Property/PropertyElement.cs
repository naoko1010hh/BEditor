﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using BEditor.Data.Property;

using Microsoft.Extensions.DependencyInjection;

namespace BEditor.Data.Property
{
    /// <summary>
    /// Represents a property used by <see cref="EffectElement"/>.
    /// </summary>
    [DataContract]
    public class PropertyElement : EditorObject, IChild<EffectElement>, IPropertyElement, IHasId, IHasName
    {
        private static readonly PropertyChangedEventArgs _metadataArgs = new(nameof(PropertyMetadata));
        private PropertyElementMetadata? _propertyMetadata;
        private int? id;


        /// <inheritdoc/>
        public virtual EffectElement? Parent { get; set; }
        /// <summary>
        /// Gets or sets the metadata for this <see cref="PropertyElement"/>.
        /// </summary>
        public PropertyElementMetadata? PropertyMetadata
        {
            get => _propertyMetadata;
            set => SetValue(value, ref _propertyMetadata, _metadataArgs);
        }
        /// <inheritdoc/>
        public int Id => (id ??= Parent?.Children?.ToList()?.IndexOf(this)) ?? -1;
        /// <inheritdoc/>
        public string Name => _propertyMetadata?.Name ?? Id.ToString();
        /// <inheritdoc/>
        public bool IsLoaded { get; private set; }


        /// <inheritdoc/>
        public override string ToString() => $"(Name:{PropertyMetadata?.Name})";
        /// <inheritdoc/>
        public void Load()
        {
            if (IsLoaded) return;

            ServiceProvider = Parent?.ServiceProvider;
            OnLoad();

            IsLoaded = true;
        }
        /// <inheritdoc/>
        public void Unload()
        {
            if (!IsLoaded) return;

            OnUnload();

            IsLoaded = false;
        }

        /// <inheritdoc cref="IElementObject.Load"/>
        protected virtual void OnLoad()
        {

        }
        /// <inheritdoc cref="IElementObject.Unload"/>
        protected virtual void OnUnload()
        {

        }
    }

    /// <inheritdoc cref="PropertyElement"/>
    /// <typeparam name="T">Type of <see cref="PropertyMetadata"/></typeparam>
    [DataContract]
    public abstract class PropertyElement<T> : PropertyElement where T : PropertyElementMetadata
    {
        /// <inheritdoc cref="PropertyElement.PropertyMetadata"/>
        public new T? PropertyMetadata
        {
            get => base.PropertyMetadata as T;
            set => base.PropertyMetadata = value;
        }
    }

    /// <summary>
    /// Represents the metadata of a property.
    /// </summary>
    public record PropertyElementMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyElementMetadata"/> class.
        /// </summary>
        /// <param name="Name">The string displayed in the property header.</param>
        public PropertyElementMetadata(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Gets the string to be displayed in the property header.
        /// </summary>
        public string Name { get; init; }
    }
}
