﻿// dnlib: See LICENSE.txt for more info

using System;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    public sealed class MsfStream
    {
        public MsfStream(IImageStream[] pages, uint length)
        {
            var buf = new byte[length];
            var offset = 0;
            foreach (var page in pages)
            {
                page.Position = 0;
                var len = Math.Min((int) page.Length, (int) (length - offset));
                offset += page.Read(buf, offset, len);
            }

            Content = new MemoryImageStream(0, buf, 0, buf.Length);
        }

        public IImageStream Content { get; set; }
    }
}
