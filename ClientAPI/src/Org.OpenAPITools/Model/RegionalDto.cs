// <auto-generated>
/*
 * Web
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Org.OpenAPITools.Client;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// RegionalDto
    /// </summary>
    public partial class RegionalDto : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionalDto" /> class.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">name</param>
        /// <param name="codeRegional">codeRegional</param>
        /// <param name="description">description</param>
        /// <param name="address">address</param>
        /// <param name="active">active</param>
        [JsonConstructor]
        public RegionalDto(Option<int?> id = default, Option<string?> name = default, Option<string?> codeRegional = default, Option<string?> description = default, Option<string?> address = default, Option<bool?> active = default)
        {
            IdOption = id;
            NameOption = name;
            CodeRegionalOption = codeRegional;
            DescriptionOption = description;
            AddressOption = address;
            ActiveOption = active;
            OnCreated();
        }

        partial void OnCreated();

        /// <summary>
        /// Used to track the state of Id
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<int?> IdOption { get; private set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get { return this.IdOption; } set { this.IdOption = new(value); } }

        /// <summary>
        /// Used to track the state of Name
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<string?> NameOption { get; private set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get { return this.NameOption; } set { this.NameOption = new(value); } }

        /// <summary>
        /// Used to track the state of CodeRegional
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<string?> CodeRegionalOption { get; private set; }

        /// <summary>
        /// Gets or Sets CodeRegional
        /// </summary>
        [JsonPropertyName("codeRegional")]
        public string? CodeRegional { get { return this.CodeRegionalOption; } set { this.CodeRegionalOption = new(value); } }

        /// <summary>
        /// Used to track the state of Description
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<string?> DescriptionOption { get; private set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get { return this.DescriptionOption; } set { this.DescriptionOption = new(value); } }

        /// <summary>
        /// Used to track the state of Address
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<string?> AddressOption { get; private set; }

        /// <summary>
        /// Gets or Sets Address
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get { return this.AddressOption; } set { this.AddressOption = new(value); } }

        /// <summary>
        /// Used to track the state of Active
        /// </summary>
        [JsonIgnore]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public Option<bool?> ActiveOption { get; private set; }

        /// <summary>
        /// Gets or Sets Active
        /// </summary>
        [JsonPropertyName("active")]
        public bool? Active { get { return this.ActiveOption; } set { this.ActiveOption = new(value); } }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class RegionalDto {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  CodeRegional: ").Append(CodeRegional).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Address: ").Append(Address).Append("\n");
            sb.Append("  Active: ").Append(Active).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

    /// <summary>
    /// A Json converter for type <see cref="RegionalDto" />
    /// </summary>
    public class RegionalDtoJsonConverter : JsonConverter<RegionalDto>
    {
        /// <summary>
        /// Deserializes json to <see cref="RegionalDto" />
        /// </summary>
        /// <param name="utf8JsonReader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override RegionalDto Read(ref Utf8JsonReader utf8JsonReader, Type typeToConvert, JsonSerializerOptions jsonSerializerOptions)
        {
            int currentDepth = utf8JsonReader.CurrentDepth;

            if (utf8JsonReader.TokenType != JsonTokenType.StartObject && utf8JsonReader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            JsonTokenType startingTokenType = utf8JsonReader.TokenType;

            Option<int?> id = default;
            Option<string?> name = default;
            Option<string?> codeRegional = default;
            Option<string?> description = default;
            Option<string?> address = default;
            Option<bool?> active = default;

            while (utf8JsonReader.Read())
            {
                if (startingTokenType == JsonTokenType.StartObject && utf8JsonReader.TokenType == JsonTokenType.EndObject && currentDepth == utf8JsonReader.CurrentDepth)
                    break;

                if (startingTokenType == JsonTokenType.StartArray && utf8JsonReader.TokenType == JsonTokenType.EndArray && currentDepth == utf8JsonReader.CurrentDepth)
                    break;

                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName && currentDepth == utf8JsonReader.CurrentDepth - 1)
                {
                    string? localVarJsonPropertyName = utf8JsonReader.GetString();
                    utf8JsonReader.Read();

                    switch (localVarJsonPropertyName)
                    {
                        case "id":
                            if (utf8JsonReader.TokenType != JsonTokenType.Null)
                                id = new Option<int?>(utf8JsonReader.GetInt32());
                            break;
                        case "name":
                            name = new Option<string?>(utf8JsonReader.GetString());
                            break;
                        case "codeRegional":
                            codeRegional = new Option<string?>(utf8JsonReader.GetString());
                            break;
                        case "description":
                            description = new Option<string?>(utf8JsonReader.GetString());
                            break;
                        case "address":
                            address = new Option<string?>(utf8JsonReader.GetString());
                            break;
                        case "active":
                            if (utf8JsonReader.TokenType != JsonTokenType.Null)
                                active = new Option<bool?>(utf8JsonReader.GetBoolean());
                            break;
                        default:
                            break;
                    }
                }
            }

            if (id.IsSet && id.Value == null)
                throw new ArgumentNullException(nameof(id), "Property is not nullable for class RegionalDto.");

            if (active.IsSet && active.Value == null)
                throw new ArgumentNullException(nameof(active), "Property is not nullable for class RegionalDto.");

            return new RegionalDto(id, name, codeRegional, description, address, active);
        }

        /// <summary>
        /// Serializes a <see cref="RegionalDto" />
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="regionalDto"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Write(Utf8JsonWriter writer, RegionalDto regionalDto, JsonSerializerOptions jsonSerializerOptions)
        {
            writer.WriteStartObject();

            WriteProperties(writer, regionalDto, jsonSerializerOptions);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serializes the properties of <see cref="RegionalDto" />
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="regionalDto"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteProperties(Utf8JsonWriter writer, RegionalDto regionalDto, JsonSerializerOptions jsonSerializerOptions)
        {
            if (regionalDto.IdOption.IsSet)
                writer.WriteNumber("id", regionalDto.IdOption.Value!.Value);

            if (regionalDto.NameOption.IsSet)
                if (regionalDto.NameOption.Value != null)
                    writer.WriteString("name", regionalDto.Name);
                else
                    writer.WriteNull("name");

            if (regionalDto.CodeRegionalOption.IsSet)
                if (regionalDto.CodeRegionalOption.Value != null)
                    writer.WriteString("codeRegional", regionalDto.CodeRegional);
                else
                    writer.WriteNull("codeRegional");

            if (regionalDto.DescriptionOption.IsSet)
                if (regionalDto.DescriptionOption.Value != null)
                    writer.WriteString("description", regionalDto.Description);
                else
                    writer.WriteNull("description");

            if (regionalDto.AddressOption.IsSet)
                if (regionalDto.AddressOption.Value != null)
                    writer.WriteString("address", regionalDto.Address);
                else
                    writer.WriteNull("address");

            if (regionalDto.ActiveOption.IsSet)
                writer.WriteBoolean("active", regionalDto.ActiveOption.Value!.Value);
        }
    }
}
