using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.BufpHelper
{
    [Serializable]
    [DelimitedRecord(",")]
    public class Distribution
    {
        public string feature { get; set; }
        public string xi { get; set; }
        public string omega { get; set; }
        public string alpha { get; set; }
        public string min { get; set; }
        public string max { get; set; }
    }

    public class DistributionManager
    {
        public static List<Distribution> GetDistribution(string path)
        {

            var engine = new FileHelperEngine<Distribution>();
            var records = engine.ReadFile(path);
            var dist_list = new List<Distribution>();

            foreach (var record in records)
            {
                dist_list.Add(new Distribution
                {
                    feature = record.feature,
                    xi = record.xi,
                    omega = record.omega,
                    alpha = record.alpha,
                    min = record.min,
                    max = record.max
                });
            }
            dist_list.RemoveAt(0);

            return dist_list;
        }

    }
}
