namespace Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string UsuarioLogin {  get; private set; }
        public string Email { get; private set; }
        public DateTime DataCadastro { get; private set; }

        private Usuario() { }

        public Usuario(string nome, string email, string usuarioLogin)
        {
            Nome = nome;
            Email = email;
            UsuarioLogin = usuarioLogin;
            DataCadastro = DateTime.UtcNow;
        }

        public void AtualizarInformacoes(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }
    }

}
