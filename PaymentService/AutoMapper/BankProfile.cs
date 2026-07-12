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
            CreateMap<TblBankAccount, BankAccountDto>()
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Bank != null ? src.Bank.Name : null))
                .ForMember(dest => dest.BankShortName, opt => opt.MapFrom(src => src.Bank != null ? src.Bank.ShortName : null));
        }
    }
}