using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IUnitOfWork
    {
        IPortDetailsRepository PortDetails { get; }


        ITerminalDetailsRepository TerminalDetails { get; }

        ICargoDetailsRepository CargoDetails { get; }

        ICargoTypeRepository CargoTypes { get; }

        ICargoFamilyRepository CargoFamilys { get; }

        IBerthDetailsRepository BerthDetails { get; }

        ICountryRepository Countrys { get; }

        IStateRepository States { get; }

        ICityListRepository Citys { get; }

        ICurrencyRepository Currencys { get; }

        IROERateRepository Rates { get; }
        ICargoHandledRepsitory cargoHandled { get; }
        IExpenseRepository Expenses { get; }
        IUserRepository User { get; }

        ICallTypeRepository CallTypes { get; }
        ITaxRepository Taxs { get; }

        IROENameRepository ROENames { get; }

        IRoleRepository Roles { get; }

        IChargeCodeRepository ChargeCodes { get; }

        ITariffMasterRepository TariffMasters { get; }

        ITariffRateRepository TariffRates { get; }
        ITariffSegment tariffSegment { get; }

        IFormulaAttributesRepository FormatAttribute { get; }
        IFormulaOpratorRepository FormulaOprator { get; }

        IFormulaRepository Formula { get; }
        IFormulaTransactionRepository FormulaTransaction { get; }
        ICustomerRepository Customer { get; }
        IPDAEstimitorRepository PDAEstimitor { get; }
        IPDAEstimitorOUTRepository PDAEstimitorOUTPUT { get; }

        IPDAEstimatorOutPutTariffRepository PDAEstimatorOutPutTariff { get; }
        IPDAEstimitorOUTNoteRepository PDAEstimitorOUTNote { get; }
        IDesignationRepository Designation { get; }
        INotesRepository Notes { get; }
        IPortActivityTypeRepository PortActivities { get; }
        ICompanyRepository Company { get; }

        IBankMasterRepository BankMaster { get; }
        ICustomerUserMaster CustomerUserMaster { get; }

        IDisclaimersRepository Disclaimers { get; }

        IEmailNotificationConfigurationRepository EmailNotificationConfigurations { get; }
    }
}
