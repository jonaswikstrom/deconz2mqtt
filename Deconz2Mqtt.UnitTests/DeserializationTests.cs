using System;
using Deconz2Mqtt.Domain.Model;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Deconz2Mqtt.UnitTests
{
    public class DeserializationTests
    {
        [Fact]
        public void DeserializationOfPresenceBool_WebSocketMessage_DeserializesCOrrectly()
        {
            var jsonMessage = @"{""e"":""changed"",""id"":""15"",""r"":""sensors"",""state"":{""lastupdated"":""2020-04-11T10:08:04"",""presence"":true},""t"":""event"",""uniqueid"":""00:15:8d:00:02:53:a4:33-01-0406""}";
            var fullState = JsonConvert.DeserializeObject<WebSocketMessage>(jsonMessage);

            fullState.State.Presence.Should().BeTrue();
        }
    }
}
