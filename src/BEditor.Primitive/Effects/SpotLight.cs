﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
namespace BEditor.Primitive.Effects
{
    /// <summary>
    /// Represents an <see cref="EffectElement"/> that sets the OpenGL spotlight.
    /// </summary>
    [DataContract]
    public class SpotLight : EffectElement
    {
        /// <inheritdoc/>
        public override string Name => throw new NotImplementedException();
        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> Properties => throw new NotImplementedException();
        /// <inheritdoc/>
        public override void Render(EffectRenderArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
