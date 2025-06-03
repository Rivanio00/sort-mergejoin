using System.Collections.Generic;
namespace sort_mergejoin;
public static class Schemas
{
    public static readonly Dictionary<string, (string[] colunas, string[] tipos)> Tabelas = new()
    {
        {
            "uva",
            (
                new[] { "uva_id", "nome", "tipo", "ano_colheita", "pais_origem_id" },
                new[] { "int", "string", "string", "int", "int" }
            )
        },
        {
            "vinho",
            (
                new[] { "vinho_id", "rotulo", "ano_producao", "uva_id", "pais_producao_id" },
                new[] { "int", "string", "int", "int", "int" }
            )
        },
        {
            "pais",
            (
                new[] { "pais_id", "nome", "sigla" },
                new[] { "int", "string", "string" }
            )
        }
    };
}