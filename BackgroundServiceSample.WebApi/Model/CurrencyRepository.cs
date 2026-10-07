using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace BackgroundServiceSample.WebApi.Model
{
    public class CurrencyRepository
    {
        private List<Currency> _currencies;

        public IReadOnlyCollection<Currency> Currencies => _currencies;

        public CurrencyRepository()
        {
            _currencies ??=
                [
                    new()
                    {
                        Id = 1,
                        Name = "Dollar",
                        Rate = 270_000
                    },
					new()
					{
						Id = 2,
						Name = "Euro",
						Rate = 324_000
					}
				];
        }

        public Currency? Update(Currency model)
        {
            Currency? currency = _currencies.Find(a => a.Id == model.Id);
            if (currency is null) return default;

            currency.Rate = model.Rate;

            return currency;
        }
    }
}
