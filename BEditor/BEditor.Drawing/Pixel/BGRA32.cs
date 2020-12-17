﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BEditor.Drawing.Pixel
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [PixelFormat(4)]
    public struct BGRA32 : IPixel<BGRA32>, IPixelConvertable<BGR24>, IPixelConvertable<RGB24>, IPixelConvertable<RGBA32>
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;

        public BGRA32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public readonly BGRA32 Blend(BGRA32 foreground)
        {
            if (A is 0) return this;

            var result = new BGRA32();

            var dstA = foreground.A;
            var blendA = (A + dstA) - A * dstA / 255;

            result.B = (byte)((B * A + foreground.B * (255 - A) * dstA / 255) / blendA);
            result.G = (byte)((G * A + foreground.G * (255 - A) * dstA / 255) / blendA);
            result.R = (byte)((R * A + foreground.R * (255 - A) * dstA / 255) / blendA);

            return result;
        }
        public readonly void Convert(out BGR24 dst)
        {
            dst = new(R, G, B);
        }
        public readonly void Convert(out RGB24 dst)
        {
            dst = new(R, G, B);
        }
        public readonly void Convert(out RGBA32 dst)
        {
            dst = new(R, G, B, A);
        }
    }
}
