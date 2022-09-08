using IPLT.Nodes;
using System;
using System.IO;
using System.Linq;

namespace IPLT
{
    class Reader
    {
        public static void SearchIP(string inputPath, byte[] ip)
        {
            using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(inputStream);

            if (!reader.ReadChars(4).SequenceEqual("IPLT".ToCharArray()) || reader.ReadByte() != 0b1)
            {
                Console.Error.WriteLine("Error: invalid filetype (wrong magic number or version number");
                return;
            }

            byte rootCount = reader.ReadByte();
            bool ipMatch = CheckElements(ref reader, rootCount, ip, 0);

            if (false == ipMatch)
            {
                Console.Error.WriteLine("No match found.");
            }
            else 
            {
                Console.WriteLine("Match found");
            }

            reader.Dispose();

        }

        private static bool CheckElements(ref BinaryReader reader,  byte elementCount, byte[] ip, byte depth)
        {
            byte ipSeg = ip[depth];
            for (byte i = 0; i < elementCount; i++)
            {
                byte type = reader.ReadByte();
                bool valueMatch = false;

                if (type == WildcardNode.GetTypeId()) // wildcard node
                {
                    valueMatch = true;
                }
                else if (type == ValueNode.GetTypeId()) // valuenode
                {
                    valueMatch = ipSeg == reader.ReadByte();
                }
                else if (type == RangeNode.GetTypeId()) // rangenode
                {
                    valueMatch = ipSeg >= reader.ReadByte() && ipSeg <= reader.ReadByte();
                }

                byte childCount = reader.ReadByte();
                if (valueMatch)
                {
                    if (childCount == 0)                    
                        return true;
                    
                    reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                    if (CheckElements(ref reader, childCount, ip, (byte)(depth + 1)))
                        return true;
                }
                else if (childCount != 0)
                    reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            return false;
        }
    }
}
