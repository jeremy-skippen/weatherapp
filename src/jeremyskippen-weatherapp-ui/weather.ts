const API_BASE_URI = `http://${import.meta.env.VITE_API_BASE_URI_HOST}`;

export async function getWeather(
  apiKey: string,
  cityName: string,
  countryName: string,
): Promise<string> {
  const response = await fetch(
    `${API_BASE_URI}/weather?cityName=${cityName}&countryName=${countryName}`,
    {
      headers: {
        Authorization: `ApiKey ${apiKey}`,
      },
    },
  );

  return response.json() as Promise<string>;
}
