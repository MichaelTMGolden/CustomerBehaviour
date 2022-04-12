
using Microsoft.ML.Data;

namespace CustomerBehaviour.Definitions
{
    public class LinearRegressionResults
    {
        [ColumnName("Score")]
        public float target { get; set; }
    }
}
