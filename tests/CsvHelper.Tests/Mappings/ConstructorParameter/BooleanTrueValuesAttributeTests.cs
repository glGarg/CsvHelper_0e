﻿// Copyright 2009-2021 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelper.Tests.Mappings.ConstructorParameter
{
	[TestClass]
    public class BooleanTrueValuesAttributeTests
    {
		[TestMethod]
		public void AutoMap_WithBooleanTrueValuesAttribute_CreatesParameterMaps()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture);
			var context = new CsvContext(config);
			var map = context.AutoMap<Foo>();

			Assert.AreEqual(2, map.ParameterMaps.Count);
			Assert.AreEqual(0, map.ParameterMaps[0].Data.TypeConverterOptions.BooleanTrueValues.Count);
			Assert.AreEqual(0, map.ParameterMaps[0].Data.TypeConverterOptions.BooleanFalseValues.Count);
			Assert.AreEqual(1, map.ParameterMaps[1].Data.TypeConverterOptions.BooleanTrueValues.Count);
			Assert.AreEqual(0, map.ParameterMaps[1].Data.TypeConverterOptions.BooleanFalseValues.Count);
			Assert.AreEqual("Bar", map.ParameterMaps[1].Data.TypeConverterOptions.BooleanTrueValues[0]);
		}

		[TestMethod]
		public void GetRecords_WithBooleanTrueValuesAttribute_HasHeader_CreatesRecords()
		{
			var parser = new ParserMock
			{
				{ "id", "boolean" },
				{ "1", "Bar" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.AreEqual(1, records.Count);
				Assert.AreEqual(1, records[0].Id);
				Assert.IsTrue(records[0].Boolean);
			}
		}

		[TestMethod]
		public void GetRecords_WithBooleanTrueValuesAttribute_NoHeader_CreatesRecords()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
			};
			var parser = new ParserMock(config)
			{
				{ "1", "Bar" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.AreEqual(1, records.Count);
				Assert.AreEqual(1, records[0].Id);
				Assert.IsTrue(records[0].Boolean);
			}
		}

		[TestMethod]
		public void WriteRecords_WithBooleanTrueValuesAttribute_DoesntUseParameterMaps()
		{
			var records = new List<Foo>
			{
				new Foo(1, true),
			};

			using (var writer = new StringWriter())
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(records);

				var expected = new StringBuilder();
				expected.Append("Id,Boolean\r\n");
				expected.Append("1,True\r\n");

				Assert.AreEqual(expected.ToString(), writer.ToString());
			}
		}

		private class Foo
		{
			public int Id { get; private set; }

			public bool Boolean { get; private set; }

			public Foo(int id, [BooleanTrueValues("Bar")]bool boolean)
			{
				Id = id;
				Boolean = boolean;
			}
		}
	}
}
