import { ErrorMessage, Field, Form, Formik } from 'formik';
import { useState } from 'react';
import * as yup from 'yup';

import { getWeather } from './weather';
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
  const [weather, setWeather] = useState('');

  return (
    <div className="window" style={{ minWidth: 300 }}>
      <div className="title-bar">
        <div className="title-bar-text">Get the Weather 🌞</div>
        <div className="title-bar-controls">
          <button aria-label="Minimize"></button>
          <button aria-label="Maximize"></button>
          <button aria-label="Close"></button>
        </div>
      </div>
      <div className="window-body">
        <Formik<FormState>
          initialValues={{ apiKey: '', cityName: '', countryName: '' }}
          validationSchema={validationSchema}
          onSubmit={({ apiKey, cityName, countryName }) =>
            getWeather(apiKey, cityName, countryName)
              .then(setWeather)
              .catch((err) => {
                console.error(err);
              })
          }
        >
          {({ isSubmitting, isValid }) => (
            <Form>
              <div className="field-row-stacked">
                <label htmlFor="apiKey">API Key</label>
                <Field name="apiKey" type="password" />
                <ErrorMessage name="apiKey" component="p" className="error" />
              </div>
              <div className="field-row-stacked">
                <label htmlFor="cityName">City</label>
                <Field name="cityName" type="text" />
                <ErrorMessage name="cityName" component="p" className="error" />
              </div>
              <div className="field-row-stacked">
                <label htmlFor="countryName">Country</label>
                <Field name="countryName" type="text" />
                <ErrorMessage
                  name="countryName"
                  component="p"
                  className="error"
                />
              </div>
              {weather ? <p>{weather}</p> : null}
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
  );
}

export default App;
