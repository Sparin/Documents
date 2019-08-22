import { reject } from "q";

export const defaultPage = 0;
export const defaultLimit = 50;

export default function makeHttpRequest<T>(
    url: string,
    method: string,
    body: any = undefined,
    apiRoot: string = ''): Promise<T> {
    url = apiRoot + url;
    const headers = new Headers();
    headers.append("Accept", "application/json");
    headers.append("Content-Type", "application/json");

    const options = {
        method: method,
        headers: headers,
        body: JSON.stringify(body)
    }

    return new Promise<T>((resolve, reject) =>
        fetch(url, options)
            .then(response => {
                if (!response.ok)
                    return reject(response.json())
                if (response.status === 200)
                    return resolve(response.json());
                resolve();
            }))
        .catch(err => reject(err));
}