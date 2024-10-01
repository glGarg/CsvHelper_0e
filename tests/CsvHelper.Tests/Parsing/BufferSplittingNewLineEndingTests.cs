﻿// Copyright 2009-2021 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvHelper.Tests.Parsing
{
	[TestClass]
	public class BufferSplittingCrLnTests
	{
		[TestMethod]
		public void Read_BufferSplitsCrLf_BufferNeedsResize_Parses()
		{
			var s = new StringBuilder();
			s.Append("1,0000000000321\r\n");
			s.Append("3,4\r\n");
			var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
			{
				BufferSize = 16,
				NewLine = "\r\n",
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				parser.Read();
				Assert.AreEqual("1", parser[0]);
				Assert.AreEqual("0000000000321", parser[1]);
			}
		}

		[TestMethod]
		public void Read_BufferSplitsCrLf_NoBufferResize_DoesntAddExtraField()
		{
			var s = new StringBuilder();
			s.Append("1,200000\r\n");
			s.Append("3,400000\r\n");
			s.Append("5,600\r\n");
			var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
			{
				BufferSize = 16,
				NewLine = "\r\n",
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				parser.Read();
				parser.Read();
				parser.Read();
				Assert.AreEqual(2, parser.Count);
			}
		}

		[TestMethod]
		public void Read_BufferSplitsCrLf_NoBufferResize_RawRecordIsCorrect()
		{
			var s = new StringBuilder();
			s.Append("1,200000\r\n");
			s.Append("3,400000\r\n");
			s.Append("5,600\r\n");
			var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
			{
				BufferSize = 16,
				NewLine = "\r\n",
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				parser.Read();
				parser.Read();
				parser.Read();
				Assert.AreEqual("5,600\r\n", parser.RawRecord);
			}
		}

		[TestMethod]
		public void BufferSplitsCrLfWithLastFieldQuotedTest()
		{
			var s = new StringBuilder();
			s.Append("1,200000\r\n");
			s.Append("3,4000\r\n");
			s.Append("5,\"600\"\r\n");
			var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
			{
				BufferSize = 16,
				NewLine = "\r\n",
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				parser.Read();
				parser.Read();
				parser.Read();
				Assert.AreEqual(2, parser.Count);
				Assert.AreEqual("5,\"600\"\r\n", parser.RawRecord);
			}
		}
	}
}
