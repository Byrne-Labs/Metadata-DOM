﻿// dnlib: See LICENSE.txt for more info

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    /// <summary>A managed PDB reader implementation for .NET modules.</summary>
    public sealed class PdbReader
    {
        private const int STREAM_DBI = 3;
        private const ushort STREAM_INVALID_INDEX = ushort.MaxValue;
        private const int STREAM_NAMES = 1;
        private const int STREAM_ROOT = 0;
        private const int STREAM_TPI = 2;
        private Dictionary<string, DbiDocument> documents;
        private uint entryPt;
        private Dictionary<uint, DbiFunction> functions;
        private List<DbiModule> modules;
        private Dictionary<string, uint> names;
        private MsfStream[] streams;
        private Dictionary<uint, string> strings;

        /// <summary>The age of PDB file.</summary>
        public uint Age { get; private set; }

        /// <summary>The GUID of PDB file.</summary>
        public Guid Guid { get; private set; }

        public SymbolToken UserEntryPoint
        {
            get
            {
                return new SymbolToken((int) entryPt);
            }
        }

        internal static string ReadCString(IImageStream stream)
        {
            var value = Encoding.UTF8.GetString(stream.ReadBytesUntilByte(0));
            stream.Position++;
            return value;
        }

        private static uint ReadSizeField(IBinaryReader reader)
        {
            var size = reader.ReadInt32();
            return size <= 0 ? 0 : (uint) size;
        }

        private static uint RoundUpDiv(uint value, uint divisor) => (value + divisor - 1) / divisor;

        public DbiDocument[] GetDocuments()
        {
            var docs = new DbiDocument[documents.Count];
            var i = 0;
            foreach (var doc in documents.Values)
            {
                docs[i++] = doc;
            }

            return docs;
        }

        public DbiFunction GetMethod(SymbolToken method)
        {
            DbiFunction symMethod;
            if (functions.TryGetValue((uint) method.GetToken(), out symMethod))
            {
                return symMethod;
            }

            return null;
        }

        /// <summary>Read the PDB in the specified stream.</summary>
        /// <param name="stream">The stream that contains the PDB file</param>
        public void Read(IImageStream stream)
        {
            try
            {
                ReadInternal(stream);
            }
            catch (Exception ex)
            {
                if (ex is PdbException)
                {
                    throw;
                }

                throw new PdbException(ex);
            }
            finally
            {
                streams = null;
                names = null;
                strings = null;
                modules = null;
            }
        }

        internal DbiDocument GetDocument(uint nameId)
        {
            var name = strings[nameId];

            DbiDocument doc;
            if (!documents.TryGetValue(name, out doc))
            {
                doc = new DbiDocument(name);

                uint streamId;
                if (names.TryGetValue("/src/files/" + name, out streamId))
                {
                    doc.Read(streams[streamId].Content);
                }
                documents.Add(name, doc);
            }
            return doc;
        }

        private void ApplyRidMap(IImageStream stream)
        {
            stream.Position = 0;
            var map = new uint[stream.Length / 4];
            for (var i = 0; i < map.Length; i++)
            {
                map[i] = stream.ReadUInt32();
            }

            foreach (var module in modules)
            foreach (var func in module.Functions)
            {
                var rid = func.Token & 0x00ffffff;
                rid = map[rid];
                func.Token = func.Token & 0xff000000 | rid;
            }

            if (entryPt != 0)
            {
                var rid = entryPt & 0x00ffffff;
                rid = map[rid];
                entryPt = entryPt & 0xff000000 | rid;
            }
        }

        private bool IsValidStreamIndex(ushort index) => index != STREAM_INVALID_INDEX && index < streams.Length;

        private void ReadGlobalSymbols(IImageStream stream)
        {
            stream.Position = 0;
            while (stream.Position < stream.Length)
            {
                var size = stream.ReadUInt16();
                var begin = stream.Position;
                var end = begin + size;

                if ((SymbolType) stream.ReadUInt16() == SymbolType.S_PUB32)
                {
                    stream.Position += 4;
                    var offset = stream.ReadUInt32();
                    stream.Position += 2;
                    var name = ReadCString(stream);

                    if (name == "COM+_Entry_Point")
                    {
                        entryPt = offset;
                    }
                }

                stream.Position = end;
            }
        }

        private void ReadInternal(IImageStream stream)
        {
            stream.Position = 0;
            var sig = Encoding.ASCII.GetString(stream.ReadBytes(30));
            if (sig != "Microsoft C/C++ MSF 7.00\r\n\u001ADS\0")
            {
                throw new PdbException("Invalid signature");
            }

            stream.Position += 2;

            var pageSize = stream.ReadUInt32();
            /*uint fpm = */
            stream.ReadUInt32();
            var pageCount = stream.ReadUInt32();
            var rootSize = stream.ReadUInt32();
            stream.ReadUInt32();
            var numOfRootPages = RoundUpDiv(rootSize, pageSize);
            var numOfPtrPages = RoundUpDiv(numOfRootPages * 4, pageSize);
            if (pageCount * pageSize != stream.Length)
            {
                throw new PdbException("File size mismatch");
            }

            var pages = new IImageStream[pageCount];
            try
            {
                FileOffset offset = 0;
                for (uint i = 0; i < pageCount; i++)
                {
                    pages[i] = stream.Create(offset, pageSize);
                    offset += pageSize;
                }

                var rootPages = new IImageStream[numOfRootPages];
                var pageIndex = 0;
                for (var i = 0; i < numOfPtrPages && pageIndex < numOfRootPages; i++)
                {
                    var ptrPage = pages[stream.ReadUInt32()];
                    ptrPage.Position = 0;
                    for (; ptrPage.Position < ptrPage.Length && pageIndex < numOfRootPages; pageIndex++)
                    {
                        rootPages[pageIndex] = pages[ptrPage.ReadUInt32()];
                    }
                }

                ReadRootDirectory(new MsfStream(rootPages, rootSize), pages, pageSize);
            }
            finally
            {
                foreach (var page in pages)
                {
                    if (page != null)
                    {
                        page.Dispose();
                    }
                }
            }

            ReadNames();
            ReadStringTable();
            var tokenMapStream = ReadModules();

            documents = new Dictionary<string, DbiDocument>(StringComparer.OrdinalIgnoreCase);
            foreach (var module in modules)
            {
                if (IsValidStreamIndex(module.StreamId))
                {
                    module.LoadFunctions(this, streams[module.StreamId].Content);
                }
            }

            if (IsValidStreamIndex(tokenMapStream ?? STREAM_INVALID_INDEX))
            {
                ApplyRidMap(streams[tokenMapStream.Value].Content);
            }

            functions = new Dictionary<uint, DbiFunction>();
            foreach (var module in modules)
            foreach (var func in module.Functions)
            {
                functions.Add(func.Token, func);
            }
        }

        private ushort? ReadModules()
        {
            var stream = streams[STREAM_DBI].Content;
            stream.Position = 20;
            var symrecStream = stream.ReadUInt16();
            stream.Position += 2;
            var gpmodiSize = ReadSizeField(stream); // gpmodiSize
            uint otherSize = 0;
            otherSize += ReadSizeField(stream); // secconSize
            otherSize += ReadSizeField(stream); // secmapSize
            otherSize += ReadSizeField(stream); // filinfSize
            otherSize += ReadSizeField(stream); // tsmapSize
            stream.ReadUInt32(); // mfcIndex
            var dbghdrSize = ReadSizeField(stream);
            otherSize += ReadSizeField(stream); // ecinfoSize
            stream.Position += 8;

            modules = new List<DbiModule>();
            using (var moduleStream = stream.Create((FileOffset) stream.Position, gpmodiSize))
            {
                while (moduleStream.Position < moduleStream.Length)
                {
                    var module = new DbiModule();
                    module.Read(moduleStream);
                    modules.Add(module);
                }
            }

            if (IsValidStreamIndex(symrecStream))
            {
                ReadGlobalSymbols(streams[symrecStream].Content);
            }

            if (dbghdrSize != 0)
            {
                stream.Position += gpmodiSize;
                stream.Position += otherSize;
                stream.Position += 12;
                return stream.ReadUInt16();
            }

            return null;
        }

        private void ReadNames()
        {
            var stream = streams[STREAM_NAMES].Content;
            stream.Position = 8;
            Age = stream.ReadUInt32();
            Guid = new Guid(stream.ReadBytes(0x10));

            var nameSize = stream.ReadUInt32();
            using (var nameData = stream.Create(stream.FileOffset + stream.Position, nameSize))
            {
                stream.Position += nameSize;

                /*uint entryCount = */
                stream.ReadUInt32();
                var entryCapacity = stream.ReadUInt32();
                var entryOk = new BitArray(stream.ReadBytes(stream.ReadInt32() * 4));
                if (stream.ReadUInt32() != 0)
                {
                    throw new NotSupportedException();
                }

                names = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
                entryCapacity = Math.Min(entryCapacity, (uint) entryOk.Length);
                for (var i = 0; i < entryCapacity; i++)
                {
                    if (!entryOk[i])
                    {
                        continue;
                    }

                    var pos = stream.ReadUInt32();
                    var streamId = stream.ReadUInt32();
                    nameData.Position = pos;
                    var streamName = ReadCString(nameData);
                    names[streamName] = streamId;
                }
            }
        }

        private void ReadRootDirectory(MsfStream stream, IImageStream[] pages, uint pageSize)
        {
            var streamNum = stream.Content.ReadUInt32();
            var streamSizes = new uint[streamNum];
            for (var i = 0; i < streamSizes.Length; i++)
            {
                streamSizes[i] = stream.Content.ReadUInt32();
            }

            streams = new MsfStream[streamNum];
            for (var i = 0; i < streamSizes.Length; i++)
            {
                if (streamSizes[i] == 0xffffffff)
                {
                    streams[i] = null;
                    continue;
                }

                var pageCount = RoundUpDiv(streamSizes[i], pageSize);
                var streamPages = new IImageStream[pageCount];
                for (var j = 0; j < streamPages.Length; j++)
                {
                    streamPages[j] = pages[stream.Content.ReadUInt32()];
                }

                streams[i] = new MsfStream(streamPages, streamSizes[i]);
            }
        }

        private void ReadStringTable()
        {
            uint streamId;
            if (!names.TryGetValue("/names", out streamId))
            {
                throw new PdbException("String table not found");
            }

            var stream = streams[streamId].Content;
            stream.Position = 8;

            var strSize = stream.ReadUInt32();
            using (var strData = stream.Create(stream.FileOffset + stream.Position, strSize))
            {
                stream.Position += strSize;

                strings = new Dictionary<uint, string>();
                var count = stream.ReadUInt32();
                for (uint i = 0; i < count; i++)
                {
                    var pos = stream.ReadUInt32();
                    if (pos == 0)
                    {
                        continue;
                    }

                    strData.Position = pos;
                    strings[pos] = ReadCString(strData);
                }
            }
        }
    }
}
