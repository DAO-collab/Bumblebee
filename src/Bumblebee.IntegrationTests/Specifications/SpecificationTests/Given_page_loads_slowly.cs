﻿using System;
using System.Linq.Expressions;

using Bumblebee.Extensions;
using Bumblebee.IntegrationTests.Shared.Hosting;
using Bumblebee.IntegrationTests.Shared.Pages;
using Bumblebee.Interfaces;
using Bumblebee.Setup;
using Bumblebee.Setup.DriverEnvironments;

using FluentAssertions;

using NUnit.Framework;

using OpenQA.Selenium;

// ReSharper disable InconsistentNaming

namespace Bumblebee.IntegrationTests.Specifications.SpecificationTests
{
	[TestFixture]
	public class Given_page_loads_slowly : HostTestFixture
	{
		private static readonly Expression<Func<SlowWebPageWithExplicitWait, ITextField>>[] WaitCasesForTextField = {
			p => p.ByAttributeWithWait,
			p => p.ByIdWithWait,
			p => p.ByClassNameWithWait,
			p => p.ByCssSelectorWithWait,
			p => p.ByFunctionWithSingleOutputSelectorWithWait,
			p => p.ByFunctionWithListOutputSelectorWithWait,
			p => p.ByNameWithWait,
			p => p.ByOrdinalWithWait,
			p => p.ByXPathWithWait,
		};

		private static readonly Expression<Func<SlowWebPageWithExplicitWait, ITextField>>[] NoWaitCasesForTextField = {
			p => p.ByAttributeWithNoWait,
			p => p.ByIdWithNoWait,
			p => p.ByClassNameWithNoWait,
			p => p.ByCssSelectorWithNoWait,
			p => p.ByFunctionWithSingleOutputSelectorWithNoWait,
			p => p.ByFunctionWithListOutputSelectorWithNoWait,
			p => p.ByNameWithNoWait,
			p => p.ByOrdinalWithNoWait,
			p => p.ByXPathWithNoWait,
		};

		private static readonly Expression<Func<SlowWebPageWithExplicitWait, IClickable<SlowWebPageWithExplicitWait>>>[] WaitCasesForClickable = {
			p => p.ByLinkTextWithWait,
			p => p.ByPartialLinkTextWithWait,
			p => p.ByTagNameWithWait,
		};

		private static readonly Expression<Func<SlowWebPageWithExplicitWait, IClickable<SlowWebPageWithExplicitWait>>>[] NoWaitCasesForClickable = {
			p => p.ByLinkTextWithNoWait,
			p => p.ByPartialLinkTextWithNoWait,
			p => p.ByTagNameWithNoWait,
		};

		[SetUp]
		public void TestSetUp()
		{
			Threaded<Session>
				.With(new PhantomJS(TimeSpan.FromSeconds(0)))
				.NavigateTo<SlowWebPageWithExplicitWait>(GetUrl("SlowWebPage.html"));
		}

		[TearDown]
		public void TestDispose()
		{
			Threaded<Session>
				.CurrentBlock<SlowWebPageWithExplicitWait>()
				.Session.End();
		}

		[TestCaseSource("WaitCasesForTextField")]
		public void When_finding_textfield_by_selector_with_wait_Then_should_return_value(Expression<Func<SlowWebPageWithExplicitWait, ITextField>> selectorWithWait)
		{
			Threaded<Session>
				.CurrentBlock<SlowWebPageWithExplicitWait>()
				.Apply(selectorWithWait)
				.Text
				.VerifyThat(t => t.Should().Be("Todd"));
		}

		[TestCaseSource("NoWaitCasesForTextField")]
		public void When_finding_textfield_by_selector_with_no_wait_Then_should_throw(Expression<Func<SlowWebPageWithExplicitWait, ITextField>> selectorWithNoWait)
		{
			Action action = () => Threaded<Session>
				.CurrentBlock<SlowWebPageWithExplicitWait>()
				.Apply(selectorWithNoWait)
				.Text
				.VerifyThat(t => t.Should().Be("Todd"));

			action.ShouldThrow<NotFoundException>();
		}

		[TestCaseSource("WaitCasesForClickable")]
		public void When_finding_clickable_by_selector_with_wait_Then_should_return_value(Expression<Func<SlowWebPageWithExplicitWait, IClickable<SlowWebPageWithExplicitWait>>> selectorWithWait)
		{
			Threaded<Session>
				.CurrentBlock<SlowWebPageWithExplicitWait>()
				.Apply(selectorWithWait)
				.Text
				.VerifyThat(t => t.Should().Be("Todd"));
		}

		[TestCaseSource("NoWaitCasesForClickable")]
		public void When_finding_clickable_by_selector_with_no_wait_Then_should_throw(Expression<Func<SlowWebPageWithExplicitWait, IClickable<SlowWebPageWithExplicitWait>>> selectorWithNoWait)
		{
			Action action = () => Threaded<Session>
				.CurrentBlock<SlowWebPageWithExplicitWait>()
				.Apply(selectorWithNoWait)
				.Text
				.VerifyThat(t => t.Should().Be("Todd"));

			action.ShouldThrow<NoSuchElementException>();
		}
	}

	public static class BlockExtensions
	{
		public static TElement Apply<TBlock, TElement>(this TBlock page, Expression<Func<TBlock, TElement>> accessor)
			where TBlock : IBlock
		{
			var fn = accessor.Compile();

			return fn(page);
		}
	}
}
