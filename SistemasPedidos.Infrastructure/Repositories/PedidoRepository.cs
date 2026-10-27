using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Application.Interfaces;
using SistemaPedidos.Domain.Entities;
using SistemaPedidos.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPedidos.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidosDbContext context;

        public PedidoRepository(PedidosDbContext context)
        {
            this.context = context;
        }

        public async Task AgregarAsync(Pedido pedido)
        {
            await context.AddAsync(pedido);
        }

        public async Task GuardarCambiosAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<Pedido?> ObtenerPorIdAsync(int id)
        {
            return await context.Pedidos.Include(p => p.Detalles).FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
