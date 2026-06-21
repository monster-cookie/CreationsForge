using CreationsForge.DataValidationTests.Validation.Parsing;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests;

public class SpriggitYamlDocumentTests
{
    [Fact]
    [Trait("Category", "SpriggitYamlParsing")]
    public void FlattenScalars_ShouldPreserveTranslatedStringFieldShape()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".yaml");
        File.WriteAllText(
            path,
            """
            Name:
              TargetLanguage: English
              Values:
              - Language: German
                String: Alchemie
              - Language: English
                String: Alchemy
            """);

        try
        {
            var fields = SpriggitYamlDocument.Load(path).FlattenScalars();

            fields["Name.TargetLanguage"].ShouldBe("English");
            fields["Name.Count"].ShouldBe("2");
            fields["Name[0].Language"].ShouldBe("German");
            fields["Name[0].String"].ShouldBe("Alchemie");
            fields["Name[1].Language"].ShouldBe("English");
            fields["Name[1].String"].ShouldBe("Alchemy");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    [Trait("Category", "SpriggitYamlParsing")]
    public void FlattenScalars_ShouldPreserveRootObjectListFields()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".yaml");
        File.WriteAllText(
            path,
            """
            PerkTree:
            - FNAM: 0x0046554C
              PerkGridX: 1291848012
              ConnectionLineToIndices:
              - 2
              Index: 0
            - Perk: 0BE127:Skyrim.esm
              FNAM: 0x01000000
              ConnectionLineToIndices:
              - 3
              Index: 2
            """);

        try
        {
            var fields = SpriggitYamlDocument.Load(path).FlattenScalars();

            fields["PerkTree.Count"].ShouldBe("2");
            fields["PerkTree[0].FNAM"].ShouldBe("0x0046554C");
            fields["PerkTree[0].PerkGridX"].ShouldBe("1291848012");
            fields["PerkTree[0].ConnectionLineToIndices.Count"].ShouldBe("1");
            fields["PerkTree[0].ConnectionLineToIndices[0]"].ShouldBe("2");
            fields["PerkTree[0].Index"].ShouldBe("0");
            fields["PerkTree[1].Perk"].ShouldBe("0BE127:Skyrim.esm");
            fields["PerkTree[1].FNAM"].ShouldBe("0x01000000");
            fields["PerkTree[1].ConnectionLineToIndices.Count"].ShouldBe("1");
            fields["PerkTree[1].ConnectionLineToIndices[0]"].ShouldBe("3");
            fields["PerkTree[1].Index"].ShouldBe("2");
        }
        finally
        {
            File.Delete(path);
        }
    }
}
