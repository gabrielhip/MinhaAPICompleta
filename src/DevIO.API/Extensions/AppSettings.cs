namespace DevIO.API.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; } //chave de criptografia do token
        public int ExpiracaoHoras { get; set; } //quantas horas o token vai levar até perder a validade 
        public string Emissor { get; set; } //quem emite (no caso, a aplicação)
        public string ValidoEm { get; set; } //em quais URLs esse token é válido
    }
}
