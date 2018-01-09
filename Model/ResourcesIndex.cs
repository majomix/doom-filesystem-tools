using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomFileSystemTools.Model
{
    public class ResourcesIndex
    {
        private readonly IEnumerable<byte> allowedVersions = new byte[] { 0x05 };
        private readonly string allowedSignature = @"SER";

        private byte myVersion;
        private string mySignature;

        public byte Version
        {
            get { return myVersion; }
            set
            {
                if (!allowedVersions.Contains(value))
                {
                    throw new InvalidDataException();
                }
                else
                {
                    myVersion = value;
                }
            }
        }
        public string Signature
        {
            get { return mySignature; }
            set
            {
                if (!allowedSignature.Equals(value))
                {
                    throw new InvalidDataException();
                }
                else
                {
                    mySignature = value;
                }
            }
        }
        public UInt32 FileSize { get; set; }
        public byte[] Padding { get; set; }
        public UInt32 NumberOfEntries { get; set; }
        public List<ResourcesIndexEntry> Entries { get; set; }

        public ResourcesIndex()
        {
            Entries = new List<ResourcesIndexEntry>();
        }
    }

    public class ResourcesIndexEntry
    {
        public UInt32 Identifier { get; set; }
        public string ResourceType { get; set; }
        public string InternalName { get; set; }
        public string FileSystemName { get; set; }
        public UInt64 Offset { get; set; }
        public UInt32 UncompressedSize { get; set; }
        public UInt32 CompressedSize { get; set; }
        public UInt32 Zero { get; set; }
        public byte PatchNumber { get; set; }
        public string Changed { get; set; }
    }
}
