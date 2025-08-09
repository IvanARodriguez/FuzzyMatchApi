
using FuzzySearchApi.Core.Models;

namespace FuzzyMatchApi.Tests.TestData;

public static class TestDataHelper
{
    public static List<LocationRecord> GetTestLocationRecords()
    {
        return
        [
            new("L301", "123 Main Street", "New York", "NY", "10001", "TRIPPLE A TRANS LLC"),
            new("A245", "123 Main Street", "New York", "NY", "10001", "GLOBAL SHIPPING CO"),
            new("B138", "9726 Rose Street", "Fresno", "CO", "68720", "EASTERN TRANSPORT"),
            new("C456", "456 Oak Avenue", "Los Angeles", "CA", "90210", "FAST DELIVERY INC"),
            new("D789", "789 Pine Road", "Chicago", "IL", "60601", "RELIABLE LOGISTICS"),
            new("E123", "321 Elm Street", "Houston", "TX", "77001", "EXPRESS TRANSPORT"),
            new("F654", "654 Cedar Lane", "Phoenix", "AZ", "85001", "PREMIER FREIGHT"),
            new("G987", "987 Maple Drive", "Philadelphia", "PA", "19101", "SUPERIOR SHIPPING"),
            new("H111", "111 First Street", "San Antonio", "TX", "78201", "ELITE CARRIERS"),
            new("I222", "222 Second Avenue", "San Diego", "CA", "92101", "RAPID TRANSIT LLC")
        ];
    }

    public static string GetTestCsvContent()
    {
        return @"code,street,city,state,zip,location_name
            L301,""123 Main Street"",""New York"",""NY"",""10001"",""TRIPPLE A TRANS LLC""
            A245,""123 Main Street"",""New York"",""NY"",""10001"",""GLOBAL SHIPPING CO""
            B138,""9726 Rose Street"",""Fresno"",""CO"",""68720"",""EASTERN TRANSPORT""
            C456,""456 Oak Avenue"",""Los Angeles"",""CA"",""90210"",""FAST DELIVERY INC""
            D789,""789 Pine Road"",""Chicago"",""IL"",""60601"",""RELIABLE LOGISTICS""";
    }

    public static LocationSearchRequest CreateSearchRequest(
        string? company = null,
        string? street = null,
        string? city = null,
        string? state = null)
    {
        return new LocationSearchRequest(company, street, city, state);
    }
}

