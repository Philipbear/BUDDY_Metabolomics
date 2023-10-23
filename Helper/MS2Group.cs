using BUDDY.RawData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BUDDY.Helper
{
    public class MS2Group
    {
        public double RTmin { get; set; }
        public double RTmax { get; set; }
        public List<RAW_PeakElement> MS1Isotope { get; set; }
        public int MS1ScanIndex { get; set; }
        public List<int> MS2ScanIndex { get; set; }
        public double PrecursorMz { get; set; }
        public List<RAW_PeakElement> MostAbundantMS2 { get; set; }
        public List<RAW_PeakElement> MergedMS2 { get; set; }
        public double HighestPrecursorIntensity { get; set; }  // precursor intensity in MS1
        public int HighestPreIntMS2ScanIndex { get; set; } // the MS2 scan index owning the highest precursor intensity
    }

    public class MS2CompareResult
    {
        public double Score { get; set; }
        public int MatchNumber { get; set; }
        public List<int> xMatchFragIndex { get; set; } // the indices of matched fragments in MS2_x
        public List<int> yMatchFragIndex { get; set; }
        public List<RAW_PeakElement> NormMS2x { get; set; } // normalized MS2 x, for plotting
        public List<RAW_PeakElement> NormMS2y { get; set; }
    }
}
