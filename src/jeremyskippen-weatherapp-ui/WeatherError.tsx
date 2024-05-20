import { WeatherRejectionReason } from './weather';

import './WeatherError.css';

interface WeatherErrorProps {
  error: WeatherRejectionReason;
}

export default function WeatherError({ error }: WeatherErrorProps) {
  return (
    <div className="error-container">
      <img src={`/${error}.png`} />
      <p>
        {error === WeatherRejectionReason.InvalidApiKey ? (
          <>Invalid API key</>
        ) : null}
        {error === WeatherRejectionReason.RateLimitExceeded ? (
          <>
            Rate limit exceeded
            <br />
            Please try again later
          </>
        ) : null}
        {error === WeatherRejectionReason.WeatherNotFound ? (
          <>City not found</>
        ) : null}
        {error === WeatherRejectionReason.Unknown ? (
          <>An unknown error occurred</>
        ) : null}
      </p>
    </div>
  );
}
