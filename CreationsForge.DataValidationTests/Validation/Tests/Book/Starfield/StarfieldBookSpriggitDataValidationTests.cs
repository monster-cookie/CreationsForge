using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Starfield;

public class StarfieldBookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "165BF3:Starfield.esm")]
    [Trait("EditorID", "NH_SouvenirSlate")]
    [Trait("SpriggitFile", "Books/NH_SouvenirSlate - 165BF3_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_NH_SouvenirSlate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "NH_SouvenirSlate");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "165BF3:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft.Count").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft.Count"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[0].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[0].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[0].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[0].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[1].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[1].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[1].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[1].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[2].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[2].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[2].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[2].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[3].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[3].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[3].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[3].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[4].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[4].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[4].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[4].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[5].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[5].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[5].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[5].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[6].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[6].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[6].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[6].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[7].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[7].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[7].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[7].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[8].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[8].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[8].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[8].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight.Count").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight.Count"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[0].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[0].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[0].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[0].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[1].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[1].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[1].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[1].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[2].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[2].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[2].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[2].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[3].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[3].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[3].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[3].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[4].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[4].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[4].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[4].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[5].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[5].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[5].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[5].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[6].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[6].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[6].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[6].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[7].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[7].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[7].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[7].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[8].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[8].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[8].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[8].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateType").ShouldBe(Helpers.GetDTOField(dto, "DataSlateType"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "InventoryArt").ShouldBe(Helpers.GetDTOField(dto, "InventoryArt"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Teaches.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Teaches.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Teaches.RawContent").ShouldBe(Helpers.GetDTOField(dto, "Teaches.RawContent"));
        Helpers.GetSpriggitField(spriggit, "Text.Count").ShouldBe(Helpers.GetDTOField(dto, "Text.Count"));
        Helpers.GetSpriggitField(spriggit, "Text.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Text.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[0].SOL").ShouldBe(Helpers.GetDTOField(dto, "Text[0].SOL"));
        Helpers.GetSpriggitField(spriggit, "Text[0].String").ShouldBe(Helpers.GetDTOField(dto, "Text[0].String"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Zuerst schickten die Erdbewohner im Jahr 2107 ein unbemanntes Schiff zum Titan, einem der Saturnmonde. Für die Suche nach Leben jenseits der Erde wurde im Jahr 2130 der Bau der Titan-Astrobasis abgeschlossen. Auch wenn die Suche ergebnislos blieb, war dies das Fundament für die größte Unternehmung der Menschheit").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Zuerst schickten die Erdbewohner im Jahr 2107 ein unbemanntes Schiff zum Titan, einem der Saturnmonde. Für die Suche nach Leben jenseits der Erde wurde im Jahr 2130 der Bau der Titan-Astrobasis abgeschlossen. Auch wenn die Suche ergebnislos blieb, war dies das Fundament für die größte Unternehmung der Menschheit"));
        Helpers.GetSpriggitField(spriggit, "Text[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[1].SOL").ShouldBe(Helpers.GetDTOField(dto, "Text[1].SOL"));
        Helpers.GetSpriggitField(spriggit, "Text[1].String").ShouldBe(Helpers.GetDTOField(dto, "Text[1].String"));
        Helpers.GetSpriggitField(spriggit, "Text[1].The people of Earth first sent an unmanned craft to Saturn's moon, Titan, in 2107. The Titan Astrobase, was completed in 2130 to search for life beyond Earth. Though fruitless, it laid the groundwork for what would eventually become humanity's greatest endeavor").ShouldBe(Helpers.GetDTOField(dto, "Text[1].The people of Earth first sent an unmanned craft to Saturn's moon, Titan, in 2107. The Titan Astrobase, was completed in 2130 to search for life beyond Earth. Though fruitless, it laid the groundwork for what would eventually become humanity's greatest endeavor"));
        Helpers.GetSpriggitField(spriggit, "Text[2].¡Contempla los majestuosos picos helados de las formaciones de tierra congelada de Nueva Hacienda mientras los vientos helados soplan a través de las espesas nubes en lo alto! ¡Sé testigo de la potencia de las plantas de procesamiento de metano más valiosas de las CU! Luego, entra en calor con una comida caliente en el restaurante más antiguo de la humanidad después de la Tierra").ShouldBe(Helpers.GetDTOField(dto, "Text[2].¡Contempla los majestuosos picos helados de las formaciones de tierra congelada de Nueva Hacienda mientras los vientos helados soplan a través de las espesas nubes en lo alto! ¡Sé testigo de la potencia de las plantas de procesamiento de metano más valiosas de las CU! Luego, entra en calor con una comida caliente en el restaurante más antiguo de la humanidad después de la Tierra"));
        Helpers.GetSpriggitField(spriggit, "Text[2].La gente de la Tierra envió por primera vez una nave no tripulada a la luna de Saturno, Titán, en 2107. La astrobase Titán se completó en 2130 para buscar vida más allá de la Tierra. Aunque fue una empresa infructuosa, sentó las bases para lo que eventualmente acabaría convirtiéndose en el mayor esfuerzo de la humanidad").ShouldBe(Helpers.GetDTOField(dto, "Text[2].La gente de la Tierra envió por primera vez una nave no tripulada a la luna de Saturno, Titán, en 2107. La astrobase Titán se completó en 2130 para buscar vida más allá de la Tierra. Aunque fue una empresa infructuosa, sentó las bases para lo que eventualmente acabaría convirtiéndose en el mayor esfuerzo de la humanidad"));
        Helpers.GetSpriggitField(spriggit, "Text[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[2].SOL").ShouldBe(Helpers.GetDTOField(dto, "Text[2].SOL"));
        Helpers.GetSpriggitField(spriggit, "Text[2].String").ShouldBe(Helpers.GetDTOField(dto, "Text[2].String"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Les habitants de la Terre envoyèrent pour la première fois un appareil inhabité sur la lune de Saturne, Titan, en 2107. La base astrale de Titan fut achevée en 2130 pour participer à la recherche d'une forme de vie au-delà de la Terre. Bien que cette mission n'ait pas abouti, elle posa les bases du plus grand accomplissement de l'histoire de l'humanité").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Les habitants de la Terre envoyèrent pour la première fois un appareil inhabité sur la lune de Saturne, Titan, en 2107. La base astrale de Titan fut achevée en 2130 pour participer à la recherche d'une forme de vie au-delà de la Terre. Bien que cette mission n'ait pas abouti, elle posa les bases du plus grand accomplissement de l'histoire de l'humanité"));
        Helpers.GetSpriggitField(spriggit, "Text[3].SOL").ShouldBe(Helpers.GetDTOField(dto, "Text[3].SOL"));
        Helpers.GetSpriggitField(spriggit, "Text[3].String").ShouldBe(Helpers.GetDTOField(dto, "Text[3].String"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Nel 2107, gli abitanti della Terra inviarono per la prima volta un mezzo senza equipaggio su Titano, la luna di Saturno. L’Astrobase Titano fu completata nel 2130 per cercare forme di vita oltre la Terra. Benché senza successo, questa operazione gettò le basi per quello che sarebbe un giorno diventata la più grande impresa dell’umanità").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Nel 2107, gli abitanti della Terra inviarono per la prima volta un mezzo senza equipaggio su Titano, la luna di Saturno. L’Astrobase Titano fu completata nel 2130 per cercare forme di vita oltre la Terra. Benché senza successo, questa operazione gettò le basi per quello che sarebbe un giorno diventata la più grande impresa dell’umanità"));
        Helpers.GetSpriggitField(spriggit, "Text[4].SISTEMA SOLARE").ShouldBe(Helpers.GetDTOField(dto, "Text[4].SISTEMA SOLARE"));
        Helpers.GetSpriggitField(spriggit, "Text[4].String").ShouldBe(Helpers.GetDTOField(dto, "Text[4].String"));
        Helpers.GetSpriggitField(spriggit, "Text[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[5].String").ShouldBe(Helpers.GetDTOField(dto, "Text[5].String"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Mieszkańcy Ziemi po raz pierwszy wysłali bezzałogowy statek na księżyc Saturna - Tytan - w roku 2107. Astrobaza Tytan została ukończona w roku 2130 i początkowo miała szukać życia poza Ziemią. Choć poszukiwania były bezowocne, baza stanowiła podwaliny tego, co ostatecznie stało się największym przedsięwzięciem w historii ludzkości").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Mieszkańcy Ziemi po raz pierwszy wysłali bezzałogowy statek na księżyc Saturna - Tytan - w roku 2107. Astrobaza Tytan została ukończona w roku 2130 i początkowo miała szukać życia poza Ziemią. Choć poszukiwania były bezowocne, baza stanowiła podwaliny tego, co ostatecznie stało się największym przedsięwzięciem w historii ludzkości"));
        Helpers.GetSpriggitField(spriggit, "Text[6].SOL").ShouldBe(Helpers.GetDTOField(dto, "Text[6].SOL"));
        Helpers.GetSpriggitField(spriggit, "Text[6].String").ShouldBe(Helpers.GetDTOField(dto, "Text[6].String"));
        Helpers.GetSpriggitField(spriggit, "Text[7].De início, as pessoas da Terra enviaram uma espaçonave não tripulada para a lua de Saturno, Titã, em 2107. A astrobase Titã foi concluída em 2130 com o intuito de procurar vida além da Terra. Embora infrutífero, foi responsável por pavimentar o que eventualmente se tornaria a maior empreitada da humanidade").ShouldBe(Helpers.GetDTOField(dto, "Text[7].De início, as pessoas da Terra enviaram uma espaçonave não tripulada para a lua de Saturno, Titã, em 2107. A astrobase Titã foi concluída em 2130 com o intuito de procurar vida além da Terra. Embora infrutífero, foi responsável por pavimentar o que eventualmente se tornaria a maior empreitada da humanidade"));
        Helpers.GetSpriggitField(spriggit, "Text[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[7].O SISTEMA SOLAR").ShouldBe(Helpers.GetDTOField(dto, "Text[7].O SISTEMA SOLAR"));
        Helpers.GetSpriggitField(spriggit, "Text[7].String").ShouldBe(Helpers.GetDTOField(dto, "Text[7].String"));
        Helpers.GetSpriggitField(spriggit, "Text[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[8].String").ShouldBe(Helpers.GetDTOField(dto, "Text[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "XALG").ShouldBe(Helpers.GetDTOField(dto, "XALG"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DataSlateHeaderLeft.Count", "DataSlateHeaderLeft.TargetLanguage", "DataSlateHeaderLeft[0].Language", "DataSlateHeaderLeft[0].String", "DataSlateHeaderLeft[1].Language", "DataSlateHeaderLeft[1].String", "DataSlateHeaderLeft[2].Language", "DataSlateHeaderLeft[2].String", "DataSlateHeaderLeft[3].Language", "DataSlateHeaderLeft[3].String", "DataSlateHeaderLeft[4].Language", "DataSlateHeaderLeft[4].String", "DataSlateHeaderLeft[5].Language", "DataSlateHeaderLeft[5].String", "DataSlateHeaderLeft[6].Language", "DataSlateHeaderLeft[6].String", "DataSlateHeaderLeft[7].Language", "DataSlateHeaderLeft[7].String", "DataSlateHeaderLeft[8].Language", "DataSlateHeaderLeft[8].String", "DataSlateHeaderRight.Count", "DataSlateHeaderRight.TargetLanguage", "DataSlateHeaderRight[0].Language", "DataSlateHeaderRight[0].String", "DataSlateHeaderRight[1].Language", "DataSlateHeaderRight[1].String", "DataSlateHeaderRight[2].Language", "DataSlateHeaderRight[2].String", "DataSlateHeaderRight[3].Language", "DataSlateHeaderRight[3].String", "DataSlateHeaderRight[4].Language", "DataSlateHeaderRight[4].String", "DataSlateHeaderRight[5].Language", "DataSlateHeaderRight[5].String", "DataSlateHeaderRight[6].Language", "DataSlateHeaderRight[6].String", "DataSlateHeaderRight[7].Language", "DataSlateHeaderRight[7].String", "DataSlateHeaderRight[8].Language", "DataSlateHeaderRight[8].String", "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].SOL", "Text[0].String", "Text[0].Zuerst schickten die Erdbewohner im Jahr 2107 ein unbemanntes Schiff zum Titan, einem der Saturnmonde. Für die Suche nach Leben jenseits der Erde wurde im Jahr 2130 der Bau der Titan-Astrobasis abgeschlossen. Auch wenn die Suche ergebnislos blieb, war dies das Fundament für die größte Unternehmung der Menschheit", "Text[1].Language", "Text[1].SOL", "Text[1].String", "Text[1].The people of Earth first sent an unmanned craft to Saturn's moon, Titan, in 2107. The Titan Astrobase, was completed in 2130 to search for life beyond Earth. Though fruitless, it laid the groundwork for what would eventually become humanity's greatest endeavor", "Text[2].¡Contempla los majestuosos picos helados de las formaciones de tierra congelada de Nueva Hacienda mientras los vientos helados soplan a través de las espesas nubes en lo alto! ¡Sé testigo de la potencia de las plantas de procesamiento de metano más valiosas de las CU! Luego, entra en calor con una comida caliente en el restaurante más antiguo de la humanidad después de la Tierra", "Text[2].La gente de la Tierra envió por primera vez una nave no tripulada a la luna de Saturno, Titán, en 2107. La astrobase Titán se completó en 2130 para buscar vida más allá de la Tierra. Aunque fue una empresa infructuosa, sentó las bases para lo que eventualmente acabaría convirtiéndose en el mayor esfuerzo de la humanidad", "Text[2].Language", "Text[2].SOL", "Text[2].String", "Text[3].Language", "Text[3].Les habitants de la Terre envoyèrent pour la première fois un appareil inhabité sur la lune de Saturne, Titan, en 2107. La base astrale de Titan fut achevée en 2130 pour participer à la recherche d'une forme de vie au-delà de la Terre. Bien que cette mission n'ait pas abouti, elle posa les bases du plus grand accomplissement de l'histoire de l'humanité", "Text[3].SOL", "Text[3].String", "Text[4].Language", "Text[4].Nel 2107, gli abitanti della Terra inviarono per la prima volta un mezzo senza equipaggio su Titano, la luna di Saturno. L’Astrobase Titano fu completata nel 2130 per cercare forme di vita oltre la Terra. Benché senza successo, questa operazione gettò le basi per quello che sarebbe un giorno diventata la più grande impresa dell’umanità", "Text[4].SISTEMA SOLARE", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].Mieszkańcy Ziemi po raz pierwszy wysłali bezzałogowy statek na księżyc Saturna - Tytan - w roku 2107. Astrobaza Tytan została ukończona w roku 2130 i początkowo miała szukać życia poza Ziemią. Choć poszukiwania były bezowocne, baza stanowiła podwaliny tego, co ostatecznie stało się największym przedsięwzięciem w historii ludzkości", "Text[6].SOL", "Text[6].String", "Text[7].De início, as pessoas da Terra enviaram uma espaçonave não tripulada para a lua de Saturno, Titã, em 2107. A astrobase Titã foi concluída em 2130 com o intuito de procurar vida além da Terra. Embora infrutífero, foi responsável por pavimentar o que eventualmente se tornaria a maior empreitada da humanidade", "Text[7].Language", "Text[7].O SISTEMA SOLAR", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "XALG");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DataSlateHeaderLeft.Count", "DataSlateHeaderLeft.TargetLanguage", "DataSlateHeaderLeft[0].Language", "DataSlateHeaderLeft[0].String", "DataSlateHeaderLeft[1].Language", "DataSlateHeaderLeft[1].String", "DataSlateHeaderLeft[2].Language", "DataSlateHeaderLeft[2].String", "DataSlateHeaderLeft[3].Language", "DataSlateHeaderLeft[3].String", "DataSlateHeaderLeft[4].Language", "DataSlateHeaderLeft[4].String", "DataSlateHeaderLeft[5].Language", "DataSlateHeaderLeft[5].String", "DataSlateHeaderLeft[6].Language", "DataSlateHeaderLeft[6].String", "DataSlateHeaderLeft[7].Language", "DataSlateHeaderLeft[7].String", "DataSlateHeaderLeft[8].Language", "DataSlateHeaderLeft[8].String", "DataSlateHeaderRight.Count", "DataSlateHeaderRight.TargetLanguage", "DataSlateHeaderRight[0].Language", "DataSlateHeaderRight[0].String", "DataSlateHeaderRight[1].Language", "DataSlateHeaderRight[1].String", "DataSlateHeaderRight[2].Language", "DataSlateHeaderRight[2].String", "DataSlateHeaderRight[3].Language", "DataSlateHeaderRight[3].String", "DataSlateHeaderRight[4].Language", "DataSlateHeaderRight[4].String", "DataSlateHeaderRight[5].Language", "DataSlateHeaderRight[5].String", "DataSlateHeaderRight[6].Language", "DataSlateHeaderRight[6].String", "DataSlateHeaderRight[7].Language", "DataSlateHeaderRight[7].String", "DataSlateHeaderRight[8].Language", "DataSlateHeaderRight[8].String", "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].SOL", "Text[0].String", "Text[0].Zuerst schickten die Erdbewohner im Jahr 2107 ein unbemanntes Schiff zum Titan, einem der Saturnmonde. Für die Suche nach Leben jenseits der Erde wurde im Jahr 2130 der Bau der Titan-Astrobasis abgeschlossen. Auch wenn die Suche ergebnislos blieb, war dies das Fundament für die größte Unternehmung der Menschheit", "Text[1].Language", "Text[1].SOL", "Text[1].String", "Text[1].The people of Earth first sent an unmanned craft to Saturn's moon, Titan, in 2107. The Titan Astrobase, was completed in 2130 to search for life beyond Earth. Though fruitless, it laid the groundwork for what would eventually become humanity's greatest endeavor", "Text[2].¡Contempla los majestuosos picos helados de las formaciones de tierra congelada de Nueva Hacienda mientras los vientos helados soplan a través de las espesas nubes en lo alto! ¡Sé testigo de la potencia de las plantas de procesamiento de metano más valiosas de las CU! Luego, entra en calor con una comida caliente en el restaurante más antiguo de la humanidad después de la Tierra", "Text[2].La gente de la Tierra envió por primera vez una nave no tripulada a la luna de Saturno, Titán, en 2107. La astrobase Titán se completó en 2130 para buscar vida más allá de la Tierra. Aunque fue una empresa infructuosa, sentó las bases para lo que eventualmente acabaría convirtiéndose en el mayor esfuerzo de la humanidad", "Text[2].Language", "Text[2].SOL", "Text[2].String", "Text[3].Language", "Text[3].Les habitants de la Terre envoyèrent pour la première fois un appareil inhabité sur la lune de Saturne, Titan, en 2107. La base astrale de Titan fut achevée en 2130 pour participer à la recherche d'une forme de vie au-delà de la Terre. Bien que cette mission n'ait pas abouti, elle posa les bases du plus grand accomplissement de l'histoire de l'humanité", "Text[3].SOL", "Text[3].String", "Text[4].Language", "Text[4].Nel 2107, gli abitanti della Terra inviarono per la prima volta un mezzo senza equipaggio su Titano, la luna di Saturno. L’Astrobase Titano fu completata nel 2130 per cercare forme di vita oltre la Terra. Benché senza successo, questa operazione gettò le basi per quello che sarebbe un giorno diventata la più grande impresa dell’umanità", "Text[4].SISTEMA SOLARE", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].Mieszkańcy Ziemi po raz pierwszy wysłali bezzałogowy statek na księżyc Saturna - Tytan - w roku 2107. Astrobaza Tytan została ukończona w roku 2130 i początkowo miała szukać życia poza Ziemią. Choć poszukiwania były bezowocne, baza stanowiła podwaliny tego, co ostatecznie stało się największym przedsięwzięciem w historii ludzkości", "Text[6].SOL", "Text[6].String", "Text[7].De início, as pessoas da Terra enviaram uma espaçonave não tripulada para a lua de Saturno, Titã, em 2107. A astrobase Titã foi concluída em 2130 com o intuito de procurar vida além da Terra. Embora infrutífero, foi responsável por pavimentar o que eventualmente se tornaria a maior empreitada da humanidade", "Text[7].Language", "Text[7].O SISTEMA SOLAR", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "XALG");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "1F40EE:Starfield.esm")]
    [Trait("EditorID", "UC07_ScrappingNiira")]
    [Trait("SpriggitFile", "Books/UC07_ScrappingNiira - 1F40EE_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_UC07_ScrappingNiira()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "UC07_ScrappingNiira");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "1F40EE:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "DataSlateType").ShouldBe(Helpers.GetDTOField(dto, "DataSlateType"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "InventoryArt").ShouldBe(Helpers.GetDTOField(dto, "InventoryArt"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Teaches.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Teaches.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Teaches.RawContent").ShouldBe(Helpers.GetDTOField(dto, "Teaches.RawContent"));
        Helpers.GetSpriggitField(spriggit, "Text.Count").ShouldBe(Helpers.GetDTOField(dto, "Text.Count"));
        Helpers.GetSpriggitField(spriggit, "Text.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Text.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[0].String").ShouldBe(Helpers.GetDTOField(dto, "Text[0].String"));
        Helpers.GetSpriggitField(spriggit, "Text[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[1].String").ShouldBe(Helpers.GetDTOField(dto, "Text[1].String"));
        Helpers.GetSpriggitField(spriggit, "Text[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[2].String").ShouldBe(Helpers.GetDTOField(dto, "Text[2].String"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[3].String").ShouldBe(Helpers.GetDTOField(dto, "Text[3].String"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[4].String").ShouldBe(Helpers.GetDTOField(dto, "Text[4].String"));
        Helpers.GetSpriggitField(spriggit, "Text[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[5].String").ShouldBe(Helpers.GetDTOField(dto, "Text[5].String"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[6].String").ShouldBe(Helpers.GetDTOField(dto, "Text[6].String"));
        Helpers.GetSpriggitField(spriggit, "Text[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[7].String").ShouldBe(Helpers.GetDTOField(dto, "Text[7].String"));
        Helpers.GetSpriggitField(spriggit, "Text[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[8].String").ShouldBe(Helpers.GetDTOField(dto, "Text[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][3].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "XALG").ShouldBe(Helpers.GetDTOField(dto, "XALG"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Count", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0][0].Name", "VirtualMachineAdapter[0][0][0].Object", "VirtualMachineAdapter[0][0][1].Name", "VirtualMachineAdapter[0][0][1].Object", "VirtualMachineAdapter[0][0][2].Name", "VirtualMachineAdapter[0][0][2].Object", "VirtualMachineAdapter[0][0][3].Name", "VirtualMachineAdapter[0][0][3].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "XALG");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Count", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0][0].Name", "VirtualMachineAdapter[0][0][0].Object", "VirtualMachineAdapter[0][0][1].Name", "VirtualMachineAdapter[0][0][1].Object", "VirtualMachineAdapter[0][0][2].Name", "VirtualMachineAdapter[0][0][2].Object", "VirtualMachineAdapter[0][0][3].Name", "VirtualMachineAdapter[0][0][3].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "XALG");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "26E6B1:Starfield.esm")]
    [Trait("EditorID", "SQ_PlanetSurveySlate00_025")]
    [Trait("SpriggitFile", "Books/SQ_PlanetSurveySlate00_025 - 26E6B1_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_SQ_PlanetSurveySlate00_025()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "SQ_PlanetSurveySlate00_025");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "26E6B1:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "DataSlateType").ShouldBe(Helpers.GetDTOField(dto, "DataSlateType"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "InventoryArt").ShouldBe(Helpers.GetDTOField(dto, "InventoryArt"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Teaches.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Teaches.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Teaches.RawContent").ShouldBe(Helpers.GetDTOField(dto, "Teaches.RawContent"));
        Helpers.GetSpriggitField(spriggit, "Text.Count").ShouldBe(Helpers.GetDTOField(dto, "Text.Count"));
        Helpers.GetSpriggitField(spriggit, "Text.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Text.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[0].String").ShouldBe(Helpers.GetDTOField(dto, "Text[0].String"));
        Helpers.GetSpriggitField(spriggit, "Text[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[1].String").ShouldBe(Helpers.GetDTOField(dto, "Text[1].String"));
        Helpers.GetSpriggitField(spriggit, "Text[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[2].String").ShouldBe(Helpers.GetDTOField(dto, "Text[2].String"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[3].String").ShouldBe(Helpers.GetDTOField(dto, "Text[3].String"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[4].String").ShouldBe(Helpers.GetDTOField(dto, "Text[4].String"));
        Helpers.GetSpriggitField(spriggit, "Text[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[5].String").ShouldBe(Helpers.GetDTOField(dto, "Text[5].String"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[6].String").ShouldBe(Helpers.GetDTOField(dto, "Text[6].String"));
        Helpers.GetSpriggitField(spriggit, "Text[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[7].String").ShouldBe(Helpers.GetDTOField(dto, "Text[7].String"));
        Helpers.GetSpriggitField(spriggit, "Text[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[8].String").ShouldBe(Helpers.GetDTOField(dto, "Text[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "XALG").ShouldBe(Helpers.GetDTOField(dto, "XALG"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "XALG");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "XALG");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "070510:Starfield.esm")]
    [Trait("EditorID", "_RENAME_TestDataslate")]
    [Trait("SpriggitFile", "Books/_RENAME_TestDataslate - 070510_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_RENAME_TestDataslate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "_RENAME_TestDataslate");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "070510:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft.Count").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft.Count"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[0].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[0].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[0].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[0].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[1].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[1].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[1].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[1].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[2].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[2].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[2].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[2].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[3].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[3].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[3].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[3].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[4].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[4].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[4].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[4].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[5].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[5].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[5].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[5].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[6].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[6].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[6].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[6].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[7].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[7].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[7].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[7].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[8].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[8].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderLeft[8].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderLeft[8].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight.Count").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight.Count"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[0].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[0].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[0].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[0].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[1].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[1].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[1].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[1].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[2].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[2].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[2].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[2].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[3].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[3].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[3].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[3].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[4].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[4].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[4].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[4].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[5].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[5].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[5].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[5].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[6].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[6].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[6].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[6].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[7].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[7].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[7].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[7].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[8].Language").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[8].Language"));
        Helpers.GetSpriggitField(spriggit, "DataSlateHeaderRight[8].String").ShouldBe(Helpers.GetDTOField(dto, "DataSlateHeaderRight[8].String"));
        Helpers.GetSpriggitField(spriggit, "DataSlateType").ShouldBe(Helpers.GetDTOField(dto, "DataSlateType"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "InventoryArt").ShouldBe(Helpers.GetDTOField(dto, "InventoryArt"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Teaches.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Teaches.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Teaches.RawContent").ShouldBe(Helpers.GetDTOField(dto, "Teaches.RawContent"));
        Helpers.GetSpriggitField(spriggit, "Text.Count").ShouldBe(Helpers.GetDTOField(dto, "Text.Count"));
        Helpers.GetSpriggitField(spriggit, "Text.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Text.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[0].String").ShouldBe(Helpers.GetDTOField(dto, "Text[0].String"));
        Helpers.GetSpriggitField(spriggit, "Text[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[1].String").ShouldBe(Helpers.GetDTOField(dto, "Text[1].String"));
        Helpers.GetSpriggitField(spriggit, "Text[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[2].String").ShouldBe(Helpers.GetDTOField(dto, "Text[2].String"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[3].String").ShouldBe(Helpers.GetDTOField(dto, "Text[3].String"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[4].String").ShouldBe(Helpers.GetDTOField(dto, "Text[4].String"));
        Helpers.GetSpriggitField(spriggit, "Text[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[5].String").ShouldBe(Helpers.GetDTOField(dto, "Text[5].String"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[6].String").ShouldBe(Helpers.GetDTOField(dto, "Text[6].String"));
        Helpers.GetSpriggitField(spriggit, "Text[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[7].String").ShouldBe(Helpers.GetDTOField(dto, "Text[7].String"));
        Helpers.GetSpriggitField(spriggit, "Text[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[8].String").ShouldBe(Helpers.GetDTOField(dto, "Text[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "XALG").ShouldBe(Helpers.GetDTOField(dto, "XALG"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DataSlateHeaderLeft.Count", "DataSlateHeaderLeft.TargetLanguage", "DataSlateHeaderLeft[0].Language", "DataSlateHeaderLeft[0].String", "DataSlateHeaderLeft[1].Language", "DataSlateHeaderLeft[1].String", "DataSlateHeaderLeft[2].Language", "DataSlateHeaderLeft[2].String", "DataSlateHeaderLeft[3].Language", "DataSlateHeaderLeft[3].String", "DataSlateHeaderLeft[4].Language", "DataSlateHeaderLeft[4].String", "DataSlateHeaderLeft[5].Language", "DataSlateHeaderLeft[5].String", "DataSlateHeaderLeft[6].Language", "DataSlateHeaderLeft[6].String", "DataSlateHeaderLeft[7].Language", "DataSlateHeaderLeft[7].String", "DataSlateHeaderLeft[8].Language", "DataSlateHeaderLeft[8].String", "DataSlateHeaderRight.Count", "DataSlateHeaderRight.TargetLanguage", "DataSlateHeaderRight[0].Language", "DataSlateHeaderRight[0].String", "DataSlateHeaderRight[1].Language", "DataSlateHeaderRight[1].String", "DataSlateHeaderRight[2].Language", "DataSlateHeaderRight[2].String", "DataSlateHeaderRight[3].Language", "DataSlateHeaderRight[3].String", "DataSlateHeaderRight[4].Language", "DataSlateHeaderRight[4].String", "DataSlateHeaderRight[5].Language", "DataSlateHeaderRight[5].String", "DataSlateHeaderRight[6].Language", "DataSlateHeaderRight[6].String", "DataSlateHeaderRight[7].Language", "DataSlateHeaderRight[7].String", "DataSlateHeaderRight[8].Language", "DataSlateHeaderRight[8].String", "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Version2", "VersionControl", "XALG");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DataSlateHeaderLeft.Count", "DataSlateHeaderLeft.TargetLanguage", "DataSlateHeaderLeft[0].Language", "DataSlateHeaderLeft[0].String", "DataSlateHeaderLeft[1].Language", "DataSlateHeaderLeft[1].String", "DataSlateHeaderLeft[2].Language", "DataSlateHeaderLeft[2].String", "DataSlateHeaderLeft[3].Language", "DataSlateHeaderLeft[3].String", "DataSlateHeaderLeft[4].Language", "DataSlateHeaderLeft[4].String", "DataSlateHeaderLeft[5].Language", "DataSlateHeaderLeft[5].String", "DataSlateHeaderLeft[6].Language", "DataSlateHeaderLeft[6].String", "DataSlateHeaderLeft[7].Language", "DataSlateHeaderLeft[7].String", "DataSlateHeaderLeft[8].Language", "DataSlateHeaderLeft[8].String", "DataSlateHeaderRight.Count", "DataSlateHeaderRight.TargetLanguage", "DataSlateHeaderRight[0].Language", "DataSlateHeaderRight[0].String", "DataSlateHeaderRight[1].Language", "DataSlateHeaderRight[1].String", "DataSlateHeaderRight[2].Language", "DataSlateHeaderRight[2].String", "DataSlateHeaderRight[3].Language", "DataSlateHeaderRight[3].String", "DataSlateHeaderRight[4].Language", "DataSlateHeaderRight[4].String", "DataSlateHeaderRight[5].Language", "DataSlateHeaderRight[5].String", "DataSlateHeaderRight[6].Language", "DataSlateHeaderRight[6].String", "DataSlateHeaderRight[7].Language", "DataSlateHeaderRight[7].String", "DataSlateHeaderRight[8].Language", "DataSlateHeaderRight[8].String", "DataSlateType", "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Version2", "VersionControl", "XALG");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "045631:Starfield.esm")]
    [Trait("EditorID", "TreasureMap_Resource_AnySystem_Unique_Aldumite")]
    [Trait("SpriggitFile", "Books/TreasureMap_Resource_AnySystem_Unique_Aldumite - 045631_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_TreasureMap_Resource_AnySystem_Unique_Aldumite()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "TreasureMap_Resource_AnySystem_Unique_Aldumite");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "045631:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "InventoryArt").ShouldBe(Helpers.GetDTOField(dto, "InventoryArt"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Teaches.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Teaches.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Teaches.RawContent").ShouldBe(Helpers.GetDTOField(dto, "Teaches.RawContent"));
        Helpers.GetSpriggitField(spriggit, "Text.Count").ShouldBe(Helpers.GetDTOField(dto, "Text.Count"));
        Helpers.GetSpriggitField(spriggit, "Text.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Text.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Text[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[0].String").ShouldBe(Helpers.GetDTOField(dto, "Text[0].String"));
        Helpers.GetSpriggitField(spriggit, "Text[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[1].String").ShouldBe(Helpers.GetDTOField(dto, "Text[1].String"));
        Helpers.GetSpriggitField(spriggit, "Text[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[2].String").ShouldBe(Helpers.GetDTOField(dto, "Text[2].String"));
        Helpers.GetSpriggitField(spriggit, "Text[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[3].String").ShouldBe(Helpers.GetDTOField(dto, "Text[3].String"));
        Helpers.GetSpriggitField(spriggit, "Text[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[4].String").ShouldBe(Helpers.GetDTOField(dto, "Text[4].String"));
        Helpers.GetSpriggitField(spriggit, "Text[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[5].String").ShouldBe(Helpers.GetDTOField(dto, "Text[5].String"));
        Helpers.GetSpriggitField(spriggit, "Text[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[6].String").ShouldBe(Helpers.GetDTOField(dto, "Text[6].String"));
        Helpers.GetSpriggitField(spriggit, "Text[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[7].String").ShouldBe(Helpers.GetDTOField(dto, "Text[7].String"));
        Helpers.GetSpriggitField(spriggit, "Text[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Text[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Text[8].String").ShouldBe(Helpers.GetDTOField(dto, "Text[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Name"));
        Helpers.GetSpriggitField(spriggit, "XALG").ShouldBe(Helpers.GetDTOField(dto, "XALG"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][4].Data", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][5].Data", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][6].Data", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "XALG");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Description.TargetLanguage", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "InventoryArt", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "REFL", "Teaches.MutagenObjectType", "Teaches.RawContent", "Text.Count", "Text.TargetLanguage", "Text[0].Language", "Text[0].String", "Text[1].Language", "Text[1].String", "Text[2].Language", "Text[2].String", "Text[3].Language", "Text[3].String", "Text[4].Language", "Text[4].String", "Text[5].Language", "Text[5].String", "Text[6].Language", "Text[6].String", "Text[7].Language", "Text[7].String", "Text[8].Language", "Text[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][4].Data", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][5].Data", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][6].Data", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "XALG");
    }
}