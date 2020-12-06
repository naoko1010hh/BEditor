﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Drawing.Interop;
using BEditor.Drawing.Pixel;
using BEditor.Drawing.Process;

namespace BEditor.Drawing
{
    public unsafe static partial class Image
    {
        public static void DrawImage(this Image<BGRA32> self, Point point, Image<BGRA32> image)
        {
            self.ThrowIfDisposed();
            var rect = new Rectangle(point, image.Size);
            var blended = self[rect];

            fixed (BGRA32* dst = blended.Data)
            fixed (BGRA32* src = image.Data)
            {
                var proc = new AlphaBlendProcess(dst, src);
                Parallel.For(0, image.Length, proc.Invoke);
            }

            self[rect] = blended;
        }
        public static Bitmap ToBitmap(this Image<BGRA32> self)
        {
            self.ThrowIfDisposed();
            var width = self.Width;
            var height = self.Height;

            var result = new Bitmap(width, height);
            var data = result.LockBits(new(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            fixed (BGRA32* srcData = self.Data)
            {
                Buffer.MemoryCopy(srcData, (void*)data.Scan0, self.DataSize, self.DataSize);
            }

            result.UnlockBits(data);

            return result;
        }
        public static Image<BGRA32> ToImage(this Bitmap self)
        {
            using var c = self.Clone(new(0, 0, self.Width, self.Height), PixelFormat.Format32bppArgb);
            var data = c.LockBits(new(0, 0, c.Width, c.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var result = new Image<BGRA32>(data.Width, data.Height, data.Scan0);

            c.UnlockBits(data);

            return result;
        }

        public static void SetAlpha(this Image<BGRA32> self, float alpha)
        {
            self.ThrowIfDisposed();

            fixed (BGRA32* data = self.Data)
            {
                var p = new SetAlphaProcess(data, alpha);
                Parallel.For(0, self.Length, p.Invoke);
            }
        }
        public static void SetColor(this Image<BGRA32> self, BGRA32 color)
        {
            self.ThrowIfDisposed();

            fixed (BGRA32* data = self.Data)
            {
                var p = new SetColorProcess(data, color);
                Parallel.For(0, self.Length, p.Invoke);
            }
        }
        public static Image<BGRA32> Border(this Image<BGRA32> self, int size, BGRA32 color)
        {
            if (size <= 0) throw new ArgumentException("size <= 0");
            self.ThrowIfDisposed();

            int nwidth = self.Width + (size + 5) * 2;
            int nheight = self.Height + (size + 5) * 2;
            var result = new Image<BGRA32>(nwidth, nheight);

            // 縁を描画
            using var border = self.Clone();
            border.SetColor(color);
            using var border_ = border.MakeBorder(nwidth, nheight);
            border_.Dilate(size);

            result.DrawImage(Point.Empty, border_);

            var x = nwidth / 2 - self.Width / 2;
            var y = nheight / 2 - self.Height / 2;

            //result.DrawImage(new Point(x, y), self);

            return result;
        }
        public static Image<BGRA32> Shadow(this Image<BGRA32> self, int x, int y, int blur, float alpha, BGRA32 color)
        {
            if (blur < 0) throw new ArgumentException("blur < 0");
            self.ThrowIfDisposed();

            var shadow = self.Clone();
            var w = shadow.Width + blur;
            var h = shadow.Height + blur;
            shadow = shadow.MakeBorder(w, h);
            shadow.SetColor(color);
            shadow.SetAlpha(alpha);

            //キャンバスのサイズ
            var size_w = (Math.Abs(x) + (shadow.Width / 2)) * 2;
            var size_h = (Math.Abs(x) + (shadow.Height / 2)) * 2;

            var result = new Image<BGRA32>(size_w, size_h);

            result.DrawImage(
                new(
                    (result.Width / 2 - shadow.Width / 2) + x,
                    (result.Height / 2 - shadow.Height / 2) + y),
                shadow);

            result.DrawImage(
                new(
                    (result.Width / 2 - self.Width / 2) + x,
                    (result.Height / 2 - self.Height / 2) + y),
                self);

            shadow.Dispose();

            return result;
        }

        public static void BoxBlur<T>(this Image<T> self, float size) where T : unmanaged, IPixel<T>
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            self.ThrowIfDisposed();

            fixed (T* data = self.Data)
            {
                var str = self.ToStruct(data);
                Native.Image_BoxBlur(str, size, str);
            }
        }
        public static void GanssBlur<T>(this Image<T> self, float size) where T : unmanaged, IPixel<T>
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            self.ThrowIfDisposed();

            fixed (T* data = self.Data)
            {
                var str = self.ToStruct(data);
                Native.Image_GaussBlur(str, size, str);
            }
        }
        public static void MedianBlur<T>(this Image<T> self, int size) where T : unmanaged, IPixel<T>
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            self.ThrowIfDisposed();

            fixed (T* data = self.Data)
            {
                var str = self.ToStruct(data);
                Native.Image_MedianBlur(str, size, str);
            }
        }
        public static void Dilate<T>(this Image<T> self, int f) where T : unmanaged, IPixel<T>
        {
            self.ThrowIfDisposed();

            fixed (T* data = self.Data)
            {
                var str = self.ToStruct(data);
                Native.Image_Dilate(str, f, str);
            }
        }
        public static void Erode<T>(this Image<T> self, int f) where T : unmanaged, IPixel<T>
        {
            self.ThrowIfDisposed();

            fixed (T* data = self.Data)
            {
                var str = self.ToStruct(data);
                Native.Image_Erode(str, f, str);
            }
        }
    }
}