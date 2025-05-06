# 🧭 Dynamic Mapping System

## 📜 Overview

To address the requirement of mapping data between internal C# data models and external partner-specific data models, I have designed and implemented an **extensible dynamic mapping system** using **.NET**.

The Mapping System is built to link data between C# models and models specific to partners in both directions. This system can check inputs and outputs, handle errors, and also have a provision for logging as part of an extendible system. 
It's also easy to expand with just a few changes to the code. To make it flexible and able to grow, the system uses a mix of design patterns.

## 🔧 Patterns Used

| Pattern            | Purpose                                                                 |
|--------------------|-------------------------------------------------------------------------|
| **Template Method** | Standardizes mapping workflow ( `ValidateInput`, `Map`, `ValidateOutput`) |
| **Factory** | Dynamically select between JSON or MongoDB providers                  |
| **Strategy** | Strategy for applying specific mapping based on object(flat, array).                  |


The system is built to be:
- **Scalable**: Supports the integration of new models and data formats with minimal changes to the existing codebase.
- **Maintainable**: Easily extendable to handle future changes, including adding new data formats or mapping rules, and mapping logic.
- **Adaptable**: Designed with flexibility in mind, allowing seamless integration of partner-specific data models without requiring modifications to internal models.

The dynamic mapping system ensures that data can flow smoothly between different systems, using flexible configurations like JSON and MongoDB as sources, and allows custom mappings and transformations when necessary.

---

## 📐 System Architecture

The system maps data between different structured JSON formats using external configuration files. It offers:

- **Rules** to transform the source data into the target format.
- **Checks** to ensure input and output JSON match format schemas.
- **Extensibility** to add new formats and systems (such as MongoDB, Amazon S3, Azure Blob).
- **Extensibility** to add mapping logic to support mapping different objects that are not yet supported.
- **Tests** to verify source/target validation and mapping accuracy.

- Follows a layered architecture:
  - `Application` layer orchestrates mapping logic.
  - `Domain` layer defines core rules and exceptions.
  - `Infrastructure` layer handles configuration storage (JSON file or MongoDB).
- The core mapping engine is driven by the **Template Method Pattern** via `BaseMapHandler`.
- Mapping configuration sources are injected dynamically using **Factory Pattern**, depending on environment or preference.
- The system accepts **pure JSON input** and produces **JSON output**, avoiding any dependency on internal data models.
- The mapping logic may grow in future, to support other objects. To support this, the mapping engine implements different strategies using **Strategy Pattern**.

---

## 🛠 Key Components

### Mapping Engine
- Responsible for the dynamic conversion between source and target models.
- Handles source model input, validation, mapping, and output to the target model.

### Validation Engine
- Validates data according to rules defined for both the source and target models.
- Ensures that any invalid data is flagged before further processing.

### Strategy Pattern For Different Object Type Mapping:
-Strategy to dynamically select and apply the correct mapping logic based on the structure of the input data
and mapping rules, enabling the system to handle different combinations for Flat and array objects.

### Factory Pattern for Data Providers
- A factory that selects between **JSON** and **MongoDB**-based providers based on configuration.
- Supports extension with other data providers in the future.

### Template Method Pattern
- Used for dynamic mapping with pre-defined steps like input validation, mapping, and output validation.
- Offers a template for adding new types of mapping steps, such as logging and analytics.

---

## 🏗 Key Classes and Methods

### 🎯 **Controller (MappingsController)**
- **Role**: Entry point for mapping requests via the HTTP API.
- **Methods**:
  - **Accepts**: `MapRequest` containing the source type, target type, version (optional), and data.
  - **Returns**: `MappingResult` with the result of the mapping operation.

### 📝 **Contracts**
- **MapRequest**:
  - Input Data Transfer Object (DTO) containing:
    - `sourceType`: The type of the source data.
    - `targetType`: The type of the target data.
    - `version`: (Optional) Version information.
    - `data`: The data to be mapped.
- **MappingResult**:
  - Output DTO containing the result of the mapping operation.

### 🧠 **MapHandler (Application.Mapping)**
- **Inherits from**: `BaseMapHandler`
- **Core Logic**:
  - **Mapping properties**: `PerformMapping`
  - **Validating input/output**: `ValidateInput`, `ValidateOutput`
- **Uses**:
  - **IMappingRuleProvider**: To get mapping rules.
  - **IFormatConfigProvider**: To fetch format definitions.
  - **IModelValidator**: For data validation.

### ✅ **Validators (Application.Validation)**
- Includes various validators such as:
  - **DataFormatValidator**: Ensures fields exist and meet criteria before/after mapping.

### 🛠 **Interfaces (Application.Interfaces)**
- **IMapHandler**: Defines the method for handling the mapping of data between different data models.
- **IFormatConfigProvider**: Fetches format definitions (e.g., `dataformat.json`) for validation purposes.
- **IMappingStrategy**: Represents a strategy interface for applying specific types of property mappings.
- **IFormatConfigProviderFactory**: Resolves the appropriate data format provider based on the format type (e.g., JSON, Mongo).
- **IMappingRuleProvider**: Defines the contract for all mapping providers. A provider is responsible for retrieving the mapping rules from the data source (JSON, MongoDB, etc.).
  - **Key Methods**:
    - `GetMappingRules()`: Returns a collection of mapping rules as defined in the external configuration file.
