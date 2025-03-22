namespace DisasterPrediction.Api.Models
{
    public class EnvironmentalData
    {
        public EnvironmentalData(WeatherData weatherData, EarthquakeData earthquakeData)
        {
            WeatherData = weatherData;
            EarthquakeData = earthquakeData;
        }

        public WeatherData WeatherData { get; set; }
        public EarthquakeData EarthquakeData { get; set; }
    }
}
