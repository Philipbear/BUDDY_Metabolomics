using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.BufpHelper
{
    [Serializable]
    public class CFDF
    {
        public CFDF()
        {
        }

        public CFDF(double DBE_frag) {
            dbe_f = DBE_frag;
        }
        public CFDF(int ExplainedFragIndex, int C, int H, int N, int O, int P, int F, int Cl, int Br, int I, int S, int Si, int B, int Se, int Na, int K)
        {
            expfragindex = ExplainedFragIndex;
            c = C;
            h = H;
            n = N;
            o = O;
            p = P;
            f = F;
            cl = Cl;
            br = Br;
            i = I;
            s = S;
            si = Si;
            b = B;
            se = Se;
            na = Na;
            k = K;
        }

            public CFDF(string ChargedFormula, double Exact_mass, double DBE, double Mass_error, int ExplainedFragIndex, //1-5
     int C, int H, int N, int O, int P, int F, int Cl, int Br, int I, int S, int Si, int B, int Se, int Na, int K, //6-20
     string Formula_frag, double Exact_mass_frag, double DBE_frag, double Mass_error_frag,
     double H2C_frag, double Hetero2C_frag, //21-26
     int C_frag, int H_frag, int N_frag, int O_frag, int P_frag, int F_frag, int Cl_frag, int Br_frag, int I_frag,
     int S_frag, int Si_frag, int B_frag, int Se_frag, int Na_frag, int K_frag, //27-41
     string Formula_loss, double Exact_mass_loss, double DBE_loss, double Mass_error_loss,
     double H2C_loss, double Hetero2C_loss, //42-47
     int C_loss, int H_loss, int N_loss, int O_loss, int P_loss, int F_loss, int Cl_loss, int Br_loss, int I_loss,
     int S_loss, int Si_loss, int B_loss, int Se_loss, int Na_loss, int K_loss, double Frag_LogFreq, double Loss_LogFreq) //48-62
        {
            charged_formula = ChargedFormula;
            exact_mass = Exact_mass;
            dbe = DBE;
            mass_error = Mass_error;
            expfragindex = ExplainedFragIndex;
            c = C;
            h = H;
            n = N;
            o = O;
            p = P;
            f = F;
            cl = Cl;
            br = Br;
            i = I;
            s = S;
            si = Si;
            b = B;
            se = Se;
            na = Na;
            k = K;
            formula_f = Formula_frag;
            exact_mass_f = Exact_mass_frag;
            dbe_f = DBE_frag;
            mass_error_f = Mass_error_frag;
            h2c_f = H2C_frag;
            hetero2c_f = Hetero2C_frag;
            c_f = C_frag;
            h_f = H_frag;
            n_f = N_frag;
            o_f = O_frag;
            p_f = P_frag;
            f_f = F_frag;
            cl_f = Cl_frag;
            br_f = Br_frag;
            i_f = I_frag;
            s_f = S_frag;
            si_f = Si_frag;
            b_f = B_frag;
            se_f = Se_frag;
            na_f = Na_frag;
            k_f = K_frag;
            formula_l = Formula_loss;
            exact_mass_l = Exact_mass_loss;
            dbe_l = DBE_loss;
            mass_error_l = Mass_error_loss;
            h2c_l = H2C_loss;
            hetero2c_l = Hetero2C_loss;
            c_l = C_loss;
            h_l = H_loss;
            n_l = N_loss;
            o_l = O_loss;
            p_l = P_loss;
            f_l = F_loss;
            cl_l = Cl_loss;
            br_l = Br_loss;
            i_l = I_loss;
            s_l = S_loss;
            si_l = Si_loss;
            b_l = B_loss;
            se_l = Se_loss;
            na_l = Na_loss;
            k_l = K_loss;
            f_logFreq = Frag_LogFreq;
            l_logFreq = Loss_LogFreq;
        }

        public string charged_formula { get; set; }
        public string neutral_formula { get; set; }
        public double exact_mass { get; set; }
        public double dbe { get; set; }
        public double mass_error { get; set; }
        public int expfragindex { get; set; }
        public int c { get; set; }
        public int h { get; set; }
        public int n { get; set; }
        public int o { get; set; }
        public int p { get; set; }
        public int f { get; set; }
        public int cl { get; set; }
        public int br { get; set; }
        public int i { get; set; }
        public int s { get; set; }
        public int si { get; set; }
        public int b { get; set; }
        public int se { get; set; }
        public int na { get; set; }
        public int k { get; set; }
        public string formula_f { get; set; }
        public double exact_mass_f { get; set; }
        public double dbe_f { get; set; }
        public double mass_error_f { get; set; }
        public double h2c_f { get; set; }
        public double hetero2c_f { get; set; }
        public int c_f { get; set; }
        public int h_f { get; set; }
        public int n_f { get; set; }
        public int o_f { get; set; }
        public int p_f { get; set; }
        public int f_f { get; set; }
        public int cl_f { get; set; }
        public int br_f { get; set; }
        public int i_f { get; set; }
        public int s_f { get; set; }
        public int si_f { get; set; }
        public int b_f { get; set; }
        public int se_f { get; set; }
        public int na_f { get; set; }
        public int k_f { get; set; }
        public string formula_l { get; set; }
        public double exact_mass_l { get; set; }
        public double dbe_l { get; set; }
        public double mass_error_l { get; set; }
        public double h2c_l { get; set; }
        public double hetero2c_l { get; set; }
        public int c_l { get; set; }
        public int h_l { get; set; }
        public int n_l { get; set; }
        public int o_l { get; set; }
        public int p_l { get; set; }
        public int f_l { get; set; }
        public int cl_l { get; set; }
        public int br_l { get; set; }
        public int i_l { get; set; }
        public int s_l { get; set; }
        public int si_l { get; set; }
        public int b_l { get; set; }
        public int se_l { get; set; }
        public int na_l { get; set; }
        public int k_l { get; set; }
        public double f_logFreq { get; set; }
        public double l_logFreq { get; set; }
        public int frequency { get; set; }

    }
}
