import { ReactNode } from 'react';

import './Popup.css';

interface PopupProps {
  title: string;
  message: ReactNode;
  onClose: () => void;
}

export default function Popup({ title, message, onClose }: PopupProps) {
  return (
    <div className="popup-container">
      <div className="window" style={{ minWidth: 180 }} role="alert">
        <div className="title-bar" role="heading">
          <div className="title-bar-text">{title}</div>
          <div className="title-bar-controls">
            <button aria-label="Close" onClick={onClose}></button>
          </div>
        </div>
        <div className="window-body">
          {message}
          <section className="field-row" style={{ justifyContent: 'flex-end' }}>
            <button type="submit" onClick={onClose}>
              OK
            </button>
          </section>
        </div>
      </div>
    </div>
  );
}
