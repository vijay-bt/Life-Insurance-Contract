using LifeInsurance.Domain.Dtos;
using LifeInsurance.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeInsurance.API.Interfaces
{
    public interface ILifeInsuranceService
    {
        public Contract CreateContract(ContractDetails contractDetails);
        public Contract GetContract(string contractId);
        public string UpdateContract(string contractId, ContractDetails contractDetails);
        public string DeleteContract(Contract contract);
        public IEnumerable<Contract> GetAllContracts();
    }
}
