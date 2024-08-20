using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Server.Configurations;
using Serilog;
using Xunit;

namespace NeoServer.Loaders.Tests.ChannelLoader;

public class ChannelLoaderTests
{
    [Theory]
    [MemberData(nameof(DependenciesData))]
    public void When_any_dependency_is_null_a_error_message_is_logged(ServerConfiguration serverConfiguration,
        ChatChannelFactory chatChannelFactory, IChatChannelStore chatChannelStore, string error)
    {
        //arrange
        var loggerMock = new Mock<ILogger>();
        var sut = new Chats.ChannelLoader(serverConfiguration, loggerMock.Object, chatChannelFactory, chatChannelStore);

        //act
        sut.Load();

        //assert
        loggerMock.Verify(x => x.Error(error), Times.Once);
        loggerMock.Verify(x => x.Error("Unable to load channels"), Times.Once);
        chatChannelStore?.All.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Channel_file_not_found_logs_an_error_message()
    {
        //arrange
        var loggerMock = new Mock<ILogger>();
        var serverConfiguration = new Fixture()
            .Build<ServerConfiguration>()
            .With(x => x.Data, "")
            .Create();
        var chatChannelFactory = new ChatChannelFactory();
        var chatChannelStore = new ChatChannelStore();

        var sut = new Chats.ChannelLoader(serverConfiguration, loggerMock.Object, chatChannelFactory, chatChannelStore);

        //act
        sut.Load();

        //assert
        loggerMock.Verify(x => x.Error("channels.json file not found at channels.json"), Times.Once);
        chatChannelStore?.All.Should().BeNullOrEmpty();
    }


    public static IEnumerable<object[]> DependenciesData()
    {
        yield return new object[]
        {
            null,
            new ChatChannelFactory(),
            new ChatChannelStore(),
            "Server configuration not found"
        };

        yield return new object[]
        {
            new Fixture().Create<ServerConfiguration>(),
            null,
            new ChatChannelStore(),
            "ChatChannelFactory not found"
        };

        yield return new object[]
        {
            new Fixture().Create<ServerConfiguration>(),
            new ChatChannelFactory(),
            null,
            "ChatChannelStore not found"
        };
    }
}