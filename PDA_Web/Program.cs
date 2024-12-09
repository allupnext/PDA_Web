using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PDA_Web.Data;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Infrastructure.Repositories;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<IPortDetailsRepository, PortDetailsRepository>();
builder.Services.AddTransient<ITerminalDetailsRepository, TerminalDetailsRepository>();
builder.Services.AddTransient<IBerthDetailsRepository, BerthDetailsRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<ICargoFamilyRepository, CargoFamilyRepository>();
builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddTransient<IROERateRepository, ROERateRepository>();
builder.Services.AddTransient<ICargoHandledRepsitory, CargoHandledRepsitory>();
builder.Services.AddTransient<IExpenseRepository, ExpenseRepository>();
builder.Services.AddTransient<ITaxRepository, TaxRepository>();
builder.Services.AddTransient<IROENameRepository, ROENameRepository>();
builder.Services.AddTransient<ICargoTypeRepository, CargoTypeRepository>();
builder.Services.AddTransient<ICargoDetailsRepository, CargoDetailsRepository>();
builder.Services.AddTransient<IStateRepository, StateRepository>();
builder.Services.AddTransient<ICityListRepository, CityListRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICallTypeRepository, CallTypeRepository>();
builder.Services.AddTransient<IRoleRepository, RolesRepository>();
builder.Services.AddTransient<IChargeCodeRepository, ChargeCodeRepository>();
builder.Services.AddTransient<ITariffMasterRepository, TariffMasterRepository>();
builder.Services.AddTransient<ITariffRateRepository, TariffRateRepository>();
builder.Services.AddTransient<ITariffSegment, TariffSegmentRepository>();
builder.Services.AddTransient<IFormulaAttributesRepository, FormatProviderRepository>();
builder.Services.AddTransient<IFormulaOpratorRepository, FormulaOpratorRepository>();
builder.Services.AddTransient<IFormulaTransactionRepository, FormulaTransactionRepository>();
builder.Services.AddTransient<IFormulaRepository, FormulaRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IPDAEstimitorRepository, PDAEstimitorRepository>();
builder.Services.AddTransient<IDesignationRepository, DesignationRepository>();
builder.Services.AddTransient<INotesRepository, NotesRepository>();
builder.Services.AddTransient<IPortActivityTypeRepository, PortActivityTypeRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IBankMasterRepository, BankMasterRepository>();
builder.Services.AddTransient<IPDAEstimitorOUTRepository, PDAEstimitorOUTRepository>();
builder.Services.AddTransient<IPDAEstimitorOUTNoteRepository, PDAEstimitorOUTNoteRepository>();
builder.Services.AddTransient<IPDAEstimatorOutPutTariffRepository, PDAEstimatorOutPutTariffRepository>();
builder.Services.AddTransient<ICustomerUserMaster, CustomerUser>();
builder.Services.AddTransient<IDisclaimersRepository, DisclaimersRepository>();
builder.Services.AddTransient<IEmailNotificationConfigurationRepository, EmailNotificationConfigurationRepository>();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//builder.Services.AddHttpContextAccessor();
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);


builder.Services.AddMvc()
               .AddNToastNotifyNoty(new NotyOptions
               {
                   ProgressBar = true,
                   Timeout = 5000,
                   Theme = "metroui",
                   Force = true
               });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=AdminLogin}/{action=Index}/{id?}");

app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapRazorPages();
app.UseRotativa();
app.Run();
