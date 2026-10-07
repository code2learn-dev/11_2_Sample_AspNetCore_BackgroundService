using BackgroundServiceSample.WebMinimalApi.Helpers;

namespace BackgroundServiceSample.WebMinimalApi.RateServices
{
    /// <summary>
    /// دز اکثر اپلیکیشن های اجرای وظایف در پس زمینه یک امر معمول است بنابراین ما 
    /// می توانیم یکسری از وظایف را همچون  پردازش دسته ایمیل ها ، مدیریت کردن 
    /// رویدادی های قرار داده شده در یک صف و یا یکسری از کارهای طولانی که باعث می شود
    /// تا تحربه کاربری خوبی را ایجاد نکند را در پس زمینه اجرا می کنیم
    /// و برای اجرای این وظایف در پس زمینه از IHostedService ها استفاده می کنیم
    /// جهت ایجاد تسک ها پس زمینه به صورت راحتر با استفاده از الگوهای کاربردی
    /// Asp.Net Core یک کلاس انتزاعی را که از اینترفیس IHostedService ارث بری می کند را 
    /// ایجاد کرده است که می توانیم از این کلاس استفاده کنیم
    /// </summary>
    public class RateHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CacheService _cacheService;

        public RateHostedService(
                            IServiceProvider serviceProvider,
							CacheService cacheService)
        {
            _serviceProvider = serviceProvider;
            _cacheService = cacheService;
        }

        /// <summary>
        /// این کلاس انتزاعی یک متد دارد که باید بازنویسی شود و در این متد یک تسک به 
        /// صورت بی نهایت تا زمانی که اپلیکیشن متوقف شود آن تسک را در بازه های زمانی
        /// خاص احرا می کند که در این مثال وطیقه این تسک گرفتن لیست نرخ ارز و کش کردن آن
        /// در هر بار احرای حلقه while است
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // اجرا در حالت عادی که اجرای برنامه اصلی به تعویق نخواهد افتاد
            //while(!stoppingToken.IsCancellationRequested)
            //{
            //    var rateClient = _serviceProvider.GetRequiredService<RateClientService>();
            //    var rates = await rateClient.GetClientCurrencies();
            //    _cacheService.SetCacheData("rates", rates);

            //    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            //}


            // در حالت دوم از یک متد کمکی استفاده می کنیم که ابتدا بررسی می کنیم که اگر
            // تا زمانی که احرای درخواست موفقیت آمیز نباشد برنامه اصلی احرا نمی شود
            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                await TryGetRates();
            }
        }

        /// <summary>
        /// در زمان هایی ممکن است تسکی که باید قبل از ارسال اولین درخواست باید در پس زمینه احرا شوذ
        /// خیلی طولانی باشد و در ارسال اولین درخواست برای نمایش نتیجه تسکی که در پس زمینه باید
        /// اجرا شود منجر به خطا می شود بنابراین در چنین حالتی باید از یک متد دیگر که در کلاس پایه
        /// می باشد استفاده کنیم که باعث می شود تا تکمیل اجرای تسک در پس زمینه اجرای برنامه اصلی به تعویق بیافند
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            bool isSuccess = false;
            while(!isSuccess && !cancellationToken.IsCancellationRequested)
            {
                isSuccess = await TryGetRates();
            }

            await base.StartAsync(cancellationToken);
        }

        private async Task<bool> TryGetRates()
        {
            try
            {
                var rateClient = _serviceProvider.GetRequiredService<RateClientService>();
                var currentcies = await rateClient.GetClientCurrencies();
                _cacheService.SetCacheData("rates", currentcies);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
