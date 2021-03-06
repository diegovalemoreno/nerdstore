using System;

using NerdStore.Core.Messages;
using NerdStore.Vendas.Application.Commands.Validators;

namespace NerdStore.Vendas.Application.Commands
{
    public class AtualizarItemPedidoCommand : Command
    {
        #region Public Properties

        public Guid ClienteId { get; private set; }        
        public Guid ProdutoId { get; private set; }
        public int Quantidade { get; private set; }

        #endregion

        #region Constructors

        public AtualizarItemPedidoCommand(Guid clienteId, Guid produtoId, int quantidade)
        {            
            ClienteId = clienteId;            
            ProdutoId = produtoId;
            Quantidade = quantidade;
        }

        #endregion

        #region Overriden Methods

        public override bool EhValido()
        {
            ValidationResult = new AtualizarItemPedidoValidator().Validate(this);
            return ValidationResult.IsValid;
        }

        #endregion
    }
}
