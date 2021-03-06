﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using BEditor.Data.Property;

namespace BEditor.Data.Property
{
    /// <summary>
    /// Represents a base class for grouping <see cref="PropertyElement"/>.
    /// </summary>
    [DataContract]
    public abstract class Group : PropertyElement, IKeyFrameProperty, IEasingProperty, IParent<PropertyElement>
    {
        private IEnumerable<PropertyElement>? _CachedList;

        /// <summary>
        /// Get the <see cref="PropertyElement"/> to display on the GUI.
        /// </summary>
        public abstract IEnumerable<PropertyElement> Properties { get; }
        /// <inheritdoc/>
        public IEnumerable<PropertyElement> Children => _CachedList ??= Properties;

        /// <inheritdoc/>
        public override EffectElement? Parent
        {
            get => base.Parent;
            set
            {
                base.Parent = value;

                Parallel.ForEach(Children, item => item.Parent = value);
            }
        }
    }
}
