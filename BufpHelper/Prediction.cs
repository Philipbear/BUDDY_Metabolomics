//using Microsoft.ML.Data;
using System;
namespace BUDDY.BufpHelper
{
    // Class used to capture predictions.
    public class Prediction
    {
        // Original label.
        public string GroupIDorig { get; set; }
        public Single Label { get; set; }
        // Score produced from the trainer.
        public Single Score { get; set; }
    }
}
