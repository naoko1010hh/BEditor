﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Command;
using BEditor.Data.Property;
using BEditor.Properties;

namespace BEditor.Data.Property.PrimitiveGroup
{
    /// <summary>
    /// Represents a property that sets the scale.
    /// </summary>
    [DataContract]
    public sealed class Zoom : ExpandGroup
    {
        /// <summary>
        /// Represents <see cref="Scale"/> metadata.
        /// </summary>
        public static readonly EasePropertyMetadata ScaleMetadata = new(Resources.Zoom, 100);
        /// <summary>
        /// Represents <see cref="ScaleX"/> metadata.
        /// </summary>
        public static readonly EasePropertyMetadata ScaleXMetadata = new(Resources.X, 100);
        /// <summary>
        /// Represents <see cref="ScaleY"/> metadata.
        /// </summary>
        public static readonly EasePropertyMetadata ScaleYMetadata = new(Resources.Y, 100);
        /// <summary>
        /// Represents <see cref="ScaleZ"/> metadata.
        /// </summary>
        public static readonly EasePropertyMetadata ScaleZMetadata = new(Resources.Z, 100);

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinate"/> class.
        /// </summary>
        /// <param name="metadata">Metadata of this property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is <see langword="null"/>.</exception>
        public Zoom(PropertyElementMetadata metadata) : base(metadata)
        {
            Scale = new(ScaleMetadata);
            ScaleX = new(ScaleXMetadata);
            ScaleY = new(ScaleYMetadata);
            ScaleZ = new(ScaleZMetadata);
        }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Scale,
            ScaleX,
            ScaleY,
            ScaleZ
        };
        /// <summary>
        /// Get the EaseProperty representing the scale.
        /// </summary>
        [DataMember(Name = "Zoom", Order = 0)]
        public EaseProperty Scale { get; private set; }
        /// <summary>
        /// Get the <see cref="EaseProperty"/> that represents the scale in the Z-axis direction.
        /// </summary>
        [DataMember(Order = 1)]
        public EaseProperty ScaleX { get; private set; }
        /// <summary>
        /// Get the <see cref="EaseProperty"/> that represents the scale in the Y-axis direction.
        /// </summary>
        [DataMember(Order = 2)]
        public EaseProperty ScaleY { get; private set; }
        /// <summary>
        /// Get the <see cref="EaseProperty"/> that represents the scale in the Z-axis direction.
        /// </summary>
        [DataMember(Order = 3)]
        public EaseProperty ScaleZ { get; private set; }

        /// <inheritdoc/>
        protected override void OnLoad()
        {
            Scale.Load(ScaleMetadata);
            ScaleX.Load(ScaleXMetadata);
            ScaleY.Load(ScaleYMetadata);
            ScaleZ.Load(ScaleZMetadata);
        }
        /// <inheritdoc/>
        protected override void OnUnload()
        {
            Scale.Unload();
            ScaleX.Unload();
            ScaleY.Unload();
            ScaleZ.Unload();
        }
    }
}
