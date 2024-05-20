import { cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import Popup from './Popup';

afterEach(() => {
  cleanup();
});

describe('Popup', () => {
  it('should render with the provided title and message', () => {
    const onClose = vi.fn();

    render(<Popup title="TITLE" message={<p>MESSAGE</p>} onClose={onClose} />);

    expect(screen.getByRole('alert').textContent).toContain('TITLE');
    expect(screen.getByRole('alert').textContent).toContain('MESSAGE');
  });

  it('should close when X button is clicked', async () => {
    const onClose = vi.fn();

    render(<Popup title="TITLE" message={<p>MESSAGE</p>} onClose={onClose} />);

    await userEvent.click(screen.getByLabelText('Close'));

    expect(onClose).toHaveBeenCalledOnce();
  });

  it('should close when OK button is clicked', async () => {
    const onClose = vi.fn();

    render(<Popup title="TITLE" message={<p>MESSAGE</p>} onClose={onClose} />);

    await userEvent.click(screen.getByText('OK'));

    expect(onClose).toHaveBeenCalledOnce();
  });
});
