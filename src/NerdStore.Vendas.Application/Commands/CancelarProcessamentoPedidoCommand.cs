using System;

using NerdStore.Core.Messages;
using NerdStore.Vendas.Application.Commands.Validators;

namespace NerdStore.Vendas.Application.Commands
{
    public class CancelarProcessamentoPedidoCommand : Command
    {
        #region Public Properties

        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }

        #endregion

        #region Constructors

        public CancelarProcessamentoPedidoCommand(Guid pedidoId, Guid clienteId)
        {
            AggregateId = pedidoId;
            PedidoId = pedidoId;
            ClienteId = clienteId;
        }

        #endregion

        #region Overriden Methods

        public override bool EhValido()
        {
            ValidationResult = new CancelarProcessamentoPedidoValidator().Validate(this);
            return ValidationResult.IsValid;
        }

        #endregion
    }
}
