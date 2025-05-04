using Moq;
using Newtonsoft.Json.Linq;
using DynamicMappingSystem.Infrastructure.Providers;
using DynamicMappingSystem.Application.Validation;
using System.Text.Json;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Application.Interfaces;

namespace DynamicMappingSystemTests;

[TestFixture]
public class MapHandlerTests
{
    private Mock<IMappingConfigService> _mockMappingConfigService;
    private Mock<IFormatConfigProvider> _mockFormatConfigProvider;
    private Mock<IMappingRuleProvider> _mockMappingRuleProvider;
    private Mock<IModelValidator> _mockValidator;
    private Mock<IMapHandler> _mockMapHandler;

    private MapHandler _mapHandler;

    [SetUp]
    public void SetUp()
    {
        _mockMappingConfigService = new Mock<IMappingConfigService>();
        _mockFormatConfigProvider = new Mock<IFormatConfigProvider>();
        _mockMappingRuleProvider = new Mock<IMappingRuleProvider>();
        _mockValidator = new Mock<IModelValidator>();
        _mockMapHandler = new Mock<IMapHandler>();

        _mapHandler = new MapHandler(
            _mockMappingRuleProvider.Object,
            new List<IModelValidator> { _mockValidator.Object },
            _mockFormatConfigProvider.Object
        );
    }

    #region Helper Methods

    private void SetupFormatConfigProviderMock()
    {
        var formatJson = @"
        [
          {
            ""Type"": ""Model.Reservation"",
            ""Properties"": [
              ""Id"",
              ""Customer.Name"",
              ""Customer.Email"",
              ""Customer.Address.City"",
              ""Customer.Address.PostalCode"",
              ""Customer.Address.Landmark"",
              ""CheckInDate"",
              ""CheckOutDate""
            ]
          },
          {
            ""Type"": ""Google.Reservation"",
            ""Properties"": [
              ""BookingId"",
              ""CustomerName"",
              ""CustomerEmail"",
              ""City"",
              ""Zip"",
              ""AdditionalDetails.Landmark"",
              ""ArrivalDate"",
              ""DepartureDate""
            ]
          }
        ]";

        var formatList = JsonSerializer.Deserialize<List<DataFormatDefinition>>(formatJson)!;

        _mockFormatConfigProvider
            .Setup(x => x.GetFormat(It.IsAny<string>()))
            .Returns((string type) =>
            {
                var format = formatList.FirstOrDefault(f => f.Type == type);
                if (format == null)
                {
                    throw new MappingException(
                        code: MappingErrorCodes.MissingFormat,
                        message: $"Format definition not found for type '{type}'"
                    );
                }
                return format;
            });
    }

    private void SetupMappingRuleProviderMock()
    {
        var mappingJson = @"
                [
                  {
                    ""SourceType"": ""Google.Reservation"",
                    ""TargetType"": ""Model.Reservation"",
                    ""PropertyMappings"": [
                      { ""SourceProperty"": ""BookingId"", ""TargetProperty"": ""Id"" },
                      { ""SourceProperty"": ""CustomerName"", ""TargetProperty"": ""Customer.Name"" },
                      { ""SourceProperty"": ""CustomerEmail"", ""TargetProperty"": ""Customer.Email"" },
                      { ""SourceProperty"": ""City"", ""TargetProperty"": ""Customer.Address.City"" },
                      { ""SourceProperty"": ""Zip"", ""TargetProperty"": ""Customer.Address.PostalCode"" },
                      { ""SourceProperty"": ""AdditionalDetails.Landmark"", ""TargetProperty"": ""Customer.Address.Landmark"" },
                      { ""SourceProperty"": ""ArrivalDate"", ""TargetProperty"": ""CheckInDate"" },
                      { ""SourceProperty"": ""DepartureDate"", ""TargetProperty"": ""CheckOutDate"" }
                    ]
                  }
                ]";

        var rules = JsonSerializer.Deserialize<List<MapperRule>>(mappingJson)!;

        _mockMappingRuleProvider
            .Setup(x => x.GetPropertyMappings(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string sourceType, string targetType) =>
            {
                var rule = rules.FirstOrDefault(r => r.SourceType == sourceType && r.TargetType == targetType);
                if (rule == null)
                {
                    throw new MappingException(
                        code: MappingErrorCodes.MappingRuleNotFound,
                        message: $"Mapping rule not found for types '{sourceType}' -> '{targetType}'"
                    );
                }
                return rule.PropertyMappings;
            });
    }

    private void SetupDefaultMocks(string sourceType, string targetType)
    {
        SetupFormatConfigProviderMock();
        SetupMappingRuleProviderMock();

        _mockValidator
            .Setup(v => v.Validate(It.IsAny<JObject>(), It.IsAny<string>(), It.IsAny<string>(), out It.Ref<string?>.IsAny))
            .Returns((JObject _, string _, string _, out string? error) =>
            {
                error = null;
                return true;
            });
    }


    #endregion

    #region ValidateInput Tests

