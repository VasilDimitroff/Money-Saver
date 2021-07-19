namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Home;

    public interface IHomeService
    {
        public Task<IndexDto> GetIndexInfoAsync(string userId);
    }
}
