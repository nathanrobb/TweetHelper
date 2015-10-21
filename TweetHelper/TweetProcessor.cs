using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TweetHelper
{
	class TweetProcessor
	{
		private static readonly Regex DateRegex = new Regex(Tweet.DatePattern);
		private static readonly Regex TweetRegex = new Regex(Tweet.Pattern);

		private readonly string[] _filesIn;
		private readonly string _fileOut;

		public TweetProcessor(string[] filesIn, string fileOut)
		{
			_filesIn = filesIn;
			_fileOut = fileOut;
		}

		public void ProcessFilesToArff(string[] hashtags)
		{
			Tweet.TestHashtags = new HashSet<string>(hashtags);
			var tweets = new SortedDictionary<DateTime, List<Tweet>>();

			foreach (var file in _filesIn)
				ParseFile(File.ReadLines(file), tweets);

			var orderedTweets = tweets.Values.SelectMany(r => r).ToArray();

			var fourfiths = (orderedTweets.Length * 4) / 5;

			// We want the class values W.R.T. each of the other hashtags and foreground / background data (so eight files).
			foreach (var hashtag in hashtags)
			{
				Tweet.ThisHashtag = hashtag;

				// Background data.
				var relationB = hashtag.Substring(1) + "_" + _fileOut + "_b";
				var path = relationB + ".arff";
				using (var tw = new StreamWriter(File.Create(path)))
				{
					PrintArffHeader(tw, relationB);
					orderedTweets.Take(fourfiths).ToList().ForEach(tw.WriteLine);
				}

				// Foreground data.
				var relationF = hashtag.Substring(1) + "_" + _fileOut + "_f";
				path = relationF + ".arff";
				using (var tw = new StreamWriter(File.Create(path)))
				{
					PrintArffHeader(tw, relationF);
					orderedTweets.Skip(fourfiths).ToList().ForEach(tw.WriteLine);
				}
			}
		}

		private static void PrintArffHeader(TextWriter tw, string relation)
		{
			tw.WriteLine("@relation " + relation);
			tw.WriteLine();
			tw.WriteLine("@attribute Date string");
			tw.WriteLine("@attribute Tweet string");
			tw.WriteLine("@attribute Class {0,1}");
			tw.WriteLine();
			tw.WriteLine("@data");
		}

		private static void ParseFile(IEnumerable<string> reader, IDictionary<DateTime, List<Tweet>> tweets)
		{
			var prevDate = string.Empty;
			var prevTweet = string.Empty;

			foreach (var line in reader.Where(l => !string.IsNullOrWhiteSpace(l)).Select(w => w.ToLower()))
			{
				var parsedLine = HttpUtility.HtmlDecode(line);              // Decode Web encodings (&quot;, etc.).
				parsedLine = TweetRegex.Replace(parsedLine, string.Empty)   // Strip bad chars and punctuation.
					.Replace("\"", string.Empty)                            // Remove Quote marks.
					.Replace("\\", "\\\\")                                  // Escape \.
					.Replace(Environment.NewLine, "");                      // Remove NewLine.

				// Format in: date, tweet.
				var splitIndex = parsedLine.IndexOf(',');                   // Find where the first date splits (tweet may have a comma in it).
				if (splitIndex != Tweet.DateFieldLength)                    // Else it's a multiline tweet.
					splitIndex = 0;

				if (splitIndex > 0)
				{   // If we have a date, this begins a tweet.
					var currDate = parsedLine.Substring(0, splitIndex++);

					if (!DateRegex.IsMatch(currDate))
					{   // Else it's a multiline tweet.
						prevTweet += " " + parsedLine;
						continue;
					}

					var currTweet = parsedLine.Substring(splitIndex);

					// Need to skip first read.
					if (prevDate != string.Empty)
						AddTweet(tweets, prevDate, prevTweet);

					prevDate = currDate;
					prevTweet = currTweet;
				}
				else
				{   // Else it's a multiline tweet.
					prevTweet += " " + parsedLine;
				}
			}

			AddTweet(tweets, prevDate, prevTweet);
		}

		private static void AddTweet(IDictionary<DateTime, List<Tweet>> tweets, string prevDate, string prevTweet)
		{
			var d = DateTime.ParseExact(prevDate, Tweet.DateFormat, System.Globalization.CultureInfo.InvariantCulture);
			List<Tweet> list;
			if (!tweets.TryGetValue(d, out list))
			{
				list = new List<Tweet>();
				tweets.Add(d, list);
			}
			var t = new Tweet(d, prevTweet);
			if (t.IsValidTweet())
				list.Add(t);
		}
	}
}
