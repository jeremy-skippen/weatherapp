import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { WeatherRejectionReason, getWeather } from './weather';

describe('getWeather', () => {
  const fetchMock = vi.fn();

  beforeEach(() => {
    vi.spyOn(global, 'fetch').mockImplementation(fetchMock);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('should reject with WeatherRejectionReason.Unknown for transport level errors', () => {
    fetchMock.mockImplementation(() =>
      Promise.reject(new TypeError('fetch failure')),
    );

    getWeather('APIKEY', 'CITY', 'COUNTRY')
      .then(() => {
        throw new Error('Expecting a rejection');
      })
      .catch((reason) => {
        expect(reason).toBe(WeatherRejectionReason.Unknown);
      });
  });

  it('should reject with WeatherRejectionReason.InvalidApiKey for authentication errors', () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: false,
        status: 401,
      }),
    );

    getWeather('APIKEY', 'CITY', 'COUNTRY')
      .then(() => {
        throw new Error('Expecting a rejection');
      })
      .catch((reason) => {
        expect(reason).toBe(WeatherRejectionReason.InvalidApiKey);
      });
  });

  it('should reject with WeatherRejectionReason.WeatherNotFound for not found errors', () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: false,
        status: 404,
      }),
    );

    getWeather('APIKEY', 'CITY', 'COUNTRY')
      .then(() => {
        throw new Error('Expecting a rejection');
      })
      .catch((reason) => {
        expect(reason).toBe(WeatherRejectionReason.WeatherNotFound);
      });
  });

  it('should reject with WeatherRejectionReason.WeatherNotFound for rate limit errors', () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: false,
        status: 429,
      }),
    );

    getWeather('APIKEY', 'CITY', 'COUNTRY')
      .then(() => {
        throw new Error('Expecting a rejection');
      })
      .catch((reason) => {
        expect(reason).toBe(WeatherRejectionReason.RateLimitExceeded);
      });
  });

  it('should reject with WeatherRejectionReason.Unknown for server errors', () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: false,
        status: 500,
      }),
    );

    getWeather('APIKEY', 'CITY', 'COUNTRY')
      .then(() => {
        throw new Error('Expecting a rejection');
      })
      .catch((reason) => {
        expect(reason).toBe(WeatherRejectionReason.Unknown);
      });
  });

  it('should resolve the response correctly for successful requests', async () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: true,
        status: 200,
        json: () => Promise.resolve('weather'),
      }),
    );

    var response = await getWeather('APIKEY', 'CITY', 'COUNTRY');
    expect(response).toBe('weather');
  });
});
