﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.OpenAPIGen.Model;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Info;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Security;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class SwaggerGenerator : ISwaggerProvider
    {
        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionsProvider;
        private readonly ISchemaRegistryFactory _schemaRegistryFactory;
        private readonly SwaggerGeneratorSettings _settings;

        public SwaggerGenerator(
            IApiDescriptionGroupCollectionProvider apiDescriptionsProvider,
            ISchemaRegistryFactory schemaRegistryFactory,
            SwaggerGeneratorSettings settings = null)
        {
            _apiDescriptionsProvider = apiDescriptionsProvider;
            _schemaRegistryFactory = schemaRegistryFactory;
            _settings = settings ?? new SwaggerGeneratorSettings();
        }

        public OpenApiDocument GetSwagger(
            string documentName,
            string host = null,
            string basePath = null,
            string[] schemes = null)
        {
            var schemaRegistry = _schemaRegistryFactory.Create();

            Info info;
            if (!_settings.SwaggerDocs.TryGetValue(documentName, out info))
                throw new UnknownSwaggerDocument(documentName);

            var apiDescriptions = _apiDescriptionsProvider.ApiDescriptionGroups.Items
                .SelectMany(group => group.Items)
                .Where(apiDesc => _settings.DocInclusionPredicate(documentName, apiDesc))
                .Where(apiDesc => !_settings.IgnoreObsoleteActions || !apiDesc.IsObsolete())
                .OrderBy(_settings.SortKeySelector);

            var paths = apiDescriptions
                .GroupBy(apiDesc => apiDesc.RelativePathSansQueryString())
                .ToDictionary(group => "/" + group.Key, group => CreatePathItem(group, schemaRegistry));

            var servers = new List<Server>
            {
                new Server { Url = host }
            };

            var components = GetComponents(_settings.SecurityDefinitions, schemaRegistry);

            var swaggerDoc = new OpenApiDocument
            {
                Info = info,
                Servers = servers,
                Components = components,
                Paths = paths,
            };

            //var filterContext = new DocumentFilterContext(
            //    _apiDescriptionsProvider.ApiDescriptionGroups,
            //    schemaRegistry);

            //foreach (var filter in _settings.DocumentFilters)
            //{
            //    filter.Apply(swaggerDoc, filterContext);
            //}

            return swaggerDoc;
        }

        private Components GetComponents(IDictionary<string, SecurityScheme> securitySchemes, ISchemaRegistry schemaRegistry)
        {
            var components = new Components();
            components.SecuritySchemes = securitySchemes;
            components.Schemas = schemaRegistry.Definitions;
            return components;
        }

        private PathItem CreatePathItem(IEnumerable<ApiDescription> apiDescriptions, ISchemaRegistry schemaRegistry)
        {
            var pathItem = new PathItem();

            // Group further by http method
            var perMethodGrouping = apiDescriptions
                .GroupBy(apiDesc => apiDesc.HttpMethod);

            foreach (var group in perMethodGrouping)
            {
                var httpMethod = group.Key;

                if (httpMethod == null)
                    throw new NotSupportedException(
                        $"Ambiguous HTTP method for action - {group.First().ActionDescriptor.DisplayName}. " +
                        "Actions require an explicit HttpMethod binding for Swagger");

                if (group.Count() > 1)
                    throw new NotSupportedException(
                        $"HTTP method \"{httpMethod}\" & path \"{@group.First().RelativePathSansQueryString()}\" overloaded by actions - {string.Join(",", group.Select(apiDesc => apiDesc.ActionDescriptor.DisplayName))}. " +
                        "Actions require unique method/path combination for Swagger");

                var apiDescription = group.Single();

                switch (httpMethod)
                {
                    case "GET":
                        pathItem.Get = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "PUT":
                        pathItem.Put = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "POST":
                        pathItem.Post = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "DELETE":
                        pathItem.Delete = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "OPTIONS":
                        pathItem.Options = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "HEAD":
                        pathItem.Head = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "PATCH":
                        pathItem.Patch = CreateOperation(apiDescription, schemaRegistry);
                        break;
                    case "TRACE":
                        pathItem.Trace = CreateOperation(apiDescription, schemaRegistry);
                        break;
                }
            }

            return pathItem;
        }

        private Operation CreateOperation(ApiDescription apiDescription, ISchemaRegistry schemaRegistry)
        {
            var parameters = apiDescription.ParameterDescriptions
                .Where(paramDesc => paramDesc.Source != BindingSource.Body || paramDesc.Source != BindingSource.Form)
                .Where(paramDesc => paramDesc.Source.IsFromRequest && !paramDesc.IsPartOfCancellationToken())
                .Select(paramDesc => CreateParameter(apiDescription, paramDesc, schemaRegistry))
                .ToList();

            var responses = apiDescription.SupportedResponseTypes
                .DefaultIfEmpty(new ApiResponseType { StatusCode = 200 })
                .ToDictionary(
                    apiResponseType => apiResponseType.StatusCode.ToString(),
                    apiResponseType => CreateResponse(apiResponseType, schemaRegistry)
                 );

            var operation = new Operation
            {
                Tags = new[] { _settings.TagSelector(apiDescription) },
                OperationId = apiDescription.FriendlyId(),
                Parameters = parameters.Any() ? parameters : null, // parameters can be null but not empty
                Responses = responses,
                Deprecated = apiDescription.IsObsolete() ? true : (bool?)null
            };

            var filterContext = new OperationFilterContext(apiDescription, schemaRegistry);
            foreach (var filter in _settings.OperationFilters)
            {
                filter.Apply(operation, filterContext);
            }

            return operation;
        }

        private Parameter CreateParameter(
            ApiDescription apiDescription,
            ApiParameterDescription paramDescription,
            ISchemaRegistry schemaRegistry)
        {
            ParameterLocation GetParameterLocation(ApiParameterDescription param)
            {
                if (param.Source == BindingSource.Header)
                    return ParameterLocation.Header;
                else if (param.Source == BindingSource.Path)
                    return ParameterLocation.Path;
                else if (param.Source == BindingSource.Query)
                    return ParameterLocation.Query;

                // None of the above, default to "query"
                // Wanted to default to "body" for PUT/POST but ApiExplorer flattens out complex params into multiple
                // params for ALL non-bound params regardless of HttpMethod. So "query" across the board makes most sense
                return ParameterLocation.Query;
            }

            string GetStyle(ParameterLocation parameterLocation)
            {
                switch (parameterLocation)
                {
                    case ParameterLocation.Query:
                    case ParameterLocation.Cookie:
                        return "form";
                    case ParameterLocation.Path:
                    case ParameterLocation.Header:
                        return "simple";
                    default:
                        return "simple";
                }
            }

            var location = GetParameterLocation(paramDescription);
            var style = GetStyle(location);

            var name = _settings.DescribeAllParametersInCamelCase
                ? paramDescription.Name.ToCamelCase()
                : paramDescription.Name;

            var schema = (paramDescription.Type == null) ? null : schemaRegistry.GetOrRegister(paramDescription.Type);
            //if (location == "body")
            //{
            //    return new Parameter
            //    {
            //        Name = name,
            //        In = "body"
            //    };
            //}

            var parameter = new Parameter
            {
                Name = name,
                In = location,
                Required = (location == ParameterLocation.Path) || paramDescription.IsRequired(),
                Style = GetStyle(location),
                Explode = style == "form"
            };

            //if (schema == null)
            //    nonBodyParam.Type = "string";
            //else
            //    nonBodyParam.PopulateFrom(schema);

            //if (nonBodyParam.Type == "array")
            //    nonBodyParam.CollectionFormat = "multi";

            return parameter;
        }

        private Response CreateResponse(ApiResponseType apiResponseType, object schemaRegistry)
        {
            var description = ResponseDescriptionMap
                .FirstOrDefault((entry) => Regex.IsMatch(apiResponseType.StatusCode.ToString(), entry.Key))
                .Value;

            return new Response
            {
                Description = description,
                //Schema = (apiResponseType.Type != null && apiResponseType.Type != typeof(void))
                //    ? schemaRegistry.GetOrRegister(apiResponseType.Type)
                //    : null
            };
        }

        private static readonly Dictionary<string, string> ResponseDescriptionMap = new Dictionary<string, string>
        {
            { "1\\d{2}", "Information" },
            { "2\\d{2}", "Success" },
            { "3\\d{2}", "Redirect" },
            { "400", "Bad Request" },
            { "401", "Unauthorized" },
            { "403", "Forbidden" },
            { "404", "Not Found" },
            { "405", "Method Not Allowed" },
            { "406", "Not Acceptable" },
            { "408", "Request Timeout" },
            { "409", "Conflict" },
            { "4\\d{2}", "Client Error" },
            { "5\\d{2}", "Server Error" }
        };
    }
}
