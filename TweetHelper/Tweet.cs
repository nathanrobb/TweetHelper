using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetHelper
{
	class Tweet
	{
		// Change fro what are valid hashtags.
		public static ISet<string> TestHashtags;

		// Change this to change the class
		public static string ThisHashtag;

		public const string Pattern = @"[^a-z0-9,+#\s:]+";

		// Change this for the Date format of tweets to import (Used in Date.ParseExact).
		public const string DatePattern = @"^[a-z][a-z][a-z]\s[a-z][a-z][a-z]\s[0-9][0-9]\s[0-9][0-9]:[0-9][0-9]:[0-9][0-9]\s\+0000\s201[0-9]$";
		public const string DateFormat = "ddd MMM dd H:mm:ss zzz yyyy";
		public static readonly int DateFieldLength = DateFormat.Length + 3;

		private static readonly char[] Split = { ' ' };

		private static readonly char[] Punc = { '+', ',', '#', ':' };

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

			var tweetWords = tweet.Split(Split, StringSplitOptions.RemoveEmptyEntries);
			var allWords = new List<string>();

			foreach (var word in tweetWords)
			{
				if (word.StartsWith("#"))
				{
					var hash = "#" + word.Trim(Punc);

					// If no hashtags in tweet.
					//if (ThisHashtag == hash)
					//	Hashtag = hash;
					//continue;

					// If only the topic of interest hashtag is removed.
					if (TestHashtags.Contains(hash))
					{
						Hashtag = hash;
						continue;
					}
					allWords.Add(hash);
					continue;
				}

				allWords.Add(word.Trim(Punc));
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