- **IMappingRuleProviderFactory**: Resolves the appropriate mapping rule provider based on the format type (e.g., JSON, Mongo).
- **IModelValidator**: Defines a contract for validating models represented as JSON objects.

### ❌ **Exceptions (Domain.Exceptions)**
- **MappingException**: Represents errors that occur during the mapping process.
- **InvalidInputFormatException**: Thrown when incoming data does not match the expected request type object.
- **MappingNotFoundException**: Thrown when mapping rules for the given source/target type are missing.
- **ValidationMappingException**: Thrown when data validation fails according to the format rules mentioned in `dataformat.json`.

### Strategy (Infrastructure.Strategy)
- Multiple Strategies for handling mappings from a JSON object to another JSON object. Like BookingId -> id, Name[]  -> Guest[*].Name etc.
This provides a clean and extendible system to support other objects in the future.

### 🔑 **Rules (Domain.Rules)**
- **MapperRule**: Represents an individual rule entry from `mappingrules.json`, including source/target property names and a list of property mappings.
- **MappingErrorCodes**: Central location for defining and maintaining error code constants, such as:
  - `MissingRequiredField`, `ValidationFailed`, `MissingSourceProperty`, `SetTargetPropertyFailed`, `MappingRuleNotFound`, `PropertyMappingFailed`, `UnexpectedError`.

### 📦 **Mapping Config Providers (Infrastructure.Providers)**
- **JsonFormatConfigProvider**: Loads format from `dataformat.json`.
- **JsonMappingRuleProvider**: Implements the `IMappingProvider` interface to retrieve mapping rules from a JSON source.
  - **Key Methods**:
    - `GetMappingRules()`: Reads mapping rules from a JSON file and deserializes them into an internal structure.
- **MongoFormatConfigProvider**: Similar to `JsonFormatConfigProvider`, but extends functionality to support MongoDB-based configuration sources.
- **MongoMappingRuleProvider**: Implements the `IMappingProvider` interface to retrieve mapping rules from a MongoDB source, making the system extensible.

### Map Handler Test Cases (DynamicMappingSystem.Tests)
-This test suite verifies the functionality of the MapHandler, ensuring correct data transformation across various mapping scenarios. The tests also cover edge cases such as missing mapping rules, format definitions, and invalid inputs, ensuring robustness and reliability of the system.

### 📂 **Configuration Files (Resources.MappingRules)**
- **dataformat.json**: Used to validate the source data before mapping and the target data after mapping has been completed.
- **mappingrule.json**: Used to execute the mapping process by following the defined rules that align properties between a specified source type and target type.

---

## ✨ Features

- 🔁 JSON-in/JSON-out dynamic mapping
- 🔎 Input and output validation using FluentValidation
- 📦 Pluggable configuration source (JSON or MongoDB)
- 🔧 Boilerplate for custom converters (e.g., DateTime, Currency)
- ❗ Robust error handling with extendable logic
- 🧪 Unit testing for validation, mapping, and config provider logic

---

## 🚀 Getting Started

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-org/dynamic-mapping-system.git
   cd dynamic-mapping-system
   ## ⚙️ Setup Configuration

- Add your `mappingrules.json` and `dataformat.json` in the appropriate location.
  Or configure MongoDB and implement the Mongo-based `IMappingConfigurationProvider`.

---

## 🚀 Run the Service

- Can be hosted as an **ASP.NET Core API**

---

## 🧩 Extending the System

### 📄 Adding New Mappings via JSON Files

You can define new property mappings in the `mappingrules.json` and `dataformat.json` files located in the `Resources/MappingRules` directory. These files follow a consistent structure specifying `SourceType`, `TargetType`, and `PropertyMappings`.

### Update `mappingrules.json`
Define how fields map between the source and target:

```json
{
  "SourceType": "NewSource.Type",
  "TargetType": "NewTarget.Type",
  "PropertyMappings": [
    {
      "SourceProperty": "Field1",
      "TargetProperty": "TargetField1"
    },
    {
      "SourceProperty": "Field2.SubField",
      "TargetProperty": "TargetField2"
    }
  ]
}
```

### Update `dataformat.json`
Define required fields for new source and target types:

```json
[
  {
    "Type": "NewSourceType",
    "Properties": ["Field1", "Field2.SubField"]
  },
  {
    "Type": "NewTargetType",
    "Properties": ["Field1", "Field2"]
  }
]
```

## 🗂 Storing Mappings in Other Sources

The system supports pluggable backends via providers, allowing mappings to be stored in various external sources.

### 🗃 MongoDB

- Implemented via the `IMappingRuleProvider` interface.
- Store mappings as documents in a MongoDB collection.
- Mappings are loaded dynamically at runtime, enabling centralized and real-time updates without needing to restart the service.

