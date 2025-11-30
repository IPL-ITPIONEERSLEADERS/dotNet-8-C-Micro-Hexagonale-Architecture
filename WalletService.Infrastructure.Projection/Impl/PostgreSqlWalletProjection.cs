using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.ValueObjects;
using WalletService.Infrastructure.Projection.Contracts;

namespace WalletService.Infrastructure.Projection.Impl
{
    public class PostgreSqlWalletProjection : IWalletProjection
    {
        private readonly string _connectionString;

        public PostgreSqlWalletProjection(string connectionString) => _connectionString = connectionString;

        public async Task UpdateBalance(WalletId walletId, decimal balance)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
            INSERT INTO wallet_balances (wallet_id, balance, updated_at)
            VALUES (@walletId, @balance, @now)
            ON CONFLICT (wallet_id) DO UPDATE
            SET balance = @balance, updated_at = @now", conn);
            cmd.Parameters.AddWithValue("walletId", walletId.Value);
            cmd.Parameters.AddWithValue("balance", balance);
            cmd.Parameters.AddWithValue("now", DateTime.UtcNow);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<decimal?> GetBalance(WalletId walletId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT balance FROM wallet_balances WHERE wallet_id = @walletId", conn);
            cmd.Parameters.AddWithValue("walletId", walletId.Value);
            var result = await cmd.ExecuteScalarAsync();
            return result == null ? null : (decimal?)result;
        }
    }
}
