#region License

// Copyright (c) 2008-2010 Ken Egozi (ken@kenegozi.com)
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

using D9.SQLQueryGenerator.Runtime.Format;

namespace D9.SQLQueryGenerator.Runtime.Model.Table
{
	/// <summary>
	/// Basic logic for tables
	/// </summary>
	public abstract class AbstractTable : IFormatableTable
	{
		private readonly string alias;
		private readonly string name;
		private readonly string schema;

		/// <summary>
		/// Creates a new aliased table
		/// </summary>
		/// <param name="schema">Schema</param>
		/// <param name="name">Name</param>
		/// <param name="alias">Alias</param>
		protected AbstractTable(string schema, string name, string alias)
		{
			this.schema = schema;
			this.name = name;
			this.alias = alias;
		}

		/// <summary>
		/// Creates a new table
		/// </summary>
		/// <param name="schema">Schema</param>
		/// <param name="name">Name</param>
		protected AbstractTable(string schema, string name)
		{
			this.schema = schema;
			this.name = name;
		}

		#region IFormatableTable Members

		string IFormatableTable.Schema
		{
			get { return schema; }
		}

		string IFormatableTable.Name
		{
			get { return name; }
		}

		string IFormatableTable.Alias
		{
			get { return alias; }
		}

		#endregion

		public override string ToString()
		{
			return Formatting.Format(this);
		}
	}
}