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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace D9.Commons.Internal
{
	/// <summary>
	/// Used to cache enum values descriptions mapper
	/// </summary>
	public class DescribedEnumHandler<T>
	{
		private readonly IDictionary<T, string> toDescription = new Dictionary<T, string>();
		private readonly IDictionary<string, T> fromDescription = new Dictionary<string, T>();

		private const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

		/// <summary>
		/// Creates a new DescribedEnumHandler instance for a given enum type
		/// </summary>
		public DescribedEnumHandler()
		{
			var type = typeof (T);
			var enumEntrys = from f in type.GetFields(PUBLIC_STATIC)
			                 let attributes = f.GetCustomAttributes(typeof (DescriptionAttribute), false)
			                 let description =
			                 	attributes.Length == 1
			                 		? ((DescriptionAttribute) attributes[0]).Description
			                 		: f.Name
			                 select new
			                        	{
			                        		Value = (T) Enum.Parse(type, f.Name),
			                        		Description = description
			                        	};

			foreach (var enumEntry in enumEntrys)
			{
				toDescription[enumEntry.Value] = enumEntry.Description;
				fromDescription[enumEntry.Description] = enumEntry.Value;
			}
		}

		/// <summary>
		/// Extracts the description for the given enum value.
		/// <remarks>if no description was set for the given value, it's name will be retrieved</remarks>
		/// </summary>
		/// <param name="value">The enum value</param>
		/// <returns>The value's description</returns>
		public string GetDescriptionFrom(T value)
		{
			return toDescription[value];
		}

		/// <summary>
		/// Parse the given string and return the enum value for with the given string acts as description
		/// </summary>
		/// <param name="description">The given description</param>
		/// <returns>A matching enum value</returns>
		public T GetValueFrom(string description)
		{
			return fromDescription[description];
		}

	}
}