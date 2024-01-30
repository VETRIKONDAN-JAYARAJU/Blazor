using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices
{
    public interface IGenericRepository<TRecord, TKey>
    {
        Task<List<TRecord>> SelectAll();

        Task<List<TRecord>> SelectActive();

        Task<TRecord> SelectById(TKey key);

        Task<bool> Insert(TRecord record);

        Task<bool> Update(TRecord record);

        Task<bool> Delete(TKey key);
    }
}