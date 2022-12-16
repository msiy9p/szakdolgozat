using Libellus.Domain.Common.Events;
using Libellus.Domain.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Libellus.Domain.JsonConverters;

public sealed class DomainEventJsonConverter : JsonConverter<BaseDomainEvent>
{
    private const string TypeId = nameof(TypeId);
    private const string TypeValue = nameof(TypeValue);

    private enum TypeDiscriminator
    {
        BaseClass = 0,
        UserInvitedEvent = 1,
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(BaseDomainEvent).IsAssignableFrom(typeToConvert);
    }

    public override BaseDomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeId = options.PropertyNamingPolicy?.ConvertName(TypeId);
        if (string.IsNullOrWhiteSpace(typeId))
        {
            typeId = TypeId;
        }

        var typeValue = options.PropertyNamingPolicy?.ConvertName(TypeValue);
        if (string.IsNullOrWhiteSpace(typeValue))
        {
            typeValue = TypeValue;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        if (!reader.Read()
            || reader.TokenType != JsonTokenType.PropertyName
            || reader.GetString() != typeId)
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        BaseDomainEvent output;
        var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
        switch (typeDiscriminator)
        {
            case TypeDiscriminator.UserInvitedEvent:
                if (!reader.Read() || reader.GetString() != typeValue)
                {
                    throw new JsonException();
                }

                if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                output = (UserInvitedEvent)JsonSerializer.Deserialize(ref reader, typeof(UserInvitedEvent), options);
                break;

            case TypeDiscriminator.BaseClass:
            default:
                throw new NotSupportedException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return output;
    }

    public override void Write(Utf8JsonWriter writer, BaseDomainEvent value, JsonSerializerOptions options)
    {
        var typeId = options.PropertyNamingPolicy?.ConvertName(TypeId);
        if (string.IsNullOrWhiteSpace(typeId))
        {
            typeId = TypeId;
        }

        var typeValue = options.PropertyNamingPolicy?.ConvertName(TypeValue);
        if (string.IsNullOrWhiteSpace(typeValue))
        {
            typeValue = TypeValue;
        }

        writer.WriteStartObject();

        if (value is UserInvitedEvent userInvitedEvent)
        {
            writer.WriteNumber(typeId, (int)TypeDiscriminator.UserInvitedEvent);
            writer.WritePropertyName(typeValue);
            JsonSerializer.Serialize(writer, userInvitedEvent, options);
        }
        else
        {
            throw new NotSupportedException();
        }

        writer.WriteEndObject();
    }
}