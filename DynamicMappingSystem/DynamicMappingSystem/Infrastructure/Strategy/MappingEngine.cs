using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using DynamicMappingSystem.Infrastructure.Helpers;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Strategy
{
    /// <summary>
    /// The MappingEngine class is responsible for managing and applying different mapping strategies
    /// to map data between source and target objects based on the provided property mappings.
    /// </summary>
    public class MappingEngine
    {
        private readonly List<IMappingStrategy> _strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingEngine"/> class with the specified strategies.
        /// </summary>
        /// <param name="strategies">A collection of strategies to apply during the mapping process.</param>
        public MappingEngine(IEnumerable<IMappingStrategy> strategies)
        {
            _strategies = strategies.ToList();
        }

        /// <summary>
        /// Maps the source data to the target format based on the provided property mappings.
        /// </summary>
        /// <param name="mappings">The property mappings that define how to transform the source data to the target.</param>
        /// <param name="inputData">The source data to be mapped to the target format.</param>
        /// <returns>A <see cref="JObject"/> containing the mapped target data.</returns>
        /// <exception cref="MappingException">Thrown if no strategy is found to handle a mapping group.</exception>
        public JObject Map(IEnumerable<PropertyMapping> mappings, JObject inputData)
        {
            var result = new JObject();

            // Group mappings by source or target collection paths
            var groupedMappings = mappings
                .Where(m => m.IsCollection == true &&
                           (m.SourceProperty.Contains("[*]") || m.TargetProperty.Contains("[*]")))
                .GroupBy(m =>
                    m.SourceProperty.Contains("[*]")
                        ? m.SourceProperty.Split("[*].")[0]
                        : m.TargetProperty.Split("[*].")[0]);

            // Apply strategies to the grouped mappings
            foreach (var group in groupedMappings)
            {
                var sourcePath = MappingHelper.NormalizeSourcePath(inputData, group.First().SourceProperty)
                    .Split(new[] { "[*]." }, StringSplitOptions.None)[0];
                var sourceToken = inputData.SelectToken(sourcePath);

                if (sourceToken == null) continue;

                // Find the strategy that can handle the current mapping group
                var strategy = _strategies.FirstOrDefault(s => s.CanHandle(group, sourceToken));
                if (strategy == null)
                    throw new MappingException(MappingErrorCodes.InvalidData, $"No strategy for mapping group: {sourcePath}");

                // Apply the strategy to the mapping group
                strategy.Apply(group, sourceToken, result, inputData);
            }

            // Handle scalar/flat mappings separately with a specific strategy
            var scalarMappings = mappings.Where(m => m.IsCollection != true);
            var flatStrategy = _strategies.OfType<FlatToFlatObjectMappingStrategy>().First();
            flatStrategy.Apply(scalarMappings, inputData, result);

            return result;
        }
    }
}
