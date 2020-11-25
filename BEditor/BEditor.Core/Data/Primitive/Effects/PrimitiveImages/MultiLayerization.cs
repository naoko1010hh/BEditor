﻿using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Primitive.Objects;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Core.Data.Primitive.Properties.PrimitiveGroup;
using BEditor.Core.Data.Property;
using BEditor.Core.Extensions;
using BEditor.Core.Graphics;
using BEditor.Core.Media;
using BEditor.Core.Properties;
using BEditor.Core.Renderings;

using OpenTK.Graphics.OpenGL;

#if OldOpenTK
using GLColor = OpenTK.Graphics.Color4;
#else
using GLColor = OpenTK.Mathematics.Color4;
#endif

namespace BEditor.Core.Data.Primitive.Effects.PrimitiveImages
{
    public class MultiLayerization : ImageEffect
    {
        public static readonly EasePropertyMetadata ZMetadata = new(Resources.Z, 50, float.NaN, 0);
        public static readonly ColorAnimationPropertyMetadata ColorMetadata = new(Resources.Color, 255, 255, 255, 255, true);


        public MultiLayerization()
        {
            Z = new(ZMetadata);
            Material = new(ImageObject.MaterialMetadata);
            Color = new(ColorMetadata);
        }


        #region Properties

        public override string Name => Resources.MultiLayerization;

        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Z,
            Color
        };


        [DataMember(Order = 0)]
        public EaseProperty Z { get; private set; }

        [DataMember(Order = 1)]
        public Material Material { get; private set; }

        [DataMember(Order = 2)]
        public ColorAnimationProperty Color { get; private set; }

        #endregion


        public override void PreviewRender(EffectRenderArgs args)
        {
            base.PreviewRender(args);

            args.Schedules.Remove(this);
            args.Schedules.Add(this);
        }

        public override void Render(ref Image source, EffectRenderArgs args)
        {
            var drawObject = (ImageObject)Parent.Effect[0];
            var frame = args.Frame;

            Point3 coordinate = new Point3(x: drawObject.Coordinate.X.GetValue(frame),
                                             y: drawObject.Coordinate.Y.GetValue(frame),
                                             z: drawObject.Coordinate.Z.GetValue(frame));

            Point3 center = new Point3(x: drawObject.Coordinate.CenterX.GetValue(frame),
                                       y: drawObject.Coordinate.CenterY.GetValue(frame),
                                       z: drawObject.Coordinate.CenterZ.GetValue(frame));


            float nx = drawObject.Angle.AngleX.GetValue(frame);
            float ny = drawObject.Angle.AngleY.GetValue(frame);
            float nz = drawObject.Angle.AngleZ.GetValue(frame);

            //サイズを再設定
            source.ToRenderable().AreaExpansion(1, 1, 1, 1);

            //var points = BorderFinder.Find(source);

            Parent.Parent.GraphicsContext.MakeCurrent();
            GLTK.Paint(coordinate, nx, ny, nz, center, () =>
            {
                GL.Color4((GLColor)Color.GetValue(frame));
                GL.Material(MaterialFace.Front, MaterialParameter.Ambient, (GLColor)Material.Ambient.GetValue(frame));
                GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, (GLColor)Material.Diffuse.GetValue(frame));
                GL.Material(MaterialFace.Front, MaterialParameter.Specular, (GLColor)Material.Specular.GetValue(frame));
                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, Material.Shininess.GetValue(frame));

                float scale = (float)(drawObject.Zoom.Scale.GetValue(frame) / 100);
                float scalex = (float)(drawObject.Zoom.ScaleX.GetValue(frame) / 100) * scale;
                float scaley = (float)(drawObject.Zoom.ScaleY.GetValue(frame) / 100) * scale;
                float scalez = (float)(drawObject.Zoom.ScaleZ.GetValue(frame) / 100) * scale;

                GL.Scale(scalex, scaley, scalez);
                GL.Begin(PrimitiveType.Quads);
                {
                    //foreach(var point points) {
                    //    GL.Vertex3(point.X, point.Y, 0);
                    //}
                }
                GL.End();

                GL.Disable(EnableCap.Blend);
            });
        }

        public override void PropertyLoaded()
        {
            Z.ExecuteLoaded(ZMetadata);
            Material.ExecuteLoaded(ImageObject.MaterialMetadata);
            Color.ExecuteLoaded(ColorMetadata);
        }
    }
}