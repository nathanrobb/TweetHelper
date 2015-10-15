using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetHelper
{
	class Tweet
	{
		private static readonly HashSet<string> TestHashtags = new HashSet<string>(Program.HashtagsPerFile.SelectMany(h => h));
		private static readonly char[] Split = { ' ' };
		private static readonly char[] PMap = "& !\",.?".ToCharArray();

		private const string Rt = "rt";

		// Change this to change the class
		public static string ThisHashtag;

		private DateTime Time { get; }
		private string[] Words { get; set; }
		private string Hashtag { get; set; }
		private int ClassValue => Convert.ToInt32(Hashtag == ThisHashtag);

		public Tweet(DateTime time, string tweet)
		{
			Time = time;
			ParseTweet(tweet);
		}

		private void ParseTweet(string tweet)
		{
			if (string.IsNullOrWhiteSpace(tweet))
				throw new Exception("No Tweet!");

			var words2 = tweet.Split(Split, StringSplitOptions.RemoveEmptyEntries);
			var allWords = new List<string>();

			foreach (var word in words2)
			{
				if (word.StartsWith("#"))
				{
					var hash = word.TrimEnd(PMap);
					if (TestHashtags.Contains(hash))
						Hashtag = hash;
					continue;
				}

				allWords.Add(word.Trim(PMap));
			}

			allWords.RemoveAll(string.IsNullOrWhiteSpace);


			Words = allWords.ToArray();
		}

		public bool IsValidTweet()
		{
			return Words != null && Words.Length != 0 && Words.FirstOrDefault() != Rt;
		}

		public override string ToString()
		{
			// No hashtag.
			//return $"\"{Time.ToUniversalTime()}\",\"{string.Join(" ", Words)}\",\"{ClassValue}\"";
			// With Hashtag.
			return $"\"{Time.ToUniversalTime()}\",\"{string.Join(" ", Words)} {Hashtag}\",\"{ClassValue}\"";
		}
	}
}
