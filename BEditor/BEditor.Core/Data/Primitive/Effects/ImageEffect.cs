﻿using System.Runtime.Serialization;

using BEditor.Core.Data.Primitive.Objects;
using BEditor.Core.Media;

namespace BEditor.Core.Data.Primitive.Effects
{
    /// <summary>
    /// Represents an effect that can be added to an <see cref="ImageObject"/>.
    /// </summary>
    [DataContract(Namespace = "")]
    public abstract class ImageEffect : EffectElement
    {
        /// <summary>
        /// It is called at rendering time
        /// </summary>
        public abstract void Render(ref Image image, EffectRenderArgs args);
        
        /// <inheritdoc/>
        public override void Render(EffectRenderArgs args) { }
    }
}