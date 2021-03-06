using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using NerdStore.Core.Data;
using NerdStore.Core.Messages;
using NerdStore.Vendas.Domain.Models;
using NerdStore.Vendas.Data.Extensions;
using NerdStore.Core.Communication.Mediator;

namespace NerdStore.Vendas.Data.Context
{
    public class VendasContext : DbContext, IUnitOfWork
    {
        #region Private Read-Only Fields

        private readonly IMediatorHandler _mediatorHandler;

        #endregion

        #region Public Properties

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }

        #endregion

        #region Constructors

        public VendasContext(DbContextOptions<VendasContext> options, IMediatorHandler mediatorHandler)
            : base(options)
        {
            _mediatorHandler = mediatorHandler ?? throw new ArgumentNullException(nameof(mediatorHandler));
        }

        #endregion

        #region IUnitOfWork Members

        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;

                if (entry.State == EntityState.Modified)
                    entry.Property("DataCadastro").IsModified = false;
            }

            var sucesso = await base.SaveChangesAsync() > 0;
            if (sucesso) await _mediatorHandler.PublicarEventos(this);

            return sucesso;
        }

        #endregion

        #region Overriden Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                x => x.GetProperties().Where(p => p.ClrType == typeof(string))))
            {
                property.SetColumnType("varchar(100)");
            }

            modelBuilder.Ignore<Event>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(
                x => x.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            modelBuilder.HasSequence<int>("MinhaSequencia").StartsAt(1000).IncrementsBy(1);
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}