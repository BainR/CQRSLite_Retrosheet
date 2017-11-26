using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(string id);
        List<T> GetByPartialKey(string keySegment, int segmentStart);
        bool Exists(string id);
        Task SaveAsync(T item, Func<T, string> MakeKey);
    }
}