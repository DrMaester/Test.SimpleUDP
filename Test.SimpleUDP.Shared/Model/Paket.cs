using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test.SimpleUDP.Shared.Model
{
    public class Paket
    {
        public int PaketNumber { get; set; }
        public int PaketMaxCount { get; set; }
        public Guid PaketGroupId { get; set; }
        public PaketType PaketType { get; set; }
        public PaketPosition PaketPosition { get; set; }
        public byte[] Data { get; set; }

        public Paket()
        {

        }

        public Paket(byte[] paketBuffer)
        {
            using (var memoryStream = new MemoryStream(paketBuffer))
            {
                PaketNumber = memoryStream.ReadByte();
                PaketMaxCount = memoryStream.ReadByte();
                
                byte[] guidRaw = new byte[16];
                memoryStream.Read(guidRaw);
                PaketGroupId = new Guid(guidRaw);

                PaketType = (PaketType)memoryStream.ReadByte();
                PaketPosition = (PaketPosition)memoryStream.ReadByte();

                Data = new byte[memoryStream.Length - memoryStream.Position];
                memoryStream.Read(Data);
            }
        }

        public byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.WriteByte((byte)PaketNumber); // 1 Byte
                memoryStream.WriteByte((byte)PaketMaxCount); // 1 Byte
                memoryStream.Write(PaketGroupId.ToByteArray()); // 16 Bytes
                memoryStream.WriteByte((byte)PaketType); // 1 Byte
                memoryStream.WriteByte((byte)PaketPosition); // 1 Byte
                memoryStream.Write(Data); // 

                return memoryStream.ToArray();
            }
        }

        public static int GetSizeOfPaketWithoutData()
        {
            // PaketNumber (int) +1
            // PaketMaxCount (int) +1
            // PaketGroupId (Guid) +16
            // PaketType (enum) +1
            // PaketPosition (enum) +1
            return 20;
        }
    }
}
