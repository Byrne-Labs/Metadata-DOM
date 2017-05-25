// dnlib: See LICENSE.txt for more info

using System;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    public sealed class DbiDocument
    {
        public DbiDocument(string url)
        {
            URL = url;
            DocumentType = SymDocumentType.Text;
        }

        public byte[] CheckSum { get; private set; }

        public Guid CheckSumAlgorithmId { get; private set; }

        public Guid DocumentType { get; private set; }

        public Guid Language { get; private set; }

        public Guid LanguageVendor { get; private set; }

        public string URL { get; private set; }

        public void Read(IImageStream stream)
        {
            stream.Position = 0;
            Language = new Guid(stream.ReadBytes(0x10));
            LanguageVendor = new Guid(stream.ReadBytes(0x10));
            DocumentType = new Guid(stream.ReadBytes(0x10));
            CheckSumAlgorithmId = new Guid(stream.ReadBytes(0x10));

            var len = stream.ReadInt32();
            if (stream.ReadUInt32() != 0)
            {
                throw new PdbException("Unexpected value");
            }

            CheckSum = stream.ReadBytes(len);
        }
    }
}
