using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public abstract class BaseContext<TSelf> : DbContext
        where TSelf : BaseContext<TSelf>
    {
        private const string DatabaseFolder = "Resources/master_data/";

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
                optionsBuilder.UseSqlite($"Data Source={DatabaseFolder}{DatabaseFileName}.db.compress");
            }
        }
    }
}