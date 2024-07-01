using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {


        public UnitOfWork( ICustomerUserMaster customerUserMaster, IPortDetailsRepository portDetailsRepository, ICountryRepository countryRepository, IStateRepository stateRepository, ICityListRepository cityListRepository, ITerminalDetailsRepository terminalDetailsRepository, IBerthDetailsRepository berthDetailsRepository, ICargoTypeRepository cargoTypeRepository, ICargoDetailsRepository cargoDetailsRepository, ICargoFamilyRepository cargoFamilyRepository, ICurrencyRepository currencysRepository, IROERateRepository ratesRepository , ICargoHandledRepsitory cargoHandledRepository, IExpenseRepository expensesRepository, IUserRepository userRepository, ICallTypeRepository callTypesRepository, ITaxRepository taxRepository, IROENameRepository rOENamesRepository , IRoleRepository rolesRepository ,  IChargeCodeRepository chargeCodeRepository, ITariffMasterRepository tariffMasterRepository, ITariffRateRepository tariffRateRepository, ITariffSegment tariffSegmentRepository,IFormulaAttributesRepository formatAttributeRepository,IFormulaOpratorRepository formulaOpratorRepository,IFormulaTransactionRepository formulaTransactionRepository,IFormulaRepository formulaRepository,ICustomerRepository customerRepository ,IPDAEstimitorRepository pDAEstimitorRepository, IPDAEstimitorOUTRepository pDAEstimitorOUTRepository, IPDAEstimatorOutPutTariffRepository pDAEstimatorOutPutTariffRepository, IPDAEstimitorOUTNoteRepository pDAEstimitorOUTNoteRepository, IDesignationRepository designationRepository,INotesRepository notesRepository, IPortActivityTypeRepository portActivityTypeRepository,IBankMasterRepository bankMasterRepository, ICompanyRepository companyRepository, IDisclaimersRepository disclaimersRepository, IEmailNotificationConfigurationRepository emailNotificationConfigurationRepository)
        {
            PortDetails = portDetailsRepository;
            Countrys = countryRepository;
            States = stateRepository;
            Citys = cityListRepository;
            TerminalDetails = terminalDetailsRepository;
            BerthDetails = berthDetailsRepository;
            CargoTypes = cargoTypeRepository;
            CargoDetails = cargoDetailsRepository;
            CargoFamilys = cargoFamilyRepository;
            Currencys = currencysRepository;
            Rates = ratesRepository;
            cargoHandled = cargoHandledRepository;
            Expenses = expensesRepository;
            User = userRepository;
            CallTypes = callTypesRepository;
            Taxs = taxRepository;
            ROENames = rOENamesRepository;
            Roles = rolesRepository;
            ChargeCodes = chargeCodeRepository;
            TariffMasters = tariffMasterRepository;
            TariffRates = tariffRateRepository;
            tariffSegment = tariffSegmentRepository;
            FormatAttribute = formatAttributeRepository;
            FormulaOprator = formulaOpratorRepository;
            Formula = formulaRepository;
            FormulaTransaction= formulaTransactionRepository;
            Customer = customerRepository;
            PDAEstimitor =pDAEstimitorRepository;
            PDAEstimitorOUTPUT = pDAEstimitorOUTRepository;
            PDAEstimatorOutPutTariff = pDAEstimatorOutPutTariffRepository;
            PDAEstimitorOUTNote = pDAEstimitorOUTNoteRepository;
            Designation = designationRepository;
            Notes = notesRepository;
            PortActivities = portActivityTypeRepository;
            Company= companyRepository;
            BankMaster = bankMasterRepository;
            CustomerUserMaster = customerUserMaster;
            Disclaimers = disclaimersRepository;
            EmailNotificationConfigurations = emailNotificationConfigurationRepository;

        }

        public IEmailNotificationConfigurationRepository EmailNotificationConfigurations { get; }
        public IPortDetailsRepository PortDetails { get; }

        public ITerminalDetailsRepository TerminalDetails { get; }

        public IBerthDetailsRepository BerthDetails { get; }

        public ICountryRepository Countrys { get; }
        public IStateRepository States { get; }

        public ICityListRepository Citys { get; }
        public ICargoTypeRepository CargoTypes { get; }
        public ICargoFamilyRepository CargoFamilys { get; }
        public ICargoDetailsRepository CargoDetails { get; }

        public ICurrencyRepository Currencys { get; }

        public IROERateRepository Rates { get; }

        public ICargoHandledRepsitory cargoHandled { get; }

        public IExpenseRepository Expenses { get; }

        public IUserRepository User { get; }

        public ICallTypeRepository CallTypes { get; }

        public ITaxRepository Taxs { get; }

        public IROENameRepository ROENames { get; }

        public IRoleRepository Roles { get; }
        public IChargeCodeRepository ChargeCodes { get; }

        public ITariffMasterRepository TariffMasters { get; }

        public ITariffRateRepository TariffRates { get; }
        public ITariffSegment tariffSegment { get; }

        public IFormulaAttributesRepository FormatAttribute { get; }

        public IFormulaOpratorRepository FormulaOprator { get; }

        public IFormulaRepository Formula { get; }
        public IFormulaTransactionRepository FormulaTransaction { get; }
        public ICustomerRepository Customer { get; }
        public IPDAEstimitorRepository PDAEstimitor { get; }

        public IPDAEstimitorOUTRepository PDAEstimitorOUTPUT { get; }

        public IPDAEstimatorOutPutTariffRepository PDAEstimatorOutPutTariff { get; }
        public IPDAEstimitorOUTNoteRepository PDAEstimitorOUTNote { get; }
        public IDesignationRepository Designation { get; }
        public INotesRepository Notes { get; }

        public IPortActivityTypeRepository PortActivities { get; }
        public ICompanyRepository Company { get; }
        public IBankMasterRepository BankMaster { get; }
        public ICustomerUserMaster CustomerUserMaster { get; }

        public IDisclaimersRepository Disclaimers { get; }

    }
}
