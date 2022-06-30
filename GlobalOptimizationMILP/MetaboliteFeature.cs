using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BUDDY.BufpHelper;
using BUDDY.FormulaData;
using BUDDY.RawData;

namespace BUDDY.GlobalOptimizationMILP
{
    public class MetaboliteFeature
    {
        public int ms2Index { get; set; } // index in ms2model, not scan number; ms2model.MS2s[ms2Index]
        public List<Feature> features { get; set; } // MLR features, each Feature refer to a candidate
        public int featureReservedForMILP { get; set; } // candidate count for MILP, candidate # used for MILP != candidates reserved outside
        public double mz { get; set; }
        public double rt { get; set; }
        public Adduct adduct { get; set; }
        public List<RAW_PeakElement> ms2 { get; set; }
        public List<FeatureConnection> featureConnections { get; set; }
        

        // for seed metabolite
        public bool seedMetabolite { get; set; }
        public string formulaGroundTruth { get; set; } // only for seed metabolite
        public int variableIndexMILP { get; set; } // only for seed metabolite
        public FormulaElement seedFormulaElement { get; set; }
        //public double ms1Tol { get; set; } // used for pre-filter metabolite feature relationships; if delta mz not in the list, no need to calculate
        //public double ms2Tol { get; set; } // used for MS2 comparison
        //public bool ppm { get; set; }

    }
    public class FeatureConnection
    {
        public Connection connection { get; set; }
        public int pairedMs2Index { get; set; } // index in ms2model, not scan number; ms2model.MS2s[ms2Index]
        public int pairedMs2ScanNumber { get; set; } // scan number
        public double ms2Similarity { get; set; }

    }

}
