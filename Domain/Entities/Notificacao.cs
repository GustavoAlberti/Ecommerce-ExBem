﻿namespace Domain.Entities
{
    public record Notificacao
    {
        public Guid NotificacaoId { get; init; } = Guid.NewGuid();
        public Guid PedidoId { get; init; }
        public string Mensagem { get; init; }
        public DateTime DataEnvio { get; init; } = DateTime.UtcNow;
    }

}
