using Application.Commands;
using Application.Handlers;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.UnitTests
{
    public class CadastrarUsuarioCommandHandlerTests
    {
        [Fact]
        public async Task CadastrarUsuario_DeveCadastrarUsuarioComSucesso()
        {
            // Arrange
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            var command = new CadastrarUsuarioCommand("João Silva", "joao@email.com", "joao123");

            usuarioRepositoryMock.Setup(repo => repo.ObterPorLoginAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario)null); 

            var handler = new CadastrarUsuarioCommandHandler(usuarioRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeTrue();
            usuarioRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveLancarExcecao_SeUsuarioJaExistir()
        {
            // Arrange
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            var command = new CadastrarUsuarioCommand("João Silva", "joao@email.com", "joao123");

            // Simula que o usuário já existe
            var usuarioExistente = new Usuario("João Silva", "joao@email.com", "joao123");
            usuarioRepositoryMock.Setup(repo => repo.ObterPorLoginAsync(It.IsAny<string>()))
                .ReturnsAsync(usuarioExistente);

            var handler = new CadastrarUsuarioCommandHandler(usuarioRepositoryMock.Object);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            usuarioRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
