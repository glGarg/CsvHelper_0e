﻿// Copyright 2009-2021 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelper.Tests.Parsing
{
	[TestClass]
    public class CsvModeTests
    {
		[TestMethod]
		public void Read_HasEscapedDelimiter_Parses()
		{
			var s = new StringBuilder();
			s.Append("a\\,b,c\n");
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Mode = CsvMode.Escape,
				Escape = '\\',
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				Assert.IsTrue(parser.Read());
				Assert.AreEqual("a,b", parser[0]);
				Assert.AreEqual("c", parser[1]);

				Assert.IsFalse(parser.Read());
			}
		}

		[TestMethod]
		public void Read_HasEscapedLineEnding_Parses()
		{
			var s = new StringBuilder();
			s.Append("a\\\nb,c\n");
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Mode = CsvMode.Escape,
				Escape = '\\',
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				Assert.IsTrue(parser.Read());
				Assert.AreEqual("a\nb", parser[0]);
				Assert.AreEqual("c", parser[1]);
				Assert.IsFalse(parser.Read());
			}
		}

		[TestMethod]
		public void Read_NoEscapeMode_HasRFC4180Format_Parses()
		{
			var s = new StringBuilder();
			s.Append("a,\"b,\"\"c\r\nd\",e");
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Mode = CsvMode.NoEscape,
				Escape = '"',
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				Assert.IsTrue(parser.Read());
				Assert.AreEqual("a", parser[0]);
				Assert.AreEqual("\"b", parser[1]);
				Assert.AreEqual("\"\"c", parser[2]);

				Assert.IsTrue(parser.Read());
				Assert.AreEqual("d\"", parser[0]);
				Assert.AreEqual("e", parser[1]);
			}
		}

		[TestMethod]
		public void Read_NoEscapeMode_HasEscapeFormat_Parses()
		{
			var s = new StringBuilder();
			s.Append("a,\\b\\,c\\\nd,e");
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Mode = CsvMode.NoEscape,
				Escape = '\\',
			};
			using (var reader = new StringReader(s.ToString()))
			using (var parser = new CsvParser(reader, config))
			{
				Assert.IsTrue(parser.Read());
				Assert.AreEqual("a", parser[0]);
				Assert.AreEqual("\\b\\", parser[1]);
				Assert.AreEqual("c\\", parser[2]);

				Assert.IsTrue(parser.Read());
				Assert.AreEqual("d", parser[0]);
				Assert.AreEqual("e", parser[1]);
			}
		}
	}
}
