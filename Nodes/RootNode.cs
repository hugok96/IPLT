using System.Collections.Generic;

namespace IPLT.Nodes
{
    class RootNode : ITreeNode
    {
        public static byte GetTypeId() { return 0b000; }
        public byte GetInstanceTypeId() { return GetTypeId(); }
        public byte Value;
        public bool IsFinalNode { get => 0 == Children.Count; }
        public uint Pointer { get; set; }
        public List<ITreeNode> Children { get; set; } = new List<ITreeNode>();

        public bool ValueMatches(byte[] input) => false;
    }
}
