using Microsoft.Extensions.DependencyInjection;

namespace DataServices
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServiceCollections(this IServiceCollection services)
        {
            services.AddSingleton<DBConnection>();
            services.AddScoped<DashboardService>();         // 0
            services.AddScoped<BankService>();              // 1
            services.AddScoped<RecordStatusService>();      // 2
            services.AddScoped<CurrencyService>();          // 3
            services.AddScoped<PaymentMethodService>();     // 4
            services.AddScoped<PaymentStatusService>();     // 5
            services.AddScoped<DeviceService>();            // 6
            services.AddScoped<MerchantService>();          // 7

            services.AddScoped<MerchantBalanceService>();

            services.AddScoped<QRCodeService>();            // 8
            services.AddScoped<EWalletService>();           // 9
            services.AddScoped<BankAccountService>();       // 10
            services.AddScoped<FeeBindService>();           // 11
            services.AddScoped<MerchantBindService>();      // 12
            services.AddScoped<DeviceBindService>();        // 13
            services.AddScoped<RecordStatusService>();      // 14
            services.AddScoped<RoleService>();              // 15
            services.AddScoped<UserService>();              // 16
            services.AddScoped<MenuService>();              // 17
            services.AddScoped<UserMenuService>();          // 18
            services.AddScoped<CommissionService>();        // 19

            services.AddScoped<DepositService>();           // 00
           // services.AddScoped<PayoutService>();
            services.AddScoped<SettlementService>();



            services.AddScoped<ReportService>();


            return services;
        }

    }
}
