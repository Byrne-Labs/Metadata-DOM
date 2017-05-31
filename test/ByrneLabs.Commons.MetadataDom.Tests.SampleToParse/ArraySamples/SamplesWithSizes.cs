namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.ArraySamples
{
    internal class SamplesWithSizes
    {
        private int[] array = new int[4];
        private int[][] arrayOfArray = new int[0][];
        private int[][][] arrayOfArrayOfArray = new int[8][][];
        private int[,,] threeDimensionalArray = new int[4, 0, 2];
        private int[,] twoDimensionalArray = new int[8, 16];
        private int[,][,] twoDimensionalArrayOfTwoDimensionalArray = new int[4, 0][,];
    }
}
