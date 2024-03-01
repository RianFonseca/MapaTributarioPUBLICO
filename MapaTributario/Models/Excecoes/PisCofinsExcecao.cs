public class PisCofinsExcecao
{
    public int IdExPisCosfins { get; set; }
    public string? TextoHtmlPisCofins { get; set; }

    public int IdExcecao { get; set; }

    public PisCofinsExcecao(int idExPisCosfins = 0, string textoHtmlPisCofins = "", int idExcecao = 0)
    {
        IdExPisCosfins = idExPisCosfins;
        TextoHtmlPisCofins = textoHtmlPisCofins;
        IdExcecao = idExcecao;
    }
}

