using System.Collections.Generic;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaoSolidaria.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Postagem> Postagens { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para Chat
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Remetente)
                .WithMany(u => u.ChatsEnviados)
                .HasForeignKey(c => c.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Destinatario)
                .WithMany(u => u.ChatsRecebidos)
                .HasForeignKey(c => c.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para Comentario
            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Postagem)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.PostagemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para Postagem
            modelBuilder.Entity<Postagem>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Postagens)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}