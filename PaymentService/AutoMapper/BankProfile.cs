using AutoMapper;
using PaymentService.Services.BankAccounts.Schemas;
using TblBankAccount = PaymentService.Databases.Schemas.BankAccount;

namespace PaymentService.AutoMapper
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            CreateMap<BankAccountCreateUpdateDto, TblBankAccount>();
            CreateMap<TblBankAccount, BankAccountDto>();
        }
    }
}