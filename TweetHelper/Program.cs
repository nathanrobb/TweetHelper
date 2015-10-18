namespace TweetHelper
{
	static class Program
	{
		// Hashtags to filter for each file group.
		private static readonly string[][] HashtagsPerFile =
		{
			new[] { "#apple", "#mlb" },
			new[] { "#fashon", "#food" }
		};

		// Input files - grouped by similar time period.
		private static readonly string[][] InFilePaths =
		{
			new[] { "stream__apple.csv", "stream__mlb.csv" },
			new[] { "stream__fashion.csv", "stream__food.csv" }
		};

		// No file extension (will always be .arff).
		private const string OutFilepath = "output";

		public static void Main(string[] args)
		{
			System.Console.WriteLine("Processing Tweets...");
			for (var i = 0; i < InFilePaths.Length; i++)
				new TweetProcessor(InFilePaths[i], OutFilepath).ProcessFilesToArff(HashtagsPerFile[i]);
		}
	}
}
