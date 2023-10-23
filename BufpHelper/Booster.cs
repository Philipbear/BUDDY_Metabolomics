using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using BUDDY.FormulaData;
using BUDDY.RawData;
using MathNet.Numerics.Statistics;
using NCDK;
using NCDK.Formula;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BUDDY.BufpHelper
{
    public sealed class Booster
    {
        public static int ION_MODE_FACTOR;
        public static double E_MASS = 0.0005485; // electron
        public static double P_MASS = 1.007276; // proton
        public static double H_MASS = 1.007825; // H
        public static double Na_MASS = 22.989769;
        public static double K_MASS = 38.963707;
        public static int GROUP_MZ_RANGE = 1;

        public Booster() { }


        public static List<Feature> Booster0(Ms2Utility ms2, bool ppm,
            List<List<AlignedFormula>> neutralDB, List<Distribution> distDF, List<Distribution> pre_dist_df, UInt32 ms2index, int topCandidateNum, bool topcandcut,
            int max_isotopic_peaks, double isotopic_abundance_cutoff, double isp_grp_mass_tol, FormulaRestriction formRestriction)
        {
            double ms2tol = ms2.Ms2Tol;
            double ms1tol = ms2.Ms1Tol;

            bool highResMS;
            if ((ppm && ms1tol > 100) || (ppm == false && ms1tol >= 0.1))
            {
                highResMS = false;
            }
            else
            {
                highResMS = true;
            }

            List<RAW_PeakElement> ms1 = ms2.Ms1;
            Adduct adduct = ms2.Adduct;

            // ion mode, sign factor
            if (ms2.Polarity == "P")
            {
                ION_MODE_FACTOR = 1;
            }
            if (ms2.Polarity == "N")
            {
                ION_MODE_FACTOR = -1;
            }

            //feature data matrix
            List<Feature> dm = new List<Feature>();
            int targetInd = (int)((ms2.Mz * adduct.AbsCharge - adduct.SumFormula.mass)/ adduct.M / GROUP_MZ_RANGE);
            //Debug.WriteLine("booster line 66: " + targetInd); // output targetindex

            if (targetInd <= 1 || targetInd >= neutralDB.Count - 1)
            {
                return new List<Feature>();
            }
            List<AlignedFormula> precursor_db = new List<AlignedFormula>();
            precursor_db.AddRange(neutralDB[targetInd - 1]);
            precursor_db.AddRange(neutralDB[targetInd]);
            precursor_db.AddRange(neutralDB[targetInd + 1]);



            // low-res MS: CHNOPS
            if (!highResMS)
            {
                precursor_db = precursor_db.Where(o =>
                        o.si == 0 &&
                        o.b == 0 &&
                        o.se == 0 &&
                        o.f == 0 &&
                        o.cl == 0 &&
                        o.br == 0 &&
                        o.i == 0).ToList();
                //element count restriction
                precursor_db = precursor_db.Where(o => o.c >= formRestriction.C_min &&
                            o.h >= formRestriction.H_min &&
                            o.n >= formRestriction.N_min &&
                            o.o >= formRestriction.O_min &&
                            o.p >= formRestriction.P_min &&
                            o.s >= formRestriction.S_min &&
                            o.c <= formRestriction.C_max &&
                            o.h <= formRestriction.H_max &&
                            o.n <= formRestriction.N_max &&
                            o.o <= formRestriction.O_max &&
                            o.p <= formRestriction.P_max &&
                            o.s <= formRestriction.S_max).ToList();
            }
            else
            {
                //element count restriction
                precursor_db = precursor_db.Where(o => o.c >= formRestriction.C_min &&
                            o.h >= formRestriction.H_min &&
                            o.n >= formRestriction.N_min &&
                            o.o >= formRestriction.O_min &&
                            o.p >= formRestriction.P_min &&
                            o.s >= formRestriction.S_min &&
                            o.f >= formRestriction.F_min &&
                            o.cl >= formRestriction.Cl_min &&
                            o.br >= formRestriction.Br_min &&
                            o.i >= formRestriction.I_min &&
                            o.si >= formRestriction.Si_min &&
                            o.b >= formRestriction.B_min &&
                            o.se >= formRestriction.Se_min &&
                            o.c <= formRestriction.C_max &&
                            o.h <= formRestriction.H_max &&
                            o.n <= formRestriction.N_max &&
                            o.o <= formRestriction.O_max &&
                            o.p <= formRestriction.P_max &&
                            o.s <= formRestriction.S_max &&
                            o.f <= formRestriction.F_max &&
                            o.cl <= formRestriction.Cl_max &&
                            o.br <= formRestriction.Br_max &&
                            o.i <= formRestriction.I_max &&
                            o.si <= formRestriction.Si_max &&
                            o.b <= formRestriction.B_max &&
                            o.se <= formRestriction.Se_max).ToList();
            }

            if (ppm)
            {
                ms1tol = ms1tol * ms2.Mz * 1e-6;
            }

            List<AlignedFormula> candidates = new List<AlignedFormula>( precursor_db.Where(o => Math.Abs(o.mass * adduct.M - (ms2.Mz * adduct.AbsCharge - adduct.SumFormula.mass)) <= ms1tol ));

            int FTpolarity;
            if (ms2.Polarity == "P")
            {
                FTpolarity = 0;
            }
            else
            {
                FTpolarity = 1;
            }

            candidates = candidates.Where(o => o.c * adduct.M >= -adduct.SumFormulaNeg.c &&
                o.h * adduct.M >= -adduct.SumFormulaNeg.h &&
                o.b * adduct.M >= -adduct.SumFormulaNeg.b &&
                o.br * adduct.M >= -adduct.SumFormulaNeg.br &&
                o.cl * adduct.M >= -adduct.SumFormulaNeg.cl &&
                o.f * adduct.M >= -adduct.SumFormulaNeg.f &&
                o.i * adduct.M >= -adduct.SumFormulaNeg.i &&
                o.k * adduct.M >= -adduct.SumFormulaNeg.k &&
                o.n * adduct.M >= -adduct.SumFormulaNeg.n &&
                o.na * adduct.M >= -adduct.SumFormulaNeg.na &&
                o.o * adduct.M >= -adduct.SumFormulaNeg.o &&
                o.p * adduct.M >= -adduct.SumFormulaNeg.p &&
                o.s * adduct.M >= -adduct.SumFormulaNeg.s &&
                o.se * adduct.M >= -adduct.SumFormulaNeg.se &&
                o.si * adduct.M >= -adduct.SumFormulaNeg.si).ToList();

            //change ms2 precursor formula to neutral form
            for (int i = 0; i < candidates.Count; i++)
            {
                string new_formula = "";
                IDictionary<string, int> v = new Dictionary<string, int>();
                v.Add("C", candidates[i].c * adduct.M + adduct.SumFormula.c);
                v.Add("H", candidates[i].h * adduct.M + adduct.SumFormula.h);
                v.Add("B", candidates[i].b * adduct.M + adduct.SumFormula.b);
                v.Add("Br", candidates[i].br * adduct.M + adduct.SumFormula.br);
                v.Add("Cl", candidates[i].cl * adduct.M + adduct.SumFormula.cl);
                v.Add("F", candidates[i].f * adduct.M + adduct.SumFormula.f);
                v.Add("I", candidates[i].i * adduct.M + adduct.SumFormula.i);
                v.Add("K", candidates[i].k * adduct.M + adduct.SumFormula.k);
                v.Add("N", candidates[i].n * adduct.M + adduct.SumFormula.n);
                v.Add("Na", candidates[i].na * adduct.M + adduct.SumFormula.na);
                v.Add("O", candidates[i].o * adduct.M + adduct.SumFormula.o);
                v.Add("P", candidates[i].p * adduct.M + adduct.SumFormula.p);
                v.Add("S", candidates[i].s * adduct.M + adduct.SumFormula.s);
                v.Add("Se", candidates[i].se * adduct.M + adduct.SumFormula.se);
                v.Add("Si", candidates[i].si * adduct.M + adduct.SumFormula.si);
                foreach (KeyValuePair<string, int> element in v)
                {
                    if (element.Value > 1)
                    {
                        new_formula = new_formula + element.Key + element.Value.ToString();
                    }
                    if (element.Value == 1)
                    {
                        new_formula = new_formula + element.Key;
                    }
                }
                candidates[i].formstr_charge = new_formula;
            }

            //more than topCandidateNum valid candidates then sort by expfragint and filter
            if (!topcandcut)
            {
                if (candidates.Count > topCandidateNum)
                {
                    candidates = candidates.OrderBy(o => Math.Abs(o.mass* adduct.M - ms2.Mz * adduct.AbsCharge + adduct.SumFormula.mass)).Take(topCandidateNum).ToList();
                }
            }

            // TO ADD: isotope similarity for multiply charged ions
            //if (ms1 != null && ms1.Count > 0)
            //{
            //    if (adduct.AbsCharge > 1)
            //    {
            //        for (int f = 0; f < ms1.Count; f++)
            //        {
            //            ms1[f] = new RAW_PeakElement { Mz = ms1[f].Mz * adduct.AbsCharge, Intensity = ms1[f].Intensity };
            //        }
            //    }
            //}

            // Feature data matrix
            Parallel.For(0, candidates.Count, i =>
            //for (int i = 0; i < df.Count; i++)
            {
                double ispsim = 0;
                IsotopePattern iPattern = null;
                if (ms1 != null && ms1.Count > 0)
                {
                    IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(candidates[i].formstr_charge);
                    mf.Charge = adduct.AbsCharge;
                    IsotopePatternGenerator IPG = new IsotopePatternGenerator(isotopic_abundance_cutoff / 100);
                    iPattern = IPG.GetIsotopes(mf);
                    //iPattern.Charge = ION_MODE_FACTOR;
                    ispsim =  IsotopeSimilarity(ms1, iPattern, max_isotopic_peaks, ms1tol, isp_grp_mass_tol, false, isotopic_abundance_cutoff,ION_MODE_FACTOR);
                }
                List<CFDF> cfdf = new List<CFDF>();
                cfdf.Add(new CFDF(-1, candidates[i].c, candidates[i].h, candidates[i].n, candidates[i].o, candidates[i].p, candidates[i].f, candidates[i].cl, candidates[i].br, candidates[i].i,
                        candidates[i].s, candidates[i].si, candidates[i].b, candidates[i].se, candidates[i].na, candidates[i].k));


                double totalAtomNo = (double)(candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o + candidates[i].p + candidates[i].f + candidates[i].cl +
                        candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k);
                int cho;
                if (candidates[i].n + candidates[i].p + candidates[i].f + candidates[i].cl +
                        candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                {
                    cho = 1;
                }
                else
                {
                    cho = 0;
                }
                int chno;
                if (candidates[i].p + candidates[i].f + candidates[i].cl +
                        candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                {
                    chno = 1;
                }
                else
                {
                    chno = 0;
                }
                int chnops;
                if (candidates[i].f + candidates[i].cl + candidates[i].br + candidates[i].i + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                {
                    chnops = 1;
                }
                else
                {
                    chnops = 0;
                }

                double preMzErrorInDM;
                //preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - (ms2.Mz - adduct.SumFormula.mass)) / ms1tol;
                if (highResMS)
                {
                    preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - (ms2.Mz * adduct.AbsCharge - adduct.SumFormula.mass)) / ms1tol;
                }
                else
                {
                    preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - (ms2.Mz * adduct.AbsCharge - adduct.SumFormula.mass));
                }

                lock (dm) dm.Add(new Feature(
                    ms2index,
                    candidates[i].formstr_neutral,
                    candidates[i].dbe,
                    H2C_formula(new int[] { candidates[i].c, candidates[i].h }),
                    Hetero2C_formula(new int[] { candidates[i].c, candidates[i].h, candidates[i].n, candidates[i].o, candidates[i].p, candidates[i].f, candidates[i].cl,
                            candidates[i].br, candidates[i].i, candidates[i].s, candidates[i].si, candidates[i].b, candidates[i].se, candidates[i].na, candidates[i].k }),
                    preMzErrorInDM,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    FTpolarity, 0, 0, ms1tol, 0, 0, 0, 0, 0, 0, 0, 0, 0, ispsim, iPattern, 0, cfdf, 0, 0, (candidates[i].f + candidates[i].cl + candidates[i].br + candidates[i].i) / totalAtomNo,
                    candidates[i].c / totalAtomNo, (candidates[i].c + candidates[i].h + candidates[i].o) / totalAtomNo, (candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o) / totalAtomNo,
                    (candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o + candidates[i].p + candidates[i].s) / totalAtomNo,
                    cho, chno, chnops, pre_psn_dist(totalAtomNo / candidates[i].mass, pre_dist_df, 3), 1
                    ));
            });
            dm.RemoveAll(item => item == null);
            dm = dm.OrderByDescending(o => o.expfIntRatio).ThenBy(o => o.p_mzErrorRatio).ToList();
            return dm;
        }
        public static List<Feature> Booster1(Ms2Utility ms2, bool ppm,
            List<List<AlignedFormula>> neutralDB, List<Distribution> distDF, List<Distribution> pre_dist_df, UInt32 ms2index, int topCandidateNum, bool topcandcut,
            int max_isotopic_peaks, double isotopic_abundance_cutoff, double isp_grp_mass_tol, double timeout, FormulaRestriction formRestriction)
        {
            double ms2tol = ms2.Ms2Tol;
            double ms1tol = ms2.Ms1Tol;

            bool highResMS;
            if ((ppm && ms1tol > 100) || (ppm == false && ms1tol >= 0.1))
            {
                highResMS = false;
            }
            else
            {
                highResMS = true;
            }

            List<RAW_PeakElement> ms1 = ms2.Ms1;
            Adduct adduct = ms2.Adduct;

            // ion mode, sign factor
            if (ms2.Polarity == "P")
            {
                ION_MODE_FACTOR = 1;
            }
            if (ms2.Polarity == "N")
            {
                ION_MODE_FACTOR = -1;
            }

            //feature data matrix
            List<Feature> dm = new List<Feature>();
            if (ms2.Spectrum.Count > 0)
            {
                //if (!use_allFrag)
                //{
                //    if (ms2.Spectrum.Count > topfragNo)
                //    {
                //        ms2.Spectrum = ms2.Spectrum.OrderByDescending(o => o.Intensity).ToList().Take(topfragNo).ToList();
                //    }
                //}

                // MS2 mz, nl, int
                double[] nl = new double[ms2.Spectrum.Count];
                double[] mz = new double[ms2.Spectrum.Count];
                double[] intensity = new double[ms2.Spectrum.Count];
                double maxInt = ms2.Spectrum.Max(t => t.Intensity);
                for (int i = 0; i < ms2.Spectrum.Count; i++)
                {
                    nl[i] = ms2.Mz - ms2.Spectrum[i].Mz;
                    mz[i] = ms2.Spectrum[i].Mz;
                    intensity[i] = Math.Sqrt(100.0 * ms2.Spectrum[i].Intensity / maxInt); // normalize to 100, then sqrt
                }

                double sum_intensity = intensity.Sum();
                for (int i = 0; i < ms2.Spectrum.Count; i++)
                {
                    intensity[i] = intensity[i] / sum_intensity;
                }
                //ms2.Spectrum = ms2.Spectrum.OrderBy(o => o.Mz).ToList();


                // candidate formula data frame template
                CancellationTokenSource cts1 = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                List<CFDF> cfdf_all = new List<CFDF>(); //candidate formula data frame (cfdf)

                try
                {
                    Parallel.For(0, ms2.Spectrum.Count,
                        new ParallelOptions { CancellationToken = cts1.Token, MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                        i =>
                    //for (int i = 0; i < ms2.Spectrum.Count; i++)
                    {
                        int frag_FDB_Index = 1;
                        int frag_FDB_H_Index = 1;
                        int frag_NLDB_Index = 1;
                        int frag_NLDB_H_Index = 1;
                        int frag_FDB_Na_Index = 1;
                        int frag_FDB_K_Index = 1;
                        int frag_NLDB_Na_Index = 1;
                        int frag_NLDB_K_Index = 1;

                        frag_FDB_Index = (int)((mz[i] + E_MASS * ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                        frag_FDB_H_Index =(int)((mz[i] - P_MASS * ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                        frag_NLDB_Index = (int)(nl[i] / GROUP_MZ_RANGE);
                        frag_NLDB_H_Index = (int)((nl[i] - H_MASS * ION_MODE_FACTOR) / GROUP_MZ_RANGE);

                        if (adduct.SumFormula.na > 0)
                        {
                            frag_FDB_Na_Index = (int)((mz[i] - (Na_MASS - E_MASS) *  ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                            frag_NLDB_Na_Index = (int)((nl[i] - Na_MASS * ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                        }
                        if (adduct.SumFormula.k > 0)
                        {
                            frag_FDB_K_Index = (int)((mz[i] - (K_MASS - E_MASS) *  ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                            frag_NLDB_K_Index = (int)((nl[i] - K_MASS * ION_MODE_FACTOR) / GROUP_MZ_RANGE);
                        }

                        List<AlignedFormulaEasyVersion> FDB = new List<AlignedFormulaEasyVersion>();
                        List<AlignedFormulaEasyVersion> FDB_H = new List<AlignedFormulaEasyVersion>();
                        List<AlignedFormulaEasyVersion> NLDB = new List<AlignedFormulaEasyVersion>();
                        List<AlignedFormulaEasyVersion> NLDB_H = new List<AlignedFormulaEasyVersion>();
                        for (int x = frag_FDB_H_Index - 1; x <= frag_FDB_H_Index + 1; x++)
                        {
                            if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                            {
                                FDB_H.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                {
                                    formstr_neutral = o.formstr_neutral,
                                    mass = o.mass + P_MASS * ION_MODE_FACTOR,
                                    dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                    charge = o.charge,
                                    c = o.c,
                                    h = o.h + 1 * ION_MODE_FACTOR,
                                    n = o.n,
                                    o = o.o,
                                    p = o.p,
                                    f = o.f,
                                    cl = o.cl,
                                    br = o.br,
                                    i = o.i,
                                    s = o.s,
                                    si = o.si,
                                    b = o.b,
                                    se = o.se,
                                    na = o.na,
                                    k = o.k,
                                    LogFreq = o.LogFreq
                                }));
                            }
                        }
                        for (int x = frag_FDB_Index - 1; x <= frag_FDB_Index + 1; x++)
                        {
                            if (x >= 0 && x < neutralDB.Count)
                            {
                                FDB.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                {
                                    formstr_neutral = o.formstr_neutral,
                                    mass = o.mass - E_MASS * ION_MODE_FACTOR,
                                    dbe = o.dbe,
                                    charge = o.charge,
                                    c = o.c,
                                    h = o.h,
                                    n = o.n,
                                    o = o.o,
                                    p = o.p,
                                    f = o.f,
                                    cl = o.cl,
                                    br = o.br,
                                    i = o.i,
                                    s = o.s,
                                    si = o.si,
                                    b = o.b,
                                    se = o.se,
                                    na = o.na,
                                    k = o.k,
                                    LogFreq = o.LogFreq

                                }));
                            }
                        }
                        for (int x = frag_NLDB_H_Index - 1; x <= frag_NLDB_H_Index + 1; x++)
                        {
                            if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                            {
                                NLDB_H.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                {
                                    formstr_neutral = o.formstr_neutral,
                                    mass = o.mass + H_MASS * (double)ION_MODE_FACTOR,
                                    dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                    charge = o.charge,
                                    c = o.c,
                                    h = o.h + 1 * ION_MODE_FACTOR,
                                    n = o.n,
                                    o = o.o,
                                    p = o.p,
                                    f = o.f,
                                    cl = o.cl,
                                    br = o.br,
                                    i = o.i,
                                    s = o.s,
                                    si = o.si,
                                    b = o.b,
                                    se = o.se,
                                    na = o.na,
                                    k = o.k,
                                    LogFreq = o.LogFreq

                                }));
                            }
                        }
                        for (int x = frag_NLDB_Index - 1; x <= frag_NLDB_Index + 1; x++)
                        {
                            if (x >= 0 && x < neutralDB.Count)
                            {
                                NLDB.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                {
                                    formstr_neutral = o.formstr_neutral,
                                    mass = o.mass,
                                    dbe = o.dbe,
                                    charge = o.charge,
                                    c = o.c,
                                    h = o.h,
                                    n = o.n,
                                    o = o.o,
                                    p = o.p,
                                    f = o.f,
                                    cl = o.cl,
                                    br = o.br,
                                    i = o.i,
                                    s = o.s,
                                    si = o.si,
                                    b = o.b,
                                    se = o.se,
                                    na = o.na,
                                    k = o.k,
                                    LogFreq = o.LogFreq

                                }));
                            }
                        }

                        FDB.AddRange(FDB_H);
                        NLDB.AddRange(NLDB_H);

                        if (adduct.SumFormula.na > 0 && adduct.SumFormula.k >= 0)
                        {
                            List<AlignedFormulaEasyVersion> FDB_Na = new List<AlignedFormulaEasyVersion>();
                            List<AlignedFormulaEasyVersion> NLDB_Na = new List<AlignedFormulaEasyVersion>();
                            for (int x = frag_FDB_Na_Index - 1; x <= frag_FDB_Na_Index + 1; x++)
                            {
                                if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                                {
                                    FDB_Na.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                    {
                                        formstr_neutral = o.formstr_neutral,
                                        mass = o.mass + (Na_MASS - E_MASS) * (double)ION_MODE_FACTOR,
                                        dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                        charge = o.charge,
                                        c = o.c,
                                        h = o.h,
                                        n = o.n,
                                        o = o.o,
                                        p = o.p,
                                        f = o.f,
                                        cl = o.cl,
                                        br = o.br,
                                        i = o.i,
                                        s = o.s,
                                        si = o.si,
                                        b = o.b,
                                        se = o.se,
                                        na = o.na + 1 * ION_MODE_FACTOR,
                                        k = o.k,
                                        LogFreq = o.LogFreq
                                    }));
                                }
                            }
                            for (int x = frag_NLDB_Na_Index - 1; x <= frag_NLDB_Na_Index + 1; x++)
                            {
                                if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                                {
                                    NLDB_Na.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                    {
                                        formstr_neutral = o.formstr_neutral,
                                        mass = o.mass + Na_MASS * (double)ION_MODE_FACTOR,
                                        dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                        charge = o.charge,
                                        c = o.c,
                                        h = o.h,
                                        n = o.n,
                                        o = o.o,
                                        p = o.p,
                                        f = o.f,
                                        cl = o.cl,
                                        br = o.br,
                                        i = o.i,
                                        s = o.s,
                                        si = o.si,
                                        b = o.b,
                                        se = o.se,
                                        na = o.na + 1 * ION_MODE_FACTOR,
                                        k = o.k,
                                        LogFreq = o.LogFreq

                                    }));
                                }
                            }

                            FDB.AddRange(FDB_Na);
                            NLDB.AddRange(NLDB_Na);
                        }
                        else if (adduct.SumFormula.na == 0 && adduct.SumFormula.k > 0)
                        {
                            List<AlignedFormulaEasyVersion> FDB_K = new List<AlignedFormulaEasyVersion>();
                            List<AlignedFormulaEasyVersion> NLDB_K = new List<AlignedFormulaEasyVersion>();
                            for (int x = frag_FDB_K_Index - 1; x <= frag_FDB_K_Index + 1; x++)
                            {
                                if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                                {
                                    FDB_K.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                    {
                                        formstr_neutral = o.formstr_neutral,
                                        mass = o.mass + (K_MASS - E_MASS) * (double)ION_MODE_FACTOR,
                                        dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                        charge = o.charge,
                                        c = o.c,
                                        h = o.h,
                                        n = o.n,
                                        o = o.o,
                                        p = o.p,
                                        f = o.f,
                                        cl = o.cl,
                                        br = o.br,
                                        i = o.i,
                                        s = o.s,
                                        si = o.si,
                                        b = o.b,
                                        se = o.se,
                                        na = o.na,
                                        k = o.k + 1 * ION_MODE_FACTOR,
                                        LogFreq = o.LogFreq
                                    }));
                                }
                            }
                            for (int x = frag_NLDB_K_Index - 1; x <= frag_NLDB_K_Index + 1; x++)
                            {
                                if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                                {
                                    NLDB_K.AddRange(neutralDB[x].ConvertAll(o => new AlignedFormulaEasyVersion
                                    {
                                        formstr_neutral = o.formstr_neutral,
                                        mass = o.mass + K_MASS * (double)ION_MODE_FACTOR,
                                        dbe = o.dbe - 0.5 * ION_MODE_FACTOR,
                                        charge = o.charge,
                                        c = o.c,
                                        h = o.h,
                                        n = o.n,
                                        o = o.o,
                                        p = o.p,
                                        f = o.f,
                                        cl = o.cl,
                                        br = o.br,
                                        i = o.i,
                                        s = o.s,
                                        si = o.si,
                                        b = o.b,
                                        se = o.se,
                                        na = o.na,
                                        k = o.k + 1 * ION_MODE_FACTOR,
                                        LogFreq = o.LogFreq

                                    }));
                                }
                            }

                            FDB.AddRange(FDB_K);
                            NLDB.AddRange(NLDB_K);
                        }

                        if (!highResMS)
                        {
                            //element count restriction
                            FDB = FDB.Where(o =>
                                o.si == 0 &&
                                o.b == 0 &&
                                o.se == 0 &&
                                o.f == 0 &&
                                o.cl == 0 &&
                                o.br == 0 &&
                                o.i == 0).ToList();
                            NLDB = NLDB.Where(o =>
                                o.si == 0 &&
                                o.b == 0 &&
                                o.se == 0 &&
                                o.f == 0 &&
                                o.cl == 0 &&
                                o.br == 0 &&
                                o.i == 0).ToList();
                        }


                        List<AlignedFormulaEasyVersion> f_cdf = new List<AlignedFormulaEasyVersion>(); // fragment, candidate data frame
                        List<AlignedFormulaEasyVersion> nl_cdf = new List<AlignedFormulaEasyVersion>(); //  NL

                        foreach (AlignedFormulaEasyVersion fc in FDB)
                        {
                            if (ppm == true)
                            {
                                if (Math.Abs(fc.mass - mz[i]) <= mz[i] * ms2tol * 1e-6)
                                {
                                    f_cdf.Add(fc);
                                }
                            }
                            else
                            {
                                if (Math.Abs(fc.mass - mz[i]) <= ms2tol)
                                {
                                    f_cdf.Add(fc);
                                }
                            }
                        }

                        foreach (AlignedFormulaEasyVersion nlc in NLDB)
                        {
                            if (ppm == true)
                            {
                                if (Math.Abs(nlc.mass - nl[i]) <= (mz[i] + ms2.Mz) * ms2tol * 1e-6)
                                {
                                    nl_cdf.Add(nlc);
                                }
                            }
                            else
                            {
                                if (Math.Abs(nlc.mass - nl[i]) <= ms2tol * 2)
                                {
                                    nl_cdf.Add(nlc);
                                }
                            }
                        }
                        if (f_cdf.Count == 0 || nl_cdf.Count == 0)
                        {
                            return;
                            //continue;
                        }

                        //filter f_cdf & nl_cdf
                        int reservedNo;
                        if (highResMS)
                        {
                            reservedNo = 250;
                        }
                        else
                        {
                            reservedNo = 500;
                        }
                        if (f_cdf.Count > reservedNo)
                        {
                            f_cdf = f_cdf.OrderByDescending(o => o.LogFreq).Take(reservedNo).ToList();
                        }
                        if (nl_cdf.Count > reservedNo)
                        {
                            nl_cdf = nl_cdf.OrderByDescending(o => o.LogFreq).Take(reservedNo).ToList();
                        }

                        // candidate formula for one fragment
                        List<CFDF> cfdf = new List<CFDF>();
                        List<string> cfdf_formulas = new List<string>();
                        for (int m = 0; m < f_cdf.Count; m++)
                        {
                            for (int n = 0; n < nl_cdf.Count; n++)
                            {
                                
                                if (f_cdf[m].c + nl_cdf[n].c == 0)
                                {
                                    continue;
                                }
                                if ((f_cdf[m].dbe + nl_cdf[n].dbe) % 1.0 == 0.0)
                                {
                                    continue;
                                }


                                double sum_mass = f_cdf[m].mass + nl_cdf[n].mass;
                                double mz_error = sum_mass - ms2.Mz;

                                if (ppm == true)
                                {
                                    if (Math.Abs(mz_error) > ms2.Mz * ms1tol * 1e-6)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (Math.Abs(mz_error) > ms1tol)
                                    {
                                        continue;
                                    }
                                }

                                string formula = "";
                                int f_cdf_c = f_cdf[m].c;
                                int f_cdf_h = f_cdf[m].h;
                                int f_cdf_n = f_cdf[m].n;
                                int f_cdf_o = f_cdf[m].o;
                                int f_cdf_p = f_cdf[m].p;
                                int f_cdf_f = f_cdf[m].f;
                                int f_cdf_cl = f_cdf[m].cl;
                                int f_cdf_br = f_cdf[m].br;
                                int f_cdf_i = f_cdf[m].i;
                                int f_cdf_s = f_cdf[m].s;
                                int f_cdf_si = f_cdf[m].si;
                                int f_cdf_b = f_cdf[m].b;
                                int f_cdf_se = f_cdf[m].se;
                                int f_cdf_na = f_cdf[m].na;
                                int f_cdf_k = f_cdf[m].k;

                                int nl_cdf_c = nl_cdf[n].c;
                                int nl_cdf_h = nl_cdf[n].h;
                                int nl_cdf_n = nl_cdf[n].n;
                                int nl_cdf_o = nl_cdf[n].o;
                                int nl_cdf_p = nl_cdf[n].p;
                                int nl_cdf_f = nl_cdf[n].f;
                                int nl_cdf_cl = nl_cdf[n].cl;
                                int nl_cdf_br = nl_cdf[n].br;
                                int nl_cdf_i = nl_cdf[n].i;
                                int nl_cdf_s = nl_cdf[n].s;
                                int nl_cdf_si = nl_cdf[n].si;
                                int nl_cdf_b = nl_cdf[n].b;
                                int nl_cdf_se = nl_cdf[n].se;
                                int nl_cdf_na = nl_cdf[n].na;
                                int nl_cdf_k = nl_cdf[n].k;

                                IDictionary<string, int> v = new Dictionary<string, int>();
                                v.Add("C", f_cdf_c + nl_cdf_c); //adding a key/value using the Add() method
                                v.Add("H", f_cdf_h + nl_cdf_h);
                                v.Add("B", f_cdf_b + nl_cdf_b);
                                v.Add("Br", f_cdf_br + nl_cdf_br);
                                v.Add("Cl", f_cdf_cl + nl_cdf_cl);
                                v.Add("F", f_cdf_f + nl_cdf_f);
                                v.Add("I", f_cdf_i + nl_cdf_i);
                                v.Add("K", f_cdf_k + nl_cdf_k);
                                v.Add("N", f_cdf_n + nl_cdf_n);
                                v.Add("Na", f_cdf_na + nl_cdf_na);
                                v.Add("O", f_cdf_o + nl_cdf_o);
                                v.Add("P", f_cdf_p + nl_cdf_p);
                                v.Add("S", f_cdf_s + nl_cdf_s);
                                v.Add("Se", f_cdf_se + nl_cdf_se);
                                v.Add("Si", f_cdf_si + nl_cdf_si);
                                foreach (KeyValuePair<string, int> element in v)
                                {
                                    if (element.Value > 1)
                                    {
                                        formula = formula + element.Key + element.Value.ToString();
                                    }
                                    if (element.Value == 1)
                                    {
                                        formula = formula + element.Key;
                                    }
                                }

                                int tmpC = v.ElementAt(0).Value - adduct.SumFormula.c;
                                int tmpH = v.ElementAt(1).Value - adduct.SumFormula.h;
                                int tmpB = v.ElementAt(2).Value - adduct.SumFormula.b;
                                int tmpBr = v.ElementAt(3).Value - adduct.SumFormula.br;
                                int tmpCl = v.ElementAt(4).Value - adduct.SumFormula.cl;
                                int tmpF = v.ElementAt(5).Value - adduct.SumFormula.f;
                                int tmpI = v.ElementAt(6).Value - adduct.SumFormula.i;
                                int tmpK = v.ElementAt(7).Value - adduct.SumFormula.k;
                                int tmpN = v.ElementAt(8).Value - adduct.SumFormula.n;
                                int tmpNa = v.ElementAt(9).Value - adduct.SumFormula.na;
                                int tmpO = v.ElementAt(10).Value - adduct.SumFormula.o;
                                int tmpP = v.ElementAt(11).Value - adduct.SumFormula.p;
                                int tmpS = v.ElementAt(12).Value - adduct.SumFormula.s;
                                int tmpSe = v.ElementAt(13).Value - adduct.SumFormula.se;
                                int tmpSi = v.ElementAt(14).Value - adduct.SumFormula.si;


                                bool allIntForM = true;
                                double[] allTmps = new double[] { tmpC, tmpH, tmpB, tmpBr, tmpCl, tmpF, tmpI, tmpK, tmpN, tmpNa, tmpO, tmpP, tmpS, tmpSe, tmpSi };
                                if (adduct.M != 1)
                                {
                                    for (int tmp = 0; tmp < allTmps.Length; tmp++)
                                    {
                                        allTmps[tmp] = allTmps[tmp] / adduct.M;
                                        if(allTmps[tmp] % 1.0 != 0)
                                        {
                                            allIntForM = false;
                                            break;
                                        }
                                    }
                                    if (allIntForM == false)
                                    {
                                        continue;
                                    }
                                    tmpC  = (int)allTmps[0];
                                    tmpH  = (int)allTmps[1];
                                    tmpB  = (int)allTmps[2];
                                    tmpBr  = (int)allTmps[3];
                                    tmpCl  = (int)allTmps[4];
                                    tmpF  = (int)allTmps[5];
                                    tmpI  = (int)allTmps[6];
                                    tmpK  = (int)allTmps[7];
                                    tmpN  = (int)allTmps[8];
                                    tmpNa  = (int)allTmps[9];
                                    tmpO  = (int)allTmps[10];
                                    tmpP  = (int)allTmps[11];
                                    tmpS  = (int)allTmps[12];
                                    tmpSe  = (int)allTmps[13];
                                    tmpSi  = (int)allTmps[14];
                                    //Debug.WriteLine(tmpC.ToString() + " C<- ->H " + tmpH.ToString());
                                }


                                double mzError_f = 0.0;
                                double mzError_nl = 0.0;
                                if (ppm)
                                {
                                    mzError_f = 1e6 * (f_cdf[m].mass - mz[i]) / mz[i] / ms2tol;
                                    mzError_nl = 1e6 * (nl_cdf[n].mass - nl[i]) / nl[i] / ms2tol;
                                }
                                else
                                {
                                    mzError_f = (f_cdf[m].mass - mz[i]) / ms2tol;
                                    mzError_nl = (nl_cdf[n].mass - nl[i]) / ms2tol;
                                }
                                cfdf.Add(new CFDF(
                                    formula, // charged
                                    sum_mass,
                                    DBE_formula(new int[] { tmpC, tmpH, tmpN, tmpO, tmpP, tmpF, tmpCl, tmpBr, tmpI, tmpS, tmpSi, tmpB, tmpSe, tmpNa, tmpK }),
                                    mz_error, i, tmpC, tmpH, tmpN, tmpO, tmpP, tmpF, tmpCl, tmpBr,
                                    tmpI, tmpS, tmpSi, tmpB, tmpSe, tmpNa, tmpK,
                                    f_cdf[m].formstr_neutral,
                                    f_cdf[m].mass,
                                    DBE_formula(new int[] { f_cdf_c, f_cdf_h, f_cdf_n, f_cdf_o, f_cdf_p, f_cdf_f, f_cdf_cl,
                                        f_cdf_br, f_cdf_i, f_cdf_s, f_cdf_si, f_cdf_b, f_cdf_se, f_cdf_na, f_cdf_k}),
                                    mzError_f,
                                    H2C_formula(new int[] { f_cdf_c, f_cdf_h }),
                                    Hetero2C_formula(new int[] { f_cdf_c, f_cdf_h, f_cdf_n, f_cdf_o, f_cdf_p, f_cdf_f, f_cdf_cl,
                                        f_cdf_br, f_cdf_i, f_cdf_s, f_cdf_si, f_cdf_b, f_cdf_se, f_cdf_na, f_cdf_k}),
                                    f_cdf_c, f_cdf_h, f_cdf_n, f_cdf_o, f_cdf_p, f_cdf_f, f_cdf_cl, f_cdf_br, f_cdf_i,
                                    f_cdf_s, f_cdf_si, f_cdf_b, f_cdf_se, f_cdf_na, f_cdf_k,
                                    nl_cdf[n].formstr_neutral,
                                    nl_cdf[n].mass,
                                    DBE_formula(new int[] { nl_cdf_c, nl_cdf_h, nl_cdf_n, nl_cdf_o, nl_cdf_p, nl_cdf_f, nl_cdf_cl,
                                        nl_cdf_br, nl_cdf_i, nl_cdf_s, nl_cdf_si, nl_cdf_b, nl_cdf_se, nl_cdf_na, nl_cdf_k}),
                                    mzError_nl,
                                    H2C_formula(new int[] { nl_cdf_c, nl_cdf_h }),
                                    Hetero2C_formula(new int[] { nl_cdf_c, nl_cdf_h, nl_cdf_n, nl_cdf_o, nl_cdf_p, nl_cdf_f, nl_cdf_cl,
                                        nl_cdf_br, nl_cdf_i, nl_cdf_s, nl_cdf_si, nl_cdf_b, nl_cdf_se, nl_cdf_na, nl_cdf_k}),
                                    nl_cdf_c, nl_cdf_h, nl_cdf_n, nl_cdf_o, nl_cdf_p, nl_cdf_f, nl_cdf_cl, nl_cdf_br,
                                    nl_cdf_i, nl_cdf_s, nl_cdf_si, nl_cdf_b, nl_cdf_se, nl_cdf_na, nl_cdf_k,
                                    f_cdf[m].LogFreq, nl_cdf[n].LogFreq
                                ));
                            }
                        }

                        //within cfdf, deal with cases that one candidate formula can explain this fragment&NL in multiple ways
                        // unique
                        List<CFDF> cfdf_uni = cfdf.GroupBy(o => new { o.charged_formula }).Select(o => o.FirstOrDefault()).ToList();

                        // multiple-explanation formulas
                        List<List<CFDF>> a = cfdf.GroupBy(o => o.charged_formula).Where(grp => grp.Count() > 1).Select(grp => grp.ToList()).ToList();

                        for (int p = 0; p < a.Count; p++)
                        {
                            List<CFDF> df_a = a[p];

                            CFDF uni_item = cfdf_uni.Where(o => o.charged_formula == df_a[0].charged_formula).First();
                            int uni_index = cfdf_uni.IndexOf(uni_item);
                            if (uni_index != -1)
                            {
                                cfdf_uni[uni_index] = df_a.OrderByDescending(o => o.f_logFreq + o.l_logFreq).First();
                                //cfdf_uni[uni_index] = df_a.OrderByDescending(o => Math.Pow(10,o.f_logFreq) + Math.Pow(10, o.l_logFreq)).First();

                                //if (highResMS)
                                //{
                                //    cfdf_uni[uni_index] = df_a.OrderByDescending(o => o.f_logFreq + o.l_logFreq).First();
                                //}
                                //else
                                //{
                                //    if (ppm)
                                //    {
                                //        cfdf_uni[uni_index] = df_a.OrderByDescending(o => o.f_logFreq + o.l_logFreq - Math.Abs(o.mass_error) / (ms1tol * ms2.Mz * 1e-6)).First();
                                //    }
                                //    else
                                //    {
                                //        cfdf_uni[uni_index] = df_a.OrderByDescending(o => o.f_logFreq + o.l_logFreq - Math.Abs(o.mass_error) / ms1tol).First();
                                //    }
                                //}
                            }

                        }
                        lock (cfdf_all) cfdf_all.AddRange(cfdf_uni);
                    });
                }
                catch (OperationCanceledException)
                {
                    // Debug.WriteLine("Timeout1!");
                    return new List<Feature>();
                }

                if (cfdf_all.Count == 0)
                {
                    int targetInd = (int)((ms2.Mz * adduct.AbsCharge - adduct.SumFormula.mass) /adduct.M / GROUP_MZ_RANGE);
                    if (targetInd <= 0 || targetInd >= neutralDB.Count - 1)
                    {
                        return new List<Feature>();
                    }
                    List<AlignedFormula> precursor_db = new List<AlignedFormula>();
                    precursor_db.AddRange(neutralDB[targetInd - 1]);
                    precursor_db.AddRange(neutralDB[targetInd]);
                    precursor_db.AddRange(neutralDB[targetInd + 1]);

                    if (!highResMS)
                    {
                        precursor_db = precursor_db.Where(o =>
                                o.si == 0 &&
                                o.b == 0 &&
                                o.se == 0 &&
                                o.f ==0 &&
                                o.cl == 0 &&
                                o.br == 0 &&
                                o.i == 0).ToList();
                        //element count restriction
                        precursor_db = precursor_db.Where(o => o.c >= formRestriction.C_min &&
                                    o.h >= formRestriction.H_min &&
                                    o.n >= formRestriction.N_min &&
                                    o.o >= formRestriction.O_min &&
                                    o.p >= formRestriction.P_min &&
                                    o.s >= formRestriction.S_min &&
                                    o.c <= formRestriction.C_max &&
                                    o.h <= formRestriction.H_max &&
                                    o.n <= formRestriction.N_max &&
                                    o.o <= formRestriction.O_max &&
                                    o.p <= formRestriction.P_max &&
                                    o.s <= formRestriction.S_max).ToList();
                    }
                    else
                    {
                        //element count restriction
                        precursor_db = precursor_db.Where(o => o.c >= formRestriction.C_min &&
                                    o.h >= formRestriction.H_min &&
                                    o.n >= formRestriction.N_min &&
                                    o.o >= formRestriction.O_min &&
                                    o.p >= formRestriction.P_min &&
                                    o.s >= formRestriction.S_min &&
                                    o.f >= formRestriction.F_min &&
                                    o.cl >= formRestriction.Cl_min &&
                                    o.br >= formRestriction.Br_min &&
                                    o.i >= formRestriction.I_min &&
                                    o.si >= formRestriction.Si_min &&
                                    o.b >= formRestriction.B_min &&
                                    o.se >= formRestriction.Se_min &&
                                    o.c <= formRestriction.C_max &&
                                    o.h <= formRestriction.H_max &&
                                    o.n <= formRestriction.N_max &&
                                    o.o <= formRestriction.O_max &&
                                    o.p <= formRestriction.P_max &&
                                    o.s <= formRestriction.S_max &&
                                    o.f <= formRestriction.F_max &&
                                    o.cl <= formRestriction.Cl_max &&
                                    o.br <= formRestriction.Br_max &&
                                    o.i <= formRestriction.I_max &&
                                    o.si <= formRestriction.Si_max &&
                                    o.b <= formRestriction.B_max &&
                                    o.se <= formRestriction.Se_max).ToList();
                    }

                    if (ppm == true)
                    {
                        ms1tol = ms1tol * ms2.Mz * 1e-6;
                    }
                    List<AlignedFormula> candidates = new List<AlignedFormula>(precursor_db.Where(o => Math.Abs(o.mass * adduct.M - (ms2.Mz - adduct.SumFormula.mass)) <= ms1tol));


                    int FTpolarity;
                    if (ms2.Polarity == "P")
                    {
                        FTpolarity = 0;
                    }
                    else
                    {
                        FTpolarity = 1;
                    }

                    candidates = candidates.Where(o => o.c * adduct.M >= -adduct.SumFormulaNeg.c &&
                             o.h * adduct.M >= -adduct.SumFormulaNeg.h &&
                             o.b * adduct.M >= -adduct.SumFormulaNeg.b &&
                             o.br * adduct.M >= -adduct.SumFormulaNeg.br &&
                             o.cl * adduct.M >= -adduct.SumFormulaNeg.cl &&
                             o.f * adduct.M >= -adduct.SumFormulaNeg.f &&
                             o.i * adduct.M >= -adduct.SumFormulaNeg.i &&
                             o.k * adduct.M >= -adduct.SumFormulaNeg.k &&
                             o.n * adduct.M >= -adduct.SumFormulaNeg.n &&
                             o.na * adduct.M >= -adduct.SumFormulaNeg.na &&
                             o.o * adduct.M >= -adduct.SumFormulaNeg.o &&
                             o.p * adduct.M >= -adduct.SumFormulaNeg.p &&
                             o.s * adduct.M >= -adduct.SumFormulaNeg.s &&
                             o.se * adduct.M >= -adduct.SumFormulaNeg.se &&
                             o.si * adduct.M >= -adduct.SumFormulaNeg.si).ToList();

                    //change ms2 precursor formula to neutral form
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        string new_formula = "";
                        IDictionary<string, int> v = new Dictionary<string, int>();
                        v.Add("C", candidates[i].c * adduct.M + adduct.SumFormula.c);
                        v.Add("H", candidates[i].h * adduct.M + adduct.SumFormula.h);
                        v.Add("B", candidates[i].b * adduct.M + adduct.SumFormula.b);
                        v.Add("Br", candidates[i].br * adduct.M + adduct.SumFormula.br);
                        v.Add("Cl", candidates[i].cl * adduct.M + adduct.SumFormula.cl);
                        v.Add("F", candidates[i].f * adduct.M + adduct.SumFormula.f);
                        v.Add("I", candidates[i].i * adduct.M + adduct.SumFormula.i);
                        v.Add("K", candidates[i].k * adduct.M + adduct.SumFormula.k);
                        v.Add("N", candidates[i].n * adduct.M + adduct.SumFormula.n);
                        v.Add("Na", candidates[i].na * adduct.M + adduct.SumFormula.na);
                        v.Add("O", candidates[i].o * adduct.M + adduct.SumFormula.o);
                        v.Add("P", candidates[i].p * adduct.M + adduct.SumFormula.p);
                        v.Add("S", candidates[i].s * adduct.M + adduct.SumFormula.s);
                        v.Add("Se", candidates[i].se * adduct.M + adduct.SumFormula.se);
                        v.Add("Si", candidates[i].si * adduct.M + adduct.SumFormula.si);
                        foreach (KeyValuePair<string, int> element in v)
                        {
                            if (element.Value > 1)
                            {
                                new_formula = new_formula + element.Key + element.Value.ToString();
                            }
                            if (element.Value == 1)
                            {
                                new_formula = new_formula + element.Key;
                            }
                        }
                        candidates[i].formstr_charge = new_formula;
                    }

                    //more than topCandidateNum valid candidates then sort by expfragint and filter
                    if (!topcandcut)
                    {
                        if (candidates.Count > topCandidateNum)
                        {
                            candidates = candidates.OrderBy(o => Math.Abs(o.mass * adduct.M - ms2.Mz + adduct.SumFormula.mass)).Take(topCandidateNum).ToList();
                        }
                    }

                    // Feature data matrix
                    ///Parallel.For(0, candidates.Count, i =>
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        double ispsim = 0;
                        IsotopePattern iPattern = null;
                        if (ms1 != null && ms1.Count > 0)
                        {
                            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(candidates[i].formstr_charge);
                            IsotopePatternGenerator IPG = new IsotopePatternGenerator(isotopic_abundance_cutoff / 100);
                            iPattern = IPG.GetIsotopes(mf);
                            //iPattern.Charge = ION_MODE_FACTOR;
                            ispsim = IsotopeSimilarity(ms1, iPattern, max_isotopic_peaks, ms1tol, isp_grp_mass_tol, false, isotopic_abundance_cutoff, ION_MODE_FACTOR);
                        }

                        List<CFDF> cfdf = new List<CFDF>();
                        cfdf.Add(new CFDF(-1, candidates[i].c, candidates[i].h, candidates[i].n, candidates[i].o, candidates[i].p, candidates[i].f, candidates[i].cl, candidates[i].br, candidates[i].i,
                                candidates[i].s, candidates[i].si, candidates[i].b, candidates[i].se, candidates[i].na, candidates[i].k));


                        double totalAtomNo = (double)(candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o + candidates[i].p + candidates[i].f + candidates[i].cl +
                                candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k);
                        int cho;
                        if (candidates[i].n + candidates[i].p + candidates[i].f + candidates[i].cl +
                                candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                        {
                            cho = 1;
                        }
                        else
                        {
                            cho = 0;
                        }
                        int chno;
                        if (candidates[i].p + candidates[i].f + candidates[i].cl +
                                candidates[i].br + candidates[i].i + candidates[i].s + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                        {
                            chno = 1;
                        }
                        else
                        {
                            chno = 0;
                        }
                        int chnops;
                        if (candidates[i].f + candidates[i].cl + candidates[i].br + candidates[i].i + candidates[i].si + candidates[i].b + candidates[i].se + candidates[i].na + candidates[i].k == 0)
                        {
                            chnops = 1;
                        }
                        else
                        {
                            chnops = 0;
                        }
                        double preMzErrorInDM;
                        //preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - ms2.Mz + adduct.SumFormula.mass) / ms1tol;
                        if (highResMS)
                        {
                            preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - ms2.Mz + adduct.SumFormula.mass) / ms1tol;
                        }
                        else
                        {
                            preMzErrorInDM = Math.Abs(candidates[i].mass * adduct.M - ms2.Mz + adduct.SumFormula.mass);
                        }
                        lock (dm) dm.Add(new Feature(
                            ms2index,
                            candidates[i].formstr_neutral,
                            candidates[i].dbe,
                            H2C_formula(new int[] { candidates[i].c, candidates[i].h }),
                            Hetero2C_formula(new int[] { candidates[i].c, candidates[i].h, candidates[i].n, candidates[i].o, candidates[i].p, candidates[i].f, candidates[i].cl,
                            candidates[i].br, candidates[i].i, candidates[i].s, candidates[i].si, candidates[i].b, candidates[i].se, candidates[i].na, candidates[i].k }),
                            preMzErrorInDM,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            FTpolarity, 0, 0, ms1tol, 0, 0, 0, 0, 0, 0, 0, 0, 0, ispsim, iPattern, 0, cfdf, 0, 0, (candidates[i].f + candidates[i].cl + candidates[i].br + candidates[i].i) / totalAtomNo,
                            candidates[i].c / totalAtomNo, (candidates[i].c + candidates[i].h + candidates[i].o) / totalAtomNo, (candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o) / totalAtomNo,
                            (candidates[i].c + candidates[i].h + candidates[i].n + candidates[i].o + candidates[i].p + candidates[i].s) / totalAtomNo,
                            cho, chno, chnops, pre_psn_dist(totalAtomNo / candidates[i].mass, pre_dist_df, 3), 1
                            ));
                    //});
                    }
                    dm.RemoveAll(item => item == null);
                }
                else
                {
                    cfdf_all = cfdf_all.Where(o => o.dbe >= 0).ToList();

                    // unique candidate formula data frame
                    List<CFDF> df = cfdf_all.GroupBy(o => new { o.charged_formula }).Select(o => o.FirstOrDefault()).ToList();

                    // SENIOR Rules
                    df = df.Where(o => Senior(o.c, o.h, o.n, o.o, o.p, o.f, o.cl, o.br, o.i, o.s, o.si, o.b, o.se, o.na, o.k) == true).ToList();
                    //cfdf_all = (List<CFDF>)cfdf_all.OrderBy(o => o.c).ThenBy(o => o.h).ToList();

                    if (ppm == true)
                    {
                        ms1tol = ms1tol * ms2.Mz * 1e-6;
                    }
                    int FTpolarity;
                    if (ms2.Polarity == "P")
                    {
                        FTpolarity = 0;
                    }
                    else
                    {
                        FTpolarity = 1;
                    }

                    // rewrite the formula string
                    for (int i = 0; i < df.Count; i++)
                    {
                        string new_formula = "";
                        IDictionary<string, int> v = new Dictionary<string, int>();
                        v.Add("C", df[i].c); //adding a key/value using the Add() method
                        v.Add("H", df[i].h);
                        v.Add("B", df[i].b);
                        v.Add("Br", df[i].br);
                        v.Add("Cl", df[i].cl);
                        v.Add("F", df[i].f);
                        v.Add("I", df[i].i);
                        v.Add("K", df[i].k);
                        v.Add("N", df[i].n);
                        v.Add("Na", df[i].na);
                        v.Add("O", df[i].o);
                        v.Add("P", df[i].p);
                        v.Add("S", df[i].s);
                        v.Add("Se", df[i].se);
                        v.Add("Si", df[i].si);
                        foreach (KeyValuePair<string, int> element in v)
                        {
                            if (element.Value > 1)
                            {
                                new_formula = new_formula + element.Key + element.Value.ToString();
                            }
                            if (element.Value == 1)
                            {
                                new_formula = new_formula + element.Key;
                            }
                        }
                        df[i].neutral_formula = new_formula;
                        //Debug.WriteLine("line 1233 (df neutral form) " + df[i].neutral_formula);
                    }

                    df = df.Where(o => o.c * adduct.M >= -adduct.SumFormulaNeg.c &&
                        o.h * adduct.M >= -adduct.SumFormulaNeg.h &&
                        o.b * adduct.M >= -adduct.SumFormulaNeg.b &&
                        o.br * adduct.M >= -adduct.SumFormulaNeg.br &&
                        o.cl * adduct.M >= -adduct.SumFormulaNeg.cl &&
                        o.f * adduct.M >= -adduct.SumFormulaNeg.f &&
                        o.i * adduct.M >= -adduct.SumFormulaNeg.i &&
                        o.k * adduct.M >= -adduct.SumFormulaNeg.k &&
                        o.n * adduct.M >= -adduct.SumFormulaNeg.n &&
                        o.na * adduct.M >= -adduct.SumFormulaNeg.na &&
                        o.o * adduct.M >= -adduct.SumFormulaNeg.o &&
                        o.p * adduct.M >= -adduct.SumFormulaNeg.p &&
                        o.s * adduct.M >= -adduct.SumFormulaNeg.s &&
                        o.se * adduct.M >= -adduct.SumFormulaNeg.se &&
                        o.si * adduct.M >= -adduct.SumFormulaNeg.si).ToList();

              
                    //only calculated expfragno and expfragint to filter
                    List<ValidFeature> validCandidates = new List<ValidFeature>();

                    Parallel.For(0, df.Count,
                        new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                        i =>
                    {
                        List<CFDF> sub_df = cfdf_all.Where(o => o.charged_formula == df[i].charged_formula).ToList();

                        int[] expfragindex = sub_df.Select(x => x.expfragindex).ToArray();
                        double[] int_expf = new double[sub_df.Count];
                        for (int j = 0; j < sub_df.Count; j++)
                        {
                            int_expf[j] = intensity[expfragindex[j]];
                        }

                        double expfIntRatio = int_expf.Sum();
                        double expfNoRatio = (double)sub_df.Count / ms2.Spectrum.Count;

                        lock (validCandidates) validCandidates.Add(new ValidFeature(i, df[i].charged_formula, expfNoRatio, expfIntRatio, 0, df[i].mass_error));
                    });


                    //more than 100 valid candidates then sort by expfragint and filter
                    if (!topcandcut)
                    {
                        if (validCandidates.Count > topCandidateNum)
                        {
                            validCandidates = validCandidates.OrderByDescending(o => o.expfIntRatio).ThenBy(o => Math.Abs(o.p_mz_error)).Take(topCandidateNum).ToList();
                        }
                    }

                    ////debug:
                    //Debug.WriteLine("line 1301");
                    //validCandidates = validCandidates.Where(o => o.formula == "C46H81ClNO6").ToList();


                    Parallel.For(0, validCandidates.Count, i =>
                    //for (int i = 0; i < validCandidates.Count; i++)
                    {
                        #region

                        int df_ind = validCandidates[i].ind;
                        List<CFDF> sub_df = cfdf_all.Where(o => o.charged_formula == validCandidates[i].formula).ToList();

                        int[] expfragindex = sub_df.Select(x => x.expfragindex).ToArray();
                        double[] int_expf = new double[sub_df.Count];
                        for (int j = 0; j < sub_df.Count; j++)
                        {
                            int_expf[j] = intensity[expfragindex[j]];
                        }
                        // 9 features
                        double[] walpFragDBE = new double[sub_df.Count];
                        double[] fDBE = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            fDBE[w] = sub_df[w].dbe_f;
                            if (sub_df[w].dbe_f % 1.0 == 0.0)
                            {
                                walpFragDBE[w] = psn_dist(sub_df[w].dbe_f, distDF, 0);
                            }
                            else
                            {
                                walpFragDBE[w] = psn_dist(sub_df[w].dbe_f, distDF, 1);
                            }
                        }
                        double fDBEsd = 0.0;
                        if (fDBE.Length > 1)
                        {
                            fDBEsd = fDBE.StandardDeviation(true); //
                        }

                        double[] walpLossDBE = new double[sub_df.Count];
                        double[] lDBE = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            lDBE[w] = sub_df[w].dbe_l;
                            if (sub_df[w].dbe_l % 1.0 == 0.0)
                            {
                                walpLossDBE[w] = psn_dist(sub_df[w].dbe_l, distDF, 2);
                            }
                            else
                            {
                                walpLossDBE[w] = psn_dist(sub_df[w].dbe_l, distDF, 3);
                            }
                        }
                        double lDBEsd = 0.0;
                        if (lDBE.Length > 1)
                        {
                            lDBEsd = lDBE.StandardDeviation(true);
                        }

                        double[] walpFragLossDBE = new double[sub_df.Count];
                        double[] flDBE = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            flDBE[w] = sub_df[w].dbe_f - sub_df[w].dbe_l;
                            walpFragLossDBE[w] = psn_dist(sub_df[w].dbe_f - sub_df[w].dbe_l, distDF, 4);
                        }
                        double flDBEsd = 0.0;
                        if (flDBE.Length > 1)
                        {
                            flDBEsd = flDBE.StandardDeviation(true);
                        }

                        double[] walpFragH2C = new double[sub_df.Count];
                        double[] fH2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            fH2C[w] = sub_df[w].h2c_f;
                            if (sub_df[w].dbe_f % 1.0 == 0.0)
                            {
                                walpFragH2C[w] = psn_dist(sub_df[w].h2c_f, distDF, 5);
                            }
                            else
                            {
                                walpFragH2C[w] = psn_dist(sub_df[w].h2c_f, distDF, 6);
                            }
                        }
                        fH2C = fH2C.Where(o => o < 888888).ToArray();
                        double fH2Csd = 0.0;
                        if (fH2C.Length > 1)
                        {
                            fH2Csd = fH2C.StandardDeviation(true);
                        }

                        double[] walpLossH2C = new double[sub_df.Count];
                        double[] lH2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            lH2C[w] = sub_df[w].h2c_l;
                            if (sub_df[w].dbe_l % 1.0 == 0.0)
                            {
                                walpLossH2C[w] = psn_dist(sub_df[w].h2c_l, distDF, 7);
                            }
                            else
                            {
                                walpLossH2C[w] = psn_dist(sub_df[w].h2c_l, distDF, 8);
                            }
                        }
                        lH2C = lH2C.Where(o => o < 888888).ToArray();
                        double lH2Csd = 0.0;
                        if (lH2C.Length > 1)
                        {
                            lH2Csd = lH2C.StandardDeviation(true);
                        }

                        double[] walpFragLossH2C = new double[sub_df.Count];
                        double[] flH2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            flH2C[w] = sub_df[w].h2c_f - sub_df[w].h2c_l;
                            walpFragLossH2C[w] = psn_dist(sub_df[w].h2c_f - sub_df[w].h2c_l, distDF, 9);
                        }
                        flH2C = flH2C.Where(o => o < 888888 && o > -888888).ToArray();
                        double flH2Csd = 0.0;
                        if (flH2C.Length > 1)
                        {
                            flH2Csd = flH2C.StandardDeviation(true);
                        }

                        double[] walpFragHetero2C = new double[sub_df.Count];
                        double[] fHetero2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            fHetero2C[w] = sub_df[w].hetero2c_f;
                            if (sub_df[w].dbe_f % 1.0 == 0.0)
                            {
                                walpFragHetero2C[w] = psn_dist(sub_df[w].hetero2c_f, distDF, 10);
                            }
                            else
                            {
                                walpFragHetero2C[w] = psn_dist(sub_df[w].hetero2c_f, distDF, 11);
                            }
                        }
                        fHetero2C = fHetero2C.Where(o => o < 888888).ToArray();
                        double fHetero2Csd = 0.0;
                        if (fHetero2C.Length > 1)
                        {
                            fHetero2Csd = fHetero2C.StandardDeviation(true);
                        }

                        double[] walpLossHetero2C = new double[sub_df.Count];
                        double[] lHetero2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            lHetero2C[w] = sub_df[w].hetero2c_l;
                            if (sub_df[w].dbe_l % 1.0 == 0.0)
                            {
                                walpLossHetero2C[w] = psn_dist(sub_df[w].hetero2c_l, distDF, 12);
                            }
                            else
                            {
                                walpLossHetero2C[w] = psn_dist(sub_df[w].hetero2c_l, distDF, 13);
                            }
                        }
                        lHetero2C = lHetero2C.Where(o => o < 888888).ToArray();
                        double lHetero2Csd = 0.0;
                        if (lHetero2C.Length > 1)
                        {
                            lHetero2Csd = lHetero2C.StandardDeviation(true);
                        }

                        double[] walpFragLossHetero2C = new double[sub_df.Count];
                        double[] flHetero2C = new double[sub_df.Count];
                        for (int w = 0; w < sub_df.Count; w++)
                        {
                            flHetero2C[w] = sub_df[w].hetero2c_f - sub_df[w].hetero2c_l;
                            walpFragLossHetero2C[w] = psn_dist(sub_df[w].hetero2c_f - sub_df[w].hetero2c_l, distDF, 14);
                        }

                        flHetero2C = flHetero2C.Where(o => o < 888888 && o > -888888).ToArray();

                        double flHetero2Csd = 0.0;
                        if (flHetero2C.Length > 1)
                        {
                            flHetero2Csd = flHetero2C.StandardDeviation(true);
                        }
                        #endregion

                        double ispsim = 0;
                        IsotopePattern iPattern = null;
                        if (ms1 != null && ms1.Count > 0)
                        {
                            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(df[df_ind].charged_formula);
                            IsotopePatternGenerator IPG = new IsotopePatternGenerator(isotopic_abundance_cutoff / 100);
                            iPattern = IPG.GetIsotopes(mf);
                            //iPattern.Charge = ION_MODE_FACTOR;
                            ispsim = IsotopeSimilarity(ms1, iPattern, max_isotopic_peaks, ms1tol, isp_grp_mass_tol, false, isotopic_abundance_cutoff, ION_MODE_FACTOR);
                        }

                        double dbeLP = pre_psn_dist(df[df_ind].dbe, pre_dist_df, 0);
                        double h2cLP = pre_psn_dist(H2C_formula(new int[] { df[df_ind].c, df[df_ind].h }), pre_dist_df, 1);
                        double hetero2cLP = pre_psn_dist(Hetero2C_formula(new int[] { df[df_ind].c, df[df_ind].h, df[df_ind].n, df[df_ind].o, df[df_ind].p, df[df_ind].f, df[df_ind].cl,
                                        df[df_ind].br, df[df_ind].i, df[df_ind].s, df[df_ind].si, df[df_ind].b, df[df_ind].se, df[df_ind].na, df[df_ind].k }), pre_dist_df, 2);
                        double wf_DBE = walp(walpFragDBE, int_expf);
                        double wl_DBE = walp(walpLossDBE, int_expf);
                        double wfl_DBE = walp(walpFragLossDBE, int_expf);
                        double wf_H2C = walp(walpFragH2C, int_expf);
                        double wl_H2C = walp(walpLossH2C, int_expf);
                        double wfl_H2C = walp(walpFragLossH2C, int_expf);
                        double wf_Hetero2C = walp(walpFragHetero2C, int_expf);
                        double wl_Hetero2C = walp(walpLossHetero2C, int_expf);
                        double wfl_Hetero2C = walp(walpFragLossHetero2C, int_expf);

                        double expfIntRatio = validCandidates[i].expfIntRatio;
                        //double waf_mzErrorRatio = sub_df.Select(x => Math.Abs(x.mass_error_f)).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;

                        double waf_mzErrorRatio;
                        //waf_mzErrorRatio = sub_df.Select(x => Math.Abs(x.mass_error_f)).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;
                        if (highResMS)
                        {
                            waf_mzErrorRatio = sub_df.Select(x => Math.Abs(x.mass_error_f)).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;
                        }
                        else
                        {
                            waf_mzErrorRatio = sub_df.Select(x => Math.Abs(x.mass_error_f) * ms2tol).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;
                        }

                        double waf_logFreq = sub_df.Select(x => x.f_logFreq).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;
                        double wal_logFreq = sub_df.Select(x => x.l_logFreq).ToArray().Zip(int_expf, (d1, d2) => d1 * d2).Sum() / expfIntRatio;

                        double p_mzErrorRatio;
                        //p_mzErrorRatio = Math.Abs(df[df_ind].mass_error) / ms1tol;
                        if (highResMS)
                        {
                            p_mzErrorRatio = Math.Abs(df[df_ind].mass_error) / ms1tol;
                        }
                        else
                        {
                            p_mzErrorRatio = Math.Abs(df[df_ind].mass_error);
                        }

                        double expfNoRatio = validCandidates[i].expfNoRatio;
                        double f_nonintDBENoRatio = (double)sub_df.Count(x => x.dbe_f % 1.0 != 0.0) / (double)sub_df.Count;

                        double totalAtomNo = (double)(df[df_ind].c + df[df_ind].h + df[df_ind].n + df[df_ind].o + df[df_ind].p + df[df_ind].f + df[df_ind].cl +
                                df[df_ind].br + df[df_ind].i + df[df_ind].s + df[df_ind].si + df[df_ind].b + df[df_ind].se + df[df_ind].na + df[df_ind].k);
                        int cho;
                        if (df[df_ind].n + df[df_ind].p + df[df_ind].f + df[df_ind].cl + df[df_ind].br + df[df_ind].i + df[df_ind].s + df[df_ind].si + df[df_ind].b
                                + df[df_ind].se + df[df_ind].na + df[df_ind].k == 0)
                        {
                            cho = 1;
                        }
                        else
                        {
                            cho = 0;
                        }
                        int chno;
                        if (df[df_ind].p + df[df_ind].f + df[df_ind].cl + df[df_ind].br + df[df_ind].i + df[df_ind].s + df[df_ind].si + df[df_ind].b
                                + df[df_ind].se + df[df_ind].na + df[df_ind].k == 0)
                        {
                            chno = 1;
                        }
                        else
                        {
                            chno = 0;
                        }
                        int chnops;
                        if (df[df_ind].f + df[df_ind].cl + df[df_ind].br + df[df_ind].i + df[df_ind].si + df[df_ind].b + df[df_ind].se + df[df_ind].na + df[df_ind].k == 0)
                        {
                            chnops = 1;
                        }
                        else
                        {
                            chnops = 0;
                        }

                        double p_halogenAtomRatio = (df[df_ind].f + df[df_ind].cl + df[df_ind].br + df[df_ind].i) / totalAtomNo;
                        double p_cAtomRatio = df[df_ind].c / totalAtomNo;
                        double p_choAtomRatio = (df[df_ind].c + df[df_ind].h + df[df_ind].o) / totalAtomNo;
                        double p_chnoAtomRatio = (df[df_ind].c + df[df_ind].h + df[df_ind].n + df[df_ind].o) / totalAtomNo;
                        double p_chnopsAtomRatio = (df[df_ind].c + df[df_ind].h + df[df_ind].n + df[df_ind].o + df[df_ind].p + df[df_ind].s) / totalAtomNo;
                        double p_atomNo2Mass = pre_psn_dist(totalAtomNo / df[df_ind].exact_mass, pre_dist_df, 3);

                        //Debug.WriteLine("line 1580:  " + i.ToString());
                        // 
                        lock (dm) dm.Add(new Feature(
                                ms2index,
                                df[df_ind].neutral_formula,
                                dbeLP,
                                h2cLP,
                                hetero2cLP,
                                p_mzErrorRatio,
                                expfNoRatio,
                                expfIntRatio,
                                wf_DBE,
                                wl_DBE,
                                wfl_DBE,
                                wf_H2C,
                                wl_H2C,
                                wfl_H2C,
                                wf_Hetero2C,
                                wl_Hetero2C,
                                wfl_Hetero2C,
                                waf_mzErrorRatio,
                                FTpolarity,
                                ms2.Spectrum.Count,
                                f_nonintDBENoRatio,
                                ms1tol,
                                fDBEsd,
                                lDBEsd,
                                flDBEsd,
                                fH2Csd,
                                lH2Csd,
                                flH2Csd,
                                fHetero2Csd,
                                lHetero2Csd,
                                flHetero2Csd,
                                ispsim,
                                iPattern,
                                0,
                                sub_df,
                                waf_logFreq,
                                wal_logFreq,
                                p_halogenAtomRatio,
                                p_cAtomRatio,
                                p_choAtomRatio,
                                p_chnoAtomRatio,
                                p_chnopsAtomRatio,
                                cho,
                                chno,
                                chnops,
                                p_atomNo2Mass,
                                1
                                ));
                    //}
                    });
                    dm.RemoveAll(item => item == null);
                }

            }

            dm = dm.OrderByDescending(o => o.expfIntRatio).ThenBy(o => o.p_mzErrorRatio).ToList();

            return dm;
        }


        // senior rules
        public static bool Senior(int c, int h, int n, int o, int p, int f, int cl, int br, int i, int s, int si, int b, int se, int na, int k)
        {
            int imf = 0;
            int senior_1_1 = 6 * (s + se) + 5 * p + 4 * (c + si) + 3 * (n + b) + 2 * o + h + f + cl + br + i + na + k - imf;
            int senior_1_2 = p + n + b + h + f + cl + br + i + na + k - imf;
            // The sum of valences or the total number of atoms having odd valences is even
            if (senior_1_1 % 2 != 0 && senior_1_2 % 2 != 0)
            {
                return false;
            }
            else
            {
                int senior_2 = c + h + n + o + p + f + cl + br + i + s + si + b + se + na + k - imf;
                // The sum of valences is greater than or equal to twice the number of atoms minus 1
                if (senior_1_1 >= 2 * (senior_2 - 1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // deisotope
        public static List<RAW_PeakElement> ms2deisotope(List<RAW_PeakElement> x, double mztol, bool ppm)
        {
            double mz_tol;
            if ((ppm && mztol < 100) || (ppm==false && mztol < 0.1))
            {
                mz_tol = 2 * mztol;
            }
            else
            {
                mz_tol = 0.2;
                ppm = false;
            }

            if (x.Count == 1)
            {
                return x;
            }
            else
            {
                List<int> remove_v = new List<int>();
                for (int i = 0; i < (x.Count - 1); i++)
                {

                    double tmpMzDiff = mz_tol;
                    if (ppm && mztol < 100)
                    {
                        tmpMzDiff = mz_tol * x[i].Mz * 1e-6;
                    }
                    int index = 0;
                    bool match = false;

                    for (int j = (i + 1); j < x.Count; j++)
                    {
                        if (Math.Abs(x[j].Mz - x[i].Mz - 1.003355) <= tmpMzDiff && x[j].Intensity < x[i].Intensity)
                        {
                            match = true;
                            index = j;
                            tmpMzDiff = Math.Abs(x[j].Mz - x[i].Mz - 1.003355);
                        }
                    }
                    if (match)
                    {
                        remove_v.Add(index);
                    }
                }

                if (remove_v.Count > 0)
                {
                    List<RAW_PeakElement> output = new List<RAW_PeakElement>();
                    for (int i = 0; i < x.Count; i++)
                    {
                        if (remove_v.Contains(i) == false)
                        {
                            output.Add(x[i]);
                        }
                    }
                    return output;
                }
                else
                {
                    return x;
                }
            }
        }


        // noise elimination
        public static List<RAW_PeakElement> ms2denoise(List<RAW_PeakElement> x, double maxNoiseFragRatio, double maxNoiseRSD, double maxNoiseInt)
        {
            // noise elimination only performs when >= 10 fragments
            if (x.Count < 10)
            {
                return x;
            }
            else
            {
                List<double> sortedMS2Int = x.OrderBy(o => o.Intensity).Select(o => o.Intensity).ToList();

                // at least 3 frags are used to estimate RSD, step 0.05, round to 0.05 to determine m_start
                int m_start = (int)Math.Round(3.0 / x.Count * 20.0);
                int m_end = (int)Math.Floor(maxNoiseFragRatio / 0.05) + 1;
                if (m_end <= m_start) { return x; }

                double optim_m = 0.05;
                double FinalIntThreshold = 0;
                double userdefined_maxIntThreshold = Math.Min(maxNoiseInt,
                    sortedMS2Int[(int)Math.Round(x.Count * maxNoiseFragRatio) - 1]);
                for (int m = m_start; m < m_end; m++)
                {
                    if ((int)Math.Round(x.Count * m * 0.05) < 3)
                    {
                        continue;
                    }
                    List<double> sub_sortedMS2Int = new List<double>(sortedMS2Int.Take((int)Math.Round(x.Count * m * 0.05)));
                    double noiseMean = sub_sortedMS2Int.Average();
                    double noiseSD = Math.Sqrt(sub_sortedMS2Int.Select(o => (o - noiseMean) * (o - noiseMean)).Sum() / (sub_sortedMS2Int.Count - 1));
                    if ((noiseSD / noiseMean) <= maxNoiseRSD)
                    {
                        double tmpIntThreshold = noiseMean + noiseSD * 3.0;
                        if (tmpIntThreshold > userdefined_maxIntThreshold)
                        {
                            break;
                        }
                        else
                        {
                            FinalIntThreshold = tmpIntThreshold;
                            optim_m = m * 0.05;
                        }
                    }
                }

                List<RAW_PeakElement> denoisedMS2 = new List<RAW_PeakElement>(x.Where(o => o.Intensity > FinalIntThreshold));
                return denoisedMS2;
            }
        }


        // rank function
        public static double[] Rank(double[] arr)
        {
            // Rank Vector 
            int n = arr.Length;
            double[] R = new double[n];

            // Sweep through all elements 
            // in A  for each element count 
            // the number  of less than and 
            // equal elements separately in 
            // r and s 
            for (int i = 0; i < n; i++)
            {
                int r = 1, s = 1;

                for (int j = 0; j < n; j++)
                {
                    if (j != i && arr[j] < arr[i])
                        r += 1;

                    if (j != i && arr[j] == arr[i])
                        s += 1;
                }

                // Use formula to obtain rank 
                R[i] = r + (double)(s - 1) / 2.0;

            }
            return R;
        }

        // precursor skew normal distribution
        static double pre_psn_dist(double v, List<Distribution> distDF, int m)
        {
            double xi = double.Parse(distDF[m].xi, CultureInfo.InvariantCulture.NumberFormat);
            double omega = double.Parse(distDF[m].omega, CultureInfo.InvariantCulture.NumberFormat);
            double alpha = double.Parse(distDF[m].alpha, CultureInfo.InvariantCulture.NumberFormat);
            var skewNormal = new SkewNormalDistribution(location: xi, scale: omega, shape: alpha);
            double cdf = skewNormal.DistributionFunction(x: v);
            if (cdf > 0.5)
            {
                cdf = 2.0 - (2.0 * cdf);
            }
            else
            {
                cdf = 2.0 * cdf;
            }
            if (Math.Log10(cdf) > -2.0)
            {
                cdf = Math.Log10(cdf);
            }
            else
            {
                cdf = -2.0;
            }
            return cdf;
        }

        // skew normal distribution
        static double psn_dist(double v, List<Distribution> distDF, int m)
        {
            double xi = double.Parse(distDF[m].xi, CultureInfo.InvariantCulture.NumberFormat);
            double omega = double.Parse(distDF[m].omega, CultureInfo.InvariantCulture.NumberFormat);
            double alpha = double.Parse(distDF[m].alpha, CultureInfo.InvariantCulture.NumberFormat);
            var skewNormal = new SkewNormalDistribution(location: xi, scale: omega, shape: alpha);
            double cdf = skewNormal.DistributionFunction(x: v);
            return cdf;
        }

        // weighted average logP, if logp < -2, logp = -2
        static double walp(double[] p, double[] ms2int)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] > 0.5)
                {
                    p[i] = 2.0 - (2.0 * p[i]);
                }
                else
                {
                    p[i] = 2.0 * p[i];
                }
                if (Math.Log10(p[i]) > -2.0)
                {
                    p[i] = Math.Log10(p[i]);
                }
                else
                {
                    p[i] = -2.0;
                }
                p[i] = p[i] * ms2int[i];
            }
            double result = p.Sum() / ms2int.Sum();
            return result;
        }

        // input formula vector
        static double DBE_formula(int[] v)
        {
            double c = (double)v[0];
            double h = (double)v[1];
            double n = (double)v[2];
            double o = (double)v[3];
            double p = (double)v[4];
            double f = (double)v[5];
            double cl = (double)v[6];
            double br = (double)v[7];
            double i = (double)v[8];
            double s = (double)v[9];
            double si = (double)v[10];
            double b = (double)v[11];
            double se = (double)v[12];
            double na = (double)v[13];
            double k = (double)v[14];
            double result = c + si + 1.0 - (h + f + cl + br + i + na + k) / 2.0 + (n + p + b) / 2.0;
            return result;
        }

        // input formula vector
        static double H2C_formula(int[] v)
        {
            if ((double)v[0] == 0.0)
            {
                return 888888;
            }
            return (double)v[1] / (double)v[0];
        }

        // input formula vector
        static double Hetero2C_formula(int[] v)
        {
            int c = v[0];
            //int h = v[1];
            int n = v[2];
            int o = v[3];
            int p = v[4];
            int f = v[5];
            int cl = v[6];
            int br = v[7];
            int i = v[8];
            int s = v[9];
            int si = v[10];
            int b = v[11];
            int se = v[12];
            int na = v[13];
            int k = v[14];
            int number = n + o + p + f + cl + br + i + s + si + b + se + na + k;
            if (c == 0)
            {
                return 888888;
            }
            return (double) number / c;
        }

        // isotope similarity
        // iPattern: emass not considered
        private static double IsotopeSimilarity(List<RAW_PeakElement> isox, IsotopePattern iPattern, int isoNo, double mzTol,
            double isotopeBinMzTol, bool ppm, double IsotopeCutoff, int ION_MODE_FACTOR)
        {
            if (mzTol <= 0.2)
            {
                mzTol *= 2;
            }
            mzTol = Math.Max(mzTol, 0.05); // sometimes feature Mz differ from MS1 mz in isotopes

            //Isox: exp. ms1 (grouped & normalized)
            //Isoy: Theoretical ms1 (to be grouped)
            if (iPattern.Isotopes.Count == 0)
            {
                return 0.0;
            }

            //double maxisoInt = isox.Max(o => o.Intensity);
            double isoxM1Int = isox[0].Intensity; // intensity normalized by M1
            List<RAW_PeakElement> normx = new List<RAW_PeakElement>();
            for (int i = 0; i < isox.Count; i++)
            {
                //normx.Add(new RAW_PeakElement { Mz = isox[i].Mz, Intensity = isox[i].Intensity / maxisoInt });
                normx.Add(new RAW_PeakElement { Mz = isox[i].Mz, Intensity = isox[i].Intensity / isoxM1Int });
            }
            isox = normx;


            List<RAW_PeakElement> isoy = new List<RAW_PeakElement>();

            double tmpMzDiff = isotopeBinMzTol;

            isoy.Add(new RAW_PeakElement() { Mz = iPattern.Isotopes[0].Mass - 0.0005485 * ION_MODE_FACTOR, Intensity = iPattern.Isotopes[0].Intensity });
            double M0Intensity = iPattern.Isotopes[0].Intensity;

            //binning & intensity normaliztion by M1
            double tmpMz = iPattern.Isotopes[0].Mass;
            for (int i = 1; i < iPattern.Isotopes.Count; i++)
            {
                bool M = false;

                List<double> MzList = new List<double>();
                List<double> IntList = new List<double>();


                for (int j = 1; j < iPattern.Isotopes.Count; j++)
                {
                    if (Math.Abs(iPattern.Isotopes[j].Mass - (tmpMz + 1.003355)) <= tmpMzDiff) // include all the ions within the mz range
                    {
                        MzList.Add(iPattern.Isotopes[j].Mass);
                        IntList.Add(iPattern.Isotopes[j].Intensity);
                        M = true;
                    }
                }
                if (M == false)
                {
                    break;
                }
                else
                {    // for all the ions within the mz range, calculate weighted average m/z, sum intensity
                    double weightedSumMz = 0;
                    double sumInt = 0;
                    for (int m = 0; m < MzList.Count; m++)
                    {
                        weightedSumMz += MzList[m] * IntList[m];
                        sumInt += IntList[m];
                    }
                    tmpMz = weightedSumMz / sumInt;

                    if ((100.0 * sumInt / M0Intensity) >= IsotopeCutoff)
                    {
                        isoy.Add(new RAW_PeakElement() { Mz = tmpMz, Intensity = sumInt / M0Intensity });
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            //int min = Math.Min(isox.Count, Math.Min(isoy.Count, isoNo));
            //isox = isox.Take(min).ToList();

            //double[] iso_align = new double[isox.Count];
            //for (int i = 0; i < isox.Count; i++)
            //{
            //    if (isoy.Select(o => Math.Abs(o.Mz - isox[i].Mz)).Min() <= mzTol)
            //    {
            //        iso_align[i] = isoy.Where(o => Math.Abs(o.Mz - isox[i].Mz) <= mzTol).ElementAt(0).Intensity;
            //    }
            //}
            //if (iso_align.Sum() == 0)
            //{
            //    return 0.0;
            //}
            //else
            //{
            //    ////normalization
            //    //double max_iso_align = iso_align.Max();
            //    //for (int j = 0; j < iso_align.Length; j++)
            //    //{
            //    //    iso_align[j] = iso_align[j] / max_iso_align;
            //    //}

            //    double score = 1.0;
            //    for (int j = 0; j < iso_align.Length; j++)
            //    {
            //        // in real cases: exp > theo: stop, theo > exp: do not stop. (exp >= theo)
            //        if ((isox[j].Intensity - iso_align[j]) >= 0.5)
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            score *= 1 - Math.Abs(isox[j].Intensity - iso_align[j]);
            //        }

            //        //double thisPeakPairScore = 1 - Math.Abs(isox[j].Intensity - iso_align[j]);
            //        //if(thisPeakPairScore >= 0.5)
            //        //{
            //        //    score *= thisPeakPairScore;
            //        //}
            //        //else
            //        //{
            //        //    break;
            //        //}

            //    }
            //    return score;
            //}


            int min = Math.Min(isoy.Count, isoNo);
            int isoXCount = isox.Count;
            if (isox.Count >= min)
            {
                isox = isox.Take(min).ToList();
            }
            else
            {
                for (int i = 0; i < (min - isoXCount); i++)
                {
                    isox.Add(new RAW_PeakElement { Mz = isox[isox.Count - 1].Mz + 1.003355, Intensity = 0 });
                }
            }

            double[] iso_align = new double[min];
            for (int i = 0; i < min; i++)
            {
                if (isoy.Min(o => Math.Abs(o.Mz - isox[i].Mz)) <= mzTol)
                {
                    iso_align[i] = isoy.Where(o => Math.Abs(o.Mz - isox[i].Mz) <= mzTol).ElementAt(0).Intensity;
                }
            }
            if (iso_align.Sum() == 0)
            {
                return 0.0;
            }
            else
            {
                ////normalization
                //double max_iso_align = iso_align.Max();
                //for (int j = 0; j < iso_align.Length; j++)
                //{
                //    iso_align[j] = iso_align[j] / max_iso_align;
                //}

                double score = 1.0;
                for (int j = 0; j < iso_align.Length; j++)
                {
                    // in real cases: exp > theo: stop, theo > exp: do not stop. (exp >= theo)
                    double thisPeakPairIntDiff = isox[j].Intensity - iso_align[j];
                    if (thisPeakPairIntDiff >= 0.5)
                    {
                        break;
                    }
                    else
                    {
                        if (Math.Abs(thisPeakPairIntDiff) <= 1)
                        {
                            score *= 1 - Math.Abs(thisPeakPairIntDiff);
                        }
                        else
                        {
                            score = 0;
                            break;
                        }
                    }

                    //double thisPeakPairScore = 1 - Math.Abs(isox[j].Intensity - iso_align[j]);
                    //if(thisPeakPairScore >= 0.5)
                    //{
                    //    score *= thisPeakPairScore;
                    //}
                    //else
                    //{
                    //    break;
                    //}

                }
                return score;
            }
        }
    }
}
