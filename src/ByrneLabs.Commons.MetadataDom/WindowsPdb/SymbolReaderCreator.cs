// dnlib: See LICENSE.txt for more info

using System;
using System.IO;
using System.Security;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    /// <summary>Creates a <see cref="PdbReader" /> instance</summary>
    public static class SymbolReaderCreator
    {
        /// <summary>Creates a new <see cref="PdbReader" /> instance</summary>
        /// <param name="pdbFileName">Path to PDB file</param>
        /// <returns>A new <see cref="PdbReader" /> instance or <c>null</c> if there's no PDB file on disk.</returns>
        public static PdbReader Create(string pdbFileName) => Create(OpenImageStream(pdbFileName));

        /// <summary>Creates a new <see cref="PdbReader" /> instance</summary>
        /// <param name="pdbData">PDB file data</param>
        /// <returns>A new <see cref="PdbReader" /> instance or <c>null</c>.</returns>
        public static PdbReader Create(byte[] pdbData) => Create(MemoryImageStream.Create(pdbData));

        /// <summary>Creates a new <see cref="PdbReader" /> instance</summary>
        /// <param name="pdbStream">PDB file stream which is now owned by this method</param>
        /// <returns>A new <see cref="PdbReader" /> instance or <c>null</c>.</returns>
        public static PdbReader Create(IImageStream pdbStream)
        {
            if (pdbStream == null)
            {
                return null;
            }

            try
            {
                var pdbReader = new PdbReader();
                pdbReader.Read(pdbStream);
                return pdbReader;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
            finally
            {
                pdbStream?.Dispose();
            }

            return null;
        }

        /// <summary>Creates a new <see cref="PdbReader" /> instance</summary>
        /// <param name="assemblyFileName">Path to assembly</param>
        /// <returns>A new <see cref="PdbReader" /> instance or <c>null</c> if there's no PDB file.</returns>
        public static PdbReader CreateFromAssemblyFile(string assemblyFileName) => Create(Path.ChangeExtension(assemblyFileName, "pdb"));

        private static IImageStream OpenImageStream(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            var fileBytes = File.ReadAllBytes(fileName);
            return MemoryImageStream.Create(fileBytes);
        }
    }
}
