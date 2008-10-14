#region License

// Copyright (c) 2008 Ken Egozi (ken@kenegozi.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of the D9 project nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion License

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using D9.SQLQueryGenerator.DatabaseMetadataProviders;
using D9.SQLQueryGenerator.Descriptors;

namespace D9.SQLQueryGenerator
{
	internal class SQLQueryGeneratorException : ApplicationException
	{
		public SQLQueryGeneratorException(string message) : base(message)
		{
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				string ns = "Generated.SQLQuery";
				string server = "(local)";
				string db = null;
				string connectionString = null;
				string userId = null;
				string password = null;
				string output = "SQLQuery.Generated.cs";
				bool withSchema = true;

				foreach (string arg in args)
				{
					string[] parts = arg.Split(':');
					switch (parts[0].ToLowerInvariant())
					{
						case "/ns":
							ns = parts[1];
							break;

						case "/output":
							output = parts[1];
							break;

						case "/server":
							server = parts[1];
							break;

						case "/db":
							db = parts[1];
							break;

						case "/userid":
							userId = parts[1];
							break;

						case "/password":
							password = parts[1];
							break;

						case "/connectionstring":
							connectionString = parts[1];
							break;

						case "/withschema":
							withSchema = bool.Parse(parts[1]);
							break;
					}
				}

				IDatabaseMetadataProvider metadataProvider = null;

				if (connectionString != null)
					metadataProvider = new SQL2005MetadataProvider(connectionString);
				else if (db != null)
				{
					if (userId != null)
						metadataProvider = new SQL2005MetadataProvider(server, db, userId, password);
					else
						metadataProvider = new SQL2005MetadataProvider(server, db);
				}

				if (metadataProvider == null)
					throw new SQLQueryGeneratorException("You must provide either /db or /connectionstring");

				var SQLQueryFile = new StringBuilder();

				IEnumerable<DbPropertyMetadata> metadata = metadataProvider.ExtractMetadata();

				var processor = new MetadataProcessor();

				ICollection<TableDescriptor> tables = processor.GetTableDescriptorsFrom(metadata, withSchema).Values;

				SQLQueryFile.AppendLine(GetSQLClassesFrom(tables));

				foreach (TableDescriptor table in tables)
				{
					SQLQueryFile.AppendLine(GetClassesFrom(table));
				}

				WrapInNamespace(ns, SQLQueryFile);

				AddGeneratedByToolNotice(SQLQueryFile);

				AddCopyrightNotice(SQLQueryFile);

				File.WriteAllText(output, SQLQueryFile.ToString(), Encoding.UTF8);

				Console.WriteLine("That's it folks.");
			}
			catch (SQLQueryGeneratorException ex)
			{
				Error(ex.Message);
			}
		}

		private static void WrapInNamespace(string ns, StringBuilder content)
		{
			content.Insert(0, string.Format(@"namespace {0}
{{
", ns));
			content.AppendLine("}");
		}

		private static string GetSQLClassesFrom(IEnumerable<TableDescriptor> tables)
		{
			StringBuilder sqlClass = new StringBuilder().Append(
				@"	public static partial class SQL
	{
");
			foreach (TableDescriptor table in tables)
			{
				sqlClass.AppendFormat(
					@"		public static {1} {0} = new {1}();
"
					, table.Name, table.ClassName);
			}
			sqlClass.Append(@"	}");

			return sqlClass.ToString();
		}

		private static string GetClassesFrom(TableDescriptor table)
		{
			StringBuilder classes = new StringBuilder()
				.AppendLine(GetTableClassFrom(table));
			foreach (DbPropertyMetadata property in table.Properties)
			{
				classes.AppendLine(GetPropertyClassFrom(table, property));
			}

			return classes.ToString();
		}

		private static string GetPropertyClassFrom(TableDescriptor table, DbPropertyMetadata property)
		{
			return string.Format(
				@"	public class {0}_{1} : D9.SQLQueryGenerator.Runtime.Model.Field.AbstractField<{2}>
	{{
		public {0}_{1}(D9.SQLQueryGenerator.Runtime.Model.Table.AbstractTable table)
			: base(table, ""{1}"")
		{{
		}}
	}}",
				table.ClassName, property.Column, property.Type.FullName);
		}

		private static string GetTableClassFrom(TableDescriptor table)
		{
			return string.Format(
				@"	public class {2} : D9.SQLQueryGenerator.Runtime.Model.Table.AbstractTable
	{{
		public {2}(string alias) : base(""{0}"", ""{1}"", alias)
		{{
{3}
		}}

		public {2}() : this(null)
		{{
		}}

{4}

		public {2} As(string alias)
		{{
			return new {2}(alias);
		}}
	}}",
				table.Schema, table.Name, table.ClassName, GetFieldInitializersFor(table), GetFieldDeclerationsFor(table));
		}

		private static string GetFieldInitializersFor(TableDescriptor table)
		{
			var initializers = new List<string>(table.Properties.Count);
			foreach (DbPropertyMetadata property in table.Properties)
			{
				initializers.Add(string.Format(
				                 	@"			{0} = new {1}_{2}(this);",
				                 	property.Column, table.ClassName, property.Column));
			}

			return string.Join(Environment.NewLine, initializers.ToArray());
		}

		private static string GetFieldDeclerationsFor(TableDescriptor table)
		{
			var declerations = new List<string>(table.Properties.Count);
			foreach (DbPropertyMetadata property in table.Properties)
			{
				declerations.Add(string.Format(
				                 	@"		public readonly {1}_{2} {0};",
				                 	property.Column, table.ClassName, property.Column));
			}

			return string.Join(Environment.NewLine, declerations.ToArray());
		}


		private static void AddCopyrightNotice(StringBuilder content)
		{
			content.Insert(0,
			               @"// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
// 
// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

");
		}

		private static void AddGeneratedByToolNotice(StringBuilder content)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string title = ((AssemblyTitleAttribute) Attribute.GetCustomAttribute(
			                                         	assembly, typeof (AssemblyTitleAttribute))).Title;

			string version = ((AssemblyFileVersionAttribute) Attribute.GetCustomAttribute(
			                                                 	assembly, typeof (AssemblyFileVersionAttribute))).Version;

			content.Insert(0,
			               string.Format(@"/*
This file was generated by {0} version {1}
At {2}
*/
", title, version,
			                             DateTime.Now));
		}

		private static void Error(string message)
		{
			Console.WriteLine(message);
			Environment.Exit(1);
		}
	}
}