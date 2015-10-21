using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetHelper
{
	class Tweet
	{
		// Change this to change the class
		public static string ThisHashtag;

		// Change this for the Date format of tweets to import (Used in Date.ParseExact).
		public const string DateFormat = "ddd MMM dd H:mm:ss zzz yyyy";
		public static readonly int DateFieldLength = DateFormat.Length + 2;

		private static readonly char[] Split = { ' ' };
		private static readonly char[] PMap = "& !\",.?".ToCharArray();

		private const string Rt = "rt";

		private DateTime Time { get; }
		private List<string> Words { get; set; }
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

					// If no hashtags in tweet.
					//if (ThisHashtag == hash)
					//	Hashtag = hash;
					//continue;

					// If only the topic of interest hashtag is removed.
					if (ThisHashtag == hash)
					{
						Hashtag = hash;
						continue;
					}
					allWords.Add(word.TrimEnd(PMap));
					continue;
				}

				allWords.Add(word.Trim(PMap));
			}

			allWords.RemoveAll(string.IsNullOrWhiteSpace);


			Words = allWords;
		}

		public bool IsValidTweet()
		{
			return Words.Count != 0 && Words.FirstOrDefault() != Rt;
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
