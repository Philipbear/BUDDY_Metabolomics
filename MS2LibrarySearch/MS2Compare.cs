using BUDDY.Helper;
using BUDDY.RawData;
using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BUDDY.MS2LibrarySearch
{
    public class MS2Compare
    {
        // ms2 searching function:  returns all matchings above the threshold
        public static List<MS2MatchResult> MS2Matching(Ms2Utility ms2, bool ppm, List<MS2DBEntry> MS2DB, int MS2SearchingAlgorithmIndex,
            double MS2ScoreThreshold, bool minMatchedFragNoBool, int minMatchedFragNo,
            bool RtMatchBool, double RtTol)
        {
            double preMz = ms2.Mz;
            double ms1tol = ms2.ms1tol * 2; // *2: experimental to experimental
            double ms2tol = ms2.ms2tol * 2;

            if (ppm)
            {
                ms1tol = preMz * ms2.ms1tol * 1e-6 * 2;
            }

            // debug
            //MS2DB = MS2DB.Where(o => o.MetaboliteName == "(R)-(-)-2-Phenylglycinol").ToList();

            // if ms2 min matched frag no:
            // restrict DB 
            List<MS2DBEntry> candMS2DB = new List<MS2DBEntry>(MS2DB);
            if (minMatchedFragNoBool)
            {
                candMS2DB = candMS2DB.Where(o => o.MS2Spec.Count >= minMatchedFragNo).ToList();
            }

            // ionMode && precursor mz direct matching, candidate MS2 DB
            //List<MS2DBEntry> candMS2DB = MS2DB.Where(o => o.IonMode == ms2.Polarity && Math.Abs(o.PrecursorMz - preMz) <= ms1tol).ToList();
            candMS2DB = candMS2DB.Where(o => Math.Abs(o.PrecursorMz - preMz) <= ms1tol).ToList();


            if (candMS2DB.Count == 0) { return new List<MS2MatchResult>(); }


            // if RT match:
            // entry with valid RT, match; w/o valid RT, keep all
            if (RtMatchBool)
            {
                candMS2DB = candMS2DB.Where(o => (o.ValidRT && Math.Abs(o.RTminute - ms2.Rt) <= RtTol) || o.ValidRT == false).ToList();
            }

            if (candMS2DB.Count == 0) { return new List<MS2MatchResult>(); }

            List<MS2MatchResult> output = new List<MS2MatchResult>();
            switch (MS2SearchingAlgorithmIndex)
            {
                case 0:
                    for (int i = 0; i < candMS2DB.Count; i++)
                    {
                        output.Add(new MS2MatchResult { ms2CompareResult = DotProduct_MS2Compare(ms2.OriSpectrum, candMS2DB[i].MS2Spec, ms2tol, ppm), matchedDBEntry = candMS2DB[i] });
                    }
                    break;
                case 1:
                    for (int i = 0; i < candMS2DB.Count; i++)
                    {
                        output.Add(new MS2MatchResult { ms2CompareResult = ReverseDotProduct_MS2Compare(ms2.OriSpectrum, candMS2DB[i].MS2Spec, ms2tol, ppm), matchedDBEntry = candMS2DB[i] });
                    }
                    break;
                case 2:
                    for (int i = 0; i < candMS2DB.Count; i++)
                    {
                        output.Add(new MS2MatchResult { ms2CompareResult = SpectralEntropySimilarity_MS2Compare(ms2.OriSpectrum, candMS2DB[i].MS2Spec, ms2tol, ppm), matchedDBEntry = candMS2DB[i] });
                    }
                    break;
                default:
                    break;
            }

            output = output.Where(o => o.ms2CompareResult.Score >= MS2ScoreThreshold).ToList();
            if (minMatchedFragNoBool)
            {
                output = output.Where(o => o.ms2CompareResult.MatchNumber >= minMatchedFragNo).ToList();
            }
            output = output.OrderByDescending(o => o.ms2CompareResult.Score).ThenByDescending(o => o.ms2CompareResult.MatchNumber).ToList();

            return output;
        }

        // all these methods use alignment methods where could generate one-to-multi matches, for reducing computational cost
        public static MS2CompareResult DotProduct_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {
            double DpScoreTop = 0;
            Ms2mzTol += 1e-6; // double type comparison
            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xMaxInt = x.Max(t => t.Intensity);
            double yMaxInt = y.Max(t => t.Intensity);

            double x_SumIntSquare = 0;
            double y_SumIntSquare = 0;

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = 100.0 * x[i].Intensity / xMaxInt });
                x_SumIntSquare += Math.Pow(x_MS2[i].Intensity, 2);
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = 100.0 * y[i].Intensity / yMaxInt });
                y_SumIntSquare += Math.Pow(y_MS2[i].Intensity, 2);
            }

            List<AlignedPeak> alignedPeak = new List<AlignedPeak>();
            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        alignedPeak.Add(new AlignedPeak { xIndex = i, yIndex = j, intProduct = x_MS2[i].Intensity * y_MS2[j].Intensity });
                    }

                }
            }

            if (alignedPeak.Count == 0)
            {
                return new MS2CompareResult
                {
                    Score = 0,
                    MatchNumber = 0,
                    xMatchFragIndex = xMatchedFragIndex,
                    yMatchFragIndex = yMatchedFragIndex,
                    NormMS2x = x_MS2,
                    NormMS2y = y_MS2
                };
            }

            alignedPeak = alignedPeak.OrderByDescending(o => o.intProduct).ToList();

            while (alignedPeak.Count > 0)
            {
                DpScoreTop += alignedPeak[0].intProduct;
                xMatchedFragIndex.Add(alignedPeak[0].xIndex);
                yMatchedFragIndex.Add(alignedPeak[0].yIndex);
                alignedPeak = alignedPeak.Where(o => o.xIndex != alignedPeak[0].xIndex && o.yIndex != alignedPeak[0].yIndex).ToList();
            }

            double DpScore = DpScoreTop / Math.Sqrt(x_SumIntSquare * y_SumIntSquare);


            MS2CompareResult output = new MS2CompareResult
            {
                Score = DpScore,
                MatchNumber = xMatchedFragIndex.Count,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
                NormMS2x = x_MS2,
                NormMS2y = y_MS2
            };

            return output;

        }
        public static MS2CompareResult ReverseDotProduct_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {
            double DpScoreTop = 0;
            Ms2mzTol += 1e-6; // double type comparison
            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xMaxInt = x.Max(t => t.Intensity);
            double yMaxInt = y.Max(t => t.Intensity);

            double x_SumIntSquare = 0;
            double y_SumIntSquare = 0;

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = 100.0 * x[i].Intensity / xMaxInt });
                x_SumIntSquare += Math.Pow(x_MS2[i].Intensity, 2);
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = 100.0 * y[i].Intensity / yMaxInt });
                y_SumIntSquare += Math.Pow(y_MS2[i].Intensity, 2);
            }

            List<AlignedPeak> alignedPeak = new List<AlignedPeak>();
            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        alignedPeak.Add(new AlignedPeak { xIndex = i, yIndex = j, intProduct = x_MS2[i].Intensity * y_MS2[j].Intensity });
                    }

                }
            }

            if (alignedPeak.Count == 0)
            {
                return new MS2CompareResult
                {
                    Score = 0,
                    MatchNumber = 0,
                    xMatchFragIndex = xMatchedFragIndex,
                    yMatchFragIndex = yMatchedFragIndex,
                    NormMS2x = x_MS2,
                    NormMS2y = y_MS2
                };
            }

            alignedPeak = alignedPeak.OrderByDescending(o => o.intProduct).ToList();

            double ySumAlignedInt = 0;
            while (alignedPeak.Count > 0)
            {
                DpScoreTop += alignedPeak[0].intProduct;
                xMatchedFragIndex.Add(alignedPeak[0].xIndex);
                yMatchedFragIndex.Add(alignedPeak[0].yIndex);
                ySumAlignedInt += Math.Pow(y_MS2[alignedPeak[0].yIndex].Intensity, 2);
                alignedPeak = alignedPeak.Where(o => o.xIndex != alignedPeak[0].xIndex && o.yIndex != alignedPeak[0].yIndex).ToList();
            }

            double DpScore = DpScoreTop / Math.Sqrt(x_SumIntSquare * ySumAlignedInt);

            MS2CompareResult output = new MS2CompareResult
            {
                Score = DpScore,
                MatchNumber = xMatchedFragIndex.Count,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
                NormMS2x = x_MS2,
                NormMS2y = y_MS2
            };

            return output;
        }
        public static MS2CompareResult SpectralEntropySimilarity_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {

            Ms2mzTol += 1e-6; // double type comparison

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xSumInt = x.Sum(t => t.Intensity);
            double ySumInt = y.Sum(t => t.Intensity);


            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = x[i].Intensity / xSumInt });
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = y[i].Intensity / ySumInt });
            }


            double xSpecEntropy = 0;
            double ySpecEntropy = 0;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
            }
            for (int i = 0; i < y_MS2.Count; i++)
            {
                ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
            }

            if (xSpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * xSpecEntropy;
                double sumWeightedInt = x_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                xSpecEntropy = 0;
                for (int i = 0; i < x_MS2.Count; i++)
                {
                    x_MS2[i] = new RAW_PeakElement() { Mz = x_MS2[i].Mz, Intensity = Math.Pow(x_MS2[i].Intensity, weight) / sumWeightedInt };
                    xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
                }
            }
            if (ySpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * ySpecEntropy;
                double sumWeightedInt = y_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                ySpecEntropy = 0;
                for (int i = 0; i < y_MS2.Count; i++)
                {
                    y_MS2[i] = new RAW_PeakElement() { Mz = y_MS2[i].Mz, Intensity = Math.Pow(y_MS2[i].Intensity, weight) / sumWeightedInt };
                    ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
                }
            }

            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();
            List<MergedSpecAlignedPeak> alignedPeak = new List<MergedSpecAlignedPeak>();
            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        alignedPeak.Add(new MergedSpecAlignedPeak { xIndex = i, yIndex = j, sumInt = x_MS2[i].Intensity + y_MS2[j].Intensity });
                    }
                }
            }

            if (alignedPeak.Count == 0)
            {
                return new MS2CompareResult
                {
                    Score = 0,
                    MatchNumber = 0,
                    xMatchFragIndex = xMatchedFragIndex,
                    yMatchFragIndex = yMatchedFragIndex
                };
            }
            alignedPeak = alignedPeak.OrderByDescending(o => o.sumInt).ToList();


            while (alignedPeak.Count > 0)
            {
                xMatchedFragIndex.Add(alignedPeak[0].xIndex);
                yMatchedFragIndex.Add(alignedPeak[0].yIndex);
                alignedPeak = alignedPeak.Where(o => o.xIndex != alignedPeak[0].xIndex && o.yIndex != alignedPeak[0].yIndex).ToList();
            }

            List<MergedSpecAlignedPeak> spec_merged = new List<MergedSpecAlignedPeak>();
            for (int i = 0; i < xMatchedFragIndex.Count; i++)
            {
                spec_merged.Add(new MergedSpecAlignedPeak { xIndex = xMatchedFragIndex[i], yIndex = yMatchedFragIndex[i], 
                    sumInt = x_MS2[xMatchedFragIndex[i]].Intensity + y_MS2[yMatchedFragIndex[i]].Intensity });
            }
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (xMatchedFragIndex.Contains(i)) { continue; }
                spec_merged.Add(new MergedSpecAlignedPeak
                {
                    xIndex = i,
                    yIndex = -1,
                    sumInt = x_MS2[i].Intensity
                });
            }
            for (int i = 0; i < y_MS2.Count; i++)
            {
                if (yMatchedFragIndex.Contains(i)) { continue; }
                spec_merged.Add(new MergedSpecAlignedPeak
                {
                    xIndex = -1,
                    yIndex = i,
                    sumInt = y_MS2[i].Intensity
                });
            }

            double mergedSpecEntropy = 0;
            for (int i = 0; i < spec_merged.Count; i++)
            {
                mergedSpecEntropy += -1 * (spec_merged[i].sumInt /2) * Math.Log(spec_merged[i].sumInt / 2);
            }

            double entropySimilairty = 1 - (2 * mergedSpecEntropy - xSpecEntropy - ySpecEntropy) / Math.Log(4);
            int matchNo = xMatchedFragIndex.Count;

            MS2CompareResult output = new MS2CompareResult
            {
                Score = entropySimilairty,
                MatchNumber = matchNo,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
            };
            return output;
        }
        // GNPS MS2 comparison, consider fragment & neutral loss
        public static double GNPS_MS2Compare_forMILP(List<RAW_PeakElement> x, double xPrecursorMz, List<RAW_PeakElement> y, double yPrecursorMz, double Ms2mzTol, bool ppm)
        {
            Ms2mzTol += 1e-6; // double type comparison
            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return 0;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            double xSumInt = Math.Sqrt(x.Sum(t => t.Intensity));
            double ySumInt = Math.Sqrt(y.Sum(t => t.Intensity));

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = Math.Sqrt(x[i].Intensity) / xSumInt });
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = Math.Sqrt(y[i].Intensity) / ySumInt });
            }


            List<AlignedPeak> alignedPeak = new List<AlignedPeak>();
            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match or neutral loss
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol || Math.Abs(yPrecursorMz - xPrecursorMz - y_MS2[j].Mz + x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        alignedPeak.Add(new AlignedPeak { xIndex = i, yIndex = j, intProduct = x_MS2[i].Intensity * y_MS2[j].Intensity });
                    }

                }
            }

            if (alignedPeak.Count == 0)
            {
                return 0;
            }

            alignedPeak = alignedPeak.OrderByDescending(o => o.intProduct).ToList();
            double Score = 0;
            while (alignedPeak.Count > 0)
            {
                Score += alignedPeak[0].intProduct;
                //xMatchedFragIndex.Add(alignedPeak[0].xIndex);
                //yMatchedFragIndex.Add(alignedPeak[0].yIndex);
                alignedPeak = alignedPeak.Where(o => o.xIndex != alignedPeak[0].xIndex && o.yIndex != alignedPeak[0].yIndex).ToList(); // update aligedPeak
            }

            return Score;
        }
        public static double ReverseDotProduct_MS2Compare_forMILP(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm, bool AMzLarger)
        {
            double DpScoreTop = 0;
            Ms2mzTol += 1e-6; // double type comparison

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return 0;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xMaxInt = x.Max(t => t.Intensity);
            double yMaxInt = y.Max(t => t.Intensity);

            double x_SumIntSquare = 0;
            double y_SumIntSquare = 0;

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = 100.0 * x[i].Intensity / xMaxInt });
                x_SumIntSquare += Math.Pow(x_MS2[i].Intensity, 2);
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = 100.0 * y[i].Intensity / yMaxInt });
                y_SumIntSquare += Math.Pow(y_MS2[i].Intensity, 2);
            }

            List<AlignedPeak> alignedPeak = new List<AlignedPeak>();
            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        alignedPeak.Add(new AlignedPeak { xIndex = i, yIndex = j, intProduct = x_MS2[i].Intensity * y_MS2[j].Intensity });
                    }
                }
            }

            if (alignedPeak.Count == 0)
            {
                return 0;
            }

            alignedPeak = alignedPeak.OrderByDescending(o => o.intProduct).ToList();

            double xSumAlignedInt = 0;
            double ySumAlignedInt = 0;
            while (alignedPeak.Count > 0)
            {
                DpScoreTop += alignedPeak[0].intProduct;
                xSumAlignedInt += Math.Pow(x_MS2[alignedPeak[0].xIndex].Intensity, 2);
                ySumAlignedInt += Math.Pow(y_MS2[alignedPeak[0].yIndex].Intensity, 2);
                alignedPeak = alignedPeak.Where(o => o.xIndex != alignedPeak[0].xIndex && o.yIndex != alignedPeak[0].yIndex).ToList();
            }
            double DpScore;
            if (AMzLarger)
            {
                DpScore = DpScoreTop / Math.Sqrt(y_SumIntSquare * xSumAlignedInt);
            }
            else
            {
                DpScore = DpScoreTop / Math.Sqrt(x_SumIntSquare * ySumAlignedInt);
            }

            return DpScore;
        }
        public static MS2CompareResult DotProductSolver_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {
            double DpScoreTop = 0;
            Ms2mzTol += 1e-6; // double type comparison
            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xMaxInt = x.Max(t => t.Intensity);
            double yMaxInt = y.Max(t => t.Intensity);

            double x_SumIntSquare = 0;
            double y_SumIntSquare = 0;

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = 100.0 * x[i].Intensity / xMaxInt });
                x_SumIntSquare += Math.Pow(x_MS2[i].Intensity, 2);
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = 100.0 * y[i].Intensity / yMaxInt });
                y_SumIntSquare += Math.Pow(y_MS2[i].Intensity, 2);
            }

            List<double> x_alignedInt = new List<double>();
            List<double> y_alignedInt = new List<double>();


            List<double[]> productArray = new List<double[]>();
            for (int i = 0; i < x_MS2.Count; i++)
            {
                productArray.Add(new double[y_MS2.Count]);
            }

            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        productArray[i][j] = x_MS2[i].Intensity * y_MS2[j].Intensity;
                    }
                }
            }
            Solver solver = Solver.CreateSolver("SCIP");

            // binary[i, j] is an array of 0-1 variables, which will be 1 if assigned.
            Variable[,] binary = new Variable[x_MS2.Count, y_MS2.Count];
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    binary[i, j] = solver.MakeIntVar(0, 1, $"x_{i}_y_{j}");
                }
            }

            // Each worker is assigned to at most one task.
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            for (int j = 0; j < y_MS2.Count; ++j)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int i = 0; i < x_MS2.Count; ++i)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            Objective objective = solver.Objective();
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    objective.SetCoefficient(binary[i, j], productArray[i][j]);
                }
            }
            objective.SetMaximization();
            Solver.ResultStatus resultStatus = solver.Solve();

            DpScoreTop = solver.Objective().Value();

            double DpScore = DpScoreTop / Math.Sqrt(x_SumIntSquare * y_SumIntSquare);

            for (int i = 0; i < x_MS2.Count; i++)
            {
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    if (binary[i, j].SolutionValue() > 0.5)
                    {
                        xMatchedFragIndex.Add(i);
                        yMatchedFragIndex.Add(j);
                        x_alignedInt.Add(x_MS2[i].Intensity);
                        y_alignedInt.Add(y_MS2[j].Intensity);
                    }
                }
            }

            MS2CompareResult output = new MS2CompareResult
            {
                Score = DpScore,
                MatchNumber = x_alignedInt.Count,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
            };

            return output;
        }
        public static MS2CompareResult ReverseDotProductSolver_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {
            double DpScoreTop = 0;
            Ms2mzTol += 1e-6; // double type comparison
            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            // save as list of tuples
            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xMaxInt = x.Max(t => t.Intensity);
            double yMaxInt = y.Max(t => t.Intensity);

            double x_SumIntSquare = 0;
            double y_SumIntSquare = 0;

            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = 100.0 * x[i].Intensity / xMaxInt });
                x_SumIntSquare += Math.Pow(x_MS2[i].Intensity, 2);
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = 100.0 * y[i].Intensity / yMaxInt });
                y_SumIntSquare += Math.Pow(y_MS2[i].Intensity, 2);
            }

            List<double> x_alignedInt = new List<double>();
            List<double> y_alignedInt = new List<double>();


            List<double[]> productArray = new List<double[]>();
            for (int i = 0; i < x_MS2.Count; i++)
            {
                productArray.Add(new double[y_MS2.Count]);
            }

            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        productArray[i][j] = x_MS2[i].Intensity * y_MS2[j].Intensity;
                    }
                }
            }
            Solver solver = Solver.CreateSolver("SCIP");

            // binary[i, j] is an array of 0-1 variables, which will be 1 if assigned.
            Variable[,] binary = new Variable[x_MS2.Count, y_MS2.Count];
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    binary[i, j] = solver.MakeIntVar(0, 1, $"x_{i}_y_{j}");
                }
            }

            // Each worker is assigned to at most one task.
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            for (int j = 0; j < y_MS2.Count; ++j)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int i = 0; i < x_MS2.Count; ++i)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            Objective objective = solver.Objective();
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    objective.SetCoefficient(binary[i, j], productArray[i][j]);
                }
            }
            objective.SetMaximization();
            Solver.ResultStatus resultStatus = solver.Solve();

            DpScoreTop = solver.Objective().Value();

            
            for (int i = 0; i < x_MS2.Count; i++)
            {
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    if (binary[i, j].SolutionValue() > 0.5)
                    {
                        xMatchedFragIndex.Add(i);
                        yMatchedFragIndex.Add(j);
                        x_alignedInt.Add(x_MS2[i].Intensity);
                        y_alignedInt.Add(y_MS2[j].Intensity);
                    }
                }
            }

            double DpScore = 0;
            if (x_alignedInt.Count > 0)
            {
                double x_SumAlignedIntSquare = 0;
                for (int i = 0; i < x_alignedInt.Count; i++)
                {
                    x_SumAlignedIntSquare += Math.Pow(x_SumAlignedIntSquare, 2);
                }
                DpScore = DpScoreTop / Math.Sqrt(x_SumAlignedIntSquare * y_SumIntSquare);
            }

            MS2CompareResult output = new MS2CompareResult
            {
                Score = DpScore,
                MatchNumber = x_alignedInt.Count,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
            };

            return output;
        }
        public static MS2CompareResult SpectralEntropySimilaritySolver_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {

            Ms2mzTol += 1e-6; // double type comparison

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xSumInt = x.Sum(t => t.Intensity);
            double ySumInt = y.Sum(t => t.Intensity);


            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = x[i].Intensity / xSumInt });
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = y[i].Intensity / ySumInt });
            }


            double xSpecEntropy = 0;
            double ySpecEntropy = 0;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
            }
            for (int i = 0; i < y_MS2.Count; i++)
            {
                ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
            }

            if (xSpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * xSpecEntropy;
                double sumWeightedInt = x_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                xSpecEntropy = 0;
                for (int i = 0; i < x_MS2.Count; i++)
                {
                    x_MS2[i] = new RAW_PeakElement() { Mz = x_MS2[i].Mz, Intensity = Math.Pow(x_MS2[i].Intensity, weight) / sumWeightedInt };
                    xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
                }
            }
            if (ySpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * ySpecEntropy;
                double sumWeightedInt = y_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                ySpecEntropy = 0;
                for (int i = 0; i < y_MS2.Count; i++)
                {
                    y_MS2[i] = new RAW_PeakElement() { Mz = y_MS2[i].Mz, Intensity = Math.Pow(y_MS2[i].Intensity, weight) / sumWeightedInt };
                    ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
                }
            }

            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();

            // peak alignment
            List<double[]> goalFunctionArray = new List<double[]>(); // in this case, we match peaks with largest total intensities
            for (int i = 0; i < x_MS2.Count; i++)
            {
                goalFunctionArray.Add(new double[y_MS2.Count]);
            }

            double tmpMs2mzTol = Ms2mzTol;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (ppm)
                {
                    tmpMs2mzTol = x_MS2[i].Mz * 1e-6 * Ms2mzTol;
                }
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    // direct match
                    if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMs2mzTol)
                    {
                        goalFunctionArray[i][j] = x_MS2[i].Intensity + y_MS2[j].Intensity;
                    }
                }
            }
            Solver solver = Solver.CreateSolver("SCIP");

            // binary[i, j] is an array of 0-1 variables, which will be 1 if assigned.
            Variable[,] binary = new Variable[x_MS2.Count, y_MS2.Count];
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    binary[i, j] = solver.MakeIntVar(0, 1, $"x_{i}_y_{j}");
                }
            }

            // Each worker is assigned to at most one task.
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            for (int j = 0; j < y_MS2.Count; ++j)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                for (int i = 0; i < x_MS2.Count; ++i)
                {
                    constraint.SetCoefficient(binary[i, j], 1);
                }
            }
            Objective objective = solver.Objective();
            for (int i = 0; i < x_MS2.Count; ++i)
            {
                for (int j = 0; j < y_MS2.Count; ++j)
                {
                    objective.SetCoefficient(binary[i, j], goalFunctionArray[i][j]);
                }
            }
            objective.SetMaximization();
            Solver.ResultStatus resultStatus = solver.Solve();

            List<RAW_PeakElement> spec_merged = new List<RAW_PeakElement>();
            for (int i = 0; i < x_MS2.Count; i++)
            {
                for (int j = 0; j < y_MS2.Count; j++)
                {
                    if (binary[i, j].SolutionValue() > 0.5)
                    {
                        xMatchedFragIndex.Add(i);
                        yMatchedFragIndex.Add(j);
                        spec_merged.Add(new RAW_PeakElement { Mz = x_MS2[i].Mz, Intensity = x_MS2[i].Intensity + y_MS2[j].Intensity });
                    }
                }
            }

            for (int i = 0; i < x_MS2.Count; i++)
            {
                if (xMatchedFragIndex.Contains(i)) { continue; }
                spec_merged.Add(new RAW_PeakElement { Mz = x_MS2[i].Mz, Intensity = x_MS2[i].Intensity });
            }
            for (int i = 0; i < y_MS2.Count; i++)
            {
                if (yMatchedFragIndex.Contains(i)) { continue; }
                spec_merged.Add(new RAW_PeakElement { Mz = y_MS2[i].Mz, Intensity = y_MS2[i].Intensity });
            }

            double mergedSpecEntropy = 0;
            for (int i = 0; i < spec_merged.Count; i++)
            {
                mergedSpecEntropy += -1 * (spec_merged[i].Intensity / 2) * Math.Log(spec_merged[i].Intensity / 2);
            }

            double entropySimilairty = 1 - (2 * mergedSpecEntropy - xSpecEntropy - ySpecEntropy) / Math.Log(4);
            int matchNo = xMatchedFragIndex.Count;

            //List<RAW_PeakElement> spec_merged = new List<RAW_PeakElement>();
            //for (int i = 0; i < x_MS2.Count; i++)
            //{
            //    double tmpMzTol = Ms2mzTol;
            //    if (ppm)
            //    {
            //        tmpMzTol = Ms2mzTol * x_MS2[i].Mz * 1e-6;
            //    }
            //    int yIndex = 0;
            //    for (int j = yIndex; j < y_MS2.Count; j++)
            //    {
            //        if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMzTol)
            //        {
            //            xMatchedFragIndex.Add(i);
            //            yMatchedFragIndex.Add(j);
            //            x_alignedInt.Add(x_MS2[i].Intensity);
            //            y_alignedInt.Add(y_MS2[j].Intensity);
            //            if (j == y_MS2.Count - 1)
            //            {
            //                break;
            //            }
            //            else
            //            {
            //                yIndex = j + 1;
            //            }

            //        }
            //    }
            //}

            MS2CompareResult output = new MS2CompareResult
            {
                Score = entropySimilairty,
                MatchNumber = matchNo,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
            };
            return output;
        }
        // this is the original version (alignment method provided by author, could generate score > 1)
        public static MS2CompareResult SpectralEntropySimilarityOriginal_MS2Compare(List<RAW_PeakElement> x, List<RAW_PeakElement> y, double Ms2mzTol, bool ppm)
        {

            Ms2mzTol += 1e-6; // double type comparison

            if (x == null || y == null || x.Count == 0 || y.Count == 0)
            {
                return null;
            }

            List<RAW_PeakElement> x_MS2 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> y_MS2 = new List<RAW_PeakElement>();

            // normalize
            double xSumInt = x.Sum(t => t.Intensity);
            double ySumInt = y.Sum(t => t.Intensity);


            for (int i = 0; i < x.Count; i++)
            {
                x_MS2.Add(new RAW_PeakElement() { Mz = x[i].Mz, Intensity = x[i].Intensity / xSumInt });
            }
            for (int i = 0; i < y.Count; i++)
            {
                y_MS2.Add(new RAW_PeakElement() { Mz = y[i].Mz, Intensity = y[i].Intensity / ySumInt });
            }


            double xSpecEntropy = 0;
            double ySpecEntropy = 0;
            for (int i = 0; i < x_MS2.Count; i++)
            {
                xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
            }
            for (int i = 0; i < y_MS2.Count; i++)
            {
                ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
            }

            if (xSpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * xSpecEntropy;
                double sumWeightedInt = x_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                xSpecEntropy = 0;
                for (int i = 0; i < x_MS2.Count; i++)
                {
                    x_MS2[i] = new RAW_PeakElement() { Mz = x_MS2[i].Mz, Intensity = Math.Pow(x_MS2[i].Intensity, weight) / sumWeightedInt };
                    xSpecEntropy += -1 * x_MS2[i].Intensity * Math.Log(x_MS2[i].Intensity);
                }
            }
            if (ySpecEntropy < 3)
            {
                double weight = 0.25 + 0.25 * ySpecEntropy;
                double sumWeightedInt = y_MS2.Sum(o => Math.Pow(o.Intensity, weight));
                ySpecEntropy = 0;
                for (int i = 0; i < y_MS2.Count; i++)
                {
                    y_MS2[i] = new RAW_PeakElement() { Mz = y_MS2[i].Mz, Intensity = Math.Pow(y_MS2[i].Intensity, weight) / sumWeightedInt };
                    ySpecEntropy += -1 * y_MS2[i].Intensity * Math.Log(y_MS2[i].Intensity);
                }
            }

            List<int> xMatchedFragIndex = new List<int>();
            List<int> yMatchedFragIndex = new List<int>();
            int a = 0;
            int b = 0;
            double peak_b_int = 0;
            List<MergedSpec> spec_merged = new List<MergedSpec>(); // mz, xInt, yInt, xInt + yInt

            while (a < x_MS2.Count && b < y_MS2.Count)
            {
                double tmpMzTol = Ms2mzTol;
                if (ppm)
                {
                    tmpMzTol = x_MS2[a].Mz * Ms2mzTol * 1e-6;
                }
                double deltaMass = x_MS2[a].Mz - y_MS2[b].Mz;
                if (deltaMass + tmpMzTol < 0)
                {//Peak only existed in spec a
                    spec_merged.Add(new MergedSpec { Mz = x_MS2[a].Mz, xInt = x_MS2[a].Intensity, yInt = peak_b_int, SumInt = x_MS2[a].Intensity + peak_b_int, NormedInt = 0 });
                    peak_b_int = 0;
                    a++;
                }
                else if (deltaMass > tmpMzTol)
                {//Peak only existed in spec b.
                    spec_merged.Add(new MergedSpec { Mz = y_MS2[b].Mz, xInt = 0, yInt = y_MS2[b].Intensity, SumInt = y_MS2[b].Intensity, NormedInt = 0 });
                    b++;
                }
                else
                {// in both
                    xMatchedFragIndex.Add(a);
                    yMatchedFragIndex.Add(b);
                    peak_b_int += y_MS2[b].Intensity;
                    b++;
                }
            }
            if (peak_b_int > 0)
            {
                spec_merged.Add(new MergedSpec { Mz = x_MS2[a].Mz, xInt = x_MS2[a].Intensity, yInt = peak_b_int, SumInt = x_MS2[a].Intensity + peak_b_int, NormedInt = 0 });
                peak_b_int = 0;
                a++;
            }
            if (b < y_MS2.Count)
            {
                for (int i = b; i < y_MS2.Count; i++)
                {
                    spec_merged.Add(new MergedSpec { Mz = y_MS2[i].Mz, xInt = 0, yInt = y_MS2[i].Intensity, SumInt = y_MS2[i].Intensity, NormedInt = 0 });
                }
            }
            if (a < x_MS2.Count)
            {
                for (int i = a; i < x_MS2.Count; i++)
                {
                    spec_merged.Add(new MergedSpec { Mz = x_MS2[i].Mz, xInt = x_MS2[i].Intensity, yInt = 0, SumInt = x_MS2[i].Intensity, NormedInt = 0 });
                }
            }
            xMatchedFragIndex = xMatchedFragIndex.Distinct().ToList();
            yMatchedFragIndex = yMatchedFragIndex.Distinct().ToList();


            double mergedSpecEntropy = 0;
            //double sumMergedInt = spec_merged.Sum(o => o.SumInt);
            for (int i = 0; i < spec_merged.Count; i++)
            {
                //spec_merged[i].NormedInt = spec_merged[i].SumInt / sumMergedInt;
                spec_merged[i].NormedInt = spec_merged[i].SumInt / 2;
                mergedSpecEntropy += -1 * spec_merged[i].NormedInt * Math.Log(spec_merged[i].NormedInt);
            }


            double entropySimilairty = 1 - (2 * mergedSpecEntropy - xSpecEntropy - ySpecEntropy) / Math.Log(4);
            int matchNo = spec_merged.Count(o => o.xInt > 0 && o.yInt > 0);

            //List<RAW_PeakElement> spec_merged = new List<RAW_PeakElement>();
            //for (int i = 0; i < x_MS2.Count; i++)
            //{
            //    double tmpMzTol = Ms2mzTol;
            //    if (ppm)
            //    {
            //        tmpMzTol = Ms2mzTol * x_MS2[i].Mz * 1e-6;
            //    }
            //    int yIndex = 0;
            //    for (int j = yIndex; j < y_MS2.Count; j++)
            //    {
            //        if (Math.Abs(y_MS2[j].Mz - x_MS2[i].Mz) <= tmpMzTol)
            //        {
            //            xMatchedFragIndex.Add(i);
            //            yMatchedFragIndex.Add(j);
            //            x_alignedInt.Add(x_MS2[i].Intensity);
            //            y_alignedInt.Add(y_MS2[j].Intensity);
            //            if (j == y_MS2.Count - 1)
            //            {
            //                break;
            //            }
            //            else
            //            {
            //                yIndex = j + 1;
            //            }

            //        }
            //    }
            //}

            MS2CompareResult output = new MS2CompareResult
            {
                Score = entropySimilairty,
                MatchNumber = matchNo,
                xMatchFragIndex = xMatchedFragIndex,
                yMatchFragIndex = yMatchedFragIndex,
            };
            return output;
        }
    }
    class AlignedPeak
    {
        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public double intProduct { get; set; }
    }
    class MergedSpecAlignedPeak // for entropy 
    {
        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public double sumInt { get; set; }
    }
    class MergedSpec
    {
        public double Mz { get; set; }
        public double xInt { get; set; }
        public double yInt { get; set; }
        public double SumInt { get; set; } // xInt + yInt
        public double NormedInt { get; set; }
    }
    [Serializable]
    public class MS2MatchResult // if template-based MS2 matching: x:exp; y:library
    {
        [JsonInclude]
        public MS2CompareResult ms2CompareResult { get; set; }
        [JsonInclude]
        public MS2DBEntry matchedDBEntry { get; set; }
    }
}
