using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public abstract class BaseContext<TSelf> : DbContext
        where TSelf : BaseContext<TSelf>
    {
        public BaseContext()
        {
        }

        public BaseContext(DbContextOptions<TSelf> options)
            : base(options)
        {
        }

        protected abstract string DatabaseFileName { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var path = Path.Combine(Config.BaseDir, $"{DatabaseFileName}.db.compress");
                optionsBuilder.UseSqlite($"Data Source={path}");
            }
        }
    }
}