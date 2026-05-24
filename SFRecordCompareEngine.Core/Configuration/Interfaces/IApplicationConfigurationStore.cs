using SFRecordCompareEngine.Core.Models.Configuration;

namespace SFRecordCompareEngine.Core.Configuration.Interfaces;

public interface IApplicationConfigurationStore
{
    string ConfigurationPath { get; }

    ApplicationConfiguration Current { get; }

    bool IsConfigurationRequired { get; }

    void Load();

    void Save(ApplicationConfiguration configuration);
}
