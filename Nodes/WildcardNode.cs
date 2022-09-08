using System.Collections.Generic;

namespace IPLT.Nodes
{
    class WildcardNode : ITreeNode
    {
        public static byte GetTypeId() { return 0b100; }
        public byte GetInstanceTypeId() { return GetTypeId(); }
        public bool IsFinalNode { get => 0 == Children.Count; }
        public uint Pointer { get; set; }
        public List<ITreeNode> Children { get; set; } = new List<ITreeNode>();
        public bool ValueMatches(byte[] input) => 0 == input.Length;
    }
}