    [Test, TestCaseSource(typeof(MappingTestCases), nameof(MappingTestCases.MissingSourceFormatTestCases))]
    public void Map_WhenSourceFormatIsMissing_ThrowsMappingException(JObject input,
    string sourceType,
    string targetType,
    string expectedErrorMessage)
    {

        SetupFormatConfigProviderMock();
        
        var mapHandler = new MapHandler(
            _mockMappingRuleProvider.Object,
            [new DataFormatValidator()],
            _mockFormatConfigProvider.Object
        );

        var result = mapHandler.Map(input, sourceType, targetType);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Has.Exactly(1).Items);
            Assert.That(result.Errors[0].Code, Is.EqualTo(MappingErrorCodes.MissingFormat));
            Assert.That(result.Errors[0].Message, Does.Contain(expectedErrorMessage));
        });
    }

    [Test, TestCaseSource(typeof(MappingTestCases), nameof(MappingTestCases.InvalidInputCases))]
    public void Map_WhenInputValidationFails_ReturnsValidationError(
    JObject input,
    string sourceType,
    string targetType,
    string expectedErrorMessage)
    {
        _mapHandler = new MapHandler(
            _mockMappingRuleProvider.Object,
            [new DataFormatValidator()],
            _mockFormatConfigProvider.Object);

        SetupFormatConfigProviderMock();
        SetupMappingRuleProviderMock(); // ensure property mappings are in place

        // Act
        var result = _mapHandler.Map(input, sourceType, targetType);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0].Code, Is.EqualTo(MappingErrorCodes.ValidationFailed));
            Assert.That(result.Errors[0].Message, Does.Contain(expectedErrorMessage));
        });

    }
    #endregion

    #region PerformMapping Tests

    [Test, TestCaseSource(typeof(MappingTestCases), nameof(MappingTestCases.MappingSuccessCases))]
    public void Map_WhenMappingIsSuccessful_ReturnsMappedResult(
    JObject input,
    string sourceType,
    string targetType,
    JObject expectedMappedData)
    {
        SetupDefaultMocks(sourceType, targetType);

        var result = _mapHandler.Map(input, sourceType, targetType);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MappedData, Is.Not.Null);

        var actualMapped = result.MappedData?.ToString();
        var actualJObject = JObject.Parse(actualMapped);

        Assert.That(JToken.DeepEquals(actualJObject, expectedMappedData), Is.True, "Mapped data does not match expected.");
    }


    [Test, TestCaseSource(typeof(MappingTestCases), nameof(MappingTestCases.MappingRulesMissingCases))]
    public void Map_WhenMappingRulesAreMissing_ReturnsMappingNotFoundError(
    JObject input,
    string sourceType,
    string targetType)
    {
        SetupFormatConfigProviderMock();

        _mockMappingRuleProvider
            .Setup(x => x.GetPropertyMappings(sourceType, targetType))
            .Returns((List<PropertyMapping>?)null!);

        // Validator succeeds (we're not testing validation here)
        _mockValidator
            .Setup(v => v.Validate(It.IsAny<JObject>(), It.IsAny<string>(), It.IsAny<string>(), out It.Ref<string?>.IsAny))
            .Returns((JObject _, string _, string _, out string? error) =>
            {
                error = null;
                return true;
            });

        var result = _mapHandler.Map(input, sourceType, targetType);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Has.Exactly(1).Items);
            Assert.That(result.Errors[0].Code, Is.EqualTo(MappingErrorCodes.MappingRuleNotFound));
        });
    }

    #endregion

    #region ValidateOutput Tests

    [Test, TestCaseSource(typeof(MappingTestCases), nameof(MappingTestCases.OutputValidationFailureTestCases))]
    public void Map_WhenOutputValidationFails_ReturnsValidationError(
    JObject input,
    string sourceType,
    string targetType,
    JObject mappedOutput,
    string expectedErrorMessage)
    {

        SetupFormatConfigProviderMock();

        _mockMappingRuleProvider
        .Setup(x => x.GetPropertyMappings(sourceType, targetType))
        .Returns(new List<PropertyMapping>
                {
                    new PropertyMapping { SourceProperty = "BookingId", TargetProperty = "Id" },
                    new PropertyMapping { SourceProperty = "CustomerName", TargetProperty = "Customer.Name" }
                });

        _mockValidator
            .Setup(v => v.Validate(
                It.Is<JObject>(j => JToken.DeepEquals(j, mappedOutput)),
                It.IsAny<string>(),
                targetType,
                out It.Ref<string?>.IsAny))
            .Returns((JObject _, string _, string _, out string? error) =>
            {
                error = expectedErrorMessage;
                return false;
            });

            var mapHandler = new MapHandler(
           _mockMappingRuleProvider.Object,
           [new DataFormatValidator()],
           _mockFormatConfigProvider.Object
           );

        var result = mapHandler.Map(input, sourceType, targetType);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Has.Exactly(1).Items);
            Assert.That(result.Errors[0].Code, Is.EqualTo(MappingErrorCodes.ValidationFailed));
            Assert.That(result.Errors[0].Message, Does.Contain(expectedErrorMessage));
        });
    }
    #endregion
}
