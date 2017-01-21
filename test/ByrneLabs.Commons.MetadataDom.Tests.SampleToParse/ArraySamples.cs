
#pragma warning disable 169 //We don't care about compiler warnings in the test project

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    internal class ArraySamples
    {
        private int[][][][] fourDimensionalArrayIntField;
        private ArraySamples[][][][] fourDimensionalArraySamplesField;
        private int[] oneDimensionalArrayIntField;
        private ArraySamples[] oneDimensionalArraySamplesField;
        private int[][][] threeDimensionalArrayIntField;
        private ArraySamples[][][] threeDimensionalArraySamplesField;
        private int[][] twoDimensionalArrayIntField;
        private ArraySamples[][] twoDimensionalArraySamplesField;

        private int[][][][] FourDimensionalArrayMethod(int zero, int[] one, int[][] two, int[][][] three, int[][][][] four) => null;

        private int[] OneDimensionalArrayMethod(int zero, int[] one, int[][] two, int[][][] three, int[][][][] four) => null;

        private int[][][] ThreeDimensionalArrayMethod(int zero, int[] one, int[][] two, int[][][] three, int[][][][] four) => null;

        private int[][][] TwoDimensionalArrayMethod(int zero, int[] one, int[][] two, int[][][] three, int[][][][] four) => null;
    }
}
