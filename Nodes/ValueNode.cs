using System.Collections.Generic;

namespace IPLT.Nodes
{
    class ValueNode : ITreeNode
    {
        public static byte GetTypeId() { return 0b001; }
        public byte GetInstanceTypeId() { return GetTypeId(); }
        public byte Value;
        public bool IsFinalNode { get => 0 == Children.Count; }
        public uint Pointer { get; set; }
        public List<ITreeNode> Children { get; set; } = new List<ITreeNode>();

        public ValueNode(byte value)
        {
            Value = value;
        }

        public bool ValueMatches(byte[] input) => 1 == input.Length && Value == input[0];
    }
}
