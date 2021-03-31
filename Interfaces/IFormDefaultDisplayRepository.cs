using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Forms;
using API.DataManagement.Models.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Interfaces
{
    public interface IFormDefaultDisplayRepository
    {
        public Task<int> InsertDefaultFormDisplayAsync(DefaultFormDisplayDTO data);
        public Task<List<DefaultFormDisplay>> GetAllDefaultFormDisplaysAsync();
        public Task<int> UpdateDefaultFormDisplayAsync(UpdateDefaultFormDisplayDTO data);
        public Task<DefaultFormDisplay> GetDefaultFormDisplayAsync(StringDTO data);
        public Task<int> DeleteDefaultFormDisplayAsync(StringDTO data);
        public Task<bool> FormExistAsync(string formName);

    }
}
