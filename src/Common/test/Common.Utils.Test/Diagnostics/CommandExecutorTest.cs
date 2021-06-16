// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Steeltoe.Common.Utils.Diagnostics;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Steeltoe.Common.Utils.Test.Diagnostics
{
    public class CommandExecutorTest
    {
        private readonly CommandExecutor _commandExecutor = new ();

        [Fact]
        public async void SuccessfulCommandShouldReturn0()
        {
            var result = await _commandExecutor.ExecuteAsync("dotnet --help");
            result.ExitCode.Should().Be(0);
            result.Output.Should().Contain("Usage: dotnet");
        }

        [Fact]
        public async void UnsuccessfulCommandShouldNotReturn0()
        {
            var result = await _commandExecutor.ExecuteAsync("dotnet --no-such-option");
            result.ExitCode.Should().NotBe(0);
            result.Error.Should().Contain("Unknown option: --no-such-option");
        }

        [Fact]
        public async void UnknownCommandShouldThrowException()
        {
            Func<Task> act = async () => { await _commandExecutor.ExecuteAsync("no-such-command"); };
            await act.Should().ThrowAsync<CommandException>().WithMessage("'no-such-command' failed to start*");
        }
    }
}
