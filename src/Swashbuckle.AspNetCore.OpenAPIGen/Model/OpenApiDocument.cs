using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Security;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Model
{
    public class OpenApiDocument
    {
        public string OpenApi { get; } = "3.0.0";

        public Info.Info Info { get; set; }

        public IEnumerable<Server> Servers { get; set; }

        public IDictionary<string, PathItem> Paths { get; set; }

        public Components Components { get; set; }

        // TODO: wrong type
        public object Security { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public IEnumerable<ExternalDocs> ExternalDocs { get; set; }
    }

    public sealed class Server
    {
        public string Url { get; set; }

        public string Description { get; set; }

        public IDictionary<string, ServerVariable> Variables { get; set; }
    }

    public sealed class ServerVariable
    {
        public IEnumerable<string> Enum { get; set; } = new List<string>();

        public string Default { get; set; }

        public string Description { get; set; }
    }

    public sealed class Components
    {
        public IDictionary<string, Schema> Schemas { get; set; }

        public IDictionary<string, Response> Responses { get; set; }

        public IDictionary<string, Parameter> Parameters { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Examples { get; set; }

        public IDictionary<string, RequestBody> RequestBodies { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Headers { get; set; }

        public IDictionary<string, SecurityScheme> SecuritySchemes { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Links { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Callbacks { get; set; }
    }

    public sealed class PathItem
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public Operation Get { get; set; }

        public Operation Put { get; set; }

        public Operation Post { get; set; }

        public Operation Delete { get; set; }

        public Operation Options { get; set; }

        public Operation Head { get; set; }

        public Operation Patch { get; set; }

        public Operation Trace { get; set; }

        public IEnumerable<Server> Servers { get; set; }

        // TODO: wrong type
        public IEnumerable<object> Parameters { get; set; }
    }

    public sealed class Operation
    {
        public IEnumerable<string> Tags { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ExternalDocs ExternalDocs { get; set; }

        public string OperationId { get; set; }

        public IReadOnlyList<Parameter> Parameters { get; set; }

        public RequestBody RequestBody { get; set; }

        public IDictionary<string, Response> Responses { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Callbacks { get; set; }

        public bool? Deprecated { get; set; }

        public SecurityScheme Security { get; set; }

        public IEnumerable<Server> Servers { get; set; }
    }

    public sealed class ExternalDocs
    {
        public string Description { get; set; }

        public string Url { get; set; }
    }

    public sealed class Parameter
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter), true)]
        public ParameterLocation In { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool? Deprecated { get; set; }

        public bool? AllowEmptyValue { get; set; }

        public string Style { get; set; }

        public bool? Explode { get; set; }

        public bool? AllowReserved { get; set; }

        public Schema Schema { get; set; }

        public object Example { get; set; }

        public IDictionary<string, Example> Examples { get; set; }

        public IDictionary<string, MediaType> Content { get; set; }
    }

    public enum ParameterLocation
    {
        Path,
        Query,
        Header,
        Cookie // Not supported by API Explorer
    }

    public sealed class RequestBody
    {
        public string Description { get; set; }

        public IDictionary<string, MediaType> Content { get; set; }

        public bool Required { get; set; }
    }

    public sealed class MediaType
    {
        public Schema Schema { get; set; }

        public object Example { get; set; }

        public IDictionary<string, Example> Examples { get; set; }

        public IDictionary<string, Encoding> Encoding { get; set; }
    }

    public sealed class Encoding
    {
        public string ContentType { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Headers { get; set; }

        public string Style { get; set; }

        public bool Explode { get; set; }

        public bool AllowReserved { get; set; }
    }

    public sealed class Response
    {
        public string Description { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Headers { get; set; }

        public IDictionary<string, MediaType> Content { get; set; }

        // TODO: wrong type
        public IDictionary<string, object> Links { get; set; }
    }

    // TODO: callback

    public sealed class Example
    {
        public string Summary { get; set; }

        public string Description { get; set; }

        public object Value { get; set; }

        public string ExternalValue { get; set; }
    }

    // TODO: Link

    // TODO: Header

    public sealed class Tag
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ExternalDocs ExternalDocs { get; set; }
    }

    // TODO: Reference

    public sealed class Schema
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }

        public string Title { get; set; }

        public int? MultipleOf { get; set; }

        public int? Maximum { get; set; }

        public bool? ExclusiveMaximum { get; set; }

        public int? Minimum { get; set; }

        public bool? ExclusiveMinimum { get; set; }

        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public string Pattern { get; set; }

        public int? MaxItems { get; set; }

        public int? MinItems { get; set; }

        public bool? UniqueItems { get; set; }

        public int? MaxProperties { get; set; }

        public int? MinProperties { get; set; }

        public IEnumerable<string> Required { get; set; }

        public IEnumerable<object> Enum { get; set; }

        public string Type { get; set; }

        public IEnumerable<Schema> AllOf { get; set; }

        public IEnumerable<Schema> OneOf { get; set; }

        public IEnumerable<Schema> AnyOf { get; set; }

        public IEnumerable<Schema> Not { get; set; }

        public Schema Items { get; set; }

        public IDictionary<string, Schema> Properties { get; set; }

        public Schema AdditionalProperties { get; set; }

        public string Description { get; set; }

        public string Format { get; set; }

        public object Default { get; set; }

        public bool? Nullable { get; set; }

        public string Discriminator { get; set; }

        public bool? ReadOnly { get; set; }

        public bool? WriteOnly { get; set; }

        public object Xml { get; set; }

        public ExternalDocs ExternalDocs { get; set; }

        public object Example { get; set; }

        public bool? Deprecated { get; set; }
    }

    // TODO: discriminator
}
