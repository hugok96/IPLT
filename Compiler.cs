using IPLT.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPLT
{
    class Compiler
    {
        public static void Compile(string inputPath, string outputPath)
        {
            #region Preprocessing
            using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            var root = new RootNode();

            foreach (string line in File.ReadLines(inputPath))
            {
                if (line.Length < 1 || false == int.TryParse(line[0].ToString(), out _))
                    continue;

                string[] segments = line.Split('.');
                if (segments.Length > 4)
                {
                    Console.WriteLine($"Warning: Only IPv4 addresses are allowed, value {line} skipped!");
                    continue;
                }

                ITreeNode parent = null;
                foreach (string segment in segments)
                {
                    ITreeNode node, possibleDuplicate;
                    List<ITreeNode> list = (null == parent ? root.Children : parent.Children);

                    // Wildcard
                    if (segment == "*")
                    {
                        if (null == parent)
                        {
                            Console.WriteLine($"Warning: Malformed line found, value {line} skipped!");
                            goto continue2;
                        }

                        possibleDuplicate = list.FirstOrDefault(n => n.ValueMatches(new byte[] { }));
                        if (null != possibleDuplicate)
                        {
                            parent = possibleDuplicate;
                            continue;
                        }

                        node = new WildcardNode();
                        list.Add(node);
                        parent = node;
                        continue;
                    }

                    // Range
                    if (-1 != segment.IndexOf('-'))
                    {
                        string[] range = segment.Split('-');
                        byte startingValue, endingValue;
                        if (2 != range.Length || false == byte.TryParse(range[0], out startingValue) || false == byte.TryParse(range[1], out endingValue))
                        {
                            Console.WriteLine($"Warning: Malformed range found, value {line} skipped!");
                            goto continue2;
                        }

                        possibleDuplicate = list.FirstOrDefault(n => n.ValueMatches(new byte[] { startingValue, endingValue }));
                        if (null != possibleDuplicate)
                        {
                            parent = possibleDuplicate;
                            continue;
                        }

                        node = new RangeNode(startingValue, endingValue);
                        list.Add(node);
                        parent = node;
                        continue;
                    }

                    byte value;
                    if (false == byte.TryParse(segment, out value))
                    {
                        Console.WriteLine($"Warning: Malformed value found, value {line} skipped!");
                        goto continue2;
                    }

                    possibleDuplicate = list.FirstOrDefault(n => n.ValueMatches(new byte[] { value }));
                    if (null != possibleDuplicate)
                    {
                        parent = possibleDuplicate;
                        continue;
                    }

                    node = new ValueNode(value);
                    list.Add(node);
                    parent = node;
                    continue;
                }

            continue2:;
            }
            #endregion

            #region Writing
            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            var outFile = new BinaryWriter(outputStream);

            // Write header
            new Header(0b1, (byte)root.Children.Count).Output(ref outFile);

            WriteNodes(ref outFile, root);
            root.Children.Select(n => n.Children);
            foreach (ITreeNode parent in root.Children)
            {
                WriteNodes(ref outFile, parent);
            }

            foreach (ITreeNode parent in root.Children.ConvertAll(n => n.Children).SelectMany(x => x))
            {
                WriteNodes(ref outFile, parent);
            }

            foreach (ITreeNode parent in root.Children.ConvertAll(n => n.Children.ConvertAll(n2 => n2.Children)).SelectMany(x => x.SelectMany(y => y)))
            {
                WriteNodes(ref outFile, parent);
            }

            outFile.Dispose();
            #endregion
        }

        private static void WriteNodes(ref BinaryWriter outFile, ITreeNode parent)
        {
            bool first = true;
            foreach (ITreeNode node in parent.Children)
            {
                var pointer = (uint)outFile.BaseStream.Position;
                outFile.Write(node.GetInstanceTypeId());
                if (node is ValueNode valueNode)
                {
                    outFile.Write(valueNode.Value);
                }
                else if (node is RangeNode rangeNode)
                {
                    outFile.Write(rangeNode.StartingValue);
                    outFile.Write(rangeNode.EndingValue);
                }
                outFile.Write((byte)node.Children.Count);

                node.Pointer = (uint)outFile.BaseStream.Position;
                if (false == node.IsFinalNode)
                    outFile.Write((uint)0);

                if (parent is RootNode || !first)
                    continue;

                outFile.Seek((int)parent.Pointer, SeekOrigin.Begin);
                outFile.Write(pointer);
                outFile.Seek(0, SeekOrigin.End);
                first = false;
            }
        }



        class Header : IOutputable
        {
            public char[] MagicNumber;
            public byte Version;
            public byte NodeCount;

            public Header(byte version, byte nodeCount)
            {
                MagicNumber = new char[4] { 'I', 'P', 'L', 'T' };
                Version = version;
                NodeCount = nodeCount;
            }

            public void Output(ref BinaryWriter writer)
            {
                writer.Write(MagicNumber);
                writer.Write(Version);
                writer.Write(NodeCount);
            }
        }

        interface IOutputable
        {
            public void Output(ref BinaryWriter writer);
        }
    }
}
