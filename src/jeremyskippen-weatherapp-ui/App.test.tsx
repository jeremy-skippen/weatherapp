import { cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import App from './App';

describe('App', () => {
  const fetchMock = vi.fn();

  beforeEach(() => {
    vi.spyOn(global, 'fetch').mockImplementation(fetchMock);
  });

  afterEach(() => {
    vi.restoreAllMocks();
    cleanup();
  });

  it('should display the weather', async () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: true,
        status: 200,
        json: () => Promise.resolve('party cloudy'),
      }),
    );

    render(<App />);

    await userEvent.click(screen.getByLabelText('API Key'));
    await userEvent.keyboard('API-KEY');

    await userEvent.click(screen.getByRole('textbox', { name: 'City' }));
    await userEvent.keyboard('CITY');

    await userEvent.click(screen.getByRole('textbox', { name: 'Country' }));
    await userEvent.keyboard('COUNTRY');

    await userEvent.click(screen.getByRole('button', { name: 'Go' }));

    expect(screen.getByRole('alert').textContent).toContain('Weather');
    expect(screen.getByRole('alert').textContent).toContain('party cloudy');
  });

  it('should display an error when the API returns an error', async () => {
    fetchMock.mockImplementation(() =>
      Promise.resolve({
        ok: true,
        status: 401,
      }),
    );

    render(<App />);

    await userEvent.click(screen.getByLabelText('API Key'));
    await userEvent.keyboard('API-KEY');

    await userEvent.click(screen.getByRole('textbox', { name: 'City' }));
    await userEvent.keyboard('CITY');

    await userEvent.click(screen.getByRole('textbox', { name: 'Country' }));
    await userEvent.keyboard('COUNTRY');

    await userEvent.click(screen.getByRole('button', { name: 'Go' }));

    expect(screen.getByRole('alert').textContent).toContain(
      'Error fetching weather',
    );
  });
});
