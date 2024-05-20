import { cleanup, render, screen } from '@testing-library/react';
import { afterEach, describe, expect, it } from 'vitest';
import WeatherError from './WeatherError';
import { WeatherRejectionReason } from './weather';

afterEach(() => {
  cleanup();
});

describe('WeatherError', () => {
  it('should render unauthorized message when error is InvalidApiKey', () => {
    render(<WeatherError error={WeatherRejectionReason.InvalidApiKey} />);

    expect(screen.getByRole('paragraph').textContent).toEqual(
      'Invalid API key',
    );
  });

  it('should render unauthorized message when error is RateLimitExceeded', () => {
    render(<WeatherError error={WeatherRejectionReason.RateLimitExceeded} />);

    expect(screen.getByRole('paragraph').textContent).toContain(
      'Rate limit exceeded',
    );
    expect(screen.getByRole('paragraph').textContent).toContain(
      'Please try again later',
    );
  });

  it('should render not found message when error is WeatherNotFound', () => {
    render(<WeatherError error={WeatherRejectionReason.WeatherNotFound} />);

    expect(screen.getByRole('paragraph').textContent).toEqual('City not found');
  });
});
