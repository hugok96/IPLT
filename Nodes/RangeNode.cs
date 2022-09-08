using System.Collections.Generic;

namespace IPLT.Nodes
{
    class RangeNode : ITreeNode
    {
        public static byte GetTypeId() { return 0b010; }
        public byte GetInstanceTypeId() { return GetTypeId(); }
        public bool IsFinalNode { get => 0 == Children.Count; }
        public uint Pointer { get; set; }
        public byte StartingValue;
        public byte EndingValue;
        public List<ITreeNode> Children { get; set; } = new List<ITreeNode>();

        public RangeNode(byte startingValue, byte endingValue)
        {
            StartingValue = startingValue;
            EndingValue = endingValue;
        }

        public bool ValueMatches(byte[] input) => 2 == input.Length && input[0] == StartingValue && input[1] == EndingValue;
    }
}
