using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMappingSystemTests
{
    public static class MappingTestCases
    {
        public static IEnumerable<TestCaseData> InvalidInputCases()
        {
            yield return new TestCaseData(
                JObject.Parse(@"{
                'CustomerName': 'John Doe'
            }"), // Missing BookingId
                "Google.Reservation",
                "Model.Reservation",
                "The property: BookingId is missing in the source data."
            ).SetName("Missing_BookingId_ShouldFailValidation");
        }

        public static IEnumerable<TestCaseData> ValidInputCases()
        {
            yield return new TestCaseData(
                JObject.Parse(@"{
                'BookingId': 'R123',
                'CustomerName': 'John Doe'
            }"),
                "Google.Reservation",
                "Model.Reservation"
            ).SetName("ValidInput_ShouldPassValidation");
        }

        public static IEnumerable<TestCaseData> MissingSourceFormatTestCases
        {
            get
            {
                yield return new TestCaseData(
                    JObject.Parse(@"{
                        'BookingId': 'R123',
                        'CustomerName': 'John Doe',
                        'CustomerEmail': 'ss.com',
                        'City': 'London',
                        'Zip': '465856',
                        'AdditionalDetails': {
                            'Landmark': 'Airport'
                        },
                        'ArrivalDate': '2025-05-10',
                        'DepartureDate': '2025-05-15'
                    }"),
                    "Unknown.SourceType",
                    "Model.Reservation",
                    "Format definition not found for type 'Unknown.SourceType'"
                ).SetName("MissingSourceFormat_ShouldThrowException");
            }
        }

        public static IEnumerable<TestCaseData> MappingSuccessCases()
        {
            yield return new TestCaseData(
                JObject.Parse(@"{
                    'BookingId': 'R123',
                    'CustomerName': 'John Doe',
                    'CustomerEmail': 'ss.com',
                    'City': 'London',
                    'Zip': '465856',
                    'AdditionalDetails': {
                        'Landmark': 'Airport'
                    },
                    'ArrivalDate': '2025-05-10',
                    'DepartureDate': '2025-05-15'
                }"),
                "Google.Reservation",  // Source type
                "Model.Reservation",  // Target type
                JObject.Parse(@"{
                    'Id': 'R123',
                    'Customer': {
                        'Name': 'John Doe',
                        'Email': 'ss.com',
                        'Address': {
                            'City': 'London',
                            'PostalCode': '465856',
                            'Landmark': 'Airport'
                        }
                    },
                    'CheckInDate': '2025-05-10',
                    'CheckOutDate': '2025-05-15'
                }")
            ).SetName("ValidMapping_FullInput_ShouldReturnMappedResult");
        }

        public static IEnumerable<TestCaseData> MappingRulesMissingCases()
        {
            yield return new TestCaseData(
                JObject.Parse(@"{
                    'BookingId': 'R123',
                    'CustomerName': 'John Doe',
                    'CustomerEmail': 'ss.com',
                    'City': 'London',
                    'Zip': '465856',
                    'AdditionalDetails': {
                        'Landmark': 'Airport'
                    },
                    'ArrivalDate': '2025-05-10',
                    'DepartureDate': '2025-05-15'
                }"),
                "Google.Reservation",  // Source type
                "Model.Reservation"
            ).SetName("ValidInput_NoMappingRule_ShouldThrowMappingRuleNotFoundException");
        }

        public static IEnumerable<TestCaseData> OutputValidationFailureTestCases
        {
            get
            {
                yield return new TestCaseData(
                    JObject.Parse(@"{
                        'BookingId': 'R123',
                        'CustomerName': 'John Doe',
                        'CustomerEmail': 'ss.com',
                        'City': 'London',
                        'Zip': '465856',
                        'AdditionalDetails': {
                            'Landmark': 'Airport'
                        },
                        'ArrivalDate': '2025-05-10',
                        'DepartureDate': '2025-05-15'
                    }"),
                    "Google.Reservation",
                    "Model.Reservation",
                    JObject.Parse(@"{
                    'Id': 'R123',
                    'Customer': {
                        'Name': 'John Doe',
                        'CustomerEmail': 'ss.com',
                        'Address': {
                            'City': 'London',
                            'PostalCode': '465856',
                            'Landmark': 'Airport'
                        }
                    },
                    'CheckInDate': '2025-05-10',
                    'CheckOutDate': '2025-05-15'
                }"),
                    "The property: Customer.Email is missing in the target data."
                ).SetName("OutputValidationFails_MissingField");
            }
        }

        public static IEnumerable<TestCaseData> MappingSuccessCasesArrayObjectTestCases()
        {
            yield return new TestCaseData(
                JObject.Parse(@"{
                    'Names': ['John', 'Jane'],
                    'Emails': ['john@example.com', 'jane@example.com'],
                    'Cities': ['London', 'New York']
                }"),
                "Model.Reservation",  // Source type
                "Google.Reservation",  // Target type
                JObject.Parse(@"{
                    'People': [
                        {
                            'Name': 'John',
                            'Email': 'john@example.com',
                            'City': 'London'
                        },
                        {
                            'Name': 'Jane',
                            'Email': 'jane@example.com',
                            'City': 'New York'
                        }
                    ]
                }")
            ).SetName("ValidArrayMapping_ShouldReturnMappedResult");
        }

    }
}
