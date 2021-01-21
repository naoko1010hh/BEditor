﻿using System.Linq;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Primitive.Effects;
using BEditor.Core.Data.Property.PrimitiveGroup;
using BEditor.Core.Data.Property;
using BEditor.Core.Extensions;
using BEditor.Core.Properties;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace BEditor.Core.Data.Primitive
{
    [DataContract]
    public abstract class ImageObject : ObjectElement
    {
        public static readonly PropertyElementMetadata CoordinateMetadata = new(Resources.Coordinate);
        public static readonly PropertyElementMetadata ZoomMetadata = new(Resources.Zoom);
        public static readonly PropertyElementMetadata BlendMetadata = new(Resources.Blend);
        public static readonly PropertyElementMetadata AngleMetadata = new(Resources.Angle);
        public static readonly PropertyElementMetadata MaterialMetadata = new(Resources.Material);

        public ImageObject()
        {
            Coordinate = new(CoordinateMetadata);
            Zoom = new(ZoomMetadata);
            Blend = new(BlendMetadata);
            Angle = new(AngleMetadata);
            Material = new(MaterialMetadata);
        }

        public override string Name => "";
        [DataMember(Order = 0)]
        public Coordinate Coordinate { get; private set; }
        [DataMember(Order = 1)]
        public Zoom Zoom { get; private set; }
        [DataMember(Order = 2)]
        public Blend Blend { get; private set; }
        [DataMember(Order = 3)]
        public Angle Angle { get; private set; }
        [DataMember(Order = 4)]
        public Material Material { get; private set; }

        public override void Render(EffectRenderArgs args)
        {
            var base_img = OnRender(args);

            if (base_img is null)
            {
                Coordinate.ResetOptional();
                return;
            }

            var imageArgs = new EffectRenderArgs<Image<BGRA32>>(args.Frame, base_img, args.Type);

            var list = Parent!.Effect.Where(x => x.IsEnabled).ToArray();
            for (int i = 1; i < list.Length; i++)
            {
                var effect = list[i];

                if (effect is ImageEffect imageEffect)
                {
                    imageEffect.Render(imageArgs);
                }
                else
                {
                    effect.Render(args);
                }


                if (args.Handled)
                {
                    Coordinate.ResetOptional();
                    return;
                }
            }


            Parent!.Parent!.GraphicsContext!.DrawImage(imageArgs.Value, Parent, args);
            base_img?.Dispose();
            imageArgs.Value?.Dispose();

            Coordinate.ResetOptional();
        }
        protected abstract Image<BGRA32>? OnRender(EffectRenderArgs args);
        protected override void OnLoad()
        {
            Coordinate.Load(CoordinateMetadata);
            Zoom.Load(ZoomMetadata);
            Blend.Load(BlendMetadata);
            Angle.Load(AngleMetadata);
            Material.Load(MaterialMetadata);
        }
        protected override void OnUnload()
        {
            Coordinate.Unload();
            Zoom.Unload();
            Blend.Unload();
            Angle.Unload();
            Material.Unload();
        }

        public override bool EffectFilter(EffectElement effect)
        {
            return effect is ImageEffect;
        }
    }
}
