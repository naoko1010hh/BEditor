﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Property;
using BEditor.Core.Data.Property.PrimitiveGroup;
using BEditor.Core.Extensions;
using BEditor.Core.Graphics;
using BEditor.Core.Properties;

using OpenTK.Graphics.OpenGL4;

using GLColor = OpenTK.Mathematics.Color4;
using Material = BEditor.Core.Data.Property.PrimitiveGroup.Material;

namespace BEditor.Core.Data.Primitive.Objects
{
    [DataContract]
    public class GL3DObject : ObjectElement
    {
        public static readonly SelectorPropertyMetadata TypeMetadata = new(Resources.Type, new string[2]
        {
            Resources.Cube,
            Resources.Ball
        });
        public static readonly EasePropertyMetadata WeightMetadata = new("Depth", 100, float.NaN, 0);

        public GL3DObject()
        {
            Coordinate = new(ImageObject.CoordinateMetadata);
            Zoom = new(ImageObject.ZoomMetadata);
            Blend = new(ImageObject.BlendMetadata);
            Angle = new(ImageObject.AngleMetadata);
            Material = new(ImageObject.MaterialMetadata);
            Type = new(TypeMetadata);
            Width = new(Figure.WidthMetadata);
            Height = new(Figure.HeightMetadata);
            Depth = new(WeightMetadata);
        }

        public override string Name => Resources._3DObject;
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Coordinate,
            Zoom,
            Blend,
            Angle,
            Material,
            Type,
            Width,
            Height,
            Depth
        };
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
        [DataMember(Order = 5)]
        public SelectorProperty Type { get; private set; }
        [DataMember(Order = 6)]
        public EaseProperty Width { get; private set; }
        [DataMember(Order = 7)]
        public EaseProperty Height { get; private set; }
        [DataMember(Order = 8)]
        public EaseProperty Depth { get; private set; }

        public override void Render(EffectRenderArgs args)
        {
            int frame = args.Frame;
            Action action;
            GLColor color4 = Blend.Color[frame].ToOpenTK();
            color4.A *= Blend.Alpha[frame];


            float scale = (float)(Zoom.Scale[frame] / 100);
            float scalex = (float)(Zoom.ScaleX[frame] / 100) * scale;
            float scaley = (float)(Zoom.ScaleY[frame] / 100) * scale;
            float scalez = (float)(Zoom.ScaleZ[frame] / 100) * scale;


            if (Type.Index == 0)
            {
                action = () =>
                {
                    //GL.Color4(color4);
                    //GL.Scale(scalex, scaley, scalez);
                    //GLTK.DrawCube(
                    //    Width.GetValue(frame),
                    //    Height.GetValue(frame),
                    //    Depth.GetValue(frame),
                    //    Material.Ambient.GetValue(frame),
                    //    Material.Diffuse.GetValue(frame),
                    //    Material.Specular.GetValue(frame),
                    //    Material.Shininess.GetValue(frame));
                };
            }
            else
            {
                action = () =>
                {
                    //GL.Color4(color4);
                    //GL.Scale(scalex, scaley, scalez);
                    //GLTK.DrawBall(
                    //    Depth.GetValue(frame),
                    //    Material.Ambient.GetValue(frame),
                    //    Material.Diffuse.GetValue(frame),
                    //    Material.Specular.GetValue(frame),
                    //    Material.Shininess.GetValue(frame));
                };
            }

            Parent!.Parent!.GraphicsContext!.MakeCurrent();
            //GLTK.Paint(
            //    new System.Numerics.Vector3(
            //        Coordinate.X.GetValue(frame),
            //        Coordinate.Y.GetValue(frame),
            //        Coordinate.Z.GetValue(frame)),
            //    Angle.AngleX.GetValue(frame),
            //    Angle.AngleY.GetValue(frame),
            //    Angle.AngleZ.GetValue(frame),
            //    new System.Numerics.Vector3(
            //        Coordinate.CenterX.GetValue(frame),
            //        Coordinate.CenterY.GetValue(frame),
            //        Coordinate.CenterZ.GetValue(frame)),
            //    action,
            //    Blend.BlentFunc[Blend.BlendType.Index]);
            using var cube = new Cube(
                Width[frame],
                Height[frame],
                Depth[frame],
                Blend.Color[frame],
                new(
                    Material.Ambient[frame],
                    Material.Diffuse[frame],
                    Material.Specular[frame],
                    Material.Shininess[frame]));

            var trans = Transform.Create(
                new(Coordinate.X[frame], Coordinate.Y[frame], Coordinate.Z[frame]),
                new(Coordinate.CenterX[frame], Coordinate.CenterY[frame], Coordinate.CenterZ[frame]),
                new(Angle.AngleX[frame], Angle.AngleY[frame], Angle.AngleZ[frame]),
                new(scalex, scaley, scalez));

            Parent.Parent.GraphicsContext.DrawCube(cube, trans);

            Coordinate.ResetOptional();
        }
        protected override void OnLoad()
        {
            Coordinate.Load(ImageObject.CoordinateMetadata);
            Zoom.Load(ImageObject.ZoomMetadata);
            Blend.Load(ImageObject.BlendMetadata);
            Angle.Load(ImageObject.AngleMetadata);
            Material.Load(ImageObject.MaterialMetadata);
            Type.Load(TypeMetadata);
            Width.Load(Figure.WidthMetadata);
            Height.Load(Figure.HeightMetadata);
            Depth.Load(WeightMetadata);
        }
        protected override void OnUnload()
        {
            Coordinate.Unload();
            Zoom.Unload();
            Blend.Unload();
            Angle.Unload();
            Material.Unload();
            Type.Unload();
            Width.Unload();
            Height.Unload();
            Depth.Unload();
        }
    }
}