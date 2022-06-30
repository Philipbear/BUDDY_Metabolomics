using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.GlobalOptimizationMILP
{

    [DelimitedRecord(",")]
    public class Connection
    {
        public string ConnectionType { get; set; }
        public string FormulaChange { get; set; }
        public double MassDiff { get; set; }
        public string Description { get; set; }

    }

    public class ConnectionManager
    {
        public static List<Connection> GetConnection(string path)
        {

            var engine = new FileHelperEngine<Connection>();
            var records = engine.ReadFile(path);
            var list = new List<Connection>();

            foreach (var record in records)
            {
                list.Add(new Connection
                {
                    ConnectionType = record.ConnectionType,
                    FormulaChange = record.FormulaChange,
                    MassDiff = record.MassDiff,
                    Description = record.Description
                });
            }
            //list.RemoveAt(0);

            return list;
        }

    }
}
