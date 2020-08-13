using LifeInsurance.API.Extensions;
using LifeInsurance.API.Interfaces;
using LifeInsurance.Domain.Dtos;
using LifeInsurance.Domain.Models;
using LifeInsurance.Domain.StaticClasses;
using LifeInsurance.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LifeInsurance.API.Services
{
    public class LifeInsuranceService: ILifeInsuranceService
    {
        private readonly ILifeInsuranceRepository<Contract> _lifeInsuranceRepository;
        public LifeInsuranceService(ILifeInsuranceRepository<Contract> lifeInsuranceRepository)
        {
            _lifeInsuranceRepository = lifeInsuranceRepository;
        }
        public Contract CreateContract(ContractDetails contractDetails)
        {
            Contract contract = new Contract();
            try
            {
                Random r = new Random();

                CoveragePlan coveragePlan = GetCoveragePlan(contractDetails);

                int age = Utility.GetAge(contractDetails.CustomerDateOfBirth);

                int netPrice = Utility.GetNetPrice(coveragePlan.CoveragePlanType.ToUpper(), age, contractDetails.CustomerGender);

                if (coveragePlan != null)
                {
                    contract = new Contract
                    {
                        ContractId = "LIC" + r.Next().ToString(),
                        CustomerName = contractDetails.CustomerName,
                        CustomerAddress = contractDetails.CustomerAddress.City,
                        CustomerGender = contractDetails.CustomerGender,
                        CustomerCountry = contractDetails.CustomerAddress.Country,
                        CustomerDateofbirth = contractDetails.CustomerDateOfBirth,
                        SaleDate = contractDetails.SaleDate,
                        CoveragePlan = coveragePlan.CoveragePlanType,
                        NetPrice = netPrice
                    };
                    _lifeInsuranceRepository.Add(contract);
                }
            }
            catch(Exception Ex)
            {
                //Log exception
            }
            return contract;
        }

        public string UpdateContract(string contractId, ContractDetails contractDetails)
        {
            Contract contract = new Contract();
            try
            {
                Contract contractToUpdate = _lifeInsuranceRepository.Get(contractId);

                if (contractToUpdate == null)
                {
                    return "The Contract record couldn't be found.";
                }

                CoveragePlan coveragePlan = GetCoveragePlan(contractDetails);

                int age = Utility.GetAge(contractDetails.CustomerDateOfBirth);

                int netPrice = Utility.GetNetPrice(coveragePlan.CoveragePlanType.ToUpper(), age, contractDetails.CustomerGender);

                if (coveragePlan != null)
                {
                    contract = new Contract
                    {
                        ContractId = contractId,
                        CustomerName = contractDetails.CustomerName,
                        CustomerAddress = contractDetails.CustomerAddress.City,
                        CustomerGender = contractDetails.CustomerGender,
                        CustomerCountry = contractDetails.CustomerAddress.Country,
                        CustomerDateofbirth = contractDetails.CustomerDateOfBirth,
                        SaleDate = contractDetails.SaleDate,
                        CoveragePlan = coveragePlan.CoveragePlanType,
                        NetPrice = netPrice
                    };
                }
                _lifeInsuranceRepository.Update(contractToUpdate, contract);
            }
            catch (Exception Ex)
            {
                return "Failed to update contract.";
            }
            return "Contract updated successfully.";
        }
        
        public Contract GetContract(string contractId)
        {
            Contract contract = _lifeInsuranceRepository.Get(contractId);
            return contract;
        }

        public string DeleteContract(Contract contract)
        {
            _lifeInsuranceRepository.Delete(contract);
            return "Contract deleted succussfully";
        }
        public CoveragePlan GetCoveragePlan(ContractDetails contractDetails)
        {
            List<CoveragePlan> coveragePlans = CoveragePlans.coveragePlans;

            var plan = coveragePlans.Where(c => (c.EligibilityDateFrom <= contractDetails.SaleDate && c.EligibilityDateTo >= contractDetails.SaleDate) && (c.EligibilityCountry == contractDetails.CustomerAddress.Country)).FirstOrDefault();

            if (plan == null)
            {
                plan = coveragePlans.Where(c => (c.EligibilityDateFrom <= contractDetails.SaleDate && c.EligibilityDateTo >= contractDetails.SaleDate) && (c.EligibilityCountry == "ANY")).FirstOrDefault();
            }
            return plan;
        }
    }
}
