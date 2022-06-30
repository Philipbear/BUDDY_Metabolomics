using System;
using System.Xml;

namespace BUDDY.MzmlDataHandler.Converter
{
    public class BinaryDataArrayConverter
    {
        private XmlReader xmlRdr;
        private Base64ArrayCompression compression;
        private Base64ArrayPrecision precision;

        public BinaryArrayContentType ContentType { get; private set; }

        public BinaryArrayValueType ValueType { get; private set; }

        public BinaryArrayUnit ValueUnit { get; private set; }

        public Array ValueArray { get; private set; }

        public int EncodedLength { get; private set; }

        private BinaryDataArrayConverter()
        {
            this.ContentType = BinaryArrayContentType.Undefined;
            this.ValueType = BinaryArrayValueType.Single;
            this.ValueArray = (Array)null;
            this.EncodedLength = 0;
        }

        public static BinaryDataArrayConverter Convert(XmlReader xmlRdr)
        {
            if (xmlRdr.NodeType != XmlNodeType.Element || xmlRdr.Name != "binaryDataArray")
                return (BinaryDataArrayConverter)null;
            BinaryDataArrayConverter dataArrayConverter = new BinaryDataArrayConverter();
            dataArrayConverter.xmlRdr = xmlRdr;
            while (xmlRdr.MoveToNextAttribute())
            {
                switch (xmlRdr.Name)
                {
                    case "encodedLength":
                        int result;
                        if (int.TryParse(xmlRdr.Value, out result))
                        {
                            dataArrayConverter.EncodedLength = result;
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }
            bool flag1 = false;
            bool flag2;
            do
            {
                flag2 = false;
                switch (xmlRdr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlRdr.Name)
                        {
                            case "binary":
                                flag1 = true;
                                break;
                            case "cvParam":
                                dataArrayConverter.parseCvParam();
                                break;
                        }
                        break;
                    case XmlNodeType.Text:
                        if (flag1)
                        {
                            dataArrayConverter.ValueArray = new Base64StringConverter()
                            {
                                Compression = dataArrayConverter.compression,
                                Precision = dataArrayConverter.precision
                            }.FromBase64ToArray(xmlRdr.Value);
                            break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (xmlRdr.Name == "binaryDataArray")
                            flag2 = true;
                        if (xmlRdr.Name == "binary")
                        {
                            flag1 = false;
                            break;
                        }
                        break;
                }
            }
            while (!flag2 && xmlRdr.Read());
            return dataArrayConverter;
        }

        private void parseCvParam()
        {
            while (this.xmlRdr.MoveToNextAttribute())
            {
                switch (this.xmlRdr.Name)
                {
                    case "accession":
                        switch (this.xmlRdr.Value)
                        {
                            case "MS:1000514":
                                this.ContentType = BinaryArrayContentType.MzArray;
                                continue;
                            case "MS:1000515":
                                this.ContentType = BinaryArrayContentType.IntensityArray;
                                continue;
                            case "MS:1000521":
                                this.ValueType = BinaryArrayValueType.Single;
                                this.precision = Base64ArrayPrecision.Real32;
                                continue;
                            case "MS:1000523":
                                this.ValueType = BinaryArrayValueType.Double;
                                this.precision = Base64ArrayPrecision.Real64;
                                continue;
                            case "MS:1000574":
                                this.compression = Base64ArrayCompression.Zlib;
                                continue;
                            case "MS:1000576":
                                this.compression = Base64ArrayCompression.None;
                                continue;
                            case "MS:1000595":
                                this.ContentType = BinaryArrayContentType.TimeArray;
                                continue;
                            default:
                                throw new NotSupportedException("BinaryDataArrayConverter: cvParam " + this.xmlRdr.Value + " is not supported yet.");
                        }
                    case "unitAccession":
                        switch (this.xmlRdr.Value)
                        {
                            case "UO:0000010":
                                this.ValueUnit = BinaryArrayUnit.Second;
                                continue;
                            case "UO:0000031":
                                this.ValueUnit = BinaryArrayUnit.Minute;
                                continue;
                            case "MS:1000040":
                                this.ValueUnit = BinaryArrayUnit.Mz;
                                continue;
                            case "MS:1000131":
                                this.ValueUnit = BinaryArrayUnit.NumberOfCount;
                                continue;
                            default:
                                throw new NotSupportedException("BinaryDataArrayConverter: cvParam (unit) " + this.xmlRdr.Value + " is not supported yet.");
                        }
                    default:
                        continue;
                }
            }
        }
    }
}
