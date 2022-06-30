using BUDDY.MzmlDataHandler.Converter;
using BUDDY.RawData;
using BUDDY.RawDataHandlerCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace BUDDY.MzmlDataHandler.Parser
{
    public class MzmlReader
    {
        private XmlReader xmlRdr;
        private RAW_Chromatogram currentChromato;
        private RAW_Spectrum currentSpectrum;
        private RAW_PrecursorIon currentPrecursor;
        private RAW_ProductIon currentProduct;
        private const double initialProgress = 0.0;
        private const double progressMax = 100.0;

        public string MzMLfilePath { get; private set; }

        public int SpectraCount { get; private set; }

        public MzMlDataFileContent FileContent { get; private set; }

        public List<RAW_SourceFileInfo> SourceFiles { get; private set; }

        public List<RAW_Sample> Samples { get; private set; }

        public DateTime StartTimeStamp { get; private set; }

        public List<RAW_Spectrum> SpectraList { get; private set; }

        public List<RAW_Spectrum> AccumulatedMs1SpectrumList { get; private set; }

        public List<RAW_Chromatogram> ChromatogramsList { get; private set; }

        public bool IsIonMobilityData { get; set; }

        public BackgroundWorker bgWorker { get; set; }

        public bool IsGuiProcess { get; set; }

        public RAW_Measurement ReadMzml(
          string inputMzMLfilePath,
          int fileID,
          bool isGuiProcess,
          BackgroundWorker bgWorker = null)
        {
            this.SourceFiles = new List<RAW_SourceFileInfo>();
            this.Samples = new List<RAW_Sample>();
            this.StartTimeStamp = new DateTime();
            this.SpectraList = (List<RAW_Spectrum>)null;
            this.ChromatogramsList = (List<RAW_Chromatogram>)null;
            this.IsIonMobilityData = false;
            this.bgWorker = bgWorker;
            this.IsGuiProcess = isGuiProcess;
            using (FileStream fileStream = new FileStream(inputMzMLfilePath, FileMode.Open, FileAccess.Read))
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader((Stream)fileStream))
                {
                    this.xmlRdr = (XmlReader)xmlTextReader;
                    while (xmlTextReader.Read())
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlTextReader.Name)
                            {
                                case "fileDescription":
                                    this.parseFileDescription();
                                    continue;
                                case "run":
                                    this.parseRun();
                                    continue;
                                case "sampleList":
                                    this.parseSampleList();
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            if (this.IsIonMobilityData)
                this.createAccumulatedSpectralData();
            if (this.SourceFiles.Count == 0)
                this.SourceFiles.Add(new RAW_SourceFileInfo()
                {
                    Id = fileID.ToString(),
                    Location = inputMzMLfilePath,
                    Name = Path.GetFileNameWithoutExtension(inputMzMLfilePath)
                });
            if (this.Samples.Count == 0)
                this.Samples.Add(new RAW_Sample()
                {
                    Id = fileID.ToString(),
                    Name = Path.GetFileNameWithoutExtension(inputMzMLfilePath)
                });
            return new RAW_Measurement()
            {
                SourceFileInfo = this.SourceFiles[0],
                Sample = this.Samples[0],
                SpectrumList = this.SpectraList,
                ChromatogramList = this.ChromatogramsList,
                AccumulatedSpectrumList = this.AccumulatedMs1SpectrumList
            };
        }

        public RAW_Measurement ReadMzmlVer2(string inputMzMLfilePath, int fileID)
        {
            this.SourceFiles = new List<RAW_SourceFileInfo>();
            this.Samples = new List<RAW_Sample>();
            this.StartTimeStamp = new DateTime();
            this.SpectraList = (List<RAW_Spectrum>)null;
            this.ChromatogramsList = (List<RAW_Chromatogram>)null;
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            using (FileStream fileStream = new FileStream(inputMzMLfilePath, FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create((Stream)fileStream, settings))
                {
                    this.xmlRdr = xmlReader;
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "fileDescription":
                                    this.parseFileDescription();
                                    continue;
                                case "run":
                                    this.parseRun();
                                    continue;
                                case "sampleList":
                                    this.parseSampleList();
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            if (this.SourceFiles.Count == 0)
                this.SourceFiles.Add(new RAW_SourceFileInfo()
                {
                    Id = fileID.ToString(),
                    Location = inputMzMLfilePath,
                    Name = Path.GetFileNameWithoutExtension(inputMzMLfilePath)
                });
            if (this.Samples.Count == 0)
                this.Samples.Add(new RAW_Sample()
                {
                    Id = fileID.ToString(),
                    Name = Path.GetFileNameWithoutExtension(inputMzMLfilePath)
                });
            return new RAW_Measurement()
            {
                SourceFileInfo = this.SourceFiles[0],
                Sample = this.Samples[0],
                SpectrumList = this.SpectraList,
                AccumulatedSpectrumList = this.AccumulatedMs1SpectrumList,
                ChromatogramList = this.ChromatogramsList
            };
        }

        private void createAccumulatedSpectralData()
        {
            if (this.SpectraList == null || this.SpectraList.Count == 0)
                return;
            this.AccumulatedMs1SpectrumList = new List<RAW_Spectrum>();
            RAW_Spectrum spectra1 = this.SpectraList[0];
            if (this.SpectraList != null && this.SpectraList.Count > 0)
            {
                this.SpectraList = this.SpectraList.OrderBy<RAW_Spectrum, double>((Func<RAW_Spectrum, double>)(n => n.ScanStartTime)).ThenBy<RAW_Spectrum, double>((Func<RAW_Spectrum, double>)(n => n.DriftTime)).ToList<RAW_Spectrum>();
                RAW_Spectrum rawSpectrum = this.SpectraList[0];
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                foreach (RAW_Spectrum spectra2 in this.SpectraList)
                {
                    spectra2.Index = num1;
                    spectra2.OriginalIndex = num1;
                    ++num1;
                    if (rawSpectrum.ScanStartTime == spectra2.ScanStartTime)
                    {
                        if (spectra2.Index != 0)
                            ++num3;
                        spectra2.ScanNumber = num2;
                        spectra2.DriftScanNumber = num3;
                    }
                    else
                    {
                        num3 = 0;
                        ++num2;
                        rawSpectrum = spectra2;
                        spectra2.ScanNumber = num2;
                        spectra2.DriftScanNumber = num3;
                    }
                }
            }
            Dictionary<int, double[]> dictionary = new Dictionary<int, double[]>();
            int num4 = 0;
            RAW_Spectrum spec = this.SpectraList[0];
            foreach (RAW_Spectrum rawSpectrum in this.SpectraList.Where<RAW_Spectrum>((Func<RAW_Spectrum, bool>)(n => n.MsLevel == 1)))
            {
                if (rawSpectrum.ScanStartTime == spec.ScanStartTime)
                {
                    RAW_PeakElement[] spectrum = rawSpectrum.Spectrum;
                    for (int index = 0; index < spectrum.Length; ++index)
                        SpectrumParser.AddToMassBinDictionary(dictionary, spectrum[index].Mz, spectrum[index].Intensity);
                }
                else
                {
                    this.AccumulatedMs1SpectrumList.Add(this.getAccumulatedSpecObject(spec, dictionary));
                    dictionary = new Dictionary<int, double[]>();
                    ++num4;
                    spec = rawSpectrum;
                }
            }
            this.AccumulatedMs1SpectrumList.Add(this.getAccumulatedSpecObject(spec, dictionary));
            if (this.AccumulatedMs1SpectrumList == null || this.AccumulatedMs1SpectrumList.Count <= 0)
                return;
            this.AccumulatedMs1SpectrumList = this.AccumulatedMs1SpectrumList.OrderBy<RAW_Spectrum, int>((Func<RAW_Spectrum, int>)(n => n.ScanNumber)).ToList<RAW_Spectrum>();
            List<RAW_Spectrum> rawSpectrumList = new List<RAW_Spectrum>();
            int num5 = -1;
            foreach (RAW_Spectrum rawSpectrum in this.SpectraList.Where<RAW_Spectrum>((Func<RAW_Spectrum, bool>)(n => n.MsLevel == 1)))
            {
                if (rawSpectrum.ScanNumber != num5)
                {
                    rawSpectrumList.Add(rawSpectrum);
                    num5 = rawSpectrum.ScanNumber;
                }
            }
            int num6 = 0;
            foreach (RAW_Spectrum accumulatedMs1Spectrum in this.AccumulatedMs1SpectrumList)
            {
                accumulatedMs1Spectrum.Index = num6;
                ++num6;
                foreach (RAW_Spectrum rawSpectrum in rawSpectrumList)
                {
                    if (accumulatedMs1Spectrum.ScanNumber == rawSpectrum.ScanNumber)
                    {
                        accumulatedMs1Spectrum.OriginalIndex = rawSpectrum.Index;
                        break;
                    }
                }
            }
        }

        private RAW_Spectrum getAccumulatedSpecObject(
          RAW_Spectrum spec,
          Dictionary<int, double[]> aSpectrum)
        {
            RAW_Spectrum rawSpectrum = new RAW_Spectrum()
            {
                ScanNumber = spec.ScanNumber,
                ScanStartTime = spec.ScanStartTime,
                ScanStartTimeUnit = spec.ScanStartTimeUnit,
                MsLevel = 1,
                ScanPolarity = spec.ScanPolarity,
                Precursor = (RAW_PrecursorIon)null
            };
            SpectrumParser.setSpectrumProperties(spec, aSpectrum);
            return spec;
        }

        private void parseRun() => this.parserCommonMethod("run", new Dictionary<string, Action<string>>()
    {
      {
        "startTimeStamp",
        (Action<string>) (v =>
        {
          DateTime result;
          if (!DateTime.TryParse(v, out result))
            return;
          this.StartTimeStamp = result;
        })
      }
    }, new Dictionary<string, Action>()
    {
      {
        "spectrumList",
        (Action) (() => this.parseSpectrumList())
      },
      {
        "chromatogramList",
        (Action) (() => this.parseChromatogramList())
      }
    });

        private void parseSpectrumList()
        {
            this.SpectraList = new List<RAW_Spectrum>();
            this.parserCommonMethod("spectrumList", new Dictionary<string, Action<string>>()
      {
        {
          "count",
          (Action<string>) (v =>
          {
            int result;
            if (!int.TryParse(v, out result))
              return;
            this.SpectraCount = result;
          })
        }
      }, new Dictionary<string, Action>()
      {
        {
          "spectrum",
          (Action) (() => this.parseSpectrum())
        }
      });
        }

        private void parseChromatogramList()
        {
            this.ChromatogramsList = new List<RAW_Chromatogram>();
            this.parserCommonMethod("chromatogramList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "chromatogram",
          (Action) (() => this.parseChromatogram())
        }
      });
        }

        private void parseSampleList()
        {
            while (this.xmlRdr.Read())
            {
                if (this.xmlRdr.NodeType == XmlNodeType.Element)
                {
                    if (this.xmlRdr.Name == "sample")
                    {
                        RAW_Sample rawSample = new RAW_Sample();
                        while (this.xmlRdr.MoveToNextAttribute())
                        {
                            switch (this.xmlRdr.Name)
                            {
                                case "id":
                                    rawSample.Id = this.xmlRdr.Value;
                                    continue;
                                case "name":
                                    rawSample.Name = this.xmlRdr.Value;
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        this.Samples.Add(rawSample);
                    }
                }
                else if (this.xmlRdr.NodeType == XmlNodeType.EndElement && this.xmlRdr.Name == "sampleList")
                    break;
            }
        }

        private void parseFileDescription()
        {
            bool flag = false;
            while (this.xmlRdr.Read())
            {
                if (this.xmlRdr.NodeType == XmlNodeType.Element)
                {
                    switch (this.xmlRdr.Name)
                    {
                        case "fileContent":
                            this.parseFileContent();
                            continue;
                        case "sourceFileList":
                            flag = true;
                            continue;
                        case "sourceFile":
                            if (flag)
                            {
                                RAW_SourceFileInfo rawSourceFileInfo = new RAW_SourceFileInfo();
                                while (this.xmlRdr.MoveToNextAttribute())
                                {
                                    switch (this.xmlRdr.Name)
                                    {
                                        case "id":
                                            rawSourceFileInfo.Id = this.xmlRdr.Value;
                                            continue;
                                        case "name":
                                            rawSourceFileInfo.Name = this.xmlRdr.Value;
                                            continue;
                                        case "location":
                                            rawSourceFileInfo.Location = this.xmlRdr.Value;
                                            continue;
                                        default:
                                            continue;
                                    }
                                }
                                this.SourceFiles.Add(rawSourceFileInfo);
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
                else if (this.xmlRdr.NodeType == XmlNodeType.EndElement)
                {
                    switch (this.xmlRdr.Name)
                    {
                        case "sourceFileList":
                            flag = false;
                            continue;
                        case "fileDescription":
                            return;
                        default:
                            continue;
                    }
                }
            }
        }

        private void parseFileContent() => this.parserCommonMethod("fileContent", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          if (cvParam.paramType != MzmlReader.CvParamTypes.DataFileContent)
            return;
          this.FileContent = (MzMlDataFileContent) cvParam.value;
        })
      }
    });

        private void parserCommonMethod(
          string returnElementName,
          Dictionary<string, Action<string>> attributeActions,
          Dictionary<string, Action> elementActions)
        {
            if (elementActions == null)
                throw new ArgumentNullException();
            if (attributeActions != null)
            {
                while (this.xmlRdr.MoveToNextAttribute())
                {
                    if (attributeActions.ContainsKey(this.xmlRdr.Name))
                        attributeActions[this.xmlRdr.Name](this.xmlRdr.Value);
                }
            }
            while (this.xmlRdr.Read())
            {
                if (this.xmlRdr.NodeType == XmlNodeType.Element)
                {
                    if (elementActions.ContainsKey(this.xmlRdr.Name))
                        elementActions[this.xmlRdr.Name]();
                }
                else if (this.xmlRdr.NodeType == XmlNodeType.EndElement && this.xmlRdr.Name == returnElementName)
                    break;
            }
        }

        private void parseChromatogram()
        {
            RAW_Chromatogram chromato = new RAW_Chromatogram();
            this.currentChromato = chromato;
            this.currentPrecursor = (RAW_PrecursorIon)null;
            this.currentProduct = (RAW_ProductIon)null;
            this.parserCommonMethod("chromatogram", new Dictionary<string, Action<string>>()
      {
        {
          "defaultArrayLength",
          (Action<string>) (v =>
          {
            int result;
            if (!int.TryParse(v, out result))
              return;
            chromato.DefaultArrayLength = result;
          })
        },
        {
          "id",
          (Action<string>) (v => chromato.Id = v)
        },
        {
          "index",
          (Action<string>) (v =>
          {
            int result;
            if (!int.TryParse(v, out result))
              return;
            chromato.Index = result;
          })
        }
      }, new Dictionary<string, Action>()
      {
        {
          "cvParam",
          (Action) (() =>
          {
            MzmlReader.CvParamValue cvParam = this.parseCvParam();
            if (cvParam.paramType != MzmlReader.CvParamTypes.DataFileContent || (MzMlDataFileContent) cvParam.value != MzMlDataFileContent.SRMchromatogram)
              return;
            chromato.IsSRM = true;
          })
        },
        {
          "precursor",
          (Action) (() => this.parsePrecursor())
        },
        {
          "product",
          (Action) (() => this.parseProduct())
        },
        {
          "binaryDataArrayList",
          (Action) (() => this.parseBinaryDataArrayListForChromatogram())
        }
      });
            chromato.Precursor = this.currentPrecursor;
            chromato.Product = this.currentProduct;
            this.ChromatogramsList.Add(chromato);
            this.currentChromato = (RAW_Chromatogram)null;
        }

        private void parseBinaryDataArrayListForChromatogram()
        {
            BinaryDataArrayConverter timeData = (BinaryDataArrayConverter)null;
            BinaryDataArrayConverter intensityData = (BinaryDataArrayConverter)null;
            this.parserCommonMethod("binaryDataArrayList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "binaryDataArray",
          (Action) (() =>
          {
            BinaryDataArrayConverter dataArrayConverter = BinaryDataArrayConverter.Convert(this.xmlRdr);
            switch (dataArrayConverter.ContentType)
            {
              case BinaryArrayContentType.TimeArray:
                timeData = dataArrayConverter;
                break;
              case BinaryArrayContentType.IntensityArray:
                intensityData = dataArrayConverter;
                break;
            }
          })
        }
      });
            if (timeData == null)
                throw new ApplicationException("binaryDataArray for RT is missing");
            if (intensityData == null)
                throw new ApplicationException("binaryDataArray for intensity is missing");
            if (timeData.ValueArray == null)
                return;
            if (timeData.ValueArray.Length != intensityData.ValueArray.Length)
                throw new ApplicationException("Length of binaryDataArray for RT and intensity mismatched");
            this.currentChromato.Chromatogram = new RAW_ChromatogramElement[timeData.ValueArray.Length];
            for (int index = 0; index < timeData.ValueArray.Length; ++index)
            {
                double num = timeData.ValueType == BinaryArrayValueType.Single ? (double)(float)timeData.ValueArray.GetValue(index) : (double)timeData.ValueArray.GetValue(index);
                if (timeData.ValueUnit == BinaryArrayUnit.Second)
                    num /= 60.0;
                this.currentChromato.Chromatogram[index].RtInMin = num;
                this.currentChromato.Chromatogram[index].Intensity = intensityData.ValueType == BinaryArrayValueType.Single ? (double)(float)intensityData.ValueArray.GetValue(index) : (double)intensityData.ValueArray.GetValue(index);
            }
        }

        internal MzmlReader.CvParamValue parseCvParam()
        {
            MzmlReader.CvParamValue cvParamValue = new MzmlReader.CvParamValue();
            while (this.xmlRdr.MoveToNextAttribute())
            {
                switch (this.xmlRdr.Name)
                {
                    case "accession":
                        switch (this.xmlRdr.Value)
                        {
                            case "MS:1000016":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.ScanStartTime;
                                continue;
                            case "MS:1000045":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.CollisionEnergy;
                                continue;
                            case "MS:1000127":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.SpectrumRepresentation;
                                cvParamValue.value = (object)MzMlSpectrumRepresentation.Centroid;
                                continue;
                            case "MS:1000128":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.SpectrumRepresentation;
                                cvParamValue.value = (object)MzMlSpectrumRepresentation.Profile;
                                continue;
                            case "MS:1000129":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.ScanPolarity;
                                cvParamValue.value = (object)MzMlScanPolarity.Negative;
                                continue;
                            case "MS:1000130":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.ScanPolarity;
                                cvParamValue.value = (object)MzMlScanPolarity.Positive;
                                continue;
                            case "MS:1000133":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.CID;
                                continue;
                            case "MS:1000134":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.PD;
                                continue;
                            case "MS:1000135":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.PSD;
                                continue;
                            case "MS:1000136":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.SID;
                                continue;
                            case "MS:1000235":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.TICchromatogram;
                                continue;
                            case "MS:1000242":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.BIRD;
                                continue;
                            case "MS:1000250":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.ECD;
                                continue;
                            case "MS:1000262":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.IRMPD;
                                continue;
                            case "MS:1000282":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.SORI;
                                continue;
                            case "MS:1000285":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.TotalIonCurrent;
                                continue;
                            case "MS:1000294":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.MassSpectrum;
                                continue;
                            case "MS:1000322":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ChargeInversionMassSpectrum;
                                continue;
                            case "MS:1000325":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ConstantNeutralGainSpectrum;
                                continue;
                            case "MS:1000326":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ConstantNeutralLossSpectrum;
                                continue;
                            case "MS:1000341":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.PrecursorIonSpectrum;
                                continue;
                            case "MS:1000343":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ProductIonSpectrum;
                                continue;
                            case "MS:1000422":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.HCD;
                                continue;
                            case "MS:1000433":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.LowEnergyCID;
                                continue;
                            case "MS:1000435":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.MPD;
                                continue;
                            case "MS:1000500":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.ScanWindowUpperLimit;
                                continue;
                            case "MS:1000501":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.ScanWindowLowerLimit;
                                continue;
                            case "MS:1000504":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.BasePeakMz;
                                continue;
                            case "MS:1000505":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.BasePeakIntensity;
                                continue;
                            case "MS:1000511":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.MsLevel;
                                continue;
                            case "MS:1000527":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.HighestObservedMz;
                                continue;
                            case "MS:1000528":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.LowestObservedMz;
                                continue;
                            case "MS:1000579":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.MS1Spectrum;
                                continue;
                            case "MS:1000580":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.MSnSpectrum;
                                continue;
                            case "MS:1000581":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.CRMSpectrum;
                                continue;
                            case "MS:1000582":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.SIMSpectrum;
                                continue;
                            case "MS:1000583":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.SRMSpectrum;
                                continue;
                            case "MS:1000598":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.ETD;
                                continue;
                            case "MS:1000599":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.PQD;
                                continue;
                            case "MS:1000620":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.PDASpectrum;
                                continue;
                            case "MS:1000627":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.SICchromatogram;
                                continue;
                            case "MS:1000628":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.BasepeakChromatogram;
                                continue;
                            case "MS:1000744":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.SelectedIonMz;
                                continue;
                            case "MS:1000789":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.EnhancedMultiplyChargedSpectrum;
                                continue;
                            case "MS:1000790":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.TimeDelayedFragmentationSpectrum;
                                continue;
                            case "MS:1000804":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ElectromagneticRadiationSpectrum;
                                continue;
                            case "MS:1000805":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.EmissionSpectrum;
                                continue;
                            case "MS:1000806":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.AbsorptionSpectrum;
                                continue;
                            case "MS:1000810":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.MassChromatogram;
                                continue;
                            case "MS:1000811":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.ElectromagneticRadiationChromatogram;
                                continue;
                            case "MS:1000812":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.AbsorptionChromatogram;
                                continue;
                            case "MS:1000813":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.EmissionChromatogram;
                                continue;
                            case "MS:1000827":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.IsolationWindowTargetMz;
                                continue;
                            case "MS:1000828":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.IsolationWindowLowerOffset;
                                continue;
                            case "MS:1000829":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.IsolationWindowUpperOffset;
                                continue;
                            case "MS:1001472":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.SIMchromatogram;
                                continue;
                            case "MS:1001473":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.SRMchromatogram;
                                continue;
                            case "MS:1001474":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DataFileContent;
                                cvParamValue.value = (object)MzMlDataFileContent.CRMchromatogram;
                                continue;
                            case "MS:1001880":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.InSourceCID;
                                continue;
                            case "MS:1002000":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.DissociationMethod;
                                cvParamValue.value = (object)MzMlDissociationMethods.LIFT;
                                continue;
                            case "MS:1002476":
                                cvParamValue.paramType = MzmlReader.CvParamTypes.IonMobilityDriftTime;
                                continue;
                            default:
                                continue;
                        }
                    case "value":
                        cvParamValue.valueString = this.xmlRdr.Value;
                        continue;
                    case "unitAccession":
                        switch (this.xmlRdr.Value)
                        {
                            case "UO:0000010":
                                cvParamValue.unit = MzMlUnits.Second;
                                continue;
                            case "UO:0000031":
                                cvParamValue.unit = MzMlUnits.Minute;
                                continue;
                            case "UO:0000266":
                                cvParamValue.unit = MzMlUnits.ElectronVolt;
                                continue;
                            case "UO:0000028":
                                cvParamValue.unit = MzMlUnits.Millisecond;
                                continue;
                            case "MS:1000040":
                                cvParamValue.unit = MzMlUnits.Mz;
                                continue;
                            case "MS:1000131":
                                cvParamValue.unit = MzMlUnits.NumberOfCounts;
                                continue;
                            default:
                                continue;
                        }
                    default:
                        continue;
                }
            }
            switch (cvParamValue.paramType)
            {
                case MzmlReader.CvParamTypes.MsLevel:
                    int result1;
                    if (int.TryParse(cvParamValue.valueString, out result1))
                    {
                        cvParamValue.value = (object)result1;
                        break;
                    }
                    break;
                case MzmlReader.CvParamTypes.BasePeakMz:
                case MzmlReader.CvParamTypes.BasePeakIntensity:
                case MzmlReader.CvParamTypes.TotalIonCurrent:
                case MzmlReader.CvParamTypes.HighestObservedMz:
                case MzmlReader.CvParamTypes.LowestObservedMz:
                case MzmlReader.CvParamTypes.ScanStartTime:
                case MzmlReader.CvParamTypes.ScanWindowLowerLimit:
                case MzmlReader.CvParamTypes.ScanWindowUpperLimit:
                case MzmlReader.CvParamTypes.IsolationWindowTargetMz:
                case MzmlReader.CvParamTypes.IsolationWindowLowerOffset:
                case MzmlReader.CvParamTypes.IsolationWindowUpperOffset:
                case MzmlReader.CvParamTypes.SelectedIonMz:
                case MzmlReader.CvParamTypes.CollisionEnergy:
                case MzmlReader.CvParamTypes.IonMobilityDriftTime:
                    double result2;
                    if (double.TryParse(cvParamValue.valueString, out result2))
                    {
                        cvParamValue.value = (object)result2;
                        break;
                    }
                    break;
                default:
                    if (cvParamValue.value == null)
                    {
                        cvParamValue.value = (object)cvParamValue.valueString;
                        break;
                    }
                    break;
            }
            return cvParamValue;
        }

        private void parseSpectrum()
        {
            RAW_Spectrum spectrum = new RAW_Spectrum();
            this.currentSpectrum = spectrum;
            this.parserCommonMethod("spectrum", new Dictionary<string, Action<string>>()
      {
        {
          "defaultArrayLength",
          (Action<string>) (v =>
          {
            int result;
            if (!int.TryParse(v, out result))
              return;
            spectrum.DefaultArrayLength = result;
          })
        },
        {
          "id",
          (Action<string>) (v =>
          {
            spectrum.Id = v;
            this.parseSpectrumId();
          })
        },
        {
          "index",
          (Action<string>) (v =>
          {
            int result;
            if (!int.TryParse(v, out result))
              return;
            spectrum.ScanNumber = result;
          })
        }
      }, new Dictionary<string, Action>()
      {
        {
          "cvParam",
          (Action) (() =>
          {
            MzmlReader.CvParamValue cvParam = this.parseCvParam();
            switch (cvParam.paramType)
            {
              case MzmlReader.CvParamTypes.MsLevel:
                spectrum.MsLevel = (int) cvParam.value;
                break;
              case MzmlReader.CvParamTypes.ScanPolarity:
                spectrum.ScanPolarity = this.convertCvParamValToScanPolarity(cvParam.value);
                break;
              case MzmlReader.CvParamTypes.SpectrumRepresentation:
                spectrum.SpectrumRepresentation = this.convertCvParamValToSpectrumPresentation(cvParam.value);
                break;
              case MzmlReader.CvParamTypes.BasePeakMz:
                spectrum.BasePeakMz = (double) cvParam.value;
                break;
              case MzmlReader.CvParamTypes.BasePeakIntensity:
                spectrum.BasePeakIntensity = (double) cvParam.value;
                break;
              case MzmlReader.CvParamTypes.TotalIonCurrent:
                spectrum.TotalIonCurrent = (double) cvParam.value;
                break;
              case MzmlReader.CvParamTypes.HighestObservedMz:
                spectrum.HighestObservedMz = (double) cvParam.value;
                break;
              case MzmlReader.CvParamTypes.LowestObservedMz:
                spectrum.LowestObservedMz = (double) cvParam.value;
                break;
            }
          })
        },
        {
          "scanList",
          (Action) (() => this.parseScanList())
        },
        {
          "precursorList",
          (Action) (() => this.parsePrecursorList())
        },
        {
          "productList",
          (Action) (() => this.parseProductList())
        },
        {
          "binaryDataArrayList",
          (Action) (() => this.parseBinaryDataArrayListForSpectrum())
        }
      });
            this.SpectraList.Add(spectrum);
            if (this.bgWorker != null)
                this.progressReports(this.SpectraList.Count, this.SpectraCount, this.bgWorker);
            else if (!this.IsGuiProcess)
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Write("{0} / {1}", (object)this.SpectraList.Count, (object)this.SpectraCount);
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
                else
                    Console.WriteLine("{0} / {1}", (object)this.SpectraList.Count, (object)this.SpectraCount);
            }
            this.currentSpectrum = (RAW_Spectrum)null;
        }

        private void progressReports(int current, int total, BackgroundWorker bgWorker) => this.bgWorker.ReportProgress((int)((double)current / (double)total * 100.0));

        private SpectrumRepresentation convertCvParamValToSpectrumPresentation(
          object presentation)
        {
            switch ((MzMlSpectrumRepresentation)presentation)
            {
                case MzMlSpectrumRepresentation.Undefined:
                    return SpectrumRepresentation.Undefined;
                case MzMlSpectrumRepresentation.Centroid:
                    return SpectrumRepresentation.Centroid;
                case MzMlSpectrumRepresentation.Profile:
                    return SpectrumRepresentation.Profile;
                default:
                    return SpectrumRepresentation.Undefined;
            }
        }

        private ScanPolarity convertCvParamValToScanPolarity(object polarity)
        {
            switch ((MzMlScanPolarity)polarity)
            {
                case MzMlScanPolarity.Undefined:
                    return ScanPolarity.Undefined;
                case MzMlScanPolarity.Positive:
                    return ScanPolarity.Positive;
                case MzMlScanPolarity.Negative:
                    return ScanPolarity.Negative;
                case MzMlScanPolarity.Alternating:
                    return ScanPolarity.Alternating;
                default:
                    return ScanPolarity.Undefined;
            }
        }

        private Units convertCvParamValToUnits(MzMlUnits mzMlUnits)
        {
            switch (mzMlUnits)
            {
                case MzMlUnits.Undefined:
                    return Units.Undefined;
                case MzMlUnits.Second:
                    return Units.Second;
                case MzMlUnits.Minute:
                    return Units.Minute;
                case MzMlUnits.Mz:
                    return Units.Mz;
                case MzMlUnits.NumberOfCounts:
                    return Units.NumberOfCounts;
                case MzMlUnits.ElectronVolt:
                    return Units.ElectronVolt;
                case MzMlUnits.Millisecond:
                    return Units.Milliseconds;
                default:
                    return Units.Undefined;
            }
        }

        private DissociationMethods convertCvParamValToDissociationMethods(
          object disMethod)
        {
            switch ((MzMlDissociationMethods)disMethod)
            {
                case MzMlDissociationMethods.Undefined:
                    return DissociationMethods.Undefined;
                case MzMlDissociationMethods.CID:
                    return DissociationMethods.CID;
                case MzMlDissociationMethods.PD:
                    return DissociationMethods.PD;
                case MzMlDissociationMethods.PSD:
                    return DissociationMethods.PSD;
                case MzMlDissociationMethods.SID:
                    return DissociationMethods.SID;
                case MzMlDissociationMethods.BIRD:
                    return DissociationMethods.BIRD;
                case MzMlDissociationMethods.ECD:
                    return DissociationMethods.ECD;
                case MzMlDissociationMethods.IRMPD:
                    return DissociationMethods.IRMPD;
                case MzMlDissociationMethods.SORI:
                    return DissociationMethods.SORI;
                case MzMlDissociationMethods.HCD:
                    return DissociationMethods.HCD;
                case MzMlDissociationMethods.LowEnergyCID:
                    return DissociationMethods.LowEnergyCID;
                case MzMlDissociationMethods.MPD:
                    return DissociationMethods.MPD;
                case MzMlDissociationMethods.ETD:
                    return DissociationMethods.ETD;
                case MzMlDissociationMethods.PQD:
                    return DissociationMethods.PQD;
                case MzMlDissociationMethods.InSourceCID:
                    return DissociationMethods.InSourceCID;
                case MzMlDissociationMethods.LIFT:
                    return DissociationMethods.LIFT;
                default:
                    return DissociationMethods.CID;
            }
        }

        private void parseSpectrumId()
        {
            string id = this.currentSpectrum.Id;
            char[] chArray1 = new char[1] { ' ' };
            foreach (string str in id.Split(chArray1))
            {
                char[] chArray2 = new char[1] { '=' };
                string[] strArray = str.Split(chArray2);
                int result;
                if (strArray[0] == "scan" && int.TryParse(strArray[1], out result) && this.currentSpectrum.ScanNumber == 0)
                    this.currentSpectrum.ScanNumber = result;
            }
        }

        private void parseScanList() => this.parserCommonMethod("scanList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "scan",
        (Action) (() => this.parseScan())
      }
    });

        private void parseScan() => this.parserCommonMethod("scan", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "scanWindowList",
        (Action) (() => this.parseScanWindowList())
      },
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          if (cvParam.paramType == MzmlReader.CvParamTypes.ScanStartTime)
          {
            this.currentSpectrum.ScanStartTime = (double) cvParam.value;
            this.currentSpectrum.ScanStartTimeUnit = this.convertCvParamValToUnits(cvParam.unit);
            if (this.currentSpectrum.ScanStartTimeUnit != Units.Second)
              return;
            this.currentSpectrum.ScanStartTime /= 60.0;
            this.currentSpectrum.ScanStartTimeUnit = Units.Minute;
          }
          else
          {
            if (cvParam.paramType != MzmlReader.CvParamTypes.IonMobilityDriftTime)
              return;
            this.currentSpectrum.DriftTime = (double) cvParam.value;
            this.currentSpectrum.DriftTimeUnit = this.convertCvParamValToUnits(cvParam.unit);
            this.IsIonMobilityData = true;
          }
        })
      }
    });

        private void parseScanWindowList() => this.parserCommonMethod("scanWindowList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "scanWindow",
        (Action) (() => this.parseScanWindow())
      }
    });

        private void parseScanWindow() => this.parserCommonMethod("scanWindow", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          switch (cvParam.paramType)
          {
            case MzmlReader.CvParamTypes.ScanWindowLowerLimit:
              this.currentSpectrum.ScanWindowLowerLimit = (double) cvParam.value;
              break;
            case MzmlReader.CvParamTypes.ScanWindowUpperLimit:
              this.currentSpectrum.ScanWindowUpperLimit = (double) cvParam.value;
              break;
          }
        })
      }
    });

        private void parsePrecursorList()
        {
            this.parserCommonMethod("precursorList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "precursor",
          (Action) (() => this.parsePrecursor())
        }
      });
            this.currentSpectrum.Precursor = this.currentPrecursor;
            this.currentPrecursor = (RAW_PrecursorIon)null;
        }

        private void parsePrecursor()
        {
            this.currentPrecursor = new RAW_PrecursorIon();
            this.parserCommonMethod("precursor", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "isolationWindow",
          (Action) (() => this.parseIsolatedWindowForPrecursor())
        },
        {
          "selectedIonList",
          (Action) (() => this.parseSelectedIonList())
        },
        {
          "activation",
          (Action) (() => this.parseActivation())
        }
      });
        }

        private void parseIsolatedWindowForPrecursor() => this.parserCommonMethod("isolationWindow", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          switch (cvParam.paramType)
          {
            case MzmlReader.CvParamTypes.IsolationWindowTargetMz:
              this.currentPrecursor.IsolationTargetMz = (double) cvParam.value;
              break;
            case MzmlReader.CvParamTypes.IsolationWindowLowerOffset:
              this.currentPrecursor.IsolationWindowLowerOffset = (double) cvParam.value;
              break;
            case MzmlReader.CvParamTypes.IsolationWindowUpperOffset:
              this.currentPrecursor.IsolationWindowUpperOffset = (double) cvParam.value;
              break;
          }
        })
      }
    });

        private void parseSelectedIonList() => this.parserCommonMethod("selectedIonList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "selectedIon",
        (Action) (() => this.parseSelectedIon())
      }
    });

        private void parseSelectedIon() => this.parserCommonMethod("selectedIon", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          if (cvParam.paramType != MzmlReader.CvParamTypes.SelectedIonMz)
            return;
          this.currentPrecursor.SelectedIonMz = (double) cvParam.value;
        })
      }
    });

        private void parseActivation() => this.parserCommonMethod("activation", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          switch (cvParam.paramType)
          {
            case MzmlReader.CvParamTypes.CollisionEnergy:
              this.currentPrecursor.CollisionEnergy = (double) cvParam.value;
              this.currentPrecursor.CollisionEnergyUnit = this.convertCvParamValToUnits(cvParam.unit);
              break;
            case MzmlReader.CvParamTypes.DissociationMethod:
              this.currentPrecursor.Dissociationmethod = this.convertCvParamValToDissociationMethods(cvParam.value);
              break;
          }
        })
      }
    });

        private void parseProductList()
        {
            this.parserCommonMethod("productList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "product",
          (Action) (() => this.parseProduct())
        }
      });
            this.currentSpectrum.Product = this.currentProduct;
            this.currentProduct = (RAW_ProductIon)null;
        }

        private void parseProduct()
        {
            this.currentProduct = new RAW_ProductIon();
            this.parserCommonMethod("product", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
      {
        {
          "isolationWindow",
          (Action) (() => this.parseIsolatedWindowForProduct())
        },
        {
          "selectedIonList",
          (Action) (() => this.parseSelectedIonList())
        },
        {
          "activation",
          (Action) (() => this.parseActivation())
        }
      });
        }

        private void parseIsolatedWindowForProduct() => this.parserCommonMethod("isolationWindow", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
    {
      {
        "cvParam",
        (Action) (() =>
        {
          MzmlReader.CvParamValue cvParam = this.parseCvParam();
          switch (cvParam.paramType)
          {
            case MzmlReader.CvParamTypes.IsolationWindowTargetMz:
              this.currentProduct.IsolationTargetMz = (double) cvParam.value;
              break;
            case MzmlReader.CvParamTypes.IsolationWindowLowerOffset:
              this.currentProduct.IsolationWindowLowerOffset = (double) cvParam.value;
              break;
            case MzmlReader.CvParamTypes.IsolationWindowUpperOffset:
              this.currentProduct.IsolationWindowUpperOffset = (double) cvParam.value;
              break;
          }
        })
      }
    });

        private void parseBinaryDataArrayListForSpectrum()
        {
            if (this.currentSpectrum.DefaultArrayLength == 0)
            {
                this.currentSpectrum.Spectrum = new RAW_PeakElement[0];
                this.currentSpectrum.LowestObservedMz = this.currentSpectrum.ScanWindowLowerLimit;
                this.currentSpectrum.HighestObservedMz = this.currentSpectrum.ScanWindowUpperLimit;
                this.currentSpectrum.MinIntensity = 0.0;
            }
            else
            {
                BinaryDataArrayConverter mzData = (BinaryDataArrayConverter)null;
                BinaryDataArrayConverter intensityData = (BinaryDataArrayConverter)null;
                this.parserCommonMethod("binaryDataArrayList", (Dictionary<string, Action<string>>)null, new Dictionary<string, Action>()
        {
          {
            "binaryDataArray",
            (Action) (() =>
            {
              BinaryDataArrayConverter dataArrayConverter = BinaryDataArrayConverter.Convert(this.xmlRdr);
              switch (dataArrayConverter.ContentType)
              {
                case BinaryArrayContentType.MzArray:
                  mzData = dataArrayConverter;
                  break;
                case BinaryArrayContentType.IntensityArray:
                  intensityData = dataArrayConverter;
                  break;
              }
            })
          }
        });
                if (mzData == null)
                    throw new ApplicationException("binaryDataArray for m/z is missing");
                if (intensityData == null)
                    throw new ApplicationException("binaryDataArray for intensity is missing");
                if (mzData.ValueArray.Length != intensityData.ValueArray.Length)
                    throw new ApplicationException("Length of binaryDataArray for m/z and intensity mismatched");
                if (mzData.ValueArray.Length == 0)
                {
                    this.currentSpectrum.DefaultArrayLength = 0;
                    this.currentSpectrum.Spectrum = new RAW_PeakElement[0];
                    this.currentSpectrum.MinIntensity = 0.0;
                    this.currentSpectrum.LowestObservedMz = this.currentSpectrum.ScanWindowLowerLimit;
                    this.currentSpectrum.HighestObservedMz = this.currentSpectrum.ScanWindowUpperLimit;
                }
                else
                {
                    int length = mzData.ValueArray.Length;
                    double num1 = double.MaxValue;
                    double num2 = double.MinValue;
                    double num3 = double.MaxValue;
                    double num4 = double.MinValue;
                    if ((mzData.ValueType == BinaryArrayValueType.Single ? (int)(float)mzData.ValueArray.GetValue(mzData.ValueArray.Length - 1) : (int)(double)mzData.ValueArray.GetValue(mzData.ValueArray.Length - 1)) == 0)
                        --length;
                    if (length == 0)
                    {
                        this.currentSpectrum.Spectrum = new RAW_PeakElement[0];
                        this.currentSpectrum.DefaultArrayLength = 0;
                        this.currentSpectrum.MinIntensity = 0.0;
                        this.currentSpectrum.LowestObservedMz = this.currentSpectrum.ScanWindowLowerLimit;
                        this.currentSpectrum.HighestObservedMz = this.currentSpectrum.ScanWindowUpperLimit;
                    }
                    else
                    {
                        this.currentSpectrum.Spectrum = new RAW_PeakElement[length];
                        for (int index = 0; index < length; ++index)
                        {
                            this.currentSpectrum.Spectrum[index].Mz = mzData.ValueType == BinaryArrayValueType.Single ? (double)(float)mzData.ValueArray.GetValue(index) : (double)mzData.ValueArray.GetValue(index);
                            this.currentSpectrum.Spectrum[index].Intensity = intensityData.ValueType == BinaryArrayValueType.Single ? (double)(float)intensityData.ValueArray.GetValue(index) : (double)intensityData.ValueArray.GetValue(index);
                            if (this.currentSpectrum.Spectrum[index].Intensity < num1)
                                num1 = this.currentSpectrum.Spectrum[index].Intensity;
                            if (this.currentSpectrum.Spectrum[index].Intensity > num2)
                                num2 = this.currentSpectrum.Spectrum[index].Intensity;
                            if (this.currentSpectrum.Spectrum[index].Mz < num3)
                                num3 = this.currentSpectrum.Spectrum[index].Mz;
                            if (this.currentSpectrum.Spectrum[index].Mz > num4)
                                num4 = this.currentSpectrum.Spectrum[index].Mz;
                        }
                        this.currentSpectrum.DefaultArrayLength = length;
                        this.currentSpectrum.MinIntensity = num1;
                        this.currentSpectrum.LowestObservedMz = num3;
                        this.currentSpectrum.HighestObservedMz = num4;
                        this.currentSpectrum.BasePeakMz = num4;
                        this.currentSpectrum.BasePeakIntensity = num2;
                    }
                }
            }
        }

        public enum CvParamTypes
        {
            Undefined,
            DataFileContent,
            MsLevel,
            ScanPolarity,
            SpectrumRepresentation,
            BasePeakMz,
            BasePeakIntensity,
            TotalIonCurrent,
            HighestObservedMz,
            LowestObservedMz,
            ScanStartTime,
            ScanWindowLowerLimit,
            ScanWindowUpperLimit,
            IsolationWindowTargetMz,
            IsolationWindowLowerOffset,
            IsolationWindowUpperOffset,
            SelectedIonMz,
            CollisionEnergy,
            DissociationMethod,
            IonMobilityDriftTime,
        }

        public class CvParamValue
        {
            internal MzmlReader.CvParamTypes paramType;
            internal string valueString = "";
            internal object value;
            internal MzMlUnits unit;
        }
    }
}
