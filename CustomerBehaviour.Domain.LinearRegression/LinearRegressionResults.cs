using Microsoft.ML;
using System;

namespace CustomerBehaviour.Domain.LinearRegression
{
    public class LinearRegressionResults
    {
        public static void Evaluate(IDataView testSet, MLContext mlContext, ITransformer model)
        {

            var predictions = model.Transform(testSet);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics output         ");
            Console.WriteLine($"*------------------------------------------------");

            Console.WriteLine($"*       R Squared Score:      {metrics.RSquared:0.###}");

            Console.WriteLine($"*       Mean Absolute Error:      {metrics.MeanAbsoluteError:#.###}");

            Console.WriteLine($"*       Root-Mean-Squared Error:      {metrics.RootMeanSquaredError:#.###}");
        }
    }
}
