﻿using System;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class XmlCommentsIdHelperTests
    {
        [Theory]
        [InlineData(nameof(FakeActions.AcceptsNothing), "M:Swashbuckle.AspNetCore.OpenAPIGen.Test.FakeActions.AcceptsNothing")]
        [InlineData(nameof(FakeActions.AcceptsNestedType), "M:Swashbuckle.AspNetCore.OpenAPIGen.Test.FakeActions.AcceptsNestedType(Swashbuckle.AspNetCore.OpenAPIGen.Test.ContainingType.NestedType)")]
        [InlineData(nameof(FakeActions.AcceptsGenericType), "M:Swashbuckle.AspNetCore.OpenAPIGen.Test.FakeActions.AcceptsGenericType(System.Collections.Generic.IEnumerable{System.String})")]
        [InlineData(nameof(FakeActions.AcceptsGenericGenericType), "M:Swashbuckle.AspNetCore.OpenAPIGen.Test.FakeActions.AcceptsGenericGenericType(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.String,System.String}})")]
        [InlineData(nameof(FakeActions.AcceptsGenericArrayType), "M:Swashbuckle.AspNetCore.OpenAPIGen.Test.FakeActions.AcceptsGenericArrayType(System.Collections.Generic.KeyValuePair{System.String,System.String}[])")]
        public void GetCommentIdForMethod_ReturnsCorrectXmlCommentId_ForGivenMethodInfo(
            string actionFixtureName,
            string expectedCommentId
        )
        {
            var methodInfo = typeof(FakeActions).GetMethod(actionFixtureName);

            var commentId = XmlCommentsIdHelper.GetCommentIdForMethod(methodInfo);

            Assert.Equal(expectedCommentId, commentId);
        }

        [Theory]
        [InlineData(typeof(ContainingType.NestedType), "T:Swashbuckle.AspNetCore.OpenAPIGen.Test.ContainingType.NestedType")]
        [InlineData(typeof(XmlAnnotatedGenericType<>), "T:Swashbuckle.AspNetCore.OpenAPIGen.Test.XmlAnnotatedGenericType`1")]
        [InlineData(typeof(NoNamespaceType), "T:NoNamespaceType")]
        public void GetCommentIdForType_ReturnsCorrectXmlCommentId_ForGivenType(
            Type type,
            string expectedCommentId
        )
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(type);

            Assert.Equal(expectedCommentId, commentId);
        }

        [Theory]
        [InlineData(typeof(ContainingType.NestedType), nameof(ContainingType.NestedType.Property2), "P:Swashbuckle.AspNetCore.OpenAPIGen.Test.ContainingType.NestedType.Property2")]
        [InlineData(typeof(XmlAnnotatedGenericType<>), "GenericProperty", "P:Swashbuckle.AspNetCore.OpenAPIGen.Test.XmlAnnotatedGenericType`1.GenericProperty")]
        public void GetCommentIdForProperty_ReturnsCorrectXmlCommentId_ForGivenPropertyInfo(
            Type type,
            string propertyName,
            string expectedCommentId
        )
        {
            var propertyInfo = type.GetProperty(propertyName);

            var commentId = XmlCommentsIdHelper.GetCommentIdForProperty(propertyInfo);

            Assert.Equal(expectedCommentId, commentId);
        }
    }
}