### ☁️ Amazon S3 / Azure Blob Storage

- Store mapping JSON files in buckets (Amazon S3) or blob containers (Azure Blob Storage).
- Extend the `IMappingRuleProvider` interface to fetch and cache mappings from cloud storage.
- Ideal for distributed or multi-region deployments, ensuring high availability and scalability of your mappings.

## 🔄 Adding Custom Converters (e.g., Currency Conversion)

Although this functionality is not yet fully implemented, the system is designed with **extensibility** in mind and includes boilerplate support for adding custom converters. These converters handle partner-specific data transformations.

For example, some partner systems may submit prices in different currencies (such as "USD"), whereas DIRS expects all prices to be in a standard currency. This can be supported by introducing a custom converter to:

1. **Read the amount and currency fields** from input JSON (e.g., `Price.Amount`, `Price.Currency`).
2. **Convert the amount** to the target currency using a predefined exchange rate or an external service.
3. **Return the normalized value** as part of the mapping process.

> ⚠️ **Note**: This functionality is still under development, but the boilerplate code to add custom converters is already in place.

### 🧱 Add a New Data Format Configuration Provider

- Implement the `IFormatConfigProvider` interface
- Register your provider via dependency injection using the `FormatConfigProviderFactory`
  
### 🛠 Add a New MappingRule Provider

- Implement the `IMappingRuleProvider` interface
- Register your provider via dependency injection using the `MappingRuleProviderFactory`
  
### Support for Versioned Mappings

The system can be extended to support version-specific mapping rules, allowing different transformations between the same source and target types based on a version field. This is useful in scenarios such as:
- Partners or APIs evolving over time with updated field names or formats.
- Backward compatibility needs to be maintained.
- Incremental mapping changes without breaking existing consumers.

The existing `version` parameter is already part of the method signature and data payload. To incorporate version-aware logic in the `IMappingRuleProvider` (e.g., to fetch versioned mapping rules) would require minimal code changes, making the system more adaptable and robust in dynamic integration environments.

### 🛠 Adding New Validators

The system uses a **pluggable validation mechanism** via the `IModelValidator` interface to ensure both source and target data conform to expected formats and rules before and after mapping.

#### Default Validator: `DataFormatValidator`

- Validates the **presence** of properties defined in `DataFormat.json`.
- Ensures **strict compliance** with allowed fields for both source and target types.
- Reports **detailed error messages** when properties are missing or extra.

#### Adding a New Validator

To add custom validation logic (e.g., regex matching, value range checks, conditional rules, or any other product-specific checks):

1. **Implement the `IModelValidator` interface**.
2. **Register the Validator** in Dependency Injection.
3. Ensure the validator is included in the `IEnumerable<IModelValidator>` injected into `MapHandler`.

#### Automatic Execution

All registered validators will automatically run during the `ValidateInput` and `ValidateOutput` steps, as shown in the `MapHandler`.


---

## ⚠️ Assumptions

- **Flat or Simple Nested Structures with Array/List**  
  The system assumes that all source and target properties follow flat or single-level nested paths, or an 
  array object 
  (e.g., `Customer.Name`, `Customer.Address.City`, `"Guests": [{ "Name": "John Doe", "Age": 35 },{ "Name": 
  "Jane Roe", "Age": 30 }]`).

- **Valid and Pre-validated JSON Input**  
  It is assumed that incoming JSON conforms to expected structural conventions.  
  Unless a specific validator is implemented, the system verifies the existence of fields—but not their data type.

- **Property Paths Are Dot-Separated**  
  Mapping rules and format definitions use dot-separated paths to access nested properties (e.g., `Parent.Child.Property`) and support array indexing using wildcard notation (e.g., `Guest[*].Name`). This structure aligns with `JObject.SelectToken` to allow accurate and flexible data traversal and transformation.


- **Data Providers Are Configured for Extensibility**  
  Both JSON and MongoDB are pre-configured as data providers.  
  New providers can be added with minimal code changes.

- **Model-Free Architecture**  
  The system does not use any strongly-typed internal C# models during the mapping process.  
  All operations are performed on dynamic JSON input/output, allowing for maximum integration flexibility without binding to internal schemas.

---

## 🚧 Potential Limitations

- **No UI for Rule Management**  
  Mapping rules must be manually written and maintained in JSON.  
  There is currently no user interface for creating, editing, or validating mapping rules.

- **No Recursive Object Handling**  
  Mapping rules and format definitions support dot-separated paths for accessing nested properties (e.g., `Parent.Child.Property`) and array handling using wildcard notation (e.g., `Guest[*].Name`). While the system handles nested objects and arrays effectively, it intentionally avoids supporting deeply recursive or highly nested object graphs (e.g., `A.B.C.D.E.F.G`) to maintain clarity, performance, and predictability.


--

## 🧪 Testing

- Unit tests are included for:
  - ✅ Input and output validation
  - ✅ Mapping rule execution

- Built using:
  - [`NUnit`](https://nunit.org/) — test framework
  - [`Moq`](https://github.com/moq/moq4) — mocking framework

---
