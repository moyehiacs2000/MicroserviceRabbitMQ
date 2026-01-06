using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Banking.Data.Repository
{
    public class AccountRepositry : IAccountRepository
    {
        private BankingDbContext _context;

        public AccountRepositry(BankingDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAcounts()
        {
            return _context.Accounts;
        }
    }
}
