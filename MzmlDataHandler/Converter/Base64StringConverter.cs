using System;
using System.IO;
using zlib;

namespace BUDDY.MzmlDataHandler.Converter
{
    public class Base64StringConverter
    {
        public Base64ArrayPrecision Precision { get; set; }

        public Base64ArrayByteOrder ByteOrder { get; set; }

        public Base64ArrayCompression Compression { get; set; }

        public Base64StringConverter()
        {
            this.Precision = Base64ArrayPrecision.Real32;
            this.ByteOrder = Base64ArrayByteOrder.LittleEndian;
            this.Compression = Base64ArrayCompression.None;
        }

        public float[] FromBase64ToFloatArray(string base64string) => (float[])this.FromBase64ToArray(base64string);

        public Array FromBase64ToArray(string base64string)
        {
            byte[] byteArray = Convert.FromBase64String(base64string);
            if (this.Compression == Base64ArrayCompression.Zlib)
                byteArray = this.getUncompressedByteArray(byteArray);
            int num = this.Precision == Base64ArrayPrecision.Real64 ? 64 : 32;
            int byteLengthOfOneElem = num / 8;
            int length = byteArray.Length / byteLengthOfOneElem;
            Array array = num != 64 ? (Array)new float[length] : (Array)new double[length];
            int i = 0;
            int index = 0;
            while (i < byteArray.Length - byteLengthOfOneElem)
            {
                byte[] bufferContents = this.getBufferContents(this.ByteOrder, byteArray, i, byteLengthOfOneElem);
                if (this.Precision == Base64ArrayPrecision.Real64)
                    ((double[])array)[index] = BitConverter.ToDouble(bufferContents, 0);
                else
                    ((float[])array)[index] = BitConverter.ToSingle(bufferContents, 0);
                i += byteLengthOfOneElem;
                ++index;
            }
            return array;
        }

        private byte[] getBufferContents(
          Base64ArrayByteOrder base64ArrayByteOrder,
          byte[] byteArray,
          int i,
          int byteLengthOfOneElem)
        {
            byte[] numArray = new byte[byteLengthOfOneElem];
            if (base64ArrayByteOrder == Base64ArrayByteOrder.BigEndian)
            {
                for (int index = 0; index < byteLengthOfOneElem; ++index)
                    numArray[byteLengthOfOneElem - index - 1] = byteArray[i + index];
            }
            else
                Array.Copy((Array)byteArray, i, (Array)numArray, 0, byteLengthOfOneElem);
            return numArray;
        }

        private byte[] getUncompressedByteArray(byte[] byteArray)
        {
            MemoryStream memoryStream1 = new MemoryStream(byteArray);
            MemoryStream memoryStream2 = new MemoryStream();
            ZOutputStream zoutputStream = new ZOutputStream((Stream)memoryStream2);
            try
            {
                this.copyStream((Stream)memoryStream1, (Stream)zoutputStream);
            }
            finally
            {
                zoutputStream.Close();
                memoryStream2.Close();
                memoryStream1.Close();
            }
            byteArray = memoryStream2.ToArray();
            return byteArray;
        }

        private void copyStream(Stream input, Stream output)
        {
            int length = (int)input.Length;
            byte[] buffer = new byte[length];
            int count;
            while ((count = input.Read(buffer, 0, length)) > 0)
                output.Write(buffer, 0, count);
            output.Flush();
        }
    }
}
