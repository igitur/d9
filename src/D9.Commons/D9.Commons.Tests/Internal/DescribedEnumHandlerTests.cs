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

using D9.Commons.Internal;
using NUnit.Framework;
using NUnit.Framework.ExtensionMethods;
using Description = System.ComponentModel.DescriptionAttribute;

namespace D9.Commons.Tests.Internal
{
	// ReSharper disable AccessToStaticMemberViaDerivedType
	[TestFixture]
	public class DescribedEnumHandlerTests
	{
		private const string VALUE_1_DESCRIPTION = "Value1";
		private const string VALUE_2_DESCRIPTION = "Value2";

		[Test]
		public void GetDescriptionOf_DescribedItem_ReturnsTheDescribedValue()
		{
			typeof(DescribedEnumeration).GetFields();

			var handler = new DescribedEnumHandler(typeof(DescribedEnumeration));

			var value = handler.GetDescriptionFrom(DescribedEnumeration.Value1);
			
			value.Should(Be.EqualTo(VALUE_1_DESCRIPTION));
		}

		[Test]
		public void GetDescriptionOf_DescribedItemInMixedEnum_ReturnsTheDescribedValue()
		{
			var handler = new DescribedEnumHandler(typeof(MixedDescribedEnumeration));

			var value = handler.GetDescriptionFrom(MixedDescribedEnumeration.Value1);

			value.Should(Be.EqualTo(VALUE_1_DESCRIPTION));
		}

		[Test]
		public void GetDescriptionOf_NonDescribedItemInMixedEnum_ReturnsTheRawValue()
		{
			var handler = new DescribedEnumHandler(typeof(MixedDescribedEnumeration));

			var value = handler.GetDescriptionFrom(MixedDescribedEnumeration.Value2);

			value.Should(Be.EqualTo(MixedDescribedEnumeration.Value2.ToString()));
		}

		[Test]
		public void GetDescriptionOf_NonDescribedEnum_ReturnsTheRawValue()
		{
			var handler = new DescribedEnumHandler(typeof(NonDescribedEnumeration));

			var value = handler.GetDescriptionFrom(NonDescribedEnumeration.Value);

			value.Should(Be.EqualTo(NonDescribedEnumeration.Value.ToString()));
		}
		
		enum DescribedEnumeration
		{
			[@Description(VALUE_1_DESCRIPTION)]
			Value1,

			[@Description(VALUE_2_DESCRIPTION)]
			Value2
		}
		enum NonDescribedEnumeration
		{
			Value
		}
		enum MixedDescribedEnumeration
		{
			[@Description(VALUE_1_DESCRIPTION)]
			Value1,

			Value2
		}
	}
}