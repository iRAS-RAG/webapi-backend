using AutoMapper;
using IRasRag.Application.Common.Mappings;
using Microsoft.Extensions.Logging;

namespace IRasRag.Test.UnitTests.Helpers
{
    public static class AutoMapperTestHelper
    {
        /// <summary>
        /// Creates a real IMapper instance configured with your application's mapping profiles
        /// </summary>
        public static IMapper GetMapper(params Profile[] profiles)
        {
            var loggerFactory = LoggerFactory.Create(builder => { });
            var configuration = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            },
            loggerFactory
            );

            // Validate the configuration
            configuration.AssertConfigurationIsValid();

            return configuration.CreateMapper();
        }
    }
}
