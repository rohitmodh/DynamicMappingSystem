using DynamicMappingSystem.Mapping;

namespace DynamicMappingSystem.Providers
{
    public interface IMappingRuleProvider
    {
        List<MapperRule> GetRules();
    }
}
