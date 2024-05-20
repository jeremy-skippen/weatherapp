import { API_BASE_URI } from './config';

export enum WeatherRejectionReason {
  InvalidApiKey = 'invalid-api-key',
  WeatherNotFound = 'not-found',
  RateLimitExceeded = 'rate-limit-exceeded',
  Unknown = 'unknown',
}

export async function getWeather(
  apiKey: string,
  cityName: string,
  countryName: string,
): Promise<string> {
  return fetch(
    `${API_BASE_URI}/weather?cityName=${cityName}&countryName=${countryName}`,
    {
      headers: {
        Authorization: `ApiKey ${apiKey}`,
      },
    },
  )
    .catch(() => Promise.reject(WeatherRejectionReason.Unknown))
    .then((response) => {
      if (response.ok) {
        return response.json() as Promise<string>;
      }

      if (response.status === 401) {
        return Promise.reject(WeatherRejectionReason.InvalidApiKey);
      }

      if (response.status === 404) {
        return Promise.reject(WeatherRejectionReason.WeatherNotFound);
      }

      if (response.status === 429) {
        return Promise.reject(WeatherRejectionReason.RateLimitExceeded);
      }

      return Promise.reject(WeatherRejectionReason.Unknown);
    });
}
