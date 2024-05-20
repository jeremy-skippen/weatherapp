import { ErrorMessage, Field, Form, Formik } from 'formik';
import { useState } from 'react';
import * as yup from 'yup';

import Popup from './Popup';
import WeatherError from './WeatherError';
import { WeatherRejectionReason, getWeather } from './weather';
import './App.css';

interface FormState {
  apiKey: string;
  cityName: string;
  countryName: string;
}

const validationSchema = yup.object<FormState>({
  apiKey: yup.string().required('Required'),
  cityName: yup.string().required('Required'),
  countryName: yup.string().required('Required'),
});

function App() {
  const [error, setError] = useState('');
  const [weather, setWeather] = useState('');

  return (
    <>
      <div className="window" style={{ minWidth: 300 }}>
        <div className="title-bar">
          <div className="title-bar-text">Get the Weather 🌞</div>
        </div>
        <div className="window-body">
          <Formik<FormState>
            initialValues={{ apiKey: '', cityName: '', countryName: '' }}
            validationSchema={validationSchema}
            onSubmit={({ apiKey, cityName, countryName }) =>
              getWeather(apiKey, cityName, countryName)
                .then(setWeather)
                .catch((reason) => {
                  setError(
                    typeof reason === 'string' ? reason : reason.toString(),
                  );
                })
            }
          >
            {({ isSubmitting, isValid }) => (
              <Form>
                <div className="field-row-stacked">
                  <label htmlFor="apiKey">API Key</label>
                  <Field id="apiKey" name="apiKey" type="password" />
                  <ErrorMessage name="apiKey" component="p" className="error" />
                </div>
                <div className="field-row-stacked">
                  <label htmlFor="cityName">City</label>
                  <Field id="cityName" name="cityName" type="text" />
                  <ErrorMessage
                    name="cityName"
                    component="p"
                    className="error"
                  />
                </div>
                <div className="field-row-stacked">
                  <label htmlFor="countryName">Country</label>
                  <Field id="countryName" name="countryName" type="text" />
                  <ErrorMessage
                    name="countryName"
                    component="p"
                    className="error"
                  />
                </div>
                <section
                  className="field-row"
                  style={{ justifyContent: 'flex-end' }}
                >
                  <button disabled={isSubmitting || !isValid} type="submit">
                    Go
                  </button>
                </section>
              </Form>
            )}
          </Formik>
        </div>
      </div>
      {error ? (
        <Popup
          title="Error fetching weather"
          message={<WeatherError error={error as WeatherRejectionReason} />}
          onClose={() => setError('')}
        />
      ) : null}
      {weather ? (
        <Popup
          title="Weather"
          message={weather}
          onClose={() => setWeather('')}
        />
      ) : null}
    </>
  );
}

export default App;
