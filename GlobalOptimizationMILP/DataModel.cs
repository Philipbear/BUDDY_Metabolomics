using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.GlobalOptimizationMILP
{
    public class DataModel
    {
        //public int[,] ConstraintCoeffs { get; set; }

        public List<int[]> ConstraintCoeffs { get; set; }
        public int[] LowerBounds { get; set; }
        public int[] UpperBounds { get; set; }
        public double[] ObjCoeffs { get; set; }
        public int NumVars { get; set; }
        public int NumConstraints { get; set; }
    }
}
