using CreationsForge.Core.Models.Configuration;

namespace CreationsForge.Core.Configuration.Interfaces;

public interface IApplicationConfigurationStore
{
    string ConfigurationPath { get; }

    ApplicationConfiguration Current { get; }

    void Load();

    void Save(ApplicationConfiguration configuration);
}
