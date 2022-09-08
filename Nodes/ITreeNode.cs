using System;
using System.Collections.Generic;
using System.IO;

namespace IPLT.Nodes
{
    interface ITreeNode
    {
        public bool IsFinalNode { get; }
        public uint Pointer { get; set; }
        public List<ITreeNode> Children { get; set; }
        public bool ValueMatches(byte[] input) { return false; }
        public static byte GetTypeId() { throw new Exception("Not implemented"); }
        public byte GetInstanceTypeId();
    }
}
